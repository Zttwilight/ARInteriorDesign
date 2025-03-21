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
    private Dictionary<string, GameObject> spawnedModelObjects = new Dictionary<string, GameObject>();

    private float fixedDistance = 0.5f;
    private float textSpacing = 0.1f;
    private float modelHeightOffset = 0.1f;

    void OnEnable()
    {
        _imageManager.trackedImagesChanged += OnTrackedImagesChanged;
        GameManager.OnGlobalDestroySent += HandleDestroyReport;
    }

    void OnDisable()
    {
        _imageManager.trackedImagesChanged -= OnTrackedImagesChanged;
        GameManager.OnGlobalDestroySent -= HandleDestroyReport;
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            SpawnObject(trackedImage);
        }

        foreach (var trackedImage in eventArgs.updated)
        {
            string imageName = trackedImage.referenceImage.name;

            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                UpdateTextPosition(trackedImage);

                if (spawnedTextObjects.ContainsKey(imageName))
                {
                    (GameObject title, GameObject desc) = spawnedTextObjects[imageName];
                    title.SetActive(true);
                    desc.SetActive(true);
                }
            }
            else
            {
                // 图像不在追踪状态，隐藏文字但保留模型
                if (spawnedTextObjects.ContainsKey(imageName))
                {
                    (GameObject title, GameObject desc) = spawnedTextObjects[imageName];
                    title.SetActive(false);
                    desc.SetActive(false);
                }
            }
        }

        foreach (var trackedImage in eventArgs.removed)
        {
            string imageName = trackedImage.referenceImage.name;

            // 只移除文字，不移除模型
            if (spawnedTextObjects.ContainsKey(imageName))
            {
                (GameObject titleText, GameObject descriptionText) = spawnedTextObjects[imageName];
                Destroy(titleText);
                Destroy(descriptionText);
                spawnedTextObjects.Remove(imageName);
            }
        }
    }

    private void SpawnObject(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;

        if (IsModelTarget(imageName))
        {
            if (!spawnedModelObjects.ContainsKey(imageName))
            {
                GameObject modelPrefab = LoadModelFromResources(imageName);
                if (modelPrefab != null)
                {
                    GameObject modelInstance = Instantiate(modelPrefab);
                    modelInstance.transform.position = trackedImage.transform.position + new Vector3(0, modelHeightOffset, 0);
                    modelInstance.transform.localScale = Vector3.one * 0.2f;
                    modelInstance.transform.rotation = Quaternion.identity;

                    spawnedModelObjects[imageName] = modelInstance;
                }
            }
        }
        else
        {
            if (!spawnedTextObjects.ContainsKey(imageName))
            {
                GameObject titleText = Instantiate(titlePrefab);
                titleText.GetComponent<TextMeshPro>().text = imageName;
                titleText.transform.localScale = Vector3.one * 0.05f;

                GameObject descriptionText = Instantiate(descriptionPrefab);
                descriptionText.GetComponent<TextMeshPro>().text = GetDescription(imageName);
                descriptionText.transform.localScale = Vector3.one * 0.05f;

                spawnedTextObjects[imageName] = (titleText, descriptionText);

                // 初始状态隐藏
                titleText.SetActive(false);
                descriptionText.SetActive(false);
            }
        }
    }

    private void UpdateTextPosition(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;

        if (spawnedTextObjects.ContainsKey(imageName))
        {
            (GameObject titleText, GameObject descriptionText) = spawnedTextObjects[imageName];

            Vector3 cameraPosition = Camera.main.transform.position;
            Vector3 directionToCamera = (trackedImage.transform.position - cameraPosition).normalized;
            Vector3 basePosition = trackedImage.transform.position;

            titleText.transform.position = basePosition + new Vector3(0, 0.05f, 0);
            descriptionText.transform.position = basePosition - new Vector3(0, 0.05f, 0);

            titleText.transform.rotation = Quaternion.LookRotation(directionToCamera);
            descriptionText.transform.rotation = Quaternion.LookRotation(directionToCamera);
        }

        if (spawnedModelObjects.ContainsKey(imageName))
        {
            GameObject model = spawnedModelObjects[imageName];
            model.transform.position = trackedImage.transform.position + new Vector3(0, modelHeightOffset, 0);
            model.transform.rotation = Quaternion.identity;
        }
    }

    private bool IsModelTarget(string imageName)
    {
        return imageName == "evir1" || imageName == "evir2";
    }

    private GameObject LoadModelFromResources(string imageName)
    {
        return Resources.Load<GameObject>("Models/" + imageName);
    }

    private string GetDescription(string imageName)
    {
        switch (imageName)
        {
            case "Desk": return "This is a beautiful desk, perfect for study and work.";
            case "Sofa": return "A comfortable sofa for relaxing and enjoying free time.";
            default: return "No description available.";
        }
    }

    private void HandleDestroyReport()
    {
        Debug.Log("ImageTrackingManager: 收到 GameManager 的全局报告！");

        foreach (var model in spawnedModelObjects.Values)
        {
            Destroy(model);
        }

        spawnedModelObjects.Clear();
    }
}
