// Assets/Scripts/Core/ScoreManager.cs
using UnityEngine;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    
    [System.Serializable]
    public class ColorProgress
    {
        public string colorName;
        public int totalCount;
        public int capturedCount;
        
        // 百分比（只读）
        public float Percentage => totalCount > 0 ? (float)capturedCount / totalCount * 100f : 0f;
        
        // 显示文本
        public string ProgressText => $"{capturedCount}/{totalCount} ({Percentage:F0}%)";
    }
    
    [Header("进度数据")]
    public List<ColorProgress> colorProgresses = new List<ColorProgress>();
    
    [Header("Total Score")]
    public int totalScore = 0;  // ✅ 改成字段，可以用 [Header]
    
    // 事件：当任何进度更新时触发
    public System.Action OnProgressUpdated;
    public System.Action<int> OnTotalScoreChanged;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
{
    // ★ 硬编码固定谱面数量（和 beatmap.json 一致）
    if (colorProgresses.Count == 0)
    {
        colorProgresses.Add(new ColorProgress { colorName = "Yellow", totalCount = 36, capturedCount = 0 });
        colorProgresses.Add(new ColorProgress { colorName = "Green", totalCount = 18, capturedCount = 0 });
        colorProgresses.Add(new ColorProgress { colorName = "BluePurple", totalCount = 8, capturedCount = 0 });
    }
    
    // ★ 强制触发一次 UI 刷新
    OnProgressUpdated?.Invoke();
}
    
    /// <summary>
    /// 从谱面设置总数量
    /// </summary>
    public void SetTotalCounts(Dictionary<string, int> totals)
    {
        foreach (var progress in colorProgresses)
        {
            if (totals.ContainsKey(progress.colorName))
            {
                progress.totalCount = totals[progress.colorName];
                progress.capturedCount = 0; // 重置已捕获数量
            }
        }
        OnProgressUpdated?.Invoke();
        Debug.Log($"✅ 设置谱面总数完成");
    }
    
    /// <summary>
    /// 添加分数（捕获元素时调用）
    /// </summary>
    public void AddScore(string colorName, int scoreValue)
    {
        totalScore += scoreValue;
        
        var progress = colorProgresses.Find(p => p.colorName == colorName);
        if (progress != null)
        {
            progress.capturedCount++;
            Debug.Log($"🎯 捕获 {colorName}！+{scoreValue}分，{progress.ProgressText}");
        }
        else
        {
            Debug.LogWarning($"⚠️ 未找到颜色：{colorName}");
        }
        
        // 触发事件
        OnProgressUpdated?.Invoke();
        OnTotalScoreChanged?.Invoke(totalScore);

        
    }
    
    /// <summary>
    /// 获取某种颜色的进度数据
    /// </summary>
    public ColorProgress GetProgress(string colorName)
    {
        return colorProgresses.Find(p => p.colorName == colorName);
    }
    
    /// <summary>
    /// 获取总进度百分比
    /// </summary>
    public float GetOverallPercentage()
    {
        int totalCaptured = 0, totalAll = 0;
        foreach (var p in colorProgresses)
        {
            totalCaptured += p.capturedCount;
            totalAll += p.totalCount;
        }
        return totalAll > 0 ? (float)totalCaptured / totalAll * 100f : 0f;
    }
    
    /// <summary>
    /// 获取总进度文本
    /// </summary>
    public string GetOverallProgressText()
    {
        int totalCaptured = 0, totalAll = 0;
        foreach (var p in colorProgresses)
        {
            totalCaptured += p.capturedCount;
            totalAll += p.totalCount;
        }
        return $"{totalCaptured}/{totalAll} ({GetOverallPercentage():F0}%)";
    }
    
    /// <summary>
    /// 重置所有数据
    /// </summary>
    public void ResetAll()
    {
        totalScore = 0;
        foreach (var p in colorProgresses)
        {
            p.capturedCount = 0;
        }
        OnProgressUpdated?.Invoke();
        OnTotalScoreChanged?.Invoke(0);
        Debug.Log("🔄 数据已重置");
    }
}