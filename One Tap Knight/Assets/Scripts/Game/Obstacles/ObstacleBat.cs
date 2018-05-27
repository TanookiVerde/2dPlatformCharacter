﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class ObstacleBat : MonoBehaviour {

    [Header("Movement Parameters")]
    public float movementSpeed;

    private Rigidbody2D myRB;
    
	void Start () {
        myRB = GetComponent<Rigidbody2D>();
	}
    private void Update()
    {
        myRB.velocity = new Vector2(movementSpeed, 0);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            movementSpeed = -movementSpeed;
            Vector3 temp = transform.localScale;
            temp = new Vector3(-temp.x, temp.y, temp.z);
            transform.localScale = temp;
        }
    }
}
