using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Random = UnityEngine.Random;

public class EnemyControl : MonoBehaviour
{
    public float moveSpeed = 1.0f;

    private Transform myTransform;
    // Start is called before the first frame update

    public GameObject explosionPrefab = null;
    private void Start()
    {
        myTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (myTransform == null)
        {
            return;
        }

        myTransform.Translate(Vector3.down * Time.deltaTime * moveSpeed);

        if (myTransform.position.y < -7.0f)
        {
            InitPosition();
        }
    }

    private void InitPosition()
    {
        transform.position = new Vector3(Random.Range(-8.0f, 8.0f), 5.0f, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("bullet"))
        {
            Instantiate(explosionPrefab, myTransform.position, Quaternion.identity);
            InitPosition();
            Destroy(other.gameObject);
        }
    }
}
