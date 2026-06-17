using UnityEngine;
using UnityEngine.Video;

// Uncontrolled version
public class VideoOverlayAutoPlay : MonoBehaviour
{
    [Header("Video Settings")]
    public VideoPlayer videoPlayer;

    void Start()
    {
        if (videoPlayer == null)
        {
            Debug.LogError("VideoOverlayAutoPlay: VideoPlayer not assigned");
            return;
        }

        Debug.Log("Video ready (controlled by GameIntroFlowController)");
    }
}