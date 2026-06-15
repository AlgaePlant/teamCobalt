// Assets/Scripts/Gameplay/NoteSpawner.cs
using System.Collections.Generic;
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    [Header("谱面数据")]
    public TextAsset beatmapFile;
    
    [Header("粒子预制体")]
    public GameObject yellowParticlePrefab;
    public GameObject greenParticlePrefab;
    public GameObject bluePurpleParticlePrefab;
    
    [Header("生成设置")]
    public float spawnRadiusX = 3f;
    public float spawnRadiusY = 2f;
    public float spawnDistanceZ = 10f;
    
    private List<ParticleData> notes = new List<ParticleData>();
    private AudioSource musicSource;
    
    void Start()
    {
        LoadBeatmap();
        musicSource = GetComponent<AudioSource>();
        
        // 通知 ScoreManager 总数量
        var scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager != null)
        {
            var totals = CountParticlesByColor();
            scoreManager.SetTotalCounts(totals);
        }
    }
    
    void LoadBeatmap()
    {
        if (beatmapFile == null) return;
        
        BeatmapData beatmap = JsonUtility.FromJson<BeatmapData>(beatmapFile.text);
        
        foreach (var note in beatmap.notes)
        {
            ParticleData particle = new ParticleData();
            particle.color = ParticleData.StringToColor(note.color);
            particle.spawnTime = note.beatTime;
            particle.startPosition = new Vector3(
                note.xOffset != 0 ? note.xOffset : Random.Range(-spawnRadiusX, spawnRadiusX),
                note.yOffset != 0 ? note.yOffset : Random.Range(-spawnRadiusY, spawnRadiusY),
                spawnDistanceZ
            );
            
            notes.Add(particle);
        }
        
        Debug.Log($"加载谱面完成，共 {notes.Count} 个粒子");
    }
    
    Dictionary<string, int> CountParticlesByColor()
    {
        var dict = new Dictionary<string, int>();
        dict["Yellow"] = 0;
        dict["Green"] = 0;
        dict["BluePurple"] = 0;
        
        foreach (var note in notes)
        {
            string colorName = note.ColorToString();
            if (dict.ContainsKey(colorName))
                dict[colorName]++;
        }
        
        return dict;
    }
    
    void Update()
    {
        if (musicSource == null || !musicSource.isPlaying) return;
        
        float currentTime = musicSource.time;
        
        List<ParticleData> toRemove = new List<ParticleData>();
        
        foreach (var note in notes)
        {
            if (!note.isCaptured && Mathf.Abs(currentTime - note.spawnTime) < 0.05f)
            {
                SpawnParticle(note);
                note.isCaptured = true;
                toRemove.Add(note);
            }
        }
        
        // 可选：清理已生成的引用
        foreach (var note in toRemove)
        {
            // 保持引用以便统计，但不再生成
        }
    }
    
    void SpawnParticle(ParticleData data)
    {
        GameObject prefab = null;
        switch (data.color)
        {
            case ParticleData.ParticleColor.Yellow:
                prefab = yellowParticlePrefab;
                break;
            case ParticleData.ParticleColor.Green:
                prefab = greenParticlePrefab;
                break;
            case ParticleData.ParticleColor.BluePurple:
                prefab = bluePurpleParticlePrefab;
                break;
        }
        
        if (prefab != null)
        {
            GameObject particle = Instantiate(prefab, data.startPosition, Quaternion.identity);
            ParticleMovement movement = particle.GetComponent<ParticleMovement>();
            if (movement == null)
                movement = particle.AddComponent<ParticleMovement>();
            movement.Initialize(data, this);
        }
    }
    
    public void OnParticleCaptured(ParticleData data)
    {
        // 通知 ScoreManager 加分
        ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.AddScore(data.ColorToString(), data.ScoreValue);
        }
        
        // 更新 UI（直接查找 UIManager）
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            uiManager.UpdateProgress(data.ColorToString());
        }
    }
}

[System.Serializable]
public class BeatmapData
{
    public List<NoteData> notes;
}

[System.Serializable]
public class NoteData
{
    public string color;
    public float beatTime;
    public float xOffset;
    public float yOffset;
}