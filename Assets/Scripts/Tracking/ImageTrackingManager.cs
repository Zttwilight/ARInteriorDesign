using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;
using System.Collections.Generic;
using UnityEngine.XR.ARSubsystems;

public class ImageTrackingManager : MonoBehaviour
{
    [SerializeField] private ARTrackedImageManager _imageManager;
    [SerializeField] private GameObject titlePrefab;
    [SerializeField] private GameObject descriptionPrefab;

    private Dictionary<string, (GameObject title, GameObject description)> spawnedTextObjects = new Dictionary<string, (GameObject, GameObject)>();

    private float fixedDistance = 0.5f; // 文字距离摄像头的固定距离（0.5 米）
    private float textSpacing = 0.1f; // 标题和正文之间的间距

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

    // 生成文字对象（标题 + 正文）
    private void SpawnText(ARTrackedImage trackedImage)
    {
        if (!spawnedTextObjects.ContainsKey(trackedImage.referenceImage.name))
        {
            // 创建标题
            GameObject titleText = Instantiate(titlePrefab);
            titleText.GetComponent<TextMeshPro>().text = trackedImage.referenceImage.name; // 直接使用图片名称
            titleText.transform.localScale = Vector3.one * 0.05f; // 调整字体大小

            // 创建正文
            GameObject descriptionText = Instantiate(descriptionPrefab);
            descriptionText.GetComponent<TextMeshPro>().text = GetDescription(trackedImage.referenceImage.name);
            descriptionText.transform.localScale = Vector3.one * 0.05f;

            // 存入字典
            spawnedTextObjects[trackedImage.referenceImage.name] = (titleText, descriptionText);

            // 默认隐藏，直到图片被识别
            titleText.SetActive(false);
            descriptionText.SetActive(false);
        }
    }

    //更新文字位置
    private void UpdateTextPosition(ARTrackedImage trackedImage)
    {
        if (spawnedTextObjects.ContainsKey(trackedImage.referenceImage.name))
        {
            (GameObject titleText, GameObject descriptionText) = spawnedTextObjects[trackedImage.referenceImage.name];

            Vector3 cameraPosition = Camera.main.transform.position;
            Vector3 directionToCamera = (cameraPosition - trackedImage.transform.position).normalized;
            Vector3 basePosition = cameraPosition - directionToCamera * fixedDistance;

            // 标题位置
            titleText.transform.position = basePosition;

            // 正文位置（在标题下面）
            descriptionText.transform.position = basePosition - new Vector3(0, textSpacing, 0);

            // 让文字始终面向摄像头
            titleText.transform.LookAt(2 * titleText.transform.position - cameraPosition);
            descriptionText.transform.LookAt(2 * descriptionText.transform.position - cameraPosition);
        }
    }





    // 设置文字可见性
    private void SetTextVisibility(string imageName, bool isVisible)
    {
        if (spawnedTextObjects.ContainsKey(imageName))
        {
            (GameObject titleText, GameObject descriptionText) = spawnedTextObjects[imageName];
            titleText.SetActive(isVisible);
            descriptionText.SetActive(isVisible);
        }
    }

    // 移除丢失的图片上的文字
    private void RemoveText(ARTrackedImage trackedImage)
    {
        if (spawnedTextObjects.ContainsKey(trackedImage.referenceImage.name))
        {
            (GameObject titleText, GameObject descriptionText) = spawnedTextObjects[trackedImage.referenceImage.name];

            Destroy(titleText);
            Destroy(descriptionText);
            spawnedTextObjects.Remove(trackedImage.referenceImage.name);
        }
    }

    // 获取不同图片的介绍内容
    private string GetDescription(string imageName)
    {
        switch (imageName)
        {
            case "Desk": return "This is a beautiful desk, perfect for study and work.";
            case "Sofa": return "A comfortable sofa for relaxing and enjoying free time.";
            default: return "No description available.";
        }
    }
}
