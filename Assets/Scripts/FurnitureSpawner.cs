/*using UnityEngine;
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

    public Button deleteButton;
    public Button changeMaterialButton;
    public GameObject materialPanel;   // �洢�������
    private GameObject selectedFurniture;
    public Material[] materials;  // �洢��ͬ�Ĳ���
    private Material selectedMaterial;  // ��ǰѡ�еĲ���
    public Button[] materialButtons;  // �洢���ʰ�ť


    private GameObject selectedPrefab; // ��ǰѡ��ļҾ�Ԥ����
    private List<GameObject> spawnedFurniture = new List<GameObject>(); // �洢�����ɵļҾ�
    private bool isScrollViewActive = false; // ��¼ScrollView�Ƿ���ʾ

    void Start()
    {
        // ��ʼ����ScrollView
        scrollView.SetActive(false);
        materialPanel.SetActive(false);

        deleteButton.gameObject.SetActive(false);
        changeMaterialButton.gameObject.SetActive(false);

        // �����Ӱ�ť����¼����л�ScrollView����ʾ/����
        selectButton.onClick.AddListener(ToggleScrollView);


        // Ϊÿ������ѡ��ť��ѡ���¼�
        for (int i = 0; i < selectButtons.Length; i++)
        {
            int index = i; // ��ֹ�հ�����
            selectButtons[i].onClick.AddListener(() => SelectChair(index));
        }
        // ��ʼ���ز������
        materialPanel.SetActive(false);

        // ��ɾ����ť�Ͳ��ʰ�ť�ĵ���¼�
        deleteButton.onClick.AddListener(DeleteFurniture);
        changeMaterialButton.onClick.AddListener(() => ChangeMaterial(selectedFurniture));

        // �󶨲��ʰ�ť����¼�
        for (int i = 0; i < materialButtons.Length; i++)
        {
            int index = i; // ��ֹ�հ�����
            materialButtons[i].onClick.AddListener(() => SelectMaterial(index));
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

        SelectFurniture(newFurniture);

        // ����ScrollView
        scrollView.SetActive(false);
        isScrollViewActive = false; // ����״̬
    }

    void SelectFurniture(GameObject furniture)
    {
        selectedFurniture = furniture;
        deleteButton.gameObject.SetActive(true);
        changeMaterialButton.gameObject.SetActive(true);
    }

    // ɾ���Ҿ�
    void DeleteFurniture()
    {
        if (spawnedFurniture.Count > 0)
        {
            GameObject furnitureToDelete = spawnedFurniture[spawnedFurniture.Count - 1];
            spawnedFurniture.Remove(furnitureToDelete);
            Destroy(furnitureToDelete);

            // ���ز��ʺ�ɾ����ť
            deleteButton.gameObject.SetActive(false);
            changeMaterialButton.gameObject.SetActive(false);
            selectedFurniture = null;  // ��յ�ǰѡ�мҾ�
        }
    }
    public void ShowMaterialPanel()
    {
        if (selectedFurniture != null)  // ֻ����ѡ���˼Ҿ�֮�����ʾ�������
        {
            materialPanel.SetActive(true);  // ��ʾ�������

            // ���ز��ʰ�ť
            changeMaterialButton.gameObject.SetActive(false);  // ���ز��ʰ�ť
        }
    }

    // ѡ�����
    public void SelectMaterial(int index)
    {
        selectedMaterial = materials[index];  // ѡ�����
        if (selectedFurniture != null)
        {
            ChangeMaterial(selectedFurniture);  // �ı�ѡ�мҾߵĲ���
        }

        // ѡ����ʺ����ز�����岢�ָ���ʾ���ʰ�ť
        materialPanel.SetActive(false);  // ���ز������
        changeMaterialButton.gameObject.SetActive(true);  // �ָ���ʾ���ʰ�ť
    }

    // �ı�Ҿ߲���
    void ChangeMaterial(GameObject furniture)
    {
        Renderer renderer = furniture.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = selectedMaterial;  // �ı����
        }
    }
}
*/



using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using UnityEngine.EventSystems;  // ��Ҫ���� UI ������

public class FurnitureSpawner : MonoBehaviour
{
    public ARRaycastManager raycastManager;
    public GameObject[] chairPrefabs;  // �洢��ͬ����Ԥ����
    public Button selectButton;         // ������Ӱ�ť��ʾScrollView

    public GameObject scrollView;      // ScrollView
    public Button[] selectButtons;     // �洢����ѡ��ť
    public GameObject[] chairImages;   // �洢����ͼƬ
    public GameObject deleteButton;    // ɾ����ť����Ҫ�� UI �ﴴ��һ�� Button��
    public GameObject materialButton; // ����ת����ť
    public GameObject materialSelectionPanel; // ����ѡ�����


    public Button materialButton1; // ���ʰ�ť 1
    public Button materialButton2; // ���ʰ�ť 2
    public Button materialButton3; // ���ʰ�ť 3

    public Material material1; // ���� 1
    public Material material2; // ���� 2
    public Material material3; // ���� 3


    private GameObject selectedPrefab; // ��ǰѡ��ļҾ�Ԥ����
    private GameObject selectedFurniture; // ��¼�û�����ļҾ�
    private List<GameObject> spawnedFurniture = new List<GameObject>(); // �洢�����ɵļҾ�
    private bool isScrollViewActive = false; // ��¼ScrollView�Ƿ���ʾ

    void Start()
    {
        // ��ʼ����ScrollView
        scrollView.SetActive(false);
        deleteButton.SetActive(false);
        materialButton.SetActive(false);
        materialSelectionPanel.SetActive(false);

        //�󶨲��ʰ�ť����¼�
        materialButton1.onClick.AddListener(() => ApplyMaterial(material1));
        materialButton2.onClick.AddListener(() => ApplyMaterial(material2));
        materialButton3.onClick.AddListener(() => ApplyMaterial(material3));

        // �����Ӱ�ť����¼����л�ScrollView����ʾ/����
        selectButton.onClick.AddListener(ToggleScrollView);


        // Ϊÿ������ѡ��ť��ѡ���¼�
        for (int i = 0; i < selectButtons.Length; i++)
        {
            int index = i; // ��ֹ�հ�����
            selectButtons[i].onClick.AddListener(() => SelectChair(index));
        }

        // ��ɾ����ť�ĵ���¼�
        deleteButton.GetComponent<Button>().onClick.AddListener(DeleteSelectedFurniture);
        // �󶨲���ת����ť�ĵ���¼�
        materialButton.GetComponent<Button>().onClick.AddListener(ShowMaterialSelection);

    }

    void Update()
    {
        DetectFurnitureClick();
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

        // ���Ҿ���� Collider �� Rigidbody�����ڼ������
        AddInteractionComponents(newFurniture);

        // ����ScrollView
        scrollView.SetActive(false);
        isScrollViewActive = false; // ����״̬
    }


    // ���Ҿ���� Collider �� Rigidbody��ȷ���Ҿ߿ɱ������
    void AddInteractionComponents(GameObject furniture)
    {
        if (furniture.GetComponent<Collider>() == null)
        {
            furniture.AddComponent<BoxCollider>(); // ��� Collider �Լ����
        }

        if (furniture.GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = furniture.AddComponent<Rigidbody>();
            rb.useGravity = false; // ��ֹ�Ҿߵ���
            rb.isKinematic = true; // �����������
        }
    }

    // ����û�������ĸ��Ҿ�
    void DetectFurnitureClick()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (spawnedFurniture.Contains(hit.transform.gameObject))
                {
                    SelectFurniture(hit.transform.gameObject);
                    return;
                }
            }
            // ������ǿհ�����
            DeselectFurniture();
        }
    }

    void DeselectFurniture()
    {
        selectedFurniture = null;
        deleteButton.SetActive(false);
        materialButton.SetActive(false);
        materialSelectionPanel.SetActive(false);
    }


    // ѡ��Ҿ߲���ʾɾ����ť
    void SelectFurniture(GameObject furniture)
    {
        selectedFurniture = furniture;
        deleteButton.SetActive(true);
        materialButton.SetActive(true); // ��ʾ����ת����ť

        // ��ɾ����ť�ŵ�����ļҾ��Ϸ�
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(furniture.transform.position);
        deleteButton.transform.position = screenPosition + new Vector3(0, 100, 0);
        materialButton.transform.position = screenPosition + new Vector3(0, -50, 0);

    }

    public void ShowMaterialSelection()
    {
        if (selectedFurniture == null) return;
        materialSelectionPanel.SetActive(true);

        // ������ѡ�������� UI ��Ļ����
        materialSelectionPanel.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);

        // ����ɾ����ť�Ͳ���ת����ť
        deleteButton.SetActive(false);
        materialButton.SetActive(false);
    }


    public void ApplyMaterial(Material newMaterial)
    {
        if (selectedFurniture == null) return;

        // ��ȡ���� MeshRenderer
        MeshRenderer[] meshRenderers = selectedFurniture.GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer renderer in meshRenderers)
        {
            renderer.material = newMaterial; // �������в��ֵĲ���
        }

        // �رղ����� UI
        materialSelectionPanel.SetActive(false);
        // ������ʾɾ����ť�Ͳ���ת����ť
        deleteButton.SetActive(true);
        materialButton.SetActive(true);
    }



    // ɾ��ѡ�еļҾ�
    void DeleteSelectedFurniture()
    {
        if (selectedFurniture != null)
        {
            spawnedFurniture.Remove(selectedFurniture);
            Destroy(selectedFurniture);
            selectedFurniture = null;
            deleteButton.SetActive(false);
            materialButton.SetActive(false);
            materialSelectionPanel.SetActive(false);
        }
    }



}
