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
    public Button deleteButton;  // 删除按钮


    private GameObject selectedPrefab; // 当前选择的家具
    private List<GameObject> spawnedFurniture = new List<GameObject>(); // 存储已生成的家具

    void Start()
    {
        // 绑定按钮点击事件
        chairButton.onClick.AddListener(() => PlaceFurniture(chairPrefab));
        tableButton.onClick.AddListener(() => PlaceFurniture(tablePrefab));
        sofaButton.onClick.AddListener(() => PlaceFurniture(sofaPrefab));
        deleteButton.onClick.AddListener(DeleteFurniture);  // 绑定删除按钮
    }

    void PlaceFurniture(GameObject furniturePrefab)
    {
        // 获取摄像机正前方的位置
        Vector3 spawnPosition = Camera.main.transform.position + Camera.main.transform.forward * 1.5f;

        // 用 AR Raycast 确保家具贴地
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        if (raycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.Planes))
        {
            spawnPosition = hits[0].pose.position;
        }

        // 生成家具
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hits[0].pose.up);
        GameObject newFurniture = Instantiate(furniturePrefab, spawnPosition, rotation);
        spawnedFurniture.Add(newFurniture);
    }

    // 删除最后一个放置的家具
    void DeleteFurniture()
    {
        if (spawnedFurniture.Count > 0)
        {
            GameObject furnitureToDelete = spawnedFurniture[spawnedFurniture.Count - 1]; // 获取最后一个放置的家具
            spawnedFurniture.Remove(furnitureToDelete);  // 从列表中移除
            Destroy(furnitureToDelete); // 删除游戏对象
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
    public GameObject[] chairPrefabs;  // 存储不同椅子预制体
    public Button selectButton;         // 点击椅子按钮显示ScrollView
  
    public GameObject scrollView;      // ScrollView
    public Button[] selectButtons;     // 存储椅子选择按钮
    public GameObject[] chairImages;   // 存储椅子图片

    private GameObject selectedPrefab; // 当前选择的家具预制体
    private List<GameObject> spawnedFurniture = new List<GameObject>(); // 存储已生成的家具
    private bool isScrollViewActive = false; // 记录ScrollView是否显示

    void Start()
    {
        // 初始隐藏ScrollView
        scrollView.SetActive(false);

        // 绑定椅子按钮点击事件，切换ScrollView的显示/隐藏
        selectButton.onClick.AddListener(ToggleScrollView);


        // 为每个椅子选择按钮绑定选择事件
        for (int i = 0; i < selectButtons.Length; i++)
        {
            int index = i; // 防止闭包问题
            selectButtons[i].onClick.AddListener(() => SelectChair(index));
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

        // 隐藏ScrollView
        scrollView.SetActive(false);
        isScrollViewActive = false; // 更新状态
    }
}
