using UnityEngine;

public class moveCube : MonoBehaviour
{
    public Rigidbody rb;

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.AddForce(new Vector3(1f, 0f, 0f), ForceMode.Force);
    }
}