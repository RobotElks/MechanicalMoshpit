using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMenu : MonoBehaviour
{
    public float moveSpeed = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += moveSpeed * Time.deltaTime * transform.right;
        transform.LookAt(Vector3.zero);
    }
}
