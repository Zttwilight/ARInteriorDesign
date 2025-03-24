using UnityEngine;
using UnityEngine.EventSystems;

public class ShootBallController : MonoBehaviour
{
    [SerializeField] private GameObject _ballPrefab;
    [SerializeField] private float shootForce = 5000f;

    public void ShootBall()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        // 创建一个新的球
        GameObject newBall = Instantiate(_ballPrefab);
        newBall.transform.position = Camera.main.transform.position;

        // 获取 Rigidbody 并添加力
        Rigidbody rb = newBall.GetComponent<Rigidbody>();
        rb.AddForce(Camera.main.transform.forward * shootForce);
    }
}
