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

    [Header("判定范围发光")]
    public float normalGlow = 0.2f;
    public float readyGlow = 2f;

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


    private Renderer[] renderers;
    private Material[] materials;
    private Color[] originalColors;


    private bool glowing = false;


    public System.Action<NoteData> OnCaptured;
    public System.Action<NoteData> OnMissed;



    public void Initialize(NoteData noteData, Vector3 target, float duration)
    {
        data = noteData;

        targetPosition = target;

        passPosition =
            targetPosition -
            Vector3.forward * 5f;


        travelDuration = duration;

        spawnTime = Time.time;

        startPosition = transform.position;



        // 找所有子物体Renderer
        renderers =
            GetComponentsInChildren<Renderer>();


        materials =
            new Material[renderers.Length];

        originalColors =
            new Color[renderers.Length];


        for (int i = 0; i < renderers.Length; i++)
        {
            materials[i] =
                renderers[i].material;

            originalColors[i] =
                materials[i].color;
        }


        transform.localScale =
            Vector3.one * minScale;


        SetGlow(normalGlow);


        HitManager.Register(this);
    }




    void Update()
    {
        if (!isActive || isCaptured)
            return;

        float progress = (Time.time - spawnTime) / travelDuration;
        progress = Mathf.Clamp01(progress);

        transform.position = Vector3.Lerp(startPosition, passPosition, progress);

        float scale = Mathf.Lerp(minScale, maxScale, progress);
        transform.localScale = Vector3.one * scale;

        // ★ 恢复旋转
        transform.Rotate(Vector3.up, 30f * Time.deltaTime);
        transform.Rotate(Vector3.right, 20f * Time.deltaTime);

        UpdateReadyGlow();

        if (progress >= 1f)
        {
            MissAndDestroy();
        }
    }


    void UpdateReadyGlow()
    {
        if (IsInHitRange() && !glowing)
        {
            glowing = true;
            SetGlow(readyGlow);
        }
        else if (!IsInHitRange() && glowing)
        {
            glowing = false;
            SetGlow(normalGlow);
        }
    }

    public void Capture()
    {
        if (!isActive)
            return;


        isActive = false;
        isCaptured = true;


        if (data != null)
        {
            string colorName = data.color;


            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(
                    colorName,
                    scoreValue
                );
            }
            else
            {
                Debug.LogWarning(
                    "ScoreManager 不存在"
                );
            }
        }


        // 保留事件，给以后扩展
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
        return Mathf.Abs(
            transform.position.z - 15f
        );
    }


    public bool IsInHitRange()
    {
        return GetDistanceToJudgeLine()
            <= HitManager.hitRange;
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

        // ★ 强制所有特效都变为 0.5 倍速（不管命中还是未命中）
        ParticleSystem[] systems = obj.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in systems)
        {
            var main = ps.main;
            main.simulationSpeed = 0.25f;
        }

        Destroy(obj, 4f);
    }





    void SetGlow(float intensity)
    {
        if (materials == null)
            return;

        for (int i = 0; i < materials.Length; i++)
        {
            Material mat = materials[i];

            mat.EnableKeyword("_EMISSION");

            // ★ 用白色发光，而不是用 originalColors[i]
            Color emission = Color.white * intensity;
            mat.SetColor("_EmissionColor", emission);

            // ★ 不改变基础颜色，保持原来的颜色
            // mat.color = originalColors[i];  // 注释掉这行
        }
    }





    void OnDestroy()
    {
        HitManager.Unregister(this);
    }
}