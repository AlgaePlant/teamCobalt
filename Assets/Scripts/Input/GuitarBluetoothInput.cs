// Assets/Scripts/Input/GuitarBluetoothInput.cs
using UnityEngine;
using System;

public class GuitarBluetoothInput : MonoBehaviour
{
    public static GuitarBluetoothInput Instance;
    public static event Action<int> OnStringPlayed;

    private int lastS1 = 1;
    private int lastS2 = 1;
    private int lastS3 = 1;

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

    /// <summary>
    /// 从 ESP32 接收蓝牙数据
    /// 格式: "s1,s2,s3" 其中 1=未按, 0=按下
    /// </summary>
    public void ParseMessage(string msg)
    {
        string[] parts = msg.Split(',');

        if (parts.Length != 3)
            return;

        if (!int.TryParse(parts[0], out int s1)) return;
        if (!int.TryParse(parts[1], out int s2)) return;
        if (!int.TryParse(parts[2], out int s3)) return;

        // 弦1：从 1→0 触发
        if (lastS1 == 1 && s1 == 0)
        {
            Debug.Log("[蓝牙] 弦1");
            OnStringPlayed?.Invoke(1);
        }

        // 弦2：从 1→0 触发
        if (lastS2 == 1 && s2 == 0)
        {
            Debug.Log("[蓝牙] 弦2");
            OnStringPlayed?.Invoke(2);
        }

        // 弦3：从 1→0 触发
        if (lastS3 == 1 && s3 == 0)
        {
            Debug.Log("[蓝牙] 弦3");
            OnStringPlayed?.Invoke(3);
        }

        lastS1 = s1;
        lastS2 = s2;
        lastS3 = s3;
    }
}