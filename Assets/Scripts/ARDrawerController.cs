using UnityEngine;

public class ARDrawerController : MonoBehaviour
{
    public Animator drawer1;
    public Animator drawer2;
    public Animator drawer3;

    private float holdTime = 2.0f;
    private bool isHolding = false;
    private float holdTimer = 0f;
    private bool animationPlayed = false;

    void Start()
    {
        animationPlayed = false;  // 确保动画不会一放置就播放
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            HandleTouch(touch.phase);
        }
        else if (Input.GetMouseButton(0))  // 用鼠标测试
        {
            HandleTouch(TouchPhase.Stationary);
        }
    }

    void HandleTouch(TouchPhase phase)
    {
        if (phase == TouchPhase.Began)
        {
            if (!animationPlayed)
            {
                isHolding = true;
                holdTimer = 0f;
            }
        }
        else if (phase == TouchPhase.Stationary)
        {
            if (isHolding)
            {
                holdTimer += Time.deltaTime;
                if (holdTimer >= holdTime && !animationPlayed)
                {
                    PlayDrawerAnimations();
                    animationPlayed = true;
                }
            }
        }
        else if (phase == TouchPhase.Ended)
        {
            isHolding = false;
        }
    }

    void PlayDrawerAnimations()
    {
        drawer1.Play("NewState0", 0, 0);
        drawer2.Play("3NewAnimation", 0, 0);
        drawer3.Play("NewAnimation", 0, 0);
    }
}