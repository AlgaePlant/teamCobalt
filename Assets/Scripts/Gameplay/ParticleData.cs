// Assets/Scripts/Gameplay/ParticleData.cs
using UnityEngine;

[System.Serializable]
public class ParticleData
{
    public enum ParticleColor { Yellow, Green, BluePurple }
    
    public ParticleColor color;
    public Vector3 startPosition;
    public float spawnTime;
    public float moveSpeed = 5f;
    public bool isCaptured = false;
    
    public int ScoreValue 
    { 
        get 
        {
            switch (color)
            {
                case ParticleColor.Yellow: return 100;
                case ParticleColor.Green: return 50;
                case ParticleColor.BluePurple: return 30;
                default: return 10;
            }
        }
    }
    
    // 从字符串转换
    public static ParticleColor StringToColor(string colorName)
    {
        switch (colorName.ToLower())
        {
            case "yellow": return ParticleColor.Yellow;
            case "green": return ParticleColor.Green;
            case "bluepurple":
            case "blue_purple":
            case "blue":
                return ParticleColor.BluePurple;
            default: return ParticleColor.Yellow;
        }
    }
    
    // 转换为字符串（用于 UI）
    public string ColorToString()
    {
        switch (color)
        {
            case ParticleColor.Yellow: return "Yellow";
            case ParticleColor.Green: return "Green";
            case ParticleColor.BluePurple: return "BluePurple";
            default: return "Unknown";
        }
    }
}