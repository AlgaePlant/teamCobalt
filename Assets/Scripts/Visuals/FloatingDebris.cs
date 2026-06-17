// Assets/Scripts/Visuals/FloatingDebris.cs
using UnityEngine;

public class FloatingDebris : MonoBehaviour
{
    [Header("漂浮参数")]
    public float floatSpeed = 0.3f;
    public float floatAmount = 0.2f;
    public float rotationSpeed = 10f;
    public float driftSpeed = 0.05f;
    
    private Vector3 startPosition;
    private float randomOffset;
    private float orbitAngle;
    private float orbitRadius;
    
    void Start()
    {
        startPosition = transform.position;
        randomOffset = Random.Range(0f, Mathf.PI * 2f);
        
        // 计算轨道参数（用于缓慢公转）
        Vector3 horizontalPos = new Vector3(startPosition.x, 0, startPosition.z);
        orbitRadius = horizontalPos.magnitude;
        orbitAngle = Mathf.Atan2(startPosition.z, startPosition.x);
        
        // 随机初始旋转
        transform.rotation = Random.rotation;
    }
    
    void Update()
    {
        // 1. 上下漂浮（正弦波）
        float newY = startPosition.y + Mathf.Sin((Time.time + randomOffset) * floatSpeed) * floatAmount;
        
        // 2. 缓慢水平公转（绕着 Y 轴旋转）
        orbitAngle += driftSpeed * Time.deltaTime;
        float x = Mathf.Cos(orbitAngle) * orbitRadius;
        float z = Mathf.Sin(orbitAngle) * orbitRadius;
        
        // 3. 位置更新
        transform.position = new Vector3(x, newY, z);
        
        // 4. 缓慢自转
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        transform.Rotate(Vector3.right, rotationSpeed * 0.3f * Time.deltaTime);
    }
}