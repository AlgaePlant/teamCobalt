// Assets/Scripts/Utils/BeatmapEditor.cs
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class BeatmapEditor : EditorWindow
{
    private List<NoteData> notes = new List<NoteData>();
    private string beatmapPath = "Assets/StreamingAssets/beatmap.json";
    
    [MenuItem("Tools/谱面编辑器")]
    public static void ShowWindow()
    {
        GetWindow<BeatmapEditor>("谱面编辑器");
    }
    
    void OnGUI()
    {
        GUILayout.Label("谱面编辑器", EditorStyles.boldLabel);
        
        if (GUILayout.Button("加载谱面"))
        {
            LoadBeatmap();
        }
        
        if (GUILayout.Button("保存谱面"))
        {
            SaveBeatmap();
        }
        
        GUILayout.Space(10);
        
        // 显示音符列表
        for (int i = 0; i < notes.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            
            // 颜色下拉选择
            string[] colors = { "Yellow", "Green", "BluePurple" };
            int selectedIndex = System.Array.IndexOf(colors, notes[i].color);
            if (selectedIndex < 0) selectedIndex = 0;
            notes[i].color = colors[EditorGUILayout.Popup(selectedIndex, colors, GUILayout.Width(80))];
            
            notes[i].beatTime = EditorGUILayout.FloatField(notes[i].beatTime, GUILayout.Width(60));
            notes[i].xOffset = EditorGUILayout.FloatField(notes[i].xOffset, GUILayout.Width(60));
            notes[i].yOffset = EditorGUILayout.FloatField(notes[i].yOffset, GUILayout.Width(60));
            
            if (GUILayout.Button("删除", GUILayout.Width(50)))
            {
                notes.RemoveAt(i);
                i--;
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("添加音符"))
        {
            notes.Add(new NoteData { color = "Yellow", beatTime = 0, xOffset = 0, yOffset = 0 });
        }
    }
    
    void LoadBeatmap()
    {
        if (File.Exists(beatmapPath))
        {
            string json = File.ReadAllText(beatmapPath);
            BeatmapData data = JsonUtility.FromJson<BeatmapData>(json);
            notes = data.notes;
            Debug.Log($"加载 {notes.Count} 个音符");
        }
        else
        {
            Debug.LogWarning($"找不到谱面文件: {beatmapPath}");
        }
    }
    
    void SaveBeatmap()
    {
        BeatmapData data = new BeatmapData();
        data.notes = notes;
        
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(beatmapPath, json);
        AssetDatabase.Refresh();
        Debug.Log($"保存 {notes.Count} 个音符到 {beatmapPath}");
    }
}