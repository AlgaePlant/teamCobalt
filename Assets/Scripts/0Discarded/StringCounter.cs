using UnityEngine;

public class StringCounter : MonoBehaviour
{
    public static StringCounter Instance;

    public int pressCount = 0;          // 当前按弦次数
    public int requiredPresses = 5;     // 需要多少次触发 Ending
    public bool endingTriggered = false;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // 重置计数器（在进入 Main Scene 时调用）
    public void ResetCounter()
    {
        pressCount = 0;
        endingTriggered = false;
        Debug.Log("计数器已重置");
    }

    // 增加计数
    public void AddPress()
    {
        if (endingTriggered) return;

        pressCount++;
        Debug.Log("当前按弦次数: " + pressCount);

        if (pressCount >= requiredPresses)
        {
            endingTriggered = true;
            // 触发跳转到 Ending Scene
            UnityEngine.SceneManagement.SceneManager.LoadScene("EndingScene");
        }
    }
}