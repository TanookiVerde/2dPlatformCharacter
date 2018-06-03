﻿using UnityEngine;
using DG.Tweening;

public class Character : MonoBehaviour
{
    private float gravityBias = 1;
	[Header("Run")]
	[SerializeField] public float velocity;
	[SerializeField] private float gravity;
    [SerializeField] public float timeToFinish = 0.5f;
    [SerializeField] public static float idealVelocity;
	[Header("Jump")]
	[SerializeField] private int jumpLimit;
	[SerializeField] private float jumpForce;
    [SerializeField] private float groundpPoundSwipeDistance;
	private int currentJump;
    [Header("Grounded")]
	[SerializeField] private Transform groundPosition;
	[SerializeField] private Vector2 groundBoxCastSize;
    [Header("Ground Pound")]
    private Vector3 mouseDownPosition;
    private bool mouseDown;
    [Header("Components")]
    private Rigidbody2D rb;
	private SpriteRenderer sr;
    private CharacterAnimation cAnim;

	public static int coinInLevel;
	public static Transform myTransform;

    private bool finished;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(groundPosition.position, groundBoxCastSize);
    }
    private void Start()
	{
        cAnim = GetComponent<CharacterAnimation>();
		Character.idealVelocity = velocity;
		Character.myTransform = transform;
		rb = GetComponent<Rigidbody2D>();
	}
    private void OnCollisionEnter2D(Collision2D obj)
	{
		if(obj.gameObject.tag == "Obstacles")
		{
			Die();
		}
	}
	private void OnTriggerExit2D(Collider2D obj)
	{
		if(obj.gameObject.tag == "End")
		{
            finished = true;
		}
	}
	public void Action()
    { 
		Move();
        Jump(IsGrounded());
		Gravity();
	}
    public bool FinishedLevel()
    {
        return finished;
    }
	private void Move()
	{
        cAnim.Move();
		rb.velocity = new Vector2(velocity,	rb.velocity.y);
	}
	private void Jump(bool isGrounded)
	{
		if(Input.GetMouseButtonDown(0) && currentJump < jumpLimit)
		{
			if((currentJump == 0 && isGrounded) || currentJump > 0)
			{
				currentJump++;
                cAnim.Jump();
                cAnim.Land(false);
				rb.velocity = new Vector2(rb.velocity.x, jumpForce*gravityBias);
            }
		}
	}
    private void GroundPound(bool isGrounded)
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseDownPosition = Input.mousePosition;
            mouseDown = true;
        }
        if (mouseDown)
        {
            GetMoveDirection();
        }
        //Jump
        if (Input.GetMouseButtonUp(0) && mouseDown)
        {
            if(currentJump < jumpLimit)
            {
                if (currentJump == 0 && isGrounded || currentJump > 0)
                {
                    currentJump++;
                    cAnim.Jump();
                    cAnim.Land(false);
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                }
            }
            mouseDown = false;
        }
    }
	private void Die()
	{
        cAnim.Die();
		PlayerPrefs.SetInt("deathCount", PlayerPrefs.GetInt("deathCount", 0)+1);
		Destroy(this.gameObject);
	}
    private void Gravity()
	{
		rb.velocity += Vector2.down * gravity * gravityBias * Time.deltaTime;
	}
    public void Finish()
    {
        DOTween.To(() => rb.velocity, x => rb.velocity = x, new Vector2(0,0), timeToFinish);
    }
    private void GetMoveDirection()
    {
        Vector3 initP = mouseDownPosition;
        Vector3 finalP = Input.mousePosition;
        finalP.z = initP.z;
        Vector3 d = finalP - initP;

        if (d.x < -groundpPoundSwipeDistance)
        {
            rb.velocity = new Vector2(rb.velocity.x, -jumpForce);
            mouseDown = false;
        }
    }
	private bool IsGrounded()
	{
        if (rb.velocity.y*gravityBias > 0) return false;
		RaycastHit2D boxCast = Physics2D.BoxCast(groundPosition.position,
                groundBoxCastSize,
                0,
                Vector2.up,
                0,
                1 << LayerMask.NameToLayer("Ground")
				);
		if (boxCast.collider != null)
		{
            cAnim.Land(true);
			currentJump = 0;
			return true;
		}
        return false;
	}
    public void InvertGravity()
    {
        transform.DOScaleY(-1*transform.localScale.y, 0.2f);
        gravityBias = -gravityBias;
    }
}
