using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;
using System.Collections.Generic;
using UnityEngine.XR.ARSubsystems;
using System;

public class ImageTrackingManager : MonoBehaviour
{
    [SerializeField] private ARTrackedImageManager _imageManager;
    [Header("介绍文字")]
    [SerializeField] private GameObject titlePrefab;
    [SerializeField] private GameObject descriptionPrefab;

    private Dictionary<string, (GameObject title, GameObject description)> spawnedTextObjects = new Dictionary<string, (GameObject, GameObject)>();
    private Dictionary<string, GameObject> spawnedModelObjects = new Dictionary<string, GameObject>();
    //private Dictionary<string, GameObject> spawnedAnimationObjects = new Dictionary<string, GameObject>();


    [Header("动画")]
    public GameObject carPrefab; // 小车的预制体
    private GameObject carInstance; // 小车实例

    public float speed = 5f; // 小车前后移动速度
    public float turnSpeed = 100f; // 小车左右转向速度
    private float fixedDistance = 0.5f;
    private float textSpacing = 0.1f;
    private float modelHeightOffset = 0.1f;
    private float carHeightOffset = 0.05f;
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
            //SpawnObject(trackedImage);
            string imageName = trackedImage.referenceImage.name;

            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                // ===== 创建模型 =====
                if (IsModelTarget(imageName))
                {
                    if (!spawnedModelObjects.ContainsKey(imageName))
                    {
                        GameObject modelPrefab = LoadModelFromResources(imageName);
                        if (modelPrefab != null)
                        {
                            GameObject modelInstance = Instantiate(modelPrefab);
                            modelInstance.transform.localScale = Vector3.one * 0.2f;
                            spawnedModelObjects[imageName] = modelInstance;
                        }
                    }

                    if (spawnedModelObjects.ContainsKey(imageName))
                    {
                        GameObject model = spawnedModelObjects[imageName];
                        model.SetActive(true);
                        model.transform.position = trackedImage.transform.position + new Vector3(0, modelHeightOffset, 0);
                        model.transform.rotation = Quaternion.identity;
                    }
                }
                else if (IsTextTarget(imageName))
                {
                    // ===== 文字 =====
                    UpdateTextPosition(trackedImage);

                    if (spawnedTextObjects.ContainsKey(imageName))
                    {
                        (GameObject title, GameObject desc) = spawnedTextObjects[imageName];
                        title.SetActive(true);
                        desc.SetActive(true);
                    }
                }
                else if (IsAnimationTarget(imageName))
                {
                    if(carInstance!=null){
                        carInstance.SetActive(true);
                    }else{
                        SpawnObject(trackedImage);
                        carInstance.SetActive(true);
                    }
                    
                }

            }
            else // Not Tracking => 销毁模型和文字
            {
                // 销毁模型
                if (spawnedModelObjects.ContainsKey(imageName))
                {
                    Destroy(spawnedModelObjects[imageName]);
                    spawnedModelObjects.Remove(imageName);
                }

                // 销毁文字
                if (spawnedTextObjects.ContainsKey(imageName))
                {
                    (GameObject title, GameObject desc) = spawnedTextObjects[imageName];
                    title.SetActive(false);
                    desc.SetActive(false);
                }

                //销毁动画
                if (IsAnimationTarget(imageName) && carInstance != null)
                {
                    Destroy(carInstance);
                    carInstance=null;
                }
            }
        }

        foreach (var trackedImage in eventArgs.removed)
        {
            string imageName = trackedImage.referenceImage.name;

            // 销毁模型
            if (spawnedModelObjects.ContainsKey(imageName))
            {
                Destroy(spawnedModelObjects[imageName]);
                spawnedModelObjects.Remove(imageName);
            }

            // 销毁文字
            if (spawnedTextObjects.ContainsKey(imageName))
            {
                (GameObject titleText, GameObject descriptionText) = spawnedTextObjects[imageName];
                // Destroy(titleText);
                // Destroy(descriptionText);
                titleText.SetActive(false);
                descriptionText.SetActive(false);
                spawnedTextObjects.Remove(imageName);
            }
        }
    }


    private void SpawnObject(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;

        if (IsModelTarget(imageName))//是模型
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
        else if (IsTextTarget(imageName))//是文字
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
        else if (IsAnimationTarget(imageName))//是动画
        {
            //
            if (carInstance == null)
            {
                carInstance = Instantiate(carPrefab);
                //carInstance.transform.position = trackedImage.transform.position + new Vector3(0, carHeightOffset, 0);

                carInstance.transform.position = trackedImage.transform.position + new Vector3(0, carHeightOffset, 0);
            }
            else
            {
                // 如果小车已经存在，更新它的位置
                carInstance.transform.position = trackedImage.transform.position + new Vector3(0, carHeightOffset, 0);

            }

            carInstance.SetActive(false);
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
        return imageName == "evir1" || imageName == "evir2" || imageName == "Car 3" || imageName == "Lamp";
    }

    private bool IsAnimationTarget(String imageName)
    {
        return imageName == "Car 1" || imageName == "bluecar";
    }

    private bool IsTextTarget(String imageName)
    {
        return imageName == "Desk" || imageName == "Sofa" || imageName == "painting1";
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
            case "Painting1": return "A painting1. Buautiful.";
            default: return "No description available.";
        }
    }






    Vector2 startTouchPosition;
    Vector2 endTouchPosition;
    bool isSwiping = false;

    void Update()
{
    HandleTouchInput(); // 处理触控滑动
    AnimationMove();    // 控制小车移动
}

Vector2 swipeDelta;
float moveInput = 0f;
float turnInput = 0f;

void HandleTouchInput()
{
    moveInput = 0f;
    turnInput = 0f;

    if (Input.touchCount > 0)
    {
        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                startTouchPosition = touch.position;
                isSwiping = true;
                break;

            case TouchPhase.Moved:
                endTouchPosition = touch.position;
                swipeDelta = endTouchPosition - startTouchPosition;

                // 计算水平/垂直方向
                if (Mathf.Abs(swipeDelta.y) > Mathf.Abs(swipeDelta.x))
                {
                    // 垂直滑动：前进或后退
                    moveInput = swipeDelta.y > 0 ? 1f : -1f;
                }
                else
                {
                    // 水平滑动：转向
                    turnInput = swipeDelta.x > 0 ? 1f : -1f;
                }
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                isSwiping = false;
                swipeDelta = Vector2.zero;
                break;
        }
    }
}
    
    void AnimationMove()
{
    if (carInstance != null)
    {
        carInstance.transform.Translate(Vector3.forward * moveInput * speed * Time.deltaTime);
        carInstance.transform.Rotate(Vector3.up * turnInput * turnSpeed * Time.deltaTime);

        RotateWheels();
    }
}


    void RotateWheels()
    {
        Transform frontLeftWheel = carInstance.transform.Find("Cylinder");
        Transform frontRightWheel = carInstance.transform.Find("Cylinder:005");
        Transform backLeftWheel = carInstance.transform.Find("Cylinder:006");
        Transform backRightWheel = carInstance.transform.Find("Cylinder:007");

        // 确保找到车轮物体，并进行旋转
        if (frontLeftWheel != null && frontRightWheel != null && backLeftWheel != null && backRightWheel != null)
        {
            // 让四个车轮旋转
            frontLeftWheel.Rotate(Vector3.right * speed * Time.deltaTime);
            frontRightWheel.Rotate(Vector3.right * speed * Time.deltaTime);
            backLeftWheel.Rotate(Vector3.right * speed * Time.deltaTime);
            backRightWheel.Rotate(Vector3.right * speed * Time.deltaTime);
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
