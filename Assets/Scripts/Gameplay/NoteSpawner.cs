using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    [Header("谱面文件")]
    public TextAsset beatmapFile;
    
    [Header("预制体")]
    public GameObject yellowNotePrefab;
    public GameObject greenNotePrefab;
    public GameObject bluePurpleNotePrefab;
    
    [Header("生成区域设置")]
    public float spawnDistanceZ = 25f;
    public float spawnRadiusX = 3f;
    public float spawnRadiusY = 2f;
    
    [Header("判定线设置")]
    public Transform judgeLine;
    
    [Header("移动设置")]
    public float moveSpeed = 5f;
    public float leadTime = 3f;
    
    private List<NoteData> notes = new List<NoteData>();
    private int nextNoteIndex = 0;
    private AudioSource musicSource;
    private bool hasStarted = false;
    
    public System.Action<NoteData> OnNoteCaptured;
    public System.Action<NoteData> OnNoteMissed;
    
    void Start()
    {
        LoadBeatmap();
        musicSource = GetComponent<AudioSource>();
        
        // ★ 直接开始，没有延迟
        hasStarted = true;
        if (musicSource != null && musicSource.clip != null)
        {
            musicSource.Play();
            Debug.Log($"🎵 音乐开始播放！共 {notes.Count} 个音符");
        }
        else
        {
            Debug.Log("⚠️ 没有音乐文件，使用时间模拟");
            StartCoroutine(SimulateMusicTime());
        }
    }
    
    void LoadBeatmap()
    {
        // 优先使用拖入的 TextAsset
        if (beatmapFile != null)
        {
            BeatmapData beatmap = JsonUtility.FromJson<BeatmapData>(beatmapFile.text);
            if (beatmap != null && beatmap.notes != null && beatmap.notes.Count > 0)
            {
                notes = beatmap.notes;
                Debug.Log($"✅ 从 TextAsset 加载谱面成功！共 {notes.Count} 个音符");
                return;
            }
        }
        
        // 从 StreamingAssets 加载
        string filePath = Application.streamingAssetsPath + "/beatmap.json";
        if (System.IO.File.Exists(filePath))
        {
            string json = System.IO.File.ReadAllText(filePath);
            BeatmapData beatmap = JsonUtility.FromJson<BeatmapData>(json);
            if (beatmap != null && beatmap.notes != null && beatmap.notes.Count > 0)
            {
                notes = beatmap.notes;
                Debug.Log($"✅ 从 StreamingAssets 加载谱面成功！共 {notes.Count} 个音符");
                return;
            }
        }
        
        // 从 Resources 加载
        TextAsset resourceFile = Resources.Load<TextAsset>("beatmap");
        if (resourceFile != null)
        {
            BeatmapData beatmap = JsonUtility.FromJson<BeatmapData>(resourceFile.text);
            if (beatmap != null && beatmap.notes != null && beatmap.notes.Count > 0)
            {
                notes = beatmap.notes;
                Debug.Log($"✅ 从 Resources 加载谱面成功！共 {notes.Count} 个音符");
                return;
            }
        }
        
        Debug.LogWarning("⚠️ 没有找到 beatmap.json，使用默认测试数据");
        CreateTestBeatmap();
    }
    
    void CreateTestBeatmap()
    {
        string[] colors = { "Yellow", "Green", "BluePurple" };
        for (int i = 0; i < 20; i++)
        {
            NoteData note = new NoteData();
            note.color = colors[Random.Range(0, colors.Length)];
            note.beatTime = 2f + i * 0.8f + Random.Range(-0.2f, 0.2f);
            note.xOffset = Random.Range(-2.5f, 2.5f);
            note.yOffset = Random.Range(-1.5f, 1.5f);
            notes.Add(note);
        }
        notes.Sort((a, b) => a.beatTime.CompareTo(b.beatTime));
        Debug.Log($"创建测试谱面，共 {notes.Count} 个音符");
    }
    
    void Update()
    {
        if (!hasStarted) return;
        
        if (musicSource == null || !musicSource.isPlaying)
        {
            if (nextNoteIndex >= notes.Count)
            {
                // 所有音符已生成
            }
            return;
        }
        
        float currentTime = musicSource.time;
        
        while (nextNoteIndex < notes.Count)
        {
            NoteData note = notes[nextNoteIndex];
            float timeToArrival = note.beatTime - currentTime;
            
            if (timeToArrival <= leadTime)
            {
                SpawnNote(note);
                nextNoteIndex++;
            }
            else
            {
                break;
            }
        }
    }
    
    void SpawnNote(NoteData data)
    {
        GameObject prefab = GetPrefabByColor(data.color);
        if (prefab == null) return;
        
        Vector3 targetPosition = GetJudgeLinePosition(data.xOffset, data.yOffset);
        Vector3 spawnPosition = targetPosition + Vector3.forward * spawnDistanceZ;
        
        float angleOffset = Random.Range(-20f, 20f);
        float radiusOffset = Random.Range(0f, 1.5f);
        spawnPosition.x += Mathf.Sin(angleOffset * Mathf.Deg2Rad) * radiusOffset;
        spawnPosition.y += Random.Range(-0.5f, 0.5f);
        
        GameObject noteObj = Instantiate(prefab, spawnPosition, Quaternion.identity);
        
        Note note = noteObj.GetComponent<Note>();
        if (note == null) note = noteObj.AddComponent<Note>();
        
        float travelDuration = spawnDistanceZ / moveSpeed;
        note.Initialize(data, targetPosition, travelDuration);
        
        note.OnCaptured += HandleNoteCaptured;
        note.OnMissed += HandleNoteMissed;
        
        SetNoteColor(noteObj, data.color);
        
        if (noteObj.GetComponent<Collider>() == null)
        {
            SphereCollider collider = noteObj.AddComponent<SphereCollider>();
            collider.radius = 0.3f;
            collider.isTrigger = true;
        }
    }
    
    GameObject GetPrefabByColor(string color)
    {
        switch (color)
        {
            case "Yellow": return yellowNotePrefab;
            case "Green": return greenNotePrefab;
            case "BluePurple": return bluePurpleNotePrefab;
            default: return yellowNotePrefab;
        }
    }
    
    void SetNoteColor(GameObject obj, string color)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer == null) return;
        
        Color targetColor = Color.white;
        switch (color)
        {
            case "Yellow": targetColor = Color.yellow; break;
            case "Green": targetColor = Color.green; break;
            case "BluePurple": targetColor = new Color(0.5f, 0.2f, 1f); break;
        }
        renderer.material.color = targetColor;
    }
    
    Vector3 GetJudgeLinePosition(float x, float y)
    {
        if (judgeLine != null)
            return judgeLine.position + new Vector3(x, y, 0);
        else
            return new Vector3(x, y, 0.5f);
    }
    
    void HandleNoteCaptured(NoteData data)
    {
        OnNoteCaptured?.Invoke(data);
        Debug.Log($"🎯 捕获: {data.color} at {data.beatTime}s");
    }
    
    void HandleNoteMissed(NoteData data)
    {
        OnNoteMissed?.Invoke(data);
        Debug.Log($"💨 错过: {data.color} at {data.beatTime}s");
    }
    
    IEnumerator SimulateMusicTime()
    {
        float time = 0;
        while (time < 120f)
        {
            time += Time.deltaTime;
            yield return null;
        }
    }
    
    public int GetTotalNotes()
    {
        return notes.Count;
    }
}