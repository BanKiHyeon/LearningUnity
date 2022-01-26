using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMove : MonoBehaviour
{
    public float maxSpeed;
    private Rigidbody2D rigid;
    private SpriteRenderer spriteRenderer;
    private Animator anim;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        // Stop Speed
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }

        // Direction Sprite
        if (Input.GetButton("Horizontal"))
        {
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }

        // Walk Animation
        if (Mathf.Abs(rigid.velocity.normalized.x) < 0.3)
        {
            anim.SetBool("isWalking", false);
        }
        else
        {
            anim.SetBool("isWalking", true);
        }
    }

    void FixedUpdate()
    {
        // Move Speed
        float h = Input.GetAxisRaw("Horizontal");
        
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        // Max Speed
        if (rigid.velocity.x > maxSpeed) // Right Max Speed
        {
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        }
        else if (rigid.velocity.x < maxSpeed * (-1)) // Left Max Speed
        {
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);
        }
    }
}