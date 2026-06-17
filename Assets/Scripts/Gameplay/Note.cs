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
    public float maxScale = 1f;

    [Header("判定增强")]
    public float activeEmission = 2f;

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
    private bool enhanced = false;


    private Renderer[] renderers;
    private Material[] materials;
    private Color[] originalColors;


    public System.Action<NoteData> OnCaptured;
    public System.Action<NoteData> OnMissed;



    public void Initialize(NoteData noteData, Vector3 target, float duration)
    {
        data = noteData;

        targetPosition = target;

        passPosition =
            targetPosition -
            Vector3.forward * 15f;


        travelDuration = duration;

        spawnTime = Time.time;

        startPosition = transform.position;


        renderers =
            GetComponentsInChildren<Renderer>();


        materials =
            new Material[renderers.Length];

        originalColors =
            new Color[renderers.Length];


        for (int i = 0; i < renderers.Length; i++)
        {
            materials[i] = renderers[i].material;
            originalColors[i] = materials[i].color;
        }


        transform.localScale =
            Vector3.one * minScale;


        HitManager.Register(this);
    }



    void Update()
    {
        if (!isActive || isCaptured)
            return;


        float progress =
            (Time.time - spawnTime)
            /
            travelDuration;


        progress = Mathf.Clamp01(progress);


        transform.position =
            Vector3.Lerp(
                startPosition,
                passPosition,
                progress
            );


        float scale =
            Mathf.Lerp(
                minScale,
                maxScale,
                progress
            );


        transform.localScale =
            Vector3.one * scale;


        UpdateEnhance();



        if (progress >= 1f)
            MissAndDestroy();
    }



    void UpdateEnhance()
    {
        bool ready =
            GetDistanceToJudgeLine()
            <= HitManager.hitRange;


        if (ready && !enhanced)
        {
            enhanced = true;
            ApplyEnhance();
        }


        if (!ready && enhanced)
        {
            enhanced = false;
            RestoreColor();
        }
    }



    void ApplyEnhance()
    {
        Color target =
            GetStrongColor();


        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].color =
                Color.Lerp(
                    originalColors[i],
                    target,
                    0.35f
                );


            materials[i].EnableKeyword("_EMISSION");


            materials[i].SetColor(
                "_EmissionColor",
                target * activeEmission
            );
        }
    }



    void RestoreColor()
    {
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].color =
                originalColors[i];


            materials[i].SetColor(
                "_EmissionColor",
                Color.black
            );
        }
    }



    Color GetStrongColor()
    {
        switch (noteColor)
        {
            case NoteColor.Yellow:
                return new Color(1f, 0.9f, 0f);


            case NoteColor.Green:
                return new Color(0f, 1f, 0.1f);


            case NoteColor.BluePurple:
                return new Color(0.1f, 0.4f, 1f);
        }


        return Color.white;
    }



    public void Capture()
    {
        if (!isActive)
            return;


        isActive = false;
        isCaptured = true;


        JudgeVisual judge =
            FindObjectOfType<JudgeVisual>();


        if (judge != null)
            judge.PlayHit();


        OnCaptured?.Invoke(data);


        PlayEffect(hitEffectPrefab);


        Destroy(gameObject);
    }



    public bool IsActive()
    {
        return isActive && !isCaptured;
    }


    public bool IsInHitRange()
    {
        return GetDistanceToJudgeLine()
            <= HitManager.hitRange;
    }



    public float GetDistanceToJudgeLine()
    {
        return Mathf.Abs(
            transform.position.z -
            targetPosition.z
        );
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


        GameObject obj =
            Instantiate(
                prefab,
                transform.position,
                Quaternion.identity
            );


        Destroy(obj, 4f);
    }



    void OnDestroy()
    {
        HitManager.Unregister(this);
    }
}