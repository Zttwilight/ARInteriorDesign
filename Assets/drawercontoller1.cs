using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.EventSystems;

public class DrawerARController : MonoBehaviour
{
    public Transform openPosition;    // 抽屉打开位置
    public Transform closedPosition;  // 抽屉关闭位置
    public float moveSpeed = 2.0f;    // 抽屉移动速度

    private Vector3 targetPosition;
    private bool isDrawerOpen = false; // 抽屉是否打开
    private bool isMoving = false;     // 是否正在移动
    private Camera arCamera;           // AR 摄像机

    void Start()
    {
        // 找到 AR 摄像机
        arCamera = Camera.main;
        targetPosition = closedPosition.position;
    }

    void Update()
    {
        if (isMoving)
        {
            // 平滑移动抽屉到目标位置
            transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // 检测抽屉是否到达目标
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                isMoving = false;
            }
        }

        // 检测用户长按屏幕
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // 长按触摸 -> 检测交互
            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = arCamera.ScreenPointToRay(touch.position);
                RaycastHit hit;

                // 检测是否点到了抽屉
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject == gameObject)
                    {
                        ToggleDrawer();
                    }
                }
            }
        }
    }

    // 切换抽屉状态
    private void ToggleDrawer()
    {
        if (!isDrawerOpen)
        {
            // 打开抽屉
            targetPosition = openPosition.position;
        }
        else
        {
            // 关闭抽屉
            targetPosition = closedPosition.position;
        }

        isMoving = true;
        isDrawerOpen = !isDrawerOpen;  // 切换抽屉状态
    }
}
