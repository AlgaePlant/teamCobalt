// Assets/Scripts/Input/KeyboardGuitarSimulator.cs
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

// ★ 类名改成 KeyboardGuitarSimulator（不是 GuitarInputHandler）
public class KeyboardGuitarSimulator : MonoBehaviour
{
    [Header("调试选项")]
    public bool logInput = true;
    
    public UnityEvent<int> OnStringPlayed;
    public static System.Action<int> OnStringPlayedStatic;
    
    private Keyboard keyboard;
    
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        
        keyboard = Keyboard.current;
        if (keyboard == null)
        {
            Debug.LogError("没有检测到键盘！");
            return;
        }
        
        Debug.Log("=== 键盘吉他模拟器已启动 ===");
        Debug.Log("弦1 (黄色/矿物): 按数字键 1");
        Debug.Log("弦2 (绿色/生命): 按数字键 2");
        Debug.Log("弦3 (蓝紫色/语言): 按数字键 3");
    }
    
    void Update()
    {
        if (keyboard == null) return;
        
        if (keyboard.digit1Key.wasPressedThisFrame)
            PlayChord(1);
        else if (keyboard.digit2Key.wasPressedThisFrame)
            PlayChord(2);
        else if (keyboard.digit3Key.wasPressedThisFrame)
            PlayChord(3);
    }
    
    void PlayChord(int chordId)
    {
        if (logInput)
        {
            string[] chordNames = { "", "弦1 - 黄色 / 矿物感", "弦2 - 绿色 / 生命感", "弦3 - 蓝紫色 / 外星语言" };
            Debug.Log($"[吉他输入] 弹奏: {chordNames[chordId]}");
        }
        
        OnStringPlayed?.Invoke(chordId);
        OnStringPlayedStatic?.Invoke(chordId);
    }
}