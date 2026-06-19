// Assets/Scripts/Sequence/EndingFlowController.cs
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.InputSystem;  // ★ 添加这一行（Keyboard 需要）

public class EndingFlowController : MonoBehaviour
{
    [Header("场景设置")]
    public string transitionSceneName = "TransitionScene";
    
    [Header("视频设置")]
    public VideoPlayer videoPlayer;
    public RenderTexture renderTexture;
    public RawImage videoDisplay;
    
    [Header("跳过设置")]
    public int skipCount = 5;
    public float skipTimeWindow = 2f;
    
    [Header("备用设置")]
    public float videoDuration = 30f;
    
    private enum State { Playing, Skipping, Done }
    private State state = State.Playing;
    
    private int chord1Count = 0;
    private float lastChord1Time = 0f;
    private bool skipTriggered = false;
    
    void Start()
    {
        GuitarInputManager.OnStringPlayed += OnGuitarInput;
        
        if (videoPlayer != null)
        {
            if (renderTexture != null)
            {
                videoPlayer.targetTexture = renderTexture;
                
                if (videoDisplay != null)
                {
                    videoDisplay.texture = renderTexture;
                    videoDisplay.enabled = true;
                }
            }
            
            videoPlayer.Prepare();
            videoPlayer.prepareCompleted += OnVideoPrepared;
            StartCoroutine(WaitForVideoReady());
        }
        else
        {
            Debug.LogError("❌ EndingFlowController: 没有 VideoPlayer！");
        }
        
        Debug.Log("🎬 Ending 场景已加载");
    }
    
    IEnumerator WaitForVideoReady()
    {
        yield return new WaitForSeconds(2f);
        
        if (videoPlayer != null && !videoPlayer.isPlaying)
        {
            Debug.Log("⏰ 视频准备超时，尝试强制播放");
            videoPlayer.Play();
        }
    }
    
    void OnVideoPrepared(VideoPlayer vp)
    {
        Debug.Log("✅ 视频准备完成，开始播放");
        vp.Play();
        StartCoroutine(MonitorVideoEnd());
    }
    
    IEnumerator MonitorVideoEnd()
    {
        while (state == State.Playing)
        {
            if (videoPlayer != null)
            {
                // ★ 修复1：检查播放是否结束
                if (videoPlayer.isPlaying == false && videoPlayer.frame > 0)
                {
                    Debug.Log("✅ 视频播放结束");
                    OnVideoFinished();
                    yield break;
                }
                
                // ★ 修复2：检查是否接近最后一帧（显式转换为 long）
                long frameCount = (long)videoPlayer.frameCount;
                if (frameCount > 0 && videoPlayer.frame >= frameCount - 2)
                {
                    Debug.Log("✅ 视频播放到最后一帧");
                    yield return new WaitForSeconds(0.5f);
                    OnVideoFinished();
                    yield break;
                }
            }
            
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    void OnGuitarInput(int stringId)
    {
        if (state == State.Done || skipTriggered) return;
        
        if (stringId == 1)
        {
            CheckSkipCondition();
        }
    }
    
    void CheckSkipCondition()
    {
        float currentTime = Time.time;
        
        if (currentTime - lastChord1Time > skipTimeWindow)
        {
            chord1Count = 0;
        }
        
        chord1Count++;
        lastChord1Time = currentTime;
        
        Debug.Log($"🔢 弦1 快速点击: {chord1Count}/{skipCount}");
        
        if (chord1Count >= skipCount)
        {
            SkipVideo();
        }
    }
    
    void SkipVideo()
    {
        if (skipTriggered) return;
        skipTriggered = true;
        state = State.Skipping;
        
        Debug.Log($"⏭️ 快速跳过视频！连按 {skipCount} 次弦1");
        
        if (videoPlayer != null && videoPlayer.isPlaying)
        {
            videoPlayer.Stop();
        }
        
        OnVideoFinished();
    }
    
    void OnVideoFinished()
    {
        if (state == State.Done) return;
        state = State.Done;
        
        Debug.Log("🎬 Ending 视频结束，跳转到中转场景");
        SceneManager.LoadScene(transitionSceneName);
    }
    
    void Update()
    {
        // ★ 修复3：用 Input.anyKeyDown 替代 Keyboard.current（不需要额外 using）
        if (Input.anyKeyDown && state == State.Playing)
        {
            // 检查是否是 ESC 键
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SkipVideo();
            }
        }
    }
    
    void OnDestroy()
    {
        GuitarInputManager.OnStringPlayed -= OnGuitarInput;
        
        if (videoPlayer != null)
        {
            videoPlayer.prepareCompleted -= OnVideoPrepared;
        }
    }
}