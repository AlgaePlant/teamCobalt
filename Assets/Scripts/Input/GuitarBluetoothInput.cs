// Assets/Scripts/Input/GuitarBluetoothInput.cs
using UnityEngine;
using System;
using System.IO.Ports;

public class GuitarBluetoothInput : MonoBehaviour
{
    public static GuitarBluetoothInput Instance;
    public static event Action<int> OnStringPlayed;

    [Header("USB 串口设置")]
    public bool useUSBSerial = true;              // 改成 true 使用 USB 串口
    public string portName = "COM3";              // Windows 用 COM3，Mac 用 /dev/tty.usbmodemxxx
    public int baudRate = 9600;

    private SerialPort serialPort;
    private int lastS1 = 1;
    private int lastS2 = 1;
    private int lastS3 = 1;

    void Awake()
    {
        Debug.Log("🔵 GuitarBluetoothInput.Awake() 被调用");
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Debug.Log($"🟢 GuitarBluetoothInput.Start() 被调用, useUSBSerial={useUSBSerial}");
        if (useUSBSerial)
        {
            try
            {
                serialPort = new SerialPort(portName, baudRate);
                serialPort.DtrEnable = true;
                serialPort.RtsEnable = true;
                serialPort.ReadTimeout = 20;
                serialPort.Open();
                Debug.Log($"✅ USB 串口已打开: {portName}");
            }
            catch (Exception e)
            {
                Debug.LogError($"❌ 打开串口失败: {e.Message}");
            }
        }
    }

    void Update()
    {
        if (!useUSBSerial || serialPort == null || !serialPort.IsOpen)
            return;

        try
        {
            string msg = serialPort.ReadLine();
            if (!string.IsNullOrEmpty(msg))
            {
                msg = msg.Trim();
                ParseMessage(msg);
            }
        }
        catch (TimeoutException)
        {
            // 正常，没有数据
        }
        catch (Exception e)
        {
            Debug.LogError($"串口读取错误: {e.Message}");
        }
    }

    void OnDestroy()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
            serialPort.Dispose();
            Debug.Log("USB 串口已关闭");
        }
    }

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
            Debug.Log("[吉他] 弦1");
            OnStringPlayed?.Invoke(1);
        }

        // 弦2：从 1→0 触发
        if (lastS2 == 1 && s2 == 0)
        {
            Debug.Log("[吉他] 弦2");
            OnStringPlayed?.Invoke(2);
        }

        // 弦3：从 1→0 触发
        if (lastS3 == 1 && s3 == 0)
        {
            Debug.Log("[吉他] 弦3");
            OnStringPlayed?.Invoke(3);
        }

        lastS1 = s1;
        lastS2 = s2;
        lastS3 = s3;
    }
}