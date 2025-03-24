using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ShadowPlaneController : MonoBehaviour
{
    [SerializeField] private GameObject shadowPlanePrefab; // 虚拟阴影平面的Prefab
    private GameObject shadowPlane; // 实际的虚拟阴影平面
    private ARPlaneManager arPlaneManager; // AR平面管理器

    void Start()
    {
        arPlaneManager = FindObjectOfType<ARPlaneManager>();
        // 你可以通过 ARPlaneManager 监听和获取平面
        if (arPlaneManager != null)
        {
            arPlaneManager.planesChanged += OnPlanesChanged;
        }
    }

    void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        // 检查是否有新的平面被检测到
        if (args.added.Count > 0)
        {
            // 获取检测到的第一个平面
            ARPlane detectedPlane = args.added[0];
            
            // 创建虚拟阴影平面
            if (shadowPlane == null)
            {
                shadowPlane = Instantiate(shadowPlanePrefab, detectedPlane.transform.position, detectedPlane.transform.rotation);
            }
            else
            {
                // 让虚拟阴影平面跟随检测到的平面移动
                shadowPlane.transform.position = detectedPlane.transform.position;
                shadowPlane.transform.rotation = detectedPlane.transform.rotation;
            }
        }
    }

    void Update()
    {
        // 你可以在这里进行其他控制，如更新虚拟阴影平面的大小或阴影效果等
    }
}
