using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpaceshipStatusController : MonoBehaviour
{
    [Header("Text Reference")]
    public TMP_Text statusText;  // Assign the TMP text in the Inspector

    [Header("Activation Threshold")]
    public int activationThreshold = 1000;  // Touch value to activate

    /// <summary>
    /// Call this method to update the spaceship status based on a touch value
    /// </summary>
    /// <param name="touch">The touch value received from Arduino</param>
    public void UpdateTouchValue(int touch)
    {
        if (statusText == null)
        {
            Debug.LogError("SpaceshipStatusController: statusText is not assigned!");
            return;
        }

        if (touch >= activationThreshold)
        {
            statusText.text = "Spaceship status: Activated!";
        }
        else
        {
            statusText.text = "Spaceship status: Deactivated!";
        }

        // Optional debug
        Debug.Log($"SpaceshipStatusController updated text: {statusText.text} (touch={touch})");
    }
}