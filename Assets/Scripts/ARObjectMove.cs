using UnityEngine;
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
