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

    public void Begin(GameIntroFlowController f)
    {
        flow = f;

        index = 0;
        finished = false;
        idleTimer = 0f;

        panel?.SetActive(true);
        Show();

        StartCoroutine(AutoNext());
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
            image.sprite = sprites[index];
    }

    void Finish()
    {
        finished = true;
        panel?.SetActive(false);

        flow?.OnDialogueFinished();
    }
}