// Assets/Scripts/Gameplay/GuitarInputHandler.cs
using UnityEngine;

public class GuitarInputHandler : MonoBehaviour
{
    [Header("扫弦设置")]
    public float captureRange = 2f;
    
    [Header("弦颜色映射")]
    public Color chord1Color = Color.yellow;
    public Color chord2Color = Color.green;
    public Color chord3Color = new Color(0.5f, 0.2f, 1f);
    
    private NoteSpawner noteSpawner;
    
    void Start()
    {
        Debug.Log("🎸 GuitarInputHandler Start() 被调用");
        
        noteSpawner = FindObjectOfType<NoteSpawner>();
        
        if (noteSpawner != null)
        {
            Debug.Log("✅ 找到 NoteSpawner，订阅事件");
            noteSpawner.OnNoteCaptured += OnNoteCaptured;
            noteSpawner.OnNoteMissed += OnNoteMissed;
        }
        else
        {
            Debug.LogError("❌ 找不到 NoteSpawner！请确保场景中有 NoteSpawner 组件");
        }
        
        // 订阅键盘输入
        KeyboardGuitarSimulator.OnStringPlayedStatic += OnGuitarInput;
        Debug.Log("✅ 已订阅键盘输入事件");
    }
    
    void OnGuitarInput(int chordId)
    {
        Debug.Log($"🎸 扫弦！弦 {chordId}");
        Color targetColor = GetColorFromChord(chordId);
        CaptureNotesInRange(targetColor);
    }
    
    Color GetColorFromChord(int chordId)
    {
        switch (chordId)
        {
            case 1: return chord1Color;
            case 2: return chord2Color;
            case 3: return chord3Color;
            default: return Color.white;
        }
    }
    
    void CaptureNotesInRange(Color targetColor)
    {
        Vector3 judgePos = noteSpawner != null ? 
            noteSpawner.transform.position + Vector3.forward * 0.5f : 
            transform.position + Vector3.forward * 0.5f;
        
        Collider[] hitColliders = Physics.OverlapSphere(judgePos, captureRange);
        
        if (hitColliders.Length == 0)
        {
            Debug.Log("❌ 判定范围内没有检测到任何物体");
            return;
        }
        
        Debug.Log($"🔍 判定范围内检测到 {hitColliders.Length} 个物体");
        
        foreach (var hit in hitColliders)
        {
            Note note = hit.GetComponent<Note>();
            if (note != null)
            {
                Debug.Log($"📝 找到 Note 组件，IsActive: {note.IsActive()}");
                
                if (!note.IsActive())
                {
                    Debug.Log("⏭️ 音符已不活跃，跳过");
                    continue;
                }
                
                Renderer renderer = hit.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Color noteColor = renderer.material.color;
                    Debug.Log($"🎨 音符颜色: {noteColor}, 目标颜色: {targetColor}");
                    
                    if (IsColorMatch(noteColor, targetColor))
                    {
                        Debug.Log("✅ 颜色匹配！捕获音符！");
                        note.Capture();
                    }
                    else
                    {
                        Debug.Log("❌ 颜色不匹配，跳过");
                    }
                }
            }
        }
    }
    
    bool IsColorMatch(Color a, Color b)
    {
        return Mathf.Abs(a.r - b.r) < 0.2f &&
               Mathf.Abs(a.g - b.g) < 0.2f &&
               Mathf.Abs(a.b - b.b) < 0.2f;
    }
    
    void OnNoteCaptured(NoteData data)
    {
        Debug.Log($"🎯 GuitarInputHandler.OnNoteCaptured 被调用！颜色: {data.color}");
        
        // ★ 根据颜色计算分数
        int scoreValue = 0;
        switch (data.color)
        {
            case "Yellow": scoreValue = 30; break;
            case "Green": scoreValue = 50; break;
            case "BluePurple": scoreValue = 100; break;
            default: scoreValue = 10; break;
        }
        
        Debug.Log($"📊 准备加分：{data.color} +{scoreValue}分");
        
        // ★ 调用 ScoreManager
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(data.color, scoreValue);
            Debug.Log($"✅ ScoreManager.AddScore 已调用，当前总分: {ScoreManager.Instance.totalScore}");
        }
        else
        {
            Debug.LogError("❌ ScoreManager.Instance 为空！请确保场景中有 ScoreManager 对象");
        }
    }
    
    void OnNoteMissed(NoteData data)
    {
        Debug.Log($"💨 [Miss] {data.color} 音符");
    }
    
    void OnDestroy()
    {
        Debug.Log("🎸 GuitarInputHandler 被销毁，取消订阅");
        KeyboardGuitarSimulator.OnStringPlayedStatic -= OnGuitarInput;
        if (noteSpawner != null)
        {
            noteSpawner.OnNoteCaptured -= OnNoteCaptured;
            noteSpawner.OnNoteMissed -= OnNoteMissed;
        }
    }
}