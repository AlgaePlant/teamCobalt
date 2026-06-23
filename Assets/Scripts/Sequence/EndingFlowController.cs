using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI;

public class EndingFlowController : MonoBehaviour
{
    [Header("场景设置")]
    public string transitionSceneName = "TransitionScene";
    
    [Header("视频设置")]
    public VideoPlayer videoPlayer;
    public RenderTexture renderTexture;
    public RawImage videoDisplay;
    
    [Header("备用设置")]
    public float videoDuration = 30f;
    
    private enum State { Playing, Done }
    private State state = State.Playing;
    private bool skipTriggered = false;
    
    void Start()
    {
        // ★ 订阅吉他输入（用于按 1 跳过）
        GuitarInputManager.OnStringPlayed += OnGuitarInput;
        
        SetupVideo();
        Debug.Log("🎬 Ending 场景已加载");
    }
    
    void SetupVideo()
    {
        if (videoPlayer == null) return;
        
        if (renderTexture != null)
        {
            videoPlayer.targetTexture = renderTexture;
            if (videoDisplay != null)
            {
                videoDisplay.texture = renderTexture;
                videoDisplay.enabled = true;
            }
            Debug.Log("Ending VideoPlayer 已连接到 RenderTexture");
        }
        else
        {
            Camera vrCamera = FindObjectOfType<Camera>();
            if (vrCamera != null)
            {
                videoPlayer.targetCamera = vrCamera;
                Debug.Log("Ending VideoPlayer 已连接到相机: " + vrCamera.name);
            }
            else
            {
                Debug.LogWarning("未找到任何 Camera，视频可能无法显示！");
            }
        }
        
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += OnVideoPrepared;
        StartCoroutine(WaitForVideoReady());
    }
    
    void OnGuitarInput(int stringId)
    {
        if (state == State.Done || skipTriggered) return;
        
        // ★ 按 1 跳过视频
        if (stringId == 1)
        {
            SkipVideo();
        }
    }
    
    void SkipVideo()
    {
        if (skipTriggered) return;
        skipTriggered = true;
        state = State.Done;
        
        Debug.Log("[Ending] 按 1 跳过视频");
        
        if (videoPlayer != null && videoPlayer.isPlaying)
        {
            videoPlayer.Stop();
        }
        
        OnVideoFinished();
    }
    
    IEnumerator WaitForVideoReady()
    {
        yield return new WaitForSeconds(2f);
        if (videoPlayer != null && !videoPlayer.isPlaying)
        {
            Debug.Log("视频准备超时，尝试强制播放");
            videoPlayer.Play();
        }
    }
    
    void OnVideoPrepared(VideoPlayer vp)
    {
        Debug.Log("视频准备完成，开始播放");
        vp.Play();
        StartCoroutine(MonitorVideoEnd());
    }
    
    IEnumerator MonitorVideoEnd()
    {
        while (state == State.Playing)
        {
            if (videoPlayer != null)
            {
                long frameCount = (long)videoPlayer.frameCount;
                
                if (videoPlayer.isPlaying == false && videoPlayer.frame > 0)
                {
                    Debug.Log("视频播放结束");
                    OnVideoFinished();
                    yield break;
                }
                
                if (frameCount > 0 && videoPlayer.frame >= frameCount - 2)
                {
                    Debug.Log("视频播放到最后一帧");
                    yield return new WaitForSeconds(0.5f);
                    OnVideoFinished();
                    yield break;
                }
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    void OnVideoFinished()
    {
        if (state == State.Done) return;
        state = State.Done;
        
        if (videoDisplay != null)
        {
            videoDisplay.enabled = false;
        }
        
        Debug.Log("Ending 视频结束，跳转到中转场景");
        SceneManager.LoadScene(transitionSceneName);
    }
    
    void Update()
    {
        // ★ 键盘 ESC 也支持跳过（方便测试）
        if (state == State.Playing && Input.GetKeyDown(KeyCode.Escape))
        {
            SkipVideo();
        }
    }

    void OnDisable()
    {
        GuitarInputManager.OnStringPlayed -= OnGuitarInput;
    }

    void OnDestroy()
    {
        GuitarInputManager.OnStringPlayed -= OnGuitarInput;
    }
}