using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float moveSpeed = 10.0f;

    public GameObject bulletPrefab = null;

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

        var horizontalInput = Input.GetAxis("Horizontal");
        myTransform.Translate(Vector3.right * horizontalInput * Time.deltaTime * moveSpeed);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(bulletPrefab, myTransform.position, Quaternion.identity);
        }
    }
}
