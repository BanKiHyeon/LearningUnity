using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControl : MonoBehaviour
{
    public float moveSpeed = 15.0f;

    private Transform myTransform;
    // Start is called before the first frame update
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

        myTransform.Translate(Vector3.up * Time.deltaTime * moveSpeed);

        if (myTransform.position.y > 15.0f)
        {
            Destroy(gameObject);
        }
    }
}
