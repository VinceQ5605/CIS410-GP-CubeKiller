using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseView : MonoBehaviour
{

    public float mouseSensitivityX = 300f;
    public float mouseSensitivityY = 500f;
    public Transform playerBody;
    float x_Rotation = 0f;
    int frameCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerBody.transform.rotation = Quaternion.identity;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivityX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivityY * Time.deltaTime;

        if (frameCount < 50) // don't rotate the camera up/down for some number of the initial frames (this keeps the player from looking down when starting the game)
        {
            frameCount++;
            mouseY = 0f;
        }

        x_Rotation -= mouseY;
        x_Rotation = Mathf.Clamp(x_Rotation, -85f, 85f);

        transform.localRotation = Quaternion.Euler(x_Rotation, 0f, 0f);

        playerBody.Rotate(Vector3.up * mouseX);
        
    }
}
