using UnityEngine;
using System.Collections.Generic;

public class DebrisSpawner : MonoBehaviour
{
    [Header("陨石预制体")]
    public GameObject debrisPrefab;
    
    [Header("生成设置")]
    public int debrisCount = 15;
    public float minRadius = 4f;
    public float maxRadius = 12f;
    public float minHeight = -3f;
    public float maxHeight = 4f;
    
    [Header("跟随玩家")]
    public Transform playerTransform;
    public bool followPlayer = true;
    
    private List<GameObject> debrisList = new List<GameObject>();
    private Vector3 lastPlayerPosition;
    private bool hasSpawned = false;
    
    void Start()
    {
        Debug.Log("=== DebrisSpawner Start() 被执行了 ===");  // 加这行
        if (debrisPrefab == null)
        {
            Debug.LogError("DebrisSpawner: 没有指定陨石预制体！");
            return;
        }
        // 自动查找玩家：直接用 Camera.main
        if (playerTransform == null)
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                // 如果相机有父对象（XR Origin），用父对象；否则用相机本身
                if (cam.transform.parent != null)
                    playerTransform = cam.transform.parent;
                else
                    playerTransform = cam.transform;
            }
        }
        
        if (playerTransform != null)
            lastPlayerPosition = playerTransform.position;
        
        SpawnDebris();
        hasSpawned = true;
        Debug.Log($"DebrisSpawner: 尝试生成 {debrisCount} 个陨石");
    }
    
    void SpawnDebris()
        {
            foreach (var debris in debrisList)
            {
                if (debris != null)
                    Destroy(debris);
            }
            debrisList.Clear();

            if (debrisPrefab == null)
            {
                Debug.LogError("DebrisSpawner: 没有指定陨石预制体！");
                return;
            }

            Vector3 center = playerTransform != null ? playerTransform.position : Vector3.zero;
            Debug.Log($"陨石生成中心: {center}");

            for (int i = 0; i < debrisCount; i++)
            {
                // 随机角度（0-360度）
                float angle = Random.Range(0f, Mathf.PI * 2f);

                // 随机半径
                float radius = Random.Range(minRadius, maxRadius);

                // 计算位置
                float x = Mathf.Cos(angle) * radius;
                float z = Mathf.Sin(angle) * radius;
                float y = Random.Range(minHeight, maxHeight);

                Vector3 position = center + new Vector3(x, y, z);

                // ★ 随机缩放：0.3 到 2.5 倍（大小不一）
                float scale = Random.Range(3f, 40f);

                // ★ 随机旋转：完全随机朝向
                Quaternion randomRotation = Random.rotation;

                GameObject debris = Instantiate(debrisPrefab, position, randomRotation);
                debris.transform.localScale = Vector3.one * scale;
                debris.name = $"Debris_{i}";

                FloatingDebris floating = debris.GetComponent<FloatingDebris>();
                if (floating == null)
                    floating = debris.AddComponent<FloatingDebris>();

                floating.floatSpeed = Random.Range(0.2f, 0.5f);
                floating.floatAmount = Random.Range(0.1f, 0.4f);
                floating.rotationSpeed = Random.Range(5f, 20f);
                floating.driftSpeed = Random.Range(0.02f, 0.08f);

                debrisList.Add(debris);
            }

            Debug.Log($"成功生成 {debrisList.Count} 个陨石");
        }
    
    void Update()
    {
        if (!hasSpawned)
        {
            SpawnDebris();
            hasSpawned = true;
        }
        
        if (debrisList.Count < debrisCount * 0.5f)
        {
            Debug.Log("陨石数量不足，重新生成");
            SpawnDebris();
        }
        
        if (followPlayer && playerTransform != null)
        {
            Vector3 delta = playerTransform.position - lastPlayerPosition;
            if (delta.magnitude > 0.01f)
            {
                foreach (var debris in debrisList)
                {
                    if (debris != null)
                        debris.transform.position += delta;
                }
                lastPlayerPosition = playerTransform.position;
            }
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Vector3 center = playerTransform != null ? playerTransform.position : Vector3.zero;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, minRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(center, maxRadius);
    }
}