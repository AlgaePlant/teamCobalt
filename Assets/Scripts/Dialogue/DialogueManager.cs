using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public GameObject panel;
    public Image image;
    public Sprite[] sprites;

    private int index;
    private bool finished;
    private float idleTimer;
    private GameIntroFlowController flow;
    private Coroutine autoNextCoroutine;

    void Awake()
    {
        if (panel == null)
            panel = gameObject;
        panel?.SetActive(false);
        Debug.Log($"DialogueManager Awake, sprites 数量: {sprites?.Length ?? 0}");
    }

    public void ResetState()
    {
        index = 0;
        finished = false;
        idleTimer = 0f;
        flow = null;

        if (autoNextCoroutine != null)
        {
            StopCoroutine(autoNextCoroutine);
            autoNextCoroutine = null;
        }

        panel?.SetActive(false);
        Debug.Log($"DialogueManager 状态已重置，共 {sprites?.Length ?? 0} 张图片");
    }

    public void Begin(GameIntroFlowController f)
    {
        flow = f;
        index = 0;
        finished = false;
        idleTimer = 0f;

        if (sprites == null || sprites.Length == 0)
        {
            Debug.LogError("DialogueManager: 没有对话图片！请检查 Inspector 中的 sprites 数组");
            Finish();
            return;
        }

        panel?.SetActive(true);
        Show();

        if (autoNextCoroutine != null)
        {
            StopCoroutine(autoNextCoroutine);
            autoNextCoroutine = null;
        }
        autoNextCoroutine = StartCoroutine(AutoNext());
        Debug.Log($"DialogueManager.Begin() 开始，共 {sprites.Length} 张图片");
    }

    void Update()
    {
        if (!finished)
            idleTimer += Time.deltaTime;
    }

    public void NextManual()
    {
        if (finished) return;
        idleTimer = 0f;
        index++;

        if (sprites == null || index >= sprites.Length)
        {
            Finish();
            return;
        }

        Show();
        Debug.Log($"对话 {index + 1}/{sprites.Length}");
    }

    IEnumerator AutoNext()
    {
        while (!finished)
        {
            yield return new WaitForSeconds(3f);
            if (idleTimer >= 3f)
                NextManual();
        }
    }

    void Show()
    {
        if (image != null && sprites != null && index < sprites.Length)
        {
            image.sprite = sprites[index];
            Debug.Log($"显示对话 {index + 1}: {sprites[index].name}");
        }
    }

    void Finish()
    {
        finished = true;
        panel?.SetActive(false);

        if (autoNextCoroutine != null)
        {
            StopCoroutine(autoNextCoroutine);
            autoNextCoroutine = null;
        }

        flow?.OnDialogueFinished();
        Debug.Log("对话结束");
    }

    void OnDisable()
    {
        // 对象隐藏时强制停止自动轮播协程，防止后台偷偷执行
        if (autoNextCoroutine != null)
        {
            StopCoroutine(autoNextCoroutine);
            autoNextCoroutine = null;
        }
    }
}