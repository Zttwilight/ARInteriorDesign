using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARCarController : MonoBehaviour
{
    public GameObject carPrefab; // 小车的预制体
    private GameObject carInstance; // 小车实例
    private ARTrackedImageManager trackedImageManager;

    public float speed = 5f; // 小车前后移动速度
    public float turnSpeed = 100f; // 小车左右转向速度

    private void Awake()
    {
        trackedImageManager = FindObjectOfType<ARTrackedImageManager>(); // 获取 AR Tracked Image Manager
    }

    private void OnEnable()
    {
        // 注册图像变化事件
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    private void OnDisable()
    {
        // 注销图像变化事件
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        // 当图像变化时执行这个方法
        foreach (var addedImage in eventArgs.added)
        {
            if (addedImage.referenceImage.name == "YourImageName") // 比对图像名称
            {
                // 如果小车实例还不存在，就实例化一个小车
                if (carInstance == null)
                {
                    carInstance = Instantiate(carPrefab, addedImage.transform.position, Quaternion.identity);
                }
                else
                {
                    // 如果小车已经存在，更新它的位置
                    carInstance.transform.position = addedImage.transform.position;
                }
            }
        }
    }

void Update()
{
    // 控制小车的前后移动和转向
    if (carInstance != null)
    {
        // 获取上下滑动输入，用于前后移动
        float moveInput = Input.GetAxis("Vertical"); // 这会从键盘的上下方向键或者触摸滑动中获取输入
        // 获取左右滑动输入，用于转向
        float turnInput = Input.GetAxis("Horizontal"); // 这会从键盘的左右方向键或者触摸滑动中获取输入

        // 小车前后移动
        carInstance.transform.Translate(Vector3.forward * moveInput * speed * Time.deltaTime);

        // 小车转向
        carInstance.transform.Rotate(Vector3.up * turnInput * turnSpeed * Time.deltaTime);

        // 控制车轮旋转
        RotateWheels();
    }
}

void RotateWheels()
{
    // 获取四个车轮的引用
    Transform frontLeftWheel = carInstance.transform.Find("FrontLeftWheel");
    Transform frontRightWheel = carInstance.transform.Find("FrontRightWheel");
    Transform backLeftWheel = carInstance.transform.Find("BackLeftWheel");
    Transform backRightWheel = carInstance.transform.Find("BackRightWheel");

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
}
