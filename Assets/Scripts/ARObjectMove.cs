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
        // ��ⵥָ��������
        if (Input.touchCount == 1)
        {
            touchPosition = Input.GetTouch(0).position;
            Ray ray = arCamera.ScreenPointToRay(touchPosition);
            RaycastHit hit;

            // ��������Ƿ��������
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.CompareTag("Furniture")) // ȷ���ǿ����ƶ�������
                {
                    if (selectedObject == null)
                    {
                        selectedObject = hit.transform.gameObject;
                    }

                    // �������ƶ�������λ��
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
