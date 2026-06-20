// Assets/Scripts/Core/UIManager.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    [Header("三个进度条")]
    public ProgressBarUI yellowProgressBar;
    public ProgressBarUI greenProgressBar;
    public ProgressBarUI bluePurpleProgressBar;
    
    [Header("总分显示")]
    public TextMeshProUGUI totalScoreText;
    public TextMeshProUGUI overallProgressText;
    
    [Header("颜色定义")]
    public Color yellowColor = new Color(1f, 0.84f, 0f);     // #FFD700
    public Color greenColor = new Color(0.2f, 0.8f, 0.2f);   // #33CC33
    public Color bluePurpleColor = new Color(0.5f, 0.2f, 1f); // #8033FF
    
    // UIManager.cs
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        // ★ 在 Awake 中订阅（比 Start 更早）
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnProgressUpdated += RefreshAllUI;
            ScoreManager.Instance.OnTotalScoreChanged += UpdateTotalScore;
        }
    }
    
    void Start()
    {
        // ★ 设置颜色（保持不变）
        if (yellowProgressBar != null) yellowProgressBar.SetColor(yellowColor);
        if (greenProgressBar != null) greenProgressBar.SetColor(greenColor);
        if (bluePurpleProgressBar != null) bluePurpleProgressBar.SetColor(bluePurpleColor);
        
        // ★ 订阅事件
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnProgressUpdated += RefreshAllUI;
            ScoreManager.Instance.OnTotalScoreChanged += UpdateTotalScore;
        }
        
        // ★ 强制刷新 UI（不管数据从哪里来）
        RefreshAllUI();
        UpdateTotalScore(0);
    }
    
    /// <summary>
    /// 刷新所有进度条
    /// </summary>
    void RefreshAllUI()
    {
        var sm = ScoreManager.Instance;
        if (sm == null) return;
        
        // 更新黄色
        var yellow = sm.GetProgress("Yellow");
        if (yellow != null && yellowProgressBar != null)
        {
            yellowProgressBar.UpdateProgress(yellow.capturedCount, yellow.totalCount);
        }
        
        // 更新绿色
        var green = sm.GetProgress("Green");
        if (green != null && greenProgressBar != null)
        {
            greenProgressBar.UpdateProgress(green.capturedCount, green.totalCount);
        }
        
        // 更新蓝紫色
        var bluePurple = sm.GetProgress("BluePurple");
        if (bluePurple != null && bluePurpleProgressBar != null)
        {
            bluePurpleProgressBar.UpdateProgress(bluePurple.capturedCount, bluePurple.totalCount);
        }
        
        // 更新总进度
        if (overallProgressText != null)
        {
            overallProgressText.text = sm.GetOverallProgressText();
        }
    }
    
    /// <summary>
    /// 更新总分
    /// </summary>
    void UpdateTotalScore(int score)
    {
        if (totalScoreText != null)
        {
            totalScoreText.text = $"Total: {score}";
        }
    }
    
    /// <summary>
    /// 手动刷新
    /// </summary>
    public void RefreshUI()
    {
        RefreshAllUI();
        if (ScoreManager.Instance != null)
        {
            UpdateTotalScore(ScoreManager.Instance.totalScore);
        }
    }
    
    void OnDestroy()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnProgressUpdated -= RefreshAllUI;
            ScoreManager.Instance.OnTotalScoreChanged -= UpdateTotalScore;
        }
    }
}