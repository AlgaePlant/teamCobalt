using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class GameIntroFlowController : MonoBehaviour
{
    public string nextScene = "MainScene";

    public AudioSource dialogueBGM;

    public VideoPlayer videoPlayer;

    public DialogueManager dialogueManager;

    private enum State { Video, Dialogue, End }
    private State state;

    void Start()
    {
            if (state == 0) // 如果 state 还是默认值，说明 OnEnable 没被执行
        {
            Debug.Log("⚠️ Start() 作为后备触发了初始化");
            OnEnable();
        }
    }

    void OnEnable()
    {
        Debug.Log("🎬 GameIntroFlowController.OnEnable() 被调用");
        
        // ★ 订阅输入事件
        GuitarInputManager.OnStringPlayed += OnGuitarInput;
        
        // ★ 重置状态并开始流程
        if (dialogueManager != null)
            dialogueManager.panel?.SetActive(false);

        state = State.Video;
        StartCoroutine(Run());
    }

    void OnDisable()
    {
        // ★ 取消订阅，防止内存泄漏
        GuitarInputManager.OnStringPlayed -= OnGuitarInput;
        
        // ★ 停止所有协程，防止残留
        StopAllCoroutines();
    }

    void OnGuitarInput(int stringId)
    {
        if (stringId == 1)
        {
            HandlePrimary();
        }

        if (state == State.Dialogue && stringId == 2)
        {
            dialogueManager?.NextManual();
        }
    }

    IEnumerator Run()
    {
        yield return new WaitForSeconds(1f);

        StartVideo();
    }

    // ================= VIDEO =================

    void StartVideo()
    {
        state = State.Video;

        if (dialogueManager != null)
        {
            dialogueManager.panel?.SetActive(false);
        }

        if (videoPlayer != null)
        {
            // ★ 自动连接到跨场景的 VR Camera
            Camera vrCamera = FindObjectOfType<Camera>();
            if (vrCamera != null)
            {
                videoPlayer.targetCamera = vrCamera;
                Debug.Log("✅ Intro VideoPlayer 已连接到相机: " + vrCamera.name);
            }
            else
            {
                Debug.LogWarning("⚠️ 未找到任何 Camera，视频可能无法显示！");
            }
            
            videoPlayer.Play();
        }

        StartCoroutine(VideoAutoEnd());
    }

    IEnumerator VideoAutoEnd()
    {
        yield return new WaitForSeconds(48f);

        if (state != State.Video) yield break;

        EndVideoToDialogue();
    }

    void EndVideoToDialogue()
    {
        videoPlayer?.Stop();

        StartDialogue();
    }

    // ================= DIALOGUE =================

    void StartDialogue()
    {
        state = State.Dialogue;

        StartCoroutine(FadeInBGM());

        if (dialogueManager != null)
        {
            dialogueManager.gameObject.SetActive(true);
            dialogueManager.Begin(this);
        }
    }

    IEnumerator FadeInBGM()
    {
        if (dialogueBGM == null) yield break;

        dialogueBGM.volume = 0;
        dialogueBGM.Play();

        float t = 0;
        while (t < 2f)
        {
            t += Time.deltaTime;
            dialogueBGM.volume = Mathf.Lerp(0, 0.5f, t / 2f);
            yield return null;
        }
    }

    // ================= INPUT =================

    void HandlePrimary()
    {
        if (state == State.Video)
        {
            videoPlayer?.Stop();
            EndVideoToDialogue();
        }
        else if (state == State.Dialogue)
        {
            EndFlow();
        }
    }

    public void OnDialogueFinished()
    {
        EndFlow();
    }

    void EndFlow()
    {
        state = State.End;

        if (dialogueBGM != null)
            dialogueBGM.Stop();

        SceneManager.LoadScene(nextScene);
    }
}