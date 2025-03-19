using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;
using System.Collections.Generic;
using UnityEngine.XR.ARSubsystems;

public class ImageTrackingManager : MonoBehaviour
{
    [SerializeField] private ARTrackedImageManager _imageManager;
    [SerializeField] private GameObject textPrefab;
    private Dictionary<string, GameObject> spawnedTextObjects = new Dictionary<string, GameObject>();

    private float fixedDistance = 0.5f; // 文字距离摄像头的固定距离（0.5 米）

    void OnEnable()
    {
        _imageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        _imageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            SpawnText(trackedImage);
        }

        foreach (var trackedImage in eventArgs.updated)
        {
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                UpdateTextPosition(trackedImage);
                SetTextVisibility(trackedImage.referenceImage.name, true);
            }
            else
            {
                SetTextVisibility(trackedImage.referenceImage.name, false);
            }
        }

        foreach (var trackedImage in eventArgs.removed)
        {
            RemoveText(trackedImage);
        }
    }

    // 生成文字对象（但默认隐藏）
    private void SpawnText(ARTrackedImage trackedImage)
    {
        if (!spawnedTextObjects.ContainsKey(trackedImage.referenceImage.name))
        {
            GameObject newText = Instantiate(textPrefab);
            newText.GetComponent<TextMeshPro>().text = GetDescription(trackedImage.referenceImage.name);
            newText.transform.localScale = Vector3.one * 0.05f; // 调整字体大小
            spawnedTextObjects[trackedImage.referenceImage.name] = newText;
            newText.SetActive(false); // 默认隐藏，直到图片被识别
        }
    }

    // 更新文字位置，使其与摄像头保持固定距离
    private void UpdateTextPosition(ARTrackedImage trackedImage)
    {
        if (spawnedTextObjects.ContainsKey(trackedImage.referenceImage.name))
        {
            GameObject textObject = spawnedTextObjects[trackedImage.referenceImage.name];

            // 获取摄像头位置
            Vector3 cameraPosition = Camera.main.transform.position;

            // 计算从图片到摄像头的方向
            Vector3 directionToCamera = (cameraPosition - trackedImage.transform.position).normalized;

            // 让文字始终位于摄像头前 `fixedDistance` 的位置
            Vector3 newPosition = cameraPosition - directionToCamera * fixedDistance;

            textObject.transform.position = newPosition;

            // 让文字始终面向摄像头
            textObject.transform.LookAt(2 * textObject.transform.position - cameraPosition);
        }
    }

    // 设置文字可见性
    private void SetTextVisibility(string imageName, bool isVisible)
    {
        if (spawnedTextObjects.ContainsKey(imageName))
        {
            spawnedTextObjects[imageName].SetActive(isVisible);
        }
    }

    // 移除丢失的图片上的文字
    private void RemoveText(ARTrackedImage trackedImage)
    {
        if (spawnedTextObjects.ContainsKey(trackedImage.referenceImage.name))
        {
            Destroy(spawnedTextObjects[trackedImage.referenceImage.name]);
            spawnedTextObjects.Remove(trackedImage.referenceImage.name);
        }
    }

    // 获取不同图片的介绍内容
    private string GetDescription(string imageName)
    {
        switch (imageName)
        {
            case "Desk": return "This is the introduction of Desk";
            case "Sofa": return "This is the introduction of Sofa";
            default: return "Unknown PIC";
        }
    }
}
