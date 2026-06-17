using UnityEngine;

public class ScanMove : MonoBehaviour
{
    public float speed = 1;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        float y = Mathf.PingPong(
            Time.time * speed,
            4
        ) - 2;

        transform.localPosition =
            new Vector3(
                startPos.x,
                y,
                startPos.z
            );
    }
}