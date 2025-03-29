using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.XR.ARFoundation;

public class ARUnitLab : MonoBehaviour
{
    // 引用球的预设和机器人预设
    [SerializeField] private GameObject _ballPrefab;
    [SerializeField] private GameObject _robotPrefab;

    // 引用 TMP_Text 用于显示 AR 系统状态
    [SerializeField] private TMP_Text _stateText;

    // AR Session 相关的变量
    private ARSession arSession;

    private void Start()
    {
        // 获取 ARSession 实例
        arSession = FindObjectOfType<ARSession>();

        // 添加 ARSession 状态改变的事件监听
        ARSession.stateChanged += OnARSessionStateChanged;
    }

    // 发射球的方法
    public void ShootBall()
    {
        // 实例化一个新的球
        GameObject newBall = Instantiate(_ballPrefab);

        // 设置球的初始位置为相机位置
        newBall.transform.position = Camera.main.transform.position;

        // 获取球的刚体组件
        Rigidbody rb = newBall.GetComponent<Rigidbody>();

        // 给球施加一个力，使其发射
        rb.AddForce(5000 * Camera.main.transform.forward);
    }

    // 处理 ARSession 状态变化的回调函数
    private void OnARSessionStateChanged(ARSessionStateChangedEventArgs args)
    {
        // 更新显示的 AR 状态
        _stateText.text = "AR Session State: " + args.state.ToString();
    }

    // 在更新射线投射时，确保点击不发生在 UI 元素上
    private void Update()
    {
        // 监听触摸事件
        foreach (Touch touch in Input.touches)
        {
            // 检查触摸是否发生在 UI 上
            if (!EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                // 在这里执行射线投射逻辑 (如发射球、创建机器人等)
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(touch.position);

                if (Physics.Raycast(ray, out hit))
                {
                    // 如果射线碰到对象，做一些处理（如与机器人互动等）
                    if (hit.transform.CompareTag("Robot"))
                    {
                        // 如果碰到机器人，添加物理效果
                        Rigidbody rb = hit.transform.GetComponent<Rigidbody>();
                        if (rb != null)
                        {
                            // 给机器人施加力
                            rb.AddForce(Vector3.up * 1000);
                        }
                    }
                }
            }
        }
    }

    // 为机器人启用物理效果的方法
    private void InstantiateRobot(Vector3 position)
    {
        // 实例化机器人
        GameObject newRobot = Instantiate(_robotPrefab, position, Quaternion.identity);

        // 获取机器人的刚体组件
        Rigidbody rb = newRobot.GetComponent<Rigidbody>();

        // 启用物理效果
        rb.useGravity = true;
        rb.isKinematic = false;

        // 初始化速度和角速度为零，确保机器人是静止的
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void OnDestroy()
    {
        // 移除事件监听器
        ARSession.stateChanged -= OnARSessionStateChanged;
    }
}
