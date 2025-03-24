using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

public class LightEstimationController : MonoBehaviour
{
    [SerializeField] private ARCameraManager _arCameraManager;
    [SerializeField] private Light _directionalLight;
    [SerializeField] private Image _imageLighting; // 用于显示光照估计颜色

    private void OnEnable()
    {
        _arCameraManager.frameReceived += OnFrameReceived;
    }

    private void OnDisable()
    {
        _arCameraManager.frameReceived -= OnFrameReceived;
    }

    private void OnFrameReceived(ARCameraFrameEventArgs args)
    {
        // 读取光照估计数据
        if (args.lightEstimation.averageBrightness.HasValue)
        {
            _directionalLight.intensity = args.lightEstimation.averageBrightness.Value;
        }

        if (args.lightEstimation.averageColorTemperature.HasValue)
        {
            _directionalLight.colorTemperature = args.lightEstimation.averageColorTemperature.Value;
        }

        if (args.lightEstimation.colorCorrection.HasValue)
        {
            _directionalLight.color = args.lightEstimation.colorCorrection.Value;
            if (_imageLighting != null)
            {
                _imageLighting.color = args.lightEstimation.colorCorrection.Value;
            }
        }
    }
}
