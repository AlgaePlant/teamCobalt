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
        noteSpawner = FindObjectOfType<NoteSpawner>();
        KeyboardGuitarSimulator.OnStringPlayedStatic += OnGuitarInput;
        
        if (noteSpawner != null)
        {
            noteSpawner.OnNoteCaptured += OnNoteCaptured;
            noteSpawner.OnNoteMissed += OnNoteMissed;
        }
    }
    
    void OnGuitarInput(int chordId)
    {
        Debug.Log($"扫弦！弦 {chordId}");
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
        
        foreach (var hit in hitColliders)
        {
            Note note = hit.GetComponent<Note>();
            if (note != null)
            {
                Renderer renderer = hit.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Color noteColor = renderer.material.color;
                    if (IsColorMatch(noteColor, targetColor))
                    {
                        note.Capture();
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
        ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager != null)
        {
            int scoreValue = data.color == "Yellow" ? 100 : data.color == "Green" ? 50 : 30;
            scoreManager.AddScore(data.color, scoreValue);
            Debug.Log($"[命中] {data.color} +{scoreValue}分");
        }
    }
    
    void OnNoteMissed(NoteData data)
    {
        Debug.Log($"[Miss] {data.color} 音符");
    }
    
    void OnDestroy()
    {
        KeyboardGuitarSimulator.OnStringPlayedStatic -= OnGuitarInput;
        if (noteSpawner != null)
        {
            noteSpawner.OnNoteCaptured -= OnNoteCaptured;
            noteSpawner.OnNoteMissed -= OnNoteMissed;
        }
    }
}