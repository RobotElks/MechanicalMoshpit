using UnityEngine;

public class CameraMultiplayer : MonoBehaviour
{

    Transform localPlayer;
    bool playerSet = false;
    public float mouseSensitivity = 3.0f;
    public float offset = 10.0f;
    public Vector2 rotationMinMax = new Vector2(0, 70);
    private float rotationY;
    private float rotationX;
    public Vector2 zoomMinMax = new Vector2(5, 20);
    private float zoomMin;
    private float zoomMax;

    public void SetLocalPlayer(Transform localPlayer)
    {
        this.localPlayer = localPlayer;
        playerSet = true;
    }

    void Update()
    {
        if (playerSet)
        {
            if (Input.GetMouseButton(1))
            {
                float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
                float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

                rotationY += mouseX;
                rotationX += mouseY;

                // Apply clamping for x rotation 
                rotationX = Mathf.Clamp(rotationX, rotationMinMax.x, rotationMinMax.y);

                Vector3 nextRotation = new Vector3(rotationX, rotationY);
                transform.localEulerAngles = nextRotation;

                // Substract forward vector of the GameObject to point its forward vector to the target
                //transform.position = player.position - transform.forward * offset;
            }

            // ZOOM 
            if (Input.mouseScrollDelta.y < 0f)
            { //forward
                offset++;
            }
            else if (Input.mouseScrollDelta.y > 0f)
            { //Backwards
                offset--;
            }

            offset = Mathf.Clamp(offset, zoomMinMax.x, zoomMinMax.y);
            transform.position = localPlayer.position - transform.forward * offset;
        }
    }
}
