using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoOverlayAutoPlay : MonoBehaviour
{
    [Header("Video Settings")]
    public VideoPlayer videoPlayer;  // Drag your VideoPlayer here

    void Start()
    {
        if (videoPlayer == null)
        {
            Debug.LogError("VideoOverlayAutoPlay: VideoPlayer is not assigned!");
            return;
        }

        // Play video immediately on scene start
        videoPlayer.Play();
        Debug.Log("Video started immediately on scene start.");

        if (SceneManager.GetActiveScene().name == "cobaltpart3" && videoPlayer.clip != null)
        {
            SceneManager.LoadScene("cobaltpart4");
        }
    }
}