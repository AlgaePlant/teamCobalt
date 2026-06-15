// Assets/Scripts/Core/UIManager.cs
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    [Header("左侧进度条")]
    public TextMeshProUGUI yellowProgressText;
    public TextMeshProUGUI greenProgressText;
    public TextMeshProUGUI blueProgressText;
    
    [Header("右上角总分")]
    public TextMeshProUGUI totalScoreText;
    
    void Awake()
    {
        Instance = this;
    }
    
    public void UpdateProgress(string colorName)
    {
        // 由 ScoreManager 提供数据，这里刷新显示
        ScoreManager sm = FindObjectOfType<ScoreManager>();
        if (sm == null) return;
        
        // 找到对应颜色的进度
        var progress = sm.colorProgresses.Find(p => p.colorName == colorName);
        if (progress != null)
        {
            switch (colorName)
            {
                case "Yellow":
                    if (yellowProgressText != null)
                        yellowProgressText.text = $"{progress.Percentage:F0}%";
                    break;
                case "Green":
                    if (greenProgressText != null)
                        greenProgressText.text = $"{progress.Percentage:F0}%";
                    break;
                case "BluePurple":
                    if (blueProgressText != null)
                        blueProgressText.text = $"{progress.Percentage:F0}%";
                    break;
            }
        }
        
        // 更新总分显示
        if (totalScoreText != null)
            totalScoreText.text = $"总分: {sm.totalScore}";
    }
}