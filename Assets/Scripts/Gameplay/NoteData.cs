// Assets/Scripts/Gameplay/NoteData.cs
using UnityEngine;

[System.Serializable]
public class NoteData
{
    public string color;
    public float beatTime;   // ← 用 beatTime，不是 time
    public float xOffset;    // ← 用 xOffset，不是 x
    public float yOffset;    // ← 用 yOffset，不是 y
}