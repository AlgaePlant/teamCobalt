// Assets/Scripts/Gameplay/GuitarInputHandler.cs
using UnityEngine;

public class GuitarInputHandler : MonoBehaviour
{
    [Header("扫弦检测范围")]
    public float captureRange = 5f;
    
    [Header("风刃特效")]
    public GameObject windBladePrefab;
    public Transform bladeSpawnPoint;
    
    [Header("弦颜色映射")]
    public Color chord1Color = Color.yellow;
    public Color chord2Color = Color.green;
    public Color chord3Color = Color.blue;
    
    void Start()
    {
        // 使用静态事件订阅
        KeyboardGuitarSimulator.OnStringPlayedStatic += OnGuitarInput;
    }
    
    void OnGuitarInput(int chordId)
    {
        Debug.Log($"吉他输入收到: 弦 {chordId}");
        
        // 1. 发射风刃特效
        SpawnWindBlade(chordId);
        
        // 2. 检测范围内的粒子并捕获
        Color targetColor = GetColorFromChord(chordId);
        CaptureParticlesInRange(targetColor);
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
    
    void SpawnWindBlade(int chordId)
    {
        if (windBladePrefab != null && bladeSpawnPoint != null)
        {
            GameObject blade = Instantiate(windBladePrefab, bladeSpawnPoint.position, Quaternion.identity);
            // 风刃向前飞行
            Rigidbody rb = blade.GetComponent<Rigidbody>();
            if (rb != null)
                rb.AddForce(transform.forward * 20f, ForceMode.Impulse);
            
            // 设置颜色
            Renderer renderer = blade.GetComponent<Renderer>();
            if (renderer != null)
                renderer.material.color = GetColorFromChord(chordId);
            
            Destroy(blade, 1f);
        }
    }
    
    void CaptureParticlesInRange(Color targetColor)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, captureRange);
        
        foreach (var hit in hitColliders)
        {
            ParticleMovement particle = hit.GetComponent<ParticleMovement>();
            if (particle != null)
            {
                if (IsColorMatch(hit.gameObject, targetColor))
                {
                    particle.Capture();
                }
            }
        }
    }
    
    bool IsColorMatch(GameObject obj, Color targetColor)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            Color objColor = renderer.material.color;
            return Mathf.Abs(objColor.r - targetColor.r) < 0.1f &&
                   Mathf.Abs(objColor.g - targetColor.g) < 0.1f &&
                   Mathf.Abs(objColor.b - targetColor.b) < 0.1f;
        }
        return false;
    }
    
    void OnDestroy()
    {
        // 取消订阅
        KeyboardGuitarSimulator.OnStringPlayedStatic -= OnGuitarInput;
    }
}