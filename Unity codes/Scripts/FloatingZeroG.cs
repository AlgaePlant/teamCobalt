using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingZeroG : MonoBehaviour
{
    public float floatSpeed = 0.5f;
    public float floatAmount = 0.2f;
    public float rotationSpeed = 10f;

    Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // gentle up and down drifting
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatAmount;
        transform.position = new Vector3(startPos.x, newY, startPos.z);

        // slow rotation
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}
