using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI; // 需要添加 UI 命名空间

public class ARLightController : MonoBehaviour
{
    // 通过 Inspector 赋值 ARCameraManager
    [SerializeField] private ARCameraManager _arCameraManager;
   
    // 引用场景中的光源
    [SerializeField] private Light _sceneLight;

    // 引用 UI 图像，用于显示颜色估计
    [SerializeField] private Image _imageLighting;

    void Start()
    {
        // 订阅 frameReceived 事件
        if (_arCameraManager != null)
        {
            _arCameraManager.frameReceived += OnCameraFrameReceived;
        }
    }

    void OnDestroy()
    {
        // 取消订阅事件，防止内存泄漏
        if (_arCameraManager != null)
        {
            _arCameraManager.frameReceived -= OnCameraFrameReceived;
        }
    }

    // 处理相机帧事件
    void OnCameraFrameReceived(ARCameraFrameEventArgs args)
    {
        // 获取光照估计数据
        ARLightEstimationData lightEstimation = args.lightEstimation;

        // 检查 averageBrightness
        if (lightEstimation.averageBrightness.HasValue)
        {
            float brightness = lightEstimation.averageBrightness.Value;
            Debug.Log($"Average Brightness: {brightness}");

            // 更新虚拟光源的强度
            _sceneLight.intensity = brightness;
        }

        // 检查 averageColorTemperature
        if (lightEstimation.averageColorTemperature.HasValue)
        {
            float colorTemperature = lightEstimation.averageColorTemperature.Value;
            Debug.Log($"Average Color Temperature: {colorTemperature}");

            // 更新虚拟光源的色温
            _sceneLight.colorTemperature = colorTemperature;
        }

        // 检查 colorCorrection
        if (lightEstimation.colorCorrection.HasValue)
        {
            Color colorCorrection = lightEstimation.colorCorrection.Value;
            Debug.Log($"Color Correction: {colorCorrection}");

            // 将颜色校正应用到光照的颜色
            _sceneLight.color = colorCorrection;

            // 这里将颜色赋值给 UI 图像
            if (_imageLighting != null)
            {
                _imageLighting.color = colorCorrection;
            }
        }
    }
}



