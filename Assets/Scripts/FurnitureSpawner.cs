/*using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class FurnitureSpawner : MonoBehaviour
{
    public ARRaycastManager raycastManager;
    public GameObject chairPrefab;
    public GameObject tablePrefab;
    public GameObject sofaPrefab;

    public Button chairButton;
    public Button tableButton;
    public Button sofaButton;
    public Button deleteButton;  // ɾ����ť


    private GameObject selectedPrefab; // ��ǰѡ��ļҾ�
    private List<GameObject> spawnedFurniture = new List<GameObject>(); // �洢�����ɵļҾ�

    void Start()
    {
        // �󶨰�ť����¼�
        chairButton.onClick.AddListener(() => PlaceFurniture(chairPrefab));
        tableButton.onClick.AddListener(() => PlaceFurniture(tablePrefab));
        sofaButton.onClick.AddListener(() => PlaceFurniture(sofaPrefab));
        deleteButton.onClick.AddListener(DeleteFurniture);  // ��ɾ����ť
    }

    void PlaceFurniture(GameObject furniturePrefab)
    {
        // ��ȡ�������ǰ����λ��
        Vector3 spawnPosition = Camera.main.transform.position + Camera.main.transform.forward * 1.5f;

        // �� AR Raycast ȷ���Ҿ�����
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        if (raycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.Planes))
        {
            spawnPosition = hits[0].pose.position;
        }

        // ���ɼҾ�
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hits[0].pose.up);
        GameObject newFurniture = Instantiate(furniturePrefab, spawnPosition, rotation);
        spawnedFurniture.Add(newFurniture);
    }

    // ɾ�����һ�����õļҾ�
    void DeleteFurniture()
    {
        if (spawnedFurniture.Count > 0)
        {
            GameObject furnitureToDelete = spawnedFurniture[spawnedFurniture.Count - 1]; // ��ȡ���һ�����õļҾ�
            spawnedFurniture.Remove(furnitureToDelete);  // ���б����Ƴ�
            Destroy(furnitureToDelete); // ɾ����Ϸ����
        }
    }
}
*/


using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class FurnitureSpawner : MonoBehaviour
{
    public ARRaycastManager raycastManager;
    public GameObject[] chairPrefabs;  // �洢��ͬ����Ԥ����
    public Button selectButton;         // ������Ӱ�ť��ʾScrollView
  
    public GameObject scrollView;      // ScrollView
    public Button[] selectButtons;     // �洢����ѡ��ť
    public GameObject[] chairImages;   // �洢����ͼƬ

    private GameObject selectedPrefab; // ��ǰѡ��ļҾ�Ԥ����
    private List<GameObject> spawnedFurniture = new List<GameObject>(); // �洢�����ɵļҾ�
    private bool isScrollViewActive = false; // ��¼ScrollView�Ƿ���ʾ

    void Start()
    {
        // ��ʼ����ScrollView
        scrollView.SetActive(false);

        // �����Ӱ�ť����¼����л�ScrollView����ʾ/����
        selectButton.onClick.AddListener(ToggleScrollView);


        // Ϊÿ������ѡ��ť��ѡ���¼�
        for (int i = 0; i < selectButtons.Length; i++)
        {
            int index = i; // ��ֹ�հ�����
            selectButtons[i].onClick.AddListener(() => SelectChair(index));
        }
    }

    // ������Ӱ�ťʱ���л�ScrollView����ʾ״̬
    void ToggleScrollView()
    {
        isScrollViewActive = !isScrollViewActive;

        if (isScrollViewActive)
        {
            scrollView.SetActive(true); // ��ʾScrollView
        }
        else
        {
            scrollView.SetActive(false); // ����ScrollView
        }
    }

    // ѡ�����Ӳ����õ���ʵ����
    void SelectChair(int index)
    {
        selectedPrefab = chairPrefabs[index]; // ѡ������

        // ��ȡ�������ǰ����λ��
        Vector3 spawnPosition = Camera.main.transform.position + Camera.main.transform.forward * 1.5f;

        // �� AR Raycast ȷ���Ҿ�����
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        if (raycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.Planes))
        {
            spawnPosition = hits[0].pose.position;
        }

        // ��������
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hits[0].pose.up);
        GameObject newFurniture = Instantiate(selectedPrefab, spawnPosition, rotation);
        spawnedFurniture.Add(newFurniture);

        // ����ScrollView
        scrollView.SetActive(false);
        isScrollViewActive = false; // ����״̬
    }
}
