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

    // ѡ�п��϶��ļҾ�
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

                // ���������봥����֮���ƫ��
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

    // �ƶ��Ҿ�λ��
    private void MoveSelectedObject(Vector2 screenPosition)
    {
        if (selectedObject == null) return;

        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        if (raycastManager.Raycast(screenPosition, hits, TrackableType.Planes))
        {
            Pose hitPose = hits[0].pose;

            // �����ƶ��������㣬�������봥�����ƫ��
            Vector3 newPosition = hitPose.position + offset;

            // ����ֻ�ƶ���ƽ���ϣ�ȷ��y��̶�
            selectedObject.transform.position = new Vector3(newPosition.x, initialObjectPosition.y, newPosition.z);

            // Debugging: output new position values
            Debug.Log($"New Position: {selectedObject.transform.position}");
        }
    }

    // ����˫ָ��������
    private void HandleTwoFingerGesture()
    {
        Touch touch1 = Input.GetTouch(0);
        Touch touch2 = Input.GetTouch(1);

        // ��ȡ��ָ�ľ���仯
        float previousDistance = (touch1.position - touch2.position).magnitude;
        float currentDistance = (touch1.position - touch2.position).magnitude;

        // �Ƚ����εľ��룬�������������
        float scaleFactor = currentDistance / previousDistance;

        // ��������
        if (selectedObject != null)
        {
            Vector3 scale = selectedObject.transform.localScale;
            scale *= scaleFactor;
            selectedObject.transform.localScale = scale;
        }
    }
}
