using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CardDrawManager : MonoBehaviour
{
    [Header("UI 元素")]
    public GameObject maskPanel;  // 灰色遮罩层
    public GameObject carddeco;    //卡堆装饰
    public GameObject cardPrefab; // 卡片预制体
    public Transform cardParent;  // 卡片父对象
    public Transform centerPosition; // 卡片翻转时的中央位置
    public Button statueButton; // 触发卡片抽取的石像按钮
    public Button closeButton; // 关闭界面的按钮

    [Header("卡片资源")]
    public Sprite[] attributeSprites; // 属性卡片的图片
    public Sprite coinSprite; // 金币卡片的图片
    public GameObject starEffectPrefab; //粒子特效预制体
    private List<GameObject> cards = new List<GameObject>(); // 存放生成的卡片
    private bool isCardSelected = false; // 标记是否已经选择卡片
    public Sprite backFaceSprite;

    private List<Vector2> cardPositions = new List<Vector2>
    {
        new Vector2(-500, 0),  // 左侧
        new Vector2(0, 0),     // 中间
        new Vector2(500, 0)    // 右侧
    };
    public List<CardData> attributeCardDataList = new List<CardData>();

    void Start()
    {
        // 绑定点击事件
        statueButton.onClick.AddListener(StartCardDraw);
        closeButton.onClick.AddListener(CloseCardDrawUI);

        // 确保初始状态是隐藏的
        maskPanel.SetActive(false);
        carddeco.SetActive(false);
        closeButton.gameObject.SetActive(false); //关闭按钮默认隐藏

        attributeCardDataList.Add(new CardData(attributeSprites[0], "+2 魅力"));
        attributeCardDataList.Add(new CardData(attributeSprites[1], "+2 勇气"));
        attributeCardDataList.Add(new CardData(attributeSprites[2], "+2 灵性"));
        attributeCardDataList.Add(new CardData(attributeSprites[3], "+2 技能"));
        attributeCardDataList.Add(new CardData(attributeSprites[4], "+2 智力"));
    }

    /// <summary>
    /// 开始抽卡流程
    /// </summary>
    void StartCardDraw()
    {
        // 显示蒙版
        maskPanel.SetActive(true);
        carddeco.SetActive(true);

        // 生成三张卡片
        GenerateCards();

        closeButton.gameObject.SetActive(false);
    }

    /// <summary>
    /// 生成三张卡片（两张属性卡 + 一张金币卡）
    /// </summary>
    void GenerateCards()
    {
        // 清空之前的卡片
        foreach (var card in cards)
        {
            Destroy(card);
        }
        cards.Clear();

        // 随机抽取两张属性卡
        List<CardData> selectedAttributes = new List<CardData>();
        List<int> usedIndices = new List<int>();

        while (selectedAttributes.Count < 2)
        {
            int index = Random.Range(0, attributeCardDataList.Count);
            if (!usedIndices.Contains(index))
            {
                usedIndices.Add(index);
                selectedAttributes.Add(attributeCardDataList[index]);
            }
        }

        // 加上金币卡
        List<(Sprite, string)> cardData = new List<(Sprite, string)>
        {
            (selectedAttributes[0].image, selectedAttributes[0].effectText),
            (selectedAttributes[1].image, selectedAttributes[1].effectText),
            (coinSprite, GetRandomGoldValue())
        };

        // **随机打乱顺序**，让金币卡不会固定在某个位置
        Shuffle(cardData);

        // 依次创建三张卡片，并放置到固定位置
        for (int i = 0; i < cardPositions.Count; i++)
        {
            CreateCard(cardData[i].Item1, cardData[i].Item2, cardPositions[i]);
        }
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]); // 交换位置
        }
    }


    /// <summary>
    /// 创建单张卡片
    /// </summary>
    GameObject CreateCard(Sprite cardSprite, string effectText, Vector2 position)
    {
        // 调试信息：检查 cardPrefab 是否为空
        if (cardPrefab == null)
        {
            Debug.LogError("cardPrefab 为空，无法生成卡片！");
            return null;
        }

        // 调试信息：检查 cardParent 是否为空
        if (cardParent == null)
        {
            Debug.LogError("cardParent 为空，无法生成卡片！");
            return null;
        }

        GameObject card = Instantiate(cardPrefab, cardParent);

        Debug.Log($"卡片 {card.name} 位置: {card.transform.position}, 尺寸: {card.GetComponent<RectTransform>().sizeDelta}");


        // 调试信息：确认卡片是否成功生成
        if (card == null)
        {
            Debug.LogError("卡片生成失败！");
            return null;
        }


        // 初始化卡片
        Card cardComponent = card.GetComponent<Card>();
        if (cardComponent == null)
        {
            Debug.LogError("cardPrefab 缺少 Card 组件！");
            return null;
        }
        cardComponent.SetFaces(cardSprite, backFaceSprite);
        cardComponent.Initialize(this, effectText);

        RectTransform rectTransform = card.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = position;
        rectTransform.localScale = Vector3.one;



        cards.Add(card);

        // 调试信息：确认卡片成功添加到列表
        Debug.Log("成功创建卡片：" + card.name);

        return card;
    }

    /// <summary>
    /// 生成随机属性加成（示例：+2智力、+2勇气）
    /// </summary>
    string GetRandomCardEffect()
    {
        string[] effects = { "+2 智力", "+2 灵性", "+2 勇气", "+2 体力", "+2 速度" };
        return effects[Random.Range(0, effects.Length)];
    }

    /// <summary>
    /// 生成随机金币奖励
    /// </summary>
    string GetRandomGoldValue()
    {
        int goldAmount = Random.Range(10, 50);
        return $"+{goldAmount} 金币";
    }

    /// <summary>
    /// 处理卡片被点击
    /// </summary>
    public void OnCardSelected(Card selectedCard)
    {
        Debug.Log("卡片选中：" + selectedCard.gameObject.name);

        // 停止所有卡片的点击功能，防止重复点击
        foreach (var card in cards)
        {
            card.GetComponent<Button>().interactable = false;
        }

        // **隐藏所有其他卡片**
        foreach (var card in cards)
        {
            if (card != selectedCard.gameObject)
            {
                card.SetActive(false); // **隐藏未选中的卡片**
            }
        }

        // 让卡片移动到中央并翻转
        selectedCard.MoveToCenter(centerPosition.position);
    }

    public void EnableCloseButton()
    {
        closeButton.gameObject.SetActive(true); // 卡片翻转完成后才激活关闭按钮
    }

    /// <summary>
    /// 关闭抽卡UI
    /// </summary>
    void CloseCardDrawUI()
    {
        // **隐藏所有卡片**
        foreach (var card in cards)
        {
            Destroy(card);
        }
        cards.Clear(); // **清空列表，防止重复生成**

        maskPanel.SetActive(false);
        carddeco.SetActive(false);

        // **隐藏关闭按钮**
        closeButton.gameObject.SetActive(false);

        isCardSelected = false;
    }
}
