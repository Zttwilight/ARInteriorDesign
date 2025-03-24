using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class ARObjectMove : MonoBehaviour
{
    private ARRaycastManager raycastManager;
    private Vector2 touchPosition;
    private GameObject selectedObject;
    private Camera arCamera;
    private bool isDragging = false;
    private Vector3 offset;
    private Vector3 initialTouchPosition;
    private Vector3 initialObjectPosition;

    void Start()
    {
        raycastManager = FindObjectOfType<ARRaycastManager>();
        arCamera = Camera.main;
    }

    void Update()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            touchPosition = touch.position;

            if (touch.phase == TouchPhase.Began)
            {
                TrySelectObject(touchPosition);
            }
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                MoveSelectedObject(touchPosition);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                isDragging = false;
                selectedObject = null;
            }
        }

        // Handle two-finger gesture (scale)
        if (Input.touchCount == 2)
        {
            HandleTwoFingerGesture();
        }
    }

    // 选中可拖动的家具
    private void TrySelectObject(Vector2 screenPosition)
    {
        Ray ray = arCamera.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.CompareTag("Furniture"))
            {
                selectedObject = hit.transform.gameObject;
                isDragging = true;

                // 计算物体与触摸点之间的偏移
                Vector3 objectPosition = selectedObject.transform.position;
                Vector3 worldTouchPosition = hit.point;
                offset = objectPosition - worldTouchPosition;

                initialTouchPosition = touchPosition;
                initialObjectPosition = selectedObject.transform.position;

                // Debugging: output offset values
                Debug.Log($"Offset: {offset}");
            }
        }
    }

    // 移动家具位置
    private void MoveSelectedObject(Vector2 screenPosition)
    {
        if (selectedObject == null) return;

        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        if (raycastManager.Raycast(screenPosition, hits, TrackableType.Planes))
        {
            Pose hitPose = hits[0].pose;

            // 物体移动到触摸点，并保持与触摸点的偏移
            Vector3 newPosition = hitPose.position + offset;

            // 物体只移动在平面上，确保y轴固定
            selectedObject.transform.position = new Vector3(newPosition.x, initialObjectPosition.y, newPosition.z);

            // Debugging: output new position values
            Debug.Log($"New Position: {selectedObject.transform.position}");
        }
    }

    // 处理双指缩放手势
    private void HandleTwoFingerGesture()
    {
        Touch touch1 = Input.GetTouch(0);
        Touch touch2 = Input.GetTouch(1);

        // 获取两指的距离变化
        float previousDistance = (touch1.position - touch2.position).magnitude;
        float currentDistance = (touch1.position - touch2.position).magnitude;

        // 比较两次的距离，计算出缩放因子
        float scaleFactor = currentDistance / previousDistance;

        // 缩放物体
        if (selectedObject != null)
        {
            Vector3 scale = selectedObject.transform.localScale;
            scale *= scaleFactor;
            selectedObject.transform.localScale = scale;
        }
    }
}
