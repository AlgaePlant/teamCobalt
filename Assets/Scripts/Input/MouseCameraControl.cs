using UnityEngine;
using UnityEngine.InputSystem;

public class MouseCameraControl : MonoBehaviour
{
    [Header("灵敏度（调低后更平滑）")]
    public float lookSpeed = 1.2f;      // 视角转动速度（原2 → 现1.2）
    public float moveSpeed = 3f;        // 移动速度（原5 → 现3）
    
    private float rotationX = 0f;
    private float rotationY = 0f;
    private Keyboard keyboard;
    private Mouse mouse;
    
    void Start()
    {
        keyboard = Keyboard.current;
        mouse = Mouse.current;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Debug.Log("MouseCameraControl 已启动 - 速度已调慢");
    }
    
    void Update()
    {
        if (keyboard == null || mouse == null) return;
        
        // ESC 释放鼠标
        if (keyboard.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        
        if (Cursor.lockState != CursorLockMode.Locked) return;
        
        // 鼠标右键控制视角
        if (mouse.rightButton.isPressed)
        {
            Vector2 delta = mouse.delta.ReadValue();
            rotationX -= delta.y * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -90f, 90f);
            rotationY += delta.x * lookSpeed;
            transform.rotation = Quaternion.Euler(0, rotationY, 0);
        }
        
        // 只有 WASD 移动（删除了 Q/E）
        Vector3 move = Vector3.zero;
        if (keyboard.wKey.isPressed) move += Vector3.forward;
        if (keyboard.sKey.isPressed) move += Vector3.back;
        if (keyboard.aKey.isPressed) move += Vector3.left;
        if (keyboard.dKey.isPressed) move += Vector3.right;
        
        if (move != Vector3.zero)
        {
            Vector3 moveDirection = transform.TransformDirection(move);
            moveDirection.y = 0;  // 保持水平移动，不上下飞
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
        }
    }
    
    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.fontSize = 14;
        style.fontStyle = FontStyle.Bold;
        
        GUILayout.BeginArea(new Rect(10, Screen.height - 100, 300, 100));
        GUILayout.Label("=== 鼠标/键盘控制 ===", style);
        GUILayout.Label("鼠标右键 + 拖动: 转动视角", style);
        GUILayout.Label("WASD: 移动", style);
        GUILayout.Label("按 1/2/3: 弹奏吉他弦", style);
        GUILayout.Label("ESC: 释放鼠标", style);
        GUILayout.EndArea();
    }
}