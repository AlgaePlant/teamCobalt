using UnityEngine;
using System;

public class GuitarBluetoothInput : MonoBehaviour
{
    public static GuitarBluetoothInput Instance;

    public static event Action<int> OnStringPlayed;

    int lastS1 = 1;
    int lastS2 = 1;
    int lastS3 = 1;

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

    public void ParseMessage(string msg)
    {
        string[] parts = msg.Split(',');

        if (parts.Length != 3)
            return;

        int s1;
        int s2;
        int s3;

        if (!int.TryParse(parts[0], out s1))
            return;

        if (!int.TryParse(parts[1], out s2))
            return;

        if (!int.TryParse(parts[2], out s3))
            return;

        //------------------------------------------------
        // String 1
        //------------------------------------------------

        if (lastS1 == 1 && s1 == 0)
    {
        Debug.Log("String1");
        OnStringPlayed?.Invoke(1);
        
        // 增加计数器
        if (StringCounter.Instance != null)
        {
            StringCounter.Instance.AddPress();
        }
    }

        //------------------------------------------------
        // String 2
        //------------------------------------------------

        if (lastS2 == 1 && s2 == 0)
        {
            Debug.Log("String2");

            OnStringPlayed?.Invoke(2);
        }

        //------------------------------------------------
        // String 3
        //------------------------------------------------

        if (lastS3 == 1 && s3 == 0)
        {
            Debug.Log("String3");

            OnStringPlayed?.Invoke(3);
        }

        lastS1 = s1;
        lastS2 = s2;
        lastS3 = s3;
    }
}