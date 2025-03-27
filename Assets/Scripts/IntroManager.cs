using UnityEngine;
using UnityEngine.UI;

public class IntroManager : MonoBehaviour
{
    public Image introImage; // 图片组件
    public Sprite[] introSprites; // 需要切换的图片数组
    public Button nextButton;
    public Button endButton;
    private int currentIndex = 0;

    void Start()
    {
        if (introSprites.Length > 0 && introImage != null)
        {
            introImage.sprite = introSprites[0]; // 显示第一张图片
        }
        nextButton.onClick.AddListener(NextImage);
        endButton.onClick.AddListener(HideImage);
        endButton.gameObject.SetActive(false); // 初始隐藏 end 按钮
    }

    void NextImage()
    {
        if (currentIndex < introSprites.Length - 1)
        {
            currentIndex++;
            introImage.sprite = introSprites[currentIndex];
        }
        
        if (currentIndex == introSprites.Length - 1)
        {
            nextButton.gameObject.SetActive(false); // 隐藏 next 按钮
            endButton.gameObject.SetActive(true); // 显示 end 按钮
        }
    }

    void HideImage()
    {
        introImage.gameObject.SetActive(false); // 隐藏图片
        nextButton.gameObject.SetActive(false); // 隐藏 next 按钮
        endButton.gameObject.SetActive(false); // 隐藏 end 按钮
    }
}
