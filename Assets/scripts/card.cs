using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler

{
    private CardDrawManager manager;
    private string effectText;


    [Header("卡片 UI 元素")]
    [SerializeField] private Image backFace;  // 卡片背面
    [SerializeField] private Image frontFace; // 卡片正面
    [SerializeField] private TextMeshProUGUI attributeText; // 翻转后显示的属性

    [Header("交互参数")]
    public float scaleFactor = 1.1f; // 鼠标悬停时的放大倍率
    public float scaleDuration = 0.2f; // 放大动画的时间
    private Vector3 originalScale; // 记录卡片原始大小
    private bool isSelected = false; // 记录是否已选中

    public GameObject starEffectPrefab; //粒子特效预制体
    private GameObject activeStarEffect; // 记录当前的粒子对象

    public void Initialize(CardDrawManager cardManager, string text)
    {
        manager = cardManager;
        effectText = text;

        //记录卡片初始大小
        originalScale = transform.localScale;

        // 获取粒子特效
        starEffectPrefab = manager.starEffectPrefab;

        Debug.Log("Card 初始化时 starEffectPrefab: " + (starEffectPrefab != null ? "存在" : "为空！"));

        // 监听点击事件
        GetComponent<Button>().onClick.AddListener(() => OnPointerClick(null));
    }

    public void SetFaces(Sprite front, Sprite back)
    {
        frontFace.sprite = front;
        backFace.sprite = back;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isSelected) return; // 防止重复点击
        isSelected = true;


        Debug.Log("卡片被点击：" + gameObject.name);

        // **选中卡片后，立即销毁粒子，防止它跟随移动**
        if (activeStarEffect != null)
        {
            Destroy(activeStarEffect);
            activeStarEffect = null;
        }

        // 通知 CardDrawManager 选中了卡片
        manager.OnCardSelected(this);
    }

    // **鼠标悬停时，放大卡片 & 生成粒子**
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isSelected) return; // 如果已经选中，则不放大
        if (activeStarEffect != null) return; // 如果粒子已经存在，不再重复生成

        LeanTween.scale(gameObject, originalScale * scaleFactor, scaleDuration).setEaseOutQuad();
        SpawnStarEffect(); // 生成星光粒子
    }

    // **鼠标移开时，恢复卡片大小**
    public void OnPointerExit(PointerEventData eventData)
    {
        if (isSelected) return; // 如果已经选中，则不缩小
        LeanTween.scale(gameObject, originalScale, scaleDuration).setEaseOutQuad();

        // 停止粒子效果
        if (activeStarEffect != null)
        {
            ParticleSystem ps = activeStarEffect.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Stop(); // 立即停止粒子播放
            }
            Destroy(activeStarEffect, 0.3f); // 0.3 秒后销毁，不要等太久
            activeStarEffect = null; // 清空引用，防止重复销毁
        }
    }

    // **生成星光粒子**
    void SpawnStarEffect()
    {

        if (starEffectPrefab != null && activeStarEffect == null)
        {
            Vector3 worldPos = transform.position;
            activeStarEffect = Instantiate(starEffectPrefab, worldPos, Quaternion.identity);

            ParticleSystem ps = activeStarEffect.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
            }
        }
        else
        {
            Debug.LogError("starEffectPrefab 为空，请检查 CardDrawManager 是否正确赋值！");
        }
    }



    // **让卡片移动到中央**
    public void MoveToCenter(Vector3 centerPosition)
    {
        LeanTween.move(gameObject, centerPosition, 0.5f).setOnComplete(() =>
        {
            FlipCard(); // **移动完成后翻转**
        });
    }

    // **卡片翻转**
    public void FlipCard()
    {
        LeanTween.rotateY(gameObject, 90, 0.3f).setOnComplete(() =>
        {
            backFace.gameObject.SetActive(false);
            frontFace.gameObject.SetActive(true);
            attributeText.text = effectText;

            LeanTween.rotateY(gameObject, 0, 0.3f).setOnComplete(() =>
            {
                // 通知 CardDrawManager 启用关闭按钮
                manager.EnableCloseButton();
            });
        });
    }
}

