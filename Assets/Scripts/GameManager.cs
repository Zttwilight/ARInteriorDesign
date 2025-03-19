using UnityEngine;
using UnityEngine.Events;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // 定义事件
    public static event Action OnGlobalDestroySent;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 确保 GameManager 在场景切换时不会被销毁
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 发送全局要摧毁相关物体
    [ContextMenu("发送全局摧毁")]
    public void SendGlobalDestroy()
    {
        Debug.Log("GameManager: 全局报告已发送！");
        
        // 触发事件，通知所有监听者
        OnGlobalDestroySent?.Invoke();

        
    }
}
