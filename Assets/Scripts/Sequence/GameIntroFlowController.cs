using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class GameIntroFlowController : MonoBehaviour
{
    public string nextScene = "MainScene";
    public AudioSource dialogueBGM;
    public RawImage introVideoImage;
    public VideoPlayer videoPlayer;
    public DialogueManager dialogueManager;

    private enum State { Video, Dialogue, End }
    private State state;
    private bool isInitialized = false;
    private bool inputLocked = true;
    
    // ★ 新增：防止重复触发
    private bool isHandlingPrimary = false;

    void Awake()
    {
        Debug.Log($"[DEBUG-AWAKE] GameIntroFlowController 实例唤醒");
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;

        inputLocked = true;
        state = State.Video;
        isInitialized = false;
        isHandlingPrimary = false;
    }

    void Start()
    {
        Scene current = SceneManager.GetActiveScene();
        Debug.Log($"[DEBUG-START] 当前场景：{current.name}");
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[DEBUG-OnSceneLoaded] 加载场景：{scene.name}");
        if (scene.name == "TitleScreen")
        {
            isInitialized = false;
            state = State.Video;
            inputLocked = true;
            isHandlingPrimary = false;
            StartCoroutine(DelayedInitialize());
        }
    }

    IEnumerator DelayedInitialize()
    {
        yield return null;
        yield return null;
        InitializeTitleScreen();
    }

    void OnEnable()
    {
        GuitarInputManager.OnStringPlayed -= OnGuitarInput;
        GuitarInputManager.OnStringPlayed += OnGuitarInput;
        Debug.Log("[DEBUG-ENABLE] 吉他输入事件已绑定");
    }

    void OnSceneUnloaded(Scene scene)
    {
        if (scene.name == "TitleScreen")
        {
            isInitialized = false;
            inputLocked = true;
            isHandlingPrimary = false;
            StopAllCoroutines();
            state = State.Video;
            if (dialogueBGM != null) dialogueBGM.Stop();
        }
    }

    void InitializeTitleScreen()
    {
        if (isInitialized)
        {
            dialogueManager = FindObjectOfType<DialogueManager>(true);
            if (dialogueManager != null) dialogueManager.ResetState();
            return;
        }

        Debug.Log("🎬 ★★★ 初始化 TitleScreen ★★★");
        isInitialized = true;

        dialogueManager = FindObjectOfType<DialogueManager>(true);
        if (dialogueManager == null)
        {
            Debug.LogError("[DEBUG-FATAL] 未找到 DialogueManager");
            return;
        }

        dialogueManager.ResetState();
        dialogueManager.panel?.SetActive(false);
        dialogueManager.gameObject.SetActive(true);

        if (videoPlayer != null)
        {
            videoPlayer.Stop();
            videoPlayer.time = 0;
        }
        if (introVideoImage != null) introVideoImage.enabled = false;

        state = State.Video;
        StartCoroutine(Run());

        if (GuitarInputManager.Instance != null)
        {
            GuitarInputManager.Instance.ResetSkip();
        }

        inputLocked = false;
        Debug.Log("[DEBUG-INPUTLOCK] 吉他输入已解锁");
    }

    void OnDisable()
    {
        GuitarInputManager.OnStringPlayed -= OnGuitarInput;
        StopAllCoroutines();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
        StopAllCoroutines();
    }

    void OnGuitarInput(int stringId)
    {
        if (inputLocked)
        {
            Debug.Log($"[DEBUG-INPUTLOCK] 丢弃按键：弦{stringId}");
            return;
        }

        Debug.Log($"[DEBUG-INPUT] 收到弦{stringId}，状态:{state}");
        if (state != State.Video && state != State.Dialogue) return;

        if (stringId == 1)
        {
            // ★ 防止重复触发
            if (!isHandlingPrimary)
            {
                isHandlingPrimary = true;
                HandlePrimary();
                // 延迟重置，防止同一帧内多次触发
                StartCoroutine(ResetHandleFlag());
            }
        }

        if (state == State.Dialogue && stringId == 2)
        {
            dialogueManager?.NextManual();
        }
    }

    IEnumerator ResetHandleFlag()
    {
        yield return new WaitForEndOfFrame();
        isHandlingPrimary = false;
    }

    IEnumerator Run()
    {
        yield return new WaitForSeconds(1f);
        StartVideo();
    }

    void StartVideo()
    {
        state = State.Video;
        if (introVideoImage != null) introVideoImage.enabled = true;
        if (dialogueManager != null) dialogueManager.panel?.SetActive(false);
        if (videoPlayer != null) videoPlayer.Play();
        StartCoroutine(VideoAutoEnd());
        Debug.Log("[DEBUG-VIDEO] 视频开始播放");
    }

    IEnumerator VideoAutoEnd()
    {
        yield return new WaitForSeconds(48f);
        if (state != State.Video) yield break;
        Debug.Log("[DEBUG-VIDEO] 视频超时结束");
        EndVideoToDialogue();
    }

    void EndVideoToDialogue()
    {
        if (state != State.Video) return;
        if (videoPlayer != null) videoPlayer.Stop();
        if (introVideoImage != null) introVideoImage.enabled = false;
        Debug.Log("[DEBUG-SKIP] 视频停止，启动对话");
        StartDialogue();
    }

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

    void HandlePrimary()
    {
        Debug.Log($"[DEBUG-HANDLE] 当前状态:{state}");
        if (state == State.Video)
        {
            // ★ 按 1 跳过视频 → 直接进入对话
            videoPlayer?.Stop();
            EndVideoToDialogue();
        }
        else if (state == State.Dialogue)
        {
            // ★ 按 1 跳过对话 → 进入 MainScene
            Debug.Log("[DEBUG-HANDLE] 跳过对话，进入 MainScene");
            EndFlow();
        }
    }

    public void OnDialogueFinished()
    {
        Debug.Log("[DEBUG-DIALOGUE-END] 对话结束，跳转场景");
        EndFlow();
    }

    void EndFlow()
    {
        state = State.End;
        if (dialogueBGM != null) dialogueBGM.Stop();
        if (dialogueManager != null)
        {
            dialogueManager.panel?.SetActive(false);
            dialogueManager.gameObject.SetActive(false);
        }
        SceneManager.LoadScene(nextScene);
    }
}