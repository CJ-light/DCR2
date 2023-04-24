using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Dependencies")]
    public Rigidbody rb;

    [Header("Rotation")]
    public bool rotationEnabled = true;
    public int rotationDevice = 0; // 0 = mouse, 1 = control, 2 = oculus
    public float rotationSpeed = 10;
    public float rotationYAngleAmplitud = 35;
    Vector2 mousePosition;
    Vector2 mousePositionOffset;

    [Header("Movement")]
    public bool movementEnabled = true;
    public int movementDevice = 0; // 0 = keyboard, 1 = control
    public float movementSpeed = 10;

    private void Start()
    {
        mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - mousePositionOffset;
        mousePositionOffset = new Vector2(Screen.width, Screen.height) / 2;
    }

    // Update is called once per frame
    void Update()
    {
        // ########################################## DEVICE SELECT 
        if ( Input.GetKeyDown(KeyCode.Return))
        {
            movementDevice = 0; // enable movement with rows
        }
        if (Input.GetMouseButtonDown(3))
        {
            rotationDevice = 0; // enable rotation with mouse
        }
        // ########################################## END DEVICE SELECT



        // ########################################## ROTATION
        if( rotationEnabled )
        {
            if (rotationDevice == 0)
            {

                Vector3 direction = transform.eulerAngles;

                Vector2 newMousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - mousePositionOffset;
                Vector2 mouseMovement = newMousePosition - mousePosition;
                mousePosition = newMousePosition;

                // horizontal rotation calculation
                direction.y += mouseMovement.x * rotationSpeed * Time.deltaTime;
                if (newMousePosition.x / mousePositionOffset.x > 0.95)
                {
                    direction.y += 3 * rotationSpeed * Time.deltaTime;
                }
                else if (newMousePosition.x / mousePositionOffset.x < -0.95)
                {
                    direction.y -= 3 * rotationSpeed * Time.deltaTime;
                }

                // vertical rotation calculation
                direction.x -= mouseMovement.y * rotationSpeed * Time.deltaTime;
                if (direction.x > 180 && direction.x < (360 - rotationYAngleAmplitud))
                {
                    direction.x = 360 - rotationYAngleAmplitud;
                }
                else if (direction.x <= 180 && direction.x > rotationYAngleAmplitud)
                {
                    direction.x = rotationYAngleAmplitud;
                }

                // rotation assigment
                transform.eulerAngles = direction;
            }
            else if (rotationDevice == 1)
            {
                // Kevs, code here
            }
            else if (rotationDevice == 2)
            {
                // Do nothing, oculus does the work
            }
        }
        // ########################################## END ROTATION



        // ########################################## MOVEMENT
        if( movementEnabled )
        {
            if (movementDevice == 0)
            {
                Vector3 movement = Input.GetAxis("Vertical") * transform.forward;
                rb.velocity = movement * movementSpeed;
            }
            else if (movementDevice == 1)
            {
                // kevs, code here
            }
            else
            {
                rb.velocity = Vector3.zero;
            }
        }
        // ########################################## END MOVEMENET
    }
}
