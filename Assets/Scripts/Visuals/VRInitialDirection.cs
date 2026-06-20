using UnityEngine;


public class VRInitialDirection : MonoBehaviour
{

    public float startY = 180f;


    void Start()
    {
        transform.rotation =
            Quaternion.Euler(
                0,
                startY,
                0
            );
    }

}