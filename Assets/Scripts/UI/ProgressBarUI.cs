// Assets/Scripts/UI/ProgressBarUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressBarUI : MonoBehaviour
{
    [Header("UI 引用")]
    public Slider progressSlider;          // 进度条
    public TextMeshProUGUI progressText;   // 显示 "3/10 (30%)"
    public Image iconImage;                // 元素图标
    public Image fillImage;                // 进度条填充颜色
    
    [Header("设置")]
    public string colorName = "Yellow";    // 对应的颜色名称
    
    void Start()
    {
        // 初始显示
        UpdateProgress(0, 36); // 默认值，实际会在 ScoreManager 中更新
    }
    
    /// <summary>
    /// 更新进度
    /// </summary>
    public void UpdateProgress(int current, int total)
    {
        // 更新 Slider
        if (progressSlider != null)
        {
            progressSlider.maxValue = total;
            progressSlider.value = current;
        }
        
        // 更新文本
        if (progressText != null)
        {
            float percentage = total > 0 ? (float)current / total * 100f : 0f;
            progressText.text = $"{current}/{total} ({percentage:F0}%)";
        }
    }
    
    /// <summary>
    /// 设置图标
    /// </summary>
    public void SetIcon(Sprite icon)
    {
        if (iconImage != null)
        {
            iconImage.sprite = icon;
        }
    }
    
    /// <summary>
    /// 设置进度条颜色
    /// </summary>
    public void SetColor(Color color)
    {
        if (fillImage != null)
        {
            fillImage.color = color;
        }
    }
}