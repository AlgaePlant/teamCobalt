using System;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GuitarInputManager : MonoBehaviour
{
    public static GuitarInputManager Instance;

    public static event Action<int> OnStringPlayed;


    [Header("快速跳过设置")]
    public int skipCount = 10;
    public float skipTimeWindow = 2f;

    public string skipSceneName = "EndingScene";


    private int chord1Count = 0;
    private float lastChord1Time = 0f;

    private bool skipTriggered = false;



    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }


        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        KeyboardGuitarSimulator.OnStringPlayedStatic += HandleInput;
        GuitarBluetoothInput.OnStringPlayed += HandleInput;

        ResetSkip();
    }
    void OnDisable()
    {
        KeyboardGuitarSimulator.OnStringPlayedStatic -= HandleInput;
        GuitarBluetoothInput.OnStringPlayed -= HandleInput;
    }

    private void HandleInput(int stringId)
    {
        Debug.Log($"GuitarInputManager 收到弦 {stringId}");

        OnStringPlayed?.Invoke(stringId);



        string scene =
            SceneManager.GetActiveScene().name;



        // 只有这两个场景允许跳过
        if (scene != "TitleScene" &&
           scene != "EndingScene")
        {
            return;
        }



        if (stringId == 1 &&
           !skipTriggered)
        {
            CheckSkipCondition();
        }
    }


    void CheckSkipCondition()
    {
        float now = Time.time;


        if (now - lastChord1Time > skipTimeWindow)
        {
            chord1Count = 0;
        }


        chord1Count++;

        lastChord1Time = now;


        Debug.Log(
        $"跳过计数 {chord1Count}/{skipCount}");



        if (chord1Count >= skipCount)
        {
            TriggerSkip();
        }
    }


    void TriggerSkip()
    {
        if (skipTriggered)
            return;


        skipTriggered = true;


        SceneManager.LoadScene(skipSceneName);
    }

    public void ResetSkip()
    {
        chord1Count = 0;
        lastChord1Time = 0;
        skipTriggered = false;
    }
}