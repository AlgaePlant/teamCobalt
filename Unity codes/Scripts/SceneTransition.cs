using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class SceneTransitionVR : MonoBehaviour
{
    public string nextSceneName = "cobaltsphereandkepler";
    public AudioClip startSound;

    private AudioSource audioSource;
    private bool hasStarted = false;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        // Subscribe to device connection events
        InputDevices.deviceConnected += OnDeviceConnected;
    }

    void OnDestroy()
    {
        InputDevices.deviceConnected -= OnDeviceConnected;
    }

    // Track right controller
    private InputDevice rightController;
    private void OnDeviceConnected(InputDevice device)
    {
        if (device.characteristics.HasFlag(InputDeviceCharacteristics.Right) &&
            device.characteristics.HasFlag(InputDeviceCharacteristics.Controller))
        {
            rightController = device;
            Debug.Log("Right controller connected: " + device.name);
        }
    }

    void Update()
    {
        if (hasStarted)
            return;

        // Ensure we have a controller
        if (!rightController.isValid)
        {
            // Try to get right controller dynamically
            var devices = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller, devices);
            if (devices.Count > 0)
                rightController = devices[0];
            else
                return; // no right controller yet
        }

        // Check trigger button press (digital)
        bool triggerPressed = false;
        if (rightController.TryGetFeatureValue(CommonUsages.triggerButton, out triggerPressed) && triggerPressed)
        {
            hasStarted = true;

            if (startSound != null)
            {
                audioSource.clip = startSound;
                audioSource.Play();
                Invoke(nameof(LoadNextScene), startSound.length);
            }
            else
            {
                LoadNextScene();
            }
        }
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}