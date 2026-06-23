using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

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
        
        textMaterial = promptText.fontMaterial;
        originalColor = promptText.color;
        
        SetTextAlpha(0f);
        SetGlow(0f);
        
        StartCoroutine(FadeInAndPulse());
        
        Debug.Log("中转场景已加载，等待玩家拨弦...");
    }
    
    IEnumerator FadeInAndPulse()
    {
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
        
        while (!hasStarted)
        {
            float pulse = Mathf.Sin(Time.time * pulseSpeed) * 0.5f + 0.5f;
            float glow = Mathf.Lerp(0.5f, glowIntensity, pulse);
            SetGlow(glow);
            
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
    
    // ★ 任意弦（1/2/3）触发进入 TitleScreen
    public void OnGuitarInput(int stringId)
    {
        if (!hasStarted)
        {
            Debug.Log($"[Transition] 按弦 {stringId}，进入 TitleScreen");
            OnPlayerReady();
        }
    }
    
    void OnPlayerReady()
    {
        if (hasStarted) return;
        hasStarted = true;
        
        StartCoroutine(FadeOutAndLoad());
    }
    
    IEnumerator FadeOutAndLoad()
    {
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
    
    void OnEnable()
    {
        GuitarInputManager.OnStringPlayed += OnGuitarInput;
    }
    
    void OnDisable()
    {
        GuitarInputManager.OnStringPlayed -= OnGuitarInput;
    }
}