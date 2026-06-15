// Assets/Scripts/Input/KeyboardGuitarSimulator.cs
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class KeyboardGuitarSimulator : MonoBehaviour
{
    [Header("调试选项")]
    public bool logInput = true;
    
    // 改为 UnityEvent，方便 Inspector 拖拽绑定，也方便代码订阅
    public UnityEvent<int> OnStringPlayed;
    
    // 保留静态事件供旧代码兼容（可选）
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
        
        // 触发 UnityEvent（可在 Inspector 绑定）
        OnStringPlayed?.Invoke(chordId);
        
        // 触发静态事件
        OnStringPlayedStatic?.Invoke(chordId);
    }
    
    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 20;
        style.normal.textColor = Color.white;
        style.fontStyle = FontStyle.Bold;
        
        GUILayout.BeginArea(new Rect(10, 10, 350, 150));
        GUILayout.Label("=== 键盘吉他模拟器 ===", style);
        GUILayout.Label("弦1 (矿物): 按数字键 1", style);
        GUILayout.Label("弦2 (生命): 按数字键 2", style);
        GUILayout.Label("弦3 (语言): 按数字键 3", style);
        GUILayout.EndArea();
    }
}