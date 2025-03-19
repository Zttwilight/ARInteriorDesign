/*using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class FurnitureSpawner : MonoBehaviour
{
    public ARRaycastManager raycastManager;
    public GameObject[] chairPrefabs;  // 存储不同椅子预制体
    public Button selectButton;         // 点击椅子按钮显示ScrollView
  
    public GameObject scrollView;      // ScrollView
    public Button[] selectButtons;     // 存储椅子选择按钮
    public GameObject[] chairImages;   // 存储椅子图片

    public Button deleteButton;
    public Button changeMaterialButton;
    public GameObject materialPanel;   // 存储材质面板
    private GameObject selectedFurniture;
    public Material[] materials;  // 存储不同的材质
    private Material selectedMaterial;  // 当前选中的材质
    public Button[] materialButtons;  // 存储材质按钮


    private GameObject selectedPrefab; // 当前选择的家具预制体
    private List<GameObject> spawnedFurniture = new List<GameObject>(); // 存储已生成的家具
    private bool isScrollViewActive = false; // 记录ScrollView是否显示

    void Start()
    {
        // 初始隐藏ScrollView
        scrollView.SetActive(false);
        materialPanel.SetActive(false);

        deleteButton.gameObject.SetActive(false);
        changeMaterialButton.gameObject.SetActive(false);

        // 绑定椅子按钮点击事件，切换ScrollView的显示/隐藏
        selectButton.onClick.AddListener(ToggleScrollView);


        // 为每个椅子选择按钮绑定选择事件
        for (int i = 0; i < selectButtons.Length; i++)
        {
            int index = i; // 防止闭包问题
            selectButtons[i].onClick.AddListener(() => SelectChair(index));
        }
        // 初始隐藏材质面板
        materialPanel.SetActive(false);

        // 绑定删除按钮和材质按钮的点击事件
        deleteButton.onClick.AddListener(DeleteFurniture);
        changeMaterialButton.onClick.AddListener(() => ChangeMaterial(selectedFurniture));

        // 绑定材质按钮点击事件
        for (int i = 0; i < materialButtons.Length; i++)
        {
            int index = i; // 防止闭包问题
            materialButtons[i].onClick.AddListener(() => SelectMaterial(index));
        }

    }

    // 点击椅子按钮时，切换ScrollView的显示状态
    void ToggleScrollView()
    {
        isScrollViewActive = !isScrollViewActive;

        if (isScrollViewActive)
        {
            scrollView.SetActive(true); // 显示ScrollView
        }
        else
        {
            scrollView.SetActive(false); // 隐藏ScrollView
        }
    }

    // 选择椅子并放置到现实世界
    void SelectChair(int index)
    {
        selectedPrefab = chairPrefabs[index]; // 选择椅子

        // 获取摄像机正前方的位置
        Vector3 spawnPosition = Camera.main.transform.position + Camera.main.transform.forward * 1.5f;

        // 用 AR Raycast 确保家具贴地
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        if (raycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.Planes))
        {
            spawnPosition = hits[0].pose.position;
        }

        // 生成椅子
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hits[0].pose.up);
        GameObject newFurniture = Instantiate(selectedPrefab, spawnPosition, rotation);
        spawnedFurniture.Add(newFurniture);

        SelectFurniture(newFurniture);

        // 隐藏ScrollView
        scrollView.SetActive(false);
        isScrollViewActive = false; // 更新状态
    }

    void SelectFurniture(GameObject furniture)
    {
        selectedFurniture = furniture;
        deleteButton.gameObject.SetActive(true);
        changeMaterialButton.gameObject.SetActive(true);
    }

    // 删除家具
    void DeleteFurniture()
    {
        if (spawnedFurniture.Count > 0)
        {
            GameObject furnitureToDelete = spawnedFurniture[spawnedFurniture.Count - 1];
            spawnedFurniture.Remove(furnitureToDelete);
            Destroy(furnitureToDelete);

            // 隐藏材质和删除按钮
            deleteButton.gameObject.SetActive(false);
            changeMaterialButton.gameObject.SetActive(false);
            selectedFurniture = null;  // 清空当前选中家具
        }
    }
    public void ShowMaterialPanel()
    {
        if (selectedFurniture != null)  // 只有在选择了家具之后才显示材质面板
        {
            materialPanel.SetActive(true);  // 显示材质面板

            // 隐藏材质按钮
            changeMaterialButton.gameObject.SetActive(false);  // 隐藏材质按钮
        }
    }

    // 选择材质
    public void SelectMaterial(int index)
    {
        selectedMaterial = materials[index];  // 选择材质
        if (selectedFurniture != null)
        {
            ChangeMaterial(selectedFurniture);  // 改变选中家具的材质
        }

        // 选择材质后，隐藏材质面板并恢复显示材质按钮
        materialPanel.SetActive(false);  // 隐藏材质面板
        changeMaterialButton.gameObject.SetActive(true);  // 恢复显示材质按钮
    }

    // 改变家具材质
    void ChangeMaterial(GameObject furniture)
    {
        Renderer renderer = furniture.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = selectedMaterial;  // 改变材质
        }
    }
}
*/



using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using UnityEngine.EventSystems;  // 需要引入 UI 交互库

public class FurnitureSpawner : MonoBehaviour
{
    public ARRaycastManager raycastManager;
    public GameObject[] chairPrefabs;  // 存储不同椅子预制体
    public Button selectButton;         // 点击椅子按钮显示ScrollView

    public GameObject scrollView;      // ScrollView
    public Button[] selectButtons;     // 存储椅子选择按钮
    public GameObject[] chairImages;   // 存储椅子图片
    public GameObject deleteButton;    // 删除按钮（需要在 UI 里创建一个 Button）
    public GameObject materialButton; // 材质转换按钮
    public GameObject materialSelectionPanel; // 材质选择面板


    public Button materialButton1; // 材质按钮 1
    public Button materialButton2; // 材质按钮 2
    public Button materialButton3; // 材质按钮 3

    public Material material1; // 材质 1
    public Material material2; // 材质 2
    public Material material3; // 材质 3


    private GameObject selectedPrefab; // 当前选择的家具预制体
    private GameObject selectedFurniture; // 记录用户点击的家具
    private List<GameObject> spawnedFurniture = new List<GameObject>(); // 存储已生成的家具
    private bool isScrollViewActive = false; // 记录ScrollView是否显示

    void Start()
    {
        // 初始隐藏ScrollView
        scrollView.SetActive(false);
        deleteButton.SetActive(false);
        materialButton.SetActive(false);
        materialSelectionPanel.SetActive(false);

        //绑定材质按钮点击事件
        materialButton1.onClick.AddListener(() => ApplyMaterial(material1));
        materialButton2.onClick.AddListener(() => ApplyMaterial(material2));
        materialButton3.onClick.AddListener(() => ApplyMaterial(material3));

        // 绑定椅子按钮点击事件，切换ScrollView的显示/隐藏
        selectButton.onClick.AddListener(ToggleScrollView);


        // 为每个椅子选择按钮绑定选择事件
        for (int i = 0; i < selectButtons.Length; i++)
        {
            int index = i; // 防止闭包问题
            selectButtons[i].onClick.AddListener(() => SelectChair(index));
        }

        // 绑定删除按钮的点击事件
        deleteButton.GetComponent<Button>().onClick.AddListener(DeleteSelectedFurniture);
        // 绑定材质转换按钮的点击事件
        materialButton.GetComponent<Button>().onClick.AddListener(ShowMaterialSelection);

    }

    void Update()
    {
        DetectFurnitureClick();
    }

    // 点击椅子按钮时，切换ScrollView的显示状态
    void ToggleScrollView()
    {
        isScrollViewActive = !isScrollViewActive;

        if (isScrollViewActive)
        {
            scrollView.SetActive(true); // 显示ScrollView
        }
        else
        {
            scrollView.SetActive(false); // 隐藏ScrollView
        }
    }



    // 选择椅子并放置到现实世界
    void SelectChair(int index)
    {
        selectedPrefab = chairPrefabs[index]; // 选择椅子

        // 获取摄像机正前方的位置
        Vector3 spawnPosition = Camera.main.transform.position + Camera.main.transform.forward * 1.5f;

        // 用 AR Raycast 确保家具贴地
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        if (raycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.Planes))
        {
            spawnPosition = hits[0].pose.position;
        }

        // 生成椅子
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hits[0].pose.up);
        GameObject newFurniture = Instantiate(selectedPrefab, spawnPosition, rotation);
        spawnedFurniture.Add(newFurniture);

        // 给家具添加 Collider 和 Rigidbody（用于检测点击）
        AddInteractionComponents(newFurniture);

        // 隐藏ScrollView
        scrollView.SetActive(false);
        isScrollViewActive = false; // 更新状态
    }


    // 给家具添加 Collider 和 Rigidbody（确保家具可被点击）
    void AddInteractionComponents(GameObject furniture)
    {
        if (furniture.GetComponent<Collider>() == null)
        {
            furniture.AddComponent<BoxCollider>(); // 添加 Collider 以检测点击
        }

        if (furniture.GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = furniture.AddComponent<Rigidbody>();
            rb.useGravity = false; // 防止家具掉落
            rb.isKinematic = true; // 避免物理干扰
        }
    }

    // 检测用户点击了哪个家具
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
            // 点击的是空白区域
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


    // 选择家具并显示删除按钮
    void SelectFurniture(GameObject furniture)
    {
        selectedFurniture = furniture;
        deleteButton.SetActive(true);
        materialButton.SetActive(true); // 显示材质转换按钮

        // 把删除按钮放到点击的家具上方
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(furniture.transform.position);
        deleteButton.transform.position = screenPosition + new Vector3(0, 100, 0);
        materialButton.transform.position = screenPosition + new Vector3(0, -50, 0);

    }

    public void ShowMaterialSelection()
    {
        if (selectedFurniture == null) return;
        materialSelectionPanel.SetActive(true);

        // 将材质选择面板放在 UI 屏幕中心
        materialSelectionPanel.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);

        // 隐藏删除按钮和材质转换按钮
        deleteButton.SetActive(false);
        materialButton.SetActive(false);
    }


    public void ApplyMaterial(Material newMaterial)
    {
        if (selectedFurniture == null) return;

        // 获取所有 MeshRenderer
        MeshRenderer[] meshRenderers = selectedFurniture.GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer renderer in meshRenderers)
        {
            renderer.material = newMaterial; // 更换所有部分的材质
        }

        // 关闭材质球 UI
        materialSelectionPanel.SetActive(false);
        // 重新显示删除按钮和材质转换按钮
        deleteButton.SetActive(true);
        materialButton.SetActive(true);
    }



    // 删除选中的家具
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
