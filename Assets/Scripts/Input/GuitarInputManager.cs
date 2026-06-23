using UnityEngine;
using UnityEngine.SceneManagement;

public class GuitarInputManager : MonoBehaviour
{
    public static GuitarInputManager Instance;
    public static event System.Action<int> OnStringPlayed;

    [Header("MainScene 快速跳转")]
    public string mainSceneName = "MainScene";
    public string skipSceneName = "EndingScene";

    private bool skipTriggered = false;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // ★ 订阅输入事件
        KeyboardGuitarSimulator.OnStringPlayedStatic += HandleInput;
        GuitarBluetoothInput.OnStringPlayed += HandleInput;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetSkip();
        Debug.Log($"[GuitarInputManager] 场景加载: {scene.name}，跳过标记已重置");
    }

    void Update()
    {
        // ★ 键盘 4：MainScene 直接跳转到 EndingScene
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            string currentScene = SceneManager.GetActiveScene().name;
            if (currentScene == mainSceneName)
            {
                Debug.Log($"[跳过] 按 4 键：{currentScene} → {skipSceneName}");
                SceneManager.LoadScene(skipSceneName);
            }
        }
    }

    private void HandleInput(int stringId)
    {
        // ★ 转发给所有订阅者
        OnStringPlayed?.Invoke(stringId);

        string scene = SceneManager.GetActiveScene().name;

        // ★ EndingScene: 按 1 跳过视频
        if (scene == "EndingScene" && stringId == 1 && !skipTriggered)
        {
            TriggerSkip();
        }
    }

    void TriggerSkip()
    {
        if (skipTriggered) return;
        skipTriggered = true;
        Debug.Log($"[跳过] 触发！加载 TransitionScene");
        SceneManager.LoadScene("TransitionScene");
    }

    public void ResetSkip()
    {
        skipTriggered = false;
    }

    void OnDestroy()
    {
        KeyboardGuitarSimulator.OnStringPlayedStatic -= HandleInput;
        GuitarBluetoothInput.OnStringPlayed -= HandleInput;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}