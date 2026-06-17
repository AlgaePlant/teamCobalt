using UnityEngine;

public enum NoteColor
{
    Yellow,
    Green,
    BluePurple
}

public class Note : MonoBehaviour
{
    [Header("颜色")]
    public NoteColor noteColor;

    [Header("分数")]
    public int scoreValue = 100;

    [Header("大小")]
    public float minScale = 0.15f;
    public float maxScale = 1.0f;

    [Header("特效")]
    public GameObject hitEffectPrefab;
    public GameObject missEffectPrefab;

    private NoteData data;

    private Vector3 targetPosition;

    private Vector3 passPosition;

    private float travelDuration;

    private float spawnTime;

    private Vector3 startPosition;

    private bool isActive = true;

    private bool isCaptured = false;

    private Material material;

    private Renderer noteRenderer;

    public System.Action<NoteData> OnCaptured;
    public System.Action<NoteData> OnMissed;

    public void Initialize(NoteData noteData, Vector3 target, float duration)
    {
        data = noteData;

        targetPosition = target;

        // 穿过玩家后的位置
        passPosition = targetPosition - Vector3.forward * 15f;

        travelDuration = duration;

        spawnTime = Time.time;

        startPosition = transform.position;

        noteRenderer = GetComponent<Renderer>();

        if (noteRenderer != null)
        {
            material = noteRenderer.material;
        }

        transform.localScale = Vector3.one * minScale;

        HitManager.Register(this);
    }

    void Update()
    {
        if (!isActive || isCaptured)
            return;

        float progress = (Time.time - spawnTime) / travelDuration;

        progress = Mathf.Clamp01(progress);

        // 移动
        transform.position = Vector3.Lerp(startPosition, passPosition, progress);

        // 恢复大小变化
        float scale = Mathf.Lerp(minScale, maxScale, progress);

        transform.localScale = Vector3.one * scale;

        // 到达终点
        if (progress >= 1f)
        {
            MissAndDestroy();
        }
    }

    public void Capture()
    {
        if (!isActive || isCaptured)
            return;

        isCaptured = true;

        isActive = false;

        OnCaptured?.Invoke(data);

        PlayEffect(hitEffectPrefab);

        Destroy(gameObject);
    }

    public bool IsActive()
    {
        return isActive && !isCaptured;
    }

    public float GetDistanceToJudgeLine()
    {
        return Mathf.Abs(transform.position.z - targetPosition.z);
    }

    void MissAndDestroy()
    {
        if (!isActive)
            return;

        isActive = false;

        OnMissed?.Invoke(data);

        PlayEffect(missEffectPrefab);

        Destroy(gameObject);
    }

    void PlayEffect(GameObject prefab)
    {
        if (prefab == null)
            return;

        GameObject obj = Instantiate(prefab, transform.position, Quaternion.identity);

        Destroy(obj, 4f);
    }

    void OnDestroy()
    {
        HitManager.Unregister(this);
    }
}