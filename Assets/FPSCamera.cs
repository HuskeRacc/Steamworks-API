using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCamera : MonoBehaviour
{
    public float sensitivity = 100f;
    public Transform playerGFX;

    //xrot is tracked this way to clamp camera rotation.
    float xRot = 0f;

    private void Start()
    {
        //self explanatory.
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        //Every frame decrease xRotation by mouse Y. 
        xRot -= mouseY;

        //Clamp the xRotation so the player can't flip.
        xRot = Mathf.Clamp(xRot, -90f, 90f);

        // Apply Rotation. Quaternions are responsible for rotation in unity. Euler uses XYZ.
        transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);

        // Left and right player rotation. Rotates player model.
        playerGFX.Rotate(Vector3.up * mouseX);


    }
}
