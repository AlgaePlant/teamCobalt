// Assets/Scripts/Gameplay/GuitarInputManager.cs
using System;
using UnityEngine;
using UnityEngine.SceneManagement;  // ★ 添加这一行

public class GuitarInputManager : MonoBehaviour
{
    public static GuitarInputManager Instance;
    
    public static event Action<int> OnStringPlayed;
    
    [Header("快速跳过设置")]
    public int skipCount = 10;
    public float skipTimeWindow = 2f;
    public string skipSceneName = "EndingScene";
    
    private int chord1Count = 0;
    private float lastChord1Time = 0f;
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
    }
    
    void OnEnable()
    {
        KeyboardGuitarSimulator.OnStringPlayedStatic += HandleInput;
        GuitarBluetoothInput.OnStringPlayed += HandleInput;
        // ★ 每次对象激活时重置（包括场景切换后）
        ResetSkip();
        
    }
    
    void OnDisable()
    {
        KeyboardGuitarSimulator.OnStringPlayedStatic -= HandleInput;
        GuitarBluetoothInput.OnStringPlayed -= HandleInput;
    }
    
    private void HandleInput(int stringId)
    {
        Debug.Log($"🎸 GuitarInputManager 收到弦 {stringId}");
        
        OnStringPlayed?.Invoke(stringId);
        
        // ★ 只在 MainScene 中检测快速跳过（防止和 EndingScene 冲突）
        if (stringId == 1 && !skipTriggered)
        {
            string currentScene = SceneManager.GetActiveScene().name;
            // 只在 MainScene 或 GameScene 中触发跳过
            if (currentScene == "MainScene" || currentScene == "GameScene")
            {
                CheckSkipCondition();
            }
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
        
        Debug.Log($"🔢 弦1 快速点击计数: {chord1Count}/{skipCount}");
        
        if (chord1Count >= skipCount)
        {
            TriggerSkip();
        }
    }
    
    void TriggerSkip()
    {
        if (skipTriggered) return;
        skipTriggered = true;
        
        Debug.Log($"⏭️ 快速跳过已触发！连按 {skipCount} 次弦1，跳转到 {skipSceneName}");
        SceneManager.LoadScene(skipSceneName);
    }
    
    public void ResetSkip()
    {
        chord1Count = 0;
        lastChord1Time = 0f;
        skipTriggered = false;
        Debug.Log("🔄 快速跳过状态已重置");
    }
}