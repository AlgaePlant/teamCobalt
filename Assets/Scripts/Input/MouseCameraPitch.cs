using UnityEngine;
using UnityEngine.InputSystem;


public class MouseCameraPitch : MonoBehaviour
{

    public float lookSpeed = 1.2f;

    float rotationX;


    void Update()
    {

        if (Mouse.current == null)
            return;


        if (Mouse.current.rightButton.isPressed)
        {

            Vector2 delta =
                Mouse.current.delta.ReadValue();


            rotationX -=
                delta.y * lookSpeed;


            rotationX =
                Mathf.Clamp(
                rotationX,
                -90,
                90
                );


            transform.localRotation =
                Quaternion.Euler(
                    rotationX,
                    0,
                    0
                );
        }

    }
}