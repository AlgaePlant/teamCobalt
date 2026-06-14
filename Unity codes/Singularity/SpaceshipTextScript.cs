using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class SpaceshipText : MonoBehaviour
{
    public TMP_Text textUI;

    // Start is called before the first frame update
    void Start()
    {
        textUI.text = "Spaceship status: Deactivated!";
    }

    // Update is called once per frame
    public void UpdateText(int touch)
    {
        Debug.Log("SpaceshipText.UpdateText called with: " + touch);

        if (textUI == null)
        {
            Debug.LogError("textUI in SpaceshipText is null!");
            return;
        }

        if (touch >= 1000)
        {
            Debug.Log("Setting text to Activated!");
            textUI.text = "Spaceship status: Activated!";
        }
        else
        {
            Debug.Log("Setting text to Deactivated!");
            textUI.text = "Spaceship status: Deactivated!";
        }
    }
}
