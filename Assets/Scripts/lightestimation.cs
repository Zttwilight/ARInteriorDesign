using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using TMPro;

public class LightEstimationController : MonoBehaviour
{
    [SerializeField] private ARCameraManager _arCameraManager;
    [SerializeField] private Light _directionalLight; // 用于影响3D场景的光
    [SerializeField] private Image _colorBar; // UI 颜色条
    [SerializeField] private TMP_Text _debugText; // 调试信息

    private void OnEnable() => _arCameraManager.frameReceived += OnFrameReceived;
    private void OnDisable() => _arCameraManager.frameReceived -= OnFrameReceived;

    private void OnFrameReceived(ARCameraFrameEventArgs args)
    {
        float? brightness = args.lightEstimation.averageBrightness;
        float? temperature = args.lightEstimation.averageColorTemperature;
        Color? color = args.lightEstimation.colorCorrection;

        if (brightness.HasValue)
        {
            float brightnessNormalized = Mathf.Clamp01(brightness.Value / 2f); // 归一化到 0~1
            _colorBar.color = Color.Lerp(Color.black, Color.white, brightnessNormalized); // 由暗到亮
        }


        if (color.HasValue) // 使用 colorCorrection 让颜色变化更明显
        {
            _directionalLight.color = color.Value;
            _colorBar.color = new Color(color.Value.r, color.Value.g, color.Value.b, 1f);
        }

        _debugText.text = $"Brightness: {brightness?.ToString("F2") ?? "none"}\n" +
                          $"Color: {color?.ToString() ?? "none"}";
    }

}
