using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerBall : MonoBehaviour
{
    public float jumpPower;
    public GameManagerLogic manager;
    private Rigidbody rigid;
    private bool isJump;
    public int itemCount;
    
    private AudioSource audio;

    private void Awake()
    {
        isJump = false;
        rigid = GetComponent<Rigidbody>();
        audio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump") && !isJump)
        {
            isJump = true;
            rigid.AddForce(new Vector3(0, jumpPower, 0), ForceMode.Impulse);
        }
    }

    // 물리기반으로 움직일 꺼라 Update 가 아니고 FixedUpdate를 사용한다고 함
    private void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        
        rigid.AddForce(new Vector3(h, 0, v), ForceMode.Impulse);
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Cube") isJump = false;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            itemCount++;
            audio.Play();
            other.gameObject.SetActive(false);
            manager.GetItem(itemCount);
        } else if (other.tag == "Finish")
        {
            if (itemCount == manager.totalItemCount)
            {
                SceneManager.LoadScene($"ExampleScene1_{manager.stage + 1}");
            }
            else
            {
                SceneManager.LoadScene($"ExampleScene1_{manager.stage}");
            }
        }
    }
}
