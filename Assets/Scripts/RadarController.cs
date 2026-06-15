using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarController : MonoBehaviour
{
    [Header("Shader Settings")]
    [SerializeField] private string rotationParamName = "_Rotation";

    [Header("Scan Settings")]
    [Tooltip("Time in seconds to complete one full 360-degree spin.")]
    public float scanDuration = 2.0f;

    private Material radarMaterial;
    private float currentRotation = 0f;

    void Start()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            radarMaterial = meshRenderer.material;
        }
        else
        {
            Debug.LogError("VRContinuousRadar needs a MeshRenderer on the same GameObject!");
            enabled = false;
        }
    }

    void Update()
    {
        if (radarMaterial == null || scanDuration <= 0f) return;

        currentRotation += Time.deltaTime / scanDuration;

        // if currentRotation > 1, makes it go back to range 0 - 1 smoothly
        currentRotation = Mathf.Repeat(currentRotation, 1f);

        radarMaterial.SetFloat(rotationParamName, currentRotation);
    }
}
