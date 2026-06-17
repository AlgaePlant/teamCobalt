using System;
using UnityEngine;
using UnityEngine.XR;

public class SceneTransition : MonoBehaviour
{
    public AudioClip startSound;

    public Action onStartPressed;

    private AudioSource audioSource;
    private bool hasTriggered = false;

    private InputDevice rightController;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        InputDevices.deviceConnected += OnDeviceConnected;
    }

    void OnDestroy()
    {
        InputDevices.deviceConnected -= OnDeviceConnected;
    }

    void OnDeviceConnected(InputDevice device)
    {
        if (device.characteristics.HasFlag(InputDeviceCharacteristics.Right) &&
            device.characteristics.HasFlag(InputDeviceCharacteristics.Controller))
        {
            rightController = device;
        }
    }

    void Update()
    {
        if (hasTriggered) return;

        if (!rightController.isValid)
        {
            var devices = new System.Collections.Generic.List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(
                InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller,
                devices);

            if (devices.Count > 0)
                rightController = devices[0];
            else
                return;
        }

        if (rightController.TryGetFeatureValue(CommonUsages.triggerButton, out bool pressed) && pressed)
        {
            hasTriggered = true;

            if (startSound != null)
            {
                audioSource.clip = startSound;
                audioSource.Play();
            }

            onStartPressed?.Invoke();
        }
    }
}