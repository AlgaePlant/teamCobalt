// Assets/Scripts/Sequence/TransitionSceneController.cs
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;

public class TransitionSceneController : MonoBehaviour
{
    [Header("场景设置")]
    public string mainSceneName = "TitleScreen";
    
    [Header("UI 文字")]
    public TextMeshProUGUI promptText;
    
    [Header("文字动画")]
    public float fadeInDuration = 1.5f;
    public float pulseSpeed = 1.2f;
    public float glowIntensity = 2f;
    
    private bool hasStarted = false;
    private Material textMaterial;
    private Color originalColor;
    
    void Start()
    {
        if (promptText == null)
        {
            Debug.LogError("TransitionSceneController: 没有指定提示文字！");
            return;
        }
        
        // 获取文字材质
        textMaterial = promptText.fontMaterial;
        originalColor = promptText.color;
        
        // 初始：完全透明
        SetTextAlpha(0f);
        SetGlow(0f);
        
        // 开始淡入动画
        StartCoroutine(FadeInAndPulse());
        
        Debug.Log("🌉 中转场景已加载，等待玩家拨弦...");
    }
    
    IEnumerator FadeInAndPulse()
    {
        // 1. 淡入
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / fadeInDuration;
            float alpha = Mathf.Lerp(0f, 1f, progress);
            SetTextAlpha(alpha);
            SetGlow(alpha * glowIntensity);
            yield return null;
        }
        
        SetTextAlpha(1f);
        SetGlow(glowIntensity);
        
        // 2. 持续脉冲发光
        while (!hasStarted)
        {
            float pulse = Mathf.Sin(Time.time * pulseSpeed) * 0.5f + 0.5f;
            float glow = Mathf.Lerp(0.5f, glowIntensity, pulse);
            SetGlow(glow);
            
            // 轻微缩放（可选）
            float scale = 1f + Mathf.Sin(Time.time * pulseSpeed) * 0.03f;
            promptText.transform.localScale = Vector3.one * scale;
            
            yield return null;
        }
    }
    
    void SetTextAlpha(float alpha)
    {
        if (promptText == null) return;
        Color c = originalColor;
        c.a = alpha;
        promptText.color = c;
    }
    
    void SetGlow(float intensity)
    {
        if (textMaterial == null) return;
        
        textMaterial.EnableKeyword("_EMISSION");
        Color glowColor = Color.white * intensity;
        textMaterial.SetColor("_EmissionColor", glowColor);
    }
    
    void Update()
    {
        if (hasStarted) return;
        
        // 检测吉他输入（弦1-3任意一根）
        if (KeyboardGuitarSimulator.OnStringPlayedStatic != null)
        {
            // 临时订阅检测（只在 Update 中检测）
            // 但更好的方式是用事件
        }
        
        // 用 InputSystem 检测任意按键（方便测试）
        if (Keyboard.current != null)
        {
            if (Keyboard.current.anyKey.wasPressedThisFrame)
            {
                OnPlayerReady();
            }
        }
    }
    
    // 这个方法会被 GuitarInputManager 调用
    public void OnGuitarInput(int stringId)
    {
        if (!hasStarted)
        {
            OnPlayerReady();
        }
    }
    
    void OnPlayerReady()
    {
        if (hasStarted) return;
        hasStarted = true;
        
        Debug.Log("🎸 玩家拨弦！进入 MainScene");
        
        // 播放点击反馈（可选）
        StartCoroutine(FadeOutAndLoad());
    }
    
    IEnumerator FadeOutAndLoad()
    {
        // 快速淡出
        float elapsed = 0f;
        float duration = 0.5f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            SetTextAlpha(alpha);
            SetGlow(alpha * glowIntensity);
            yield return null;
        }
        
        SceneManager.LoadScene(mainSceneName);
    }
    
    // 在场景中挂载时，订阅吉他输入
    void OnEnable()
    {
        KeyboardGuitarSimulator.OnStringPlayedStatic += OnGuitarInput;
    }
    
    void OnDisable()
    {
        KeyboardGuitarSimulator.OnStringPlayedStatic -= OnGuitarInput;
    }
}