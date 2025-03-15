/*using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARObjectMove : MonoBehaviour
{
    private ARRaycastManager raycastManager;
    private Vector2 touchPosition;
    private GameObject selectedObject;
    private Camera arCamera;

    void Start()
    {
        raycastManager = FindObjectOfType<ARRaycastManager>();
        arCamera = Camera.main;
    }

    void Update()
    {
        // 检测单指触摸输入
        if (Input.touchCount == 1)
        {
            touchPosition = Input.GetTouch(0).position;
            Ray ray = arCamera.ScreenPointToRay(touchPosition);
            RaycastHit hit;

            // 检测射线是否击中物体
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.CompareTag("Furniture")) // 确保是可以移动的物体
                {
                    if (selectedObject == null)
                    {
                        selectedObject = hit.transform.gameObject;
                    }

                    // 将物体移动到触摸位置
                    Vector3 worldPosition = hit.point;
                    selectedObject.transform.position = new Vector3(worldPosition.x, selectedObject.transform.position.y, worldPosition.z);
                }
            }
        }
        else
        {
            selectedObject = null;
        }
    }
}
*/

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
            selectedObject.transform.position = hitPose.position;
        }
    }
}
