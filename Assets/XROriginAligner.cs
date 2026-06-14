using UnityEngine;
using Unity.XR.CoreUtils;

public class XRStartAlign : MonoBehaviour
{
    void Start()
    {
        Invoke(nameof(Align), 0.5f);
    }

    void Align()
    {
        var cam = Camera.main.transform;

        Vector3 forward = cam.forward;
        forward.y = 0;

        if (forward == Vector3.zero)
            forward = transform.forward;

        transform.rotation = Quaternion.LookRotation(forward);
    }
}