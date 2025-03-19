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
    private Dictionary<string, GameObject> spawnedModelObjects = new Dictionary<string, GameObject>(); // 3D 模型管理

    private float fixedDistance = 0.5f; // 文字距离摄像头的固定距离（0.5 米）
    private float textSpacing = 0.1f; // 标题和正文之间的间距
    private float modelHeightOffset = 0.1f; // 3D 模型离图片的高度

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
            SpawnObject(trackedImage);
        }

        foreach (var trackedImage in eventArgs.updated)
        {
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                UpdateTextPosition(trackedImage);
                SetObjectVisibility(trackedImage.referenceImage.name, true);
            }
            else
            {
                SetObjectVisibility(trackedImage.referenceImage.name, false);
            }
        }

        foreach (var trackedImage in eventArgs.removed)
        {
            RemoveObject(trackedImage);
        }
    }

    // **生成 3D 模型或文字**
    private void SpawnObject(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;

        if (IsModelTarget(imageName)) // 如果图片需要显示 3D 模型
        {
            if (!spawnedModelObjects.ContainsKey(imageName))
            {
                GameObject modelPrefab = LoadModelFromResources(imageName);
                if (modelPrefab != null)
                {
                    GameObject modelInstance = Instantiate(modelPrefab);
                    modelInstance.transform.position = trackedImage.transform.position + new Vector3(0, modelHeightOffset, 0); // 生成在图片上方
                    modelInstance.transform.localScale = Vector3.one * 0.2f; // 调整大小
                    modelInstance.transform.rotation = Quaternion.identity; // 使其水平

                    spawnedModelObjects[imageName] = modelInstance;
                }
            }
        }
        else // 否则显示文字
        {
            if (!spawnedTextObjects.ContainsKey(imageName))
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
    }

    // **更新文字或 3D 模型的位置**
    private void UpdateTextPosition(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;

        if (spawnedTextObjects.ContainsKey(imageName))
        {
            (GameObject titleText, GameObject descriptionText) = spawnedTextObjects[imageName];

            Vector3 cameraPosition = Camera.main.transform.position;
            Vector3 directionToCamera = (trackedImage.transform.position - cameraPosition).normalized;
            Vector3 basePosition = trackedImage.transform.position;

            // 标题位置
            titleText.transform.position = basePosition + new Vector3(0, 0.05f, 0);
            descriptionText.transform.position = basePosition - new Vector3(0, 0.05f, 0);

            // 让文字始终正对摄像头
            titleText.transform.rotation = Quaternion.LookRotation(directionToCamera);
            descriptionText.transform.rotation = Quaternion.LookRotation(directionToCamera);
        }

        if (spawnedModelObjects.ContainsKey(imageName))
        {
            GameObject model = spawnedModelObjects[imageName];
            model.transform.position = trackedImage.transform.position + new Vector3(0, modelHeightOffset, 0);
            model.transform.rotation = Quaternion.identity; // 使模型保持水平
        }
    }

    // **设置可见性**
    private void SetObjectVisibility(string imageName, bool isVisible)
    {
        if (spawnedTextObjects.ContainsKey(imageName))
        {
            (GameObject titleText, GameObject descriptionText) = spawnedTextObjects[imageName];
            titleText.SetActive(isVisible);
            descriptionText.SetActive(isVisible);
        }

        if (spawnedModelObjects.ContainsKey(imageName))
        {
            spawnedModelObjects[imageName].SetActive(isVisible);
        }
    }

    // **移除对象**
    private void RemoveObject(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;

        if (spawnedTextObjects.ContainsKey(imageName))
        {
            (GameObject titleText, GameObject descriptionText) = spawnedTextObjects[imageName];
            Destroy(titleText);
            Destroy(descriptionText);
            spawnedTextObjects.Remove(imageName);
        }

        if (spawnedModelObjects.ContainsKey(imageName))
        {
            Destroy(spawnedModelObjects[imageName]);
            spawnedModelObjects.Remove(imageName);
        }
    }

    // **判断是否为 3D 模型目标**
    private bool IsModelTarget(string imageName)
    {
        return imageName == "evir1" || imageName == "evir2";
    }

    // **从 Resources 目录动态加载 3D 模型**
    private GameObject LoadModelFromResources(string imageName)
    {
        return Resources.Load<GameObject>("Models/" + imageName);
    }

    // **获取文字描述**
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
