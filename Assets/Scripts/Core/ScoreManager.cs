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
        public int totalCount;      // 总数量（从谱面读取）
        public int capturedCount;   // 已捕获数量
        
        public float Percentage => totalCount > 0 ? (float)capturedCount / totalCount * 100f : 0f;
    }
    
    public List<ColorProgress> colorProgresses = new List<ColorProgress>();
    public int totalScore { get; private set; }
    
    void Awake()
    {
        Instance = this;
    }
    
    public void SetTotalCounts(Dictionary<string, int> totals)
    {
        foreach (var progress in colorProgresses)
        {
            if (totals.ContainsKey(progress.colorName))
                progress.totalCount = totals[progress.colorName];
        }
    }
    
    public void AddScore(string colorName, int scoreValue)
    {
        totalScore += scoreValue;
        
        var progress = colorProgresses.Find(p => p.colorName == colorName);
        if (progress != null)
            progress.capturedCount++;
        
        Debug.Log($"捕获 {colorName}！+{scoreValue}分，总分：{totalScore}");
    }
    
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
}