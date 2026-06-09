using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialoguePanel;
    public Image dialogueImage;

    [Header("Dialogue Images")]
    public Sprite[] dialogueSprites;

    [Header("Timing")]
    public float startDelay = 3f;

    private int currentIndex = 0;
    private bool dialogueStarted = false;
    private bool dialogueFinished = false;

    // ✅ XR controller
    private InputDevice rightController;

    // ✅ Prevent multiple triggers per press
    private bool triggerWasPressedLastFrame = false;

    void Start()
    {
        dialoguePanel.SetActive(false);
        Invoke(nameof(StartDialogue), startDelay);

        // Listen for controller connection
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
            Debug.Log("Right controller connected: " + device.name);
        }
    }

    void Update()
    {
        if (!dialogueStarted || dialogueFinished)
            return;

        // Ensure controller exists
        if (!rightController.isValid)
        {
            var devices = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(
                InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller,
                devices);

            if (devices.Count > 0)
                rightController = devices[0];
            else
                return;
        }

        // ✅ Use digital trigger button (best for UI)
        bool triggerPressed = false;

        if (rightController.TryGetFeatureValue(CommonUsages.triggerButton, out triggerPressed))
        {
            // Fire ONLY once per press
            if (triggerPressed && !triggerWasPressedLastFrame)
            {
                NextDialogue();
            }

            triggerWasPressedLastFrame = triggerPressed;
        }
    }

    void StartDialogue()
    {
        if (dialogueSprites == null || dialogueSprites.Length == 0)
        {
            return;
        }

        dialogueStarted = true;
        currentIndex = 0;
        dialoguePanel.SetActive(true);
        dialogueImage.sprite = dialogueSprites[currentIndex];
    }

    public void NextDialogue()
    {
        currentIndex++;

        if (currentIndex < dialogueSprites.Length)
        {
            dialogueImage.sprite = dialogueSprites[currentIndex];
        }
        else
        {
                if (SceneManager.GetActiveScene().name == "cobaltsphereandkepler")
                {
                    SceneManager.LoadScene("cobaltpart2");
                }
                if (SceneManager.GetActiveScene().name == "cobaltpart4")
                {
                    SceneManager.LoadScene("TitleScreen");
                }

            EndDialogue();
        }
    }

    void EndDialogue()
    {
        dialogueFinished = true;
        dialoguePanel.SetActive(false);
    }
}