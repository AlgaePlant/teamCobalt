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
        // 只允许视频/对话流程内部处理
        if (state != State.Video &&
            state != State.Dialogue)
            return;


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

        if (introVideoImage != null)
            introVideoImage.enabled = true;

        if (dialogueManager != null)
            dialogueManager.panel?.SetActive(false);

        if (videoPlayer != null)
            videoPlayer.Play();

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
        if (state != State.Video)
            return;

        if (videoPlayer != null)
            videoPlayer.Stop();

        if (introVideoImage != null)
            introVideoImage.enabled = false;

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



        if (dialogueManager != null)
        {
            if (dialogueManager.panel != null)
                dialogueManager.panel.SetActive(false);


            dialogueManager.gameObject.SetActive(false);
        }



        SceneManager.LoadScene(nextScene);
    }
}