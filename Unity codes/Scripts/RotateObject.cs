using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float speed = 30f; // degrees per second

    void LateUpdate()
    {
        transform.Rotate(0f, speed * Time.deltaTime, 0f, Space.World);
        Debug.Log("Spinning: " + transform.rotation.eulerAngles);
    }
}
