using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CardDrawManager : MonoBehaviour
{
    [Header("UI Ԫ��")]
    public GameObject maskPanel;  // ��ɫ���ֲ�
    public GameObject carddeco;    //����װ��
    public GameObject cardPrefab; // ��ƬԤ����
    public Transform cardParent;  // ��Ƭ������
    public Transform centerPosition; // ��Ƭ��תʱ������λ��
    public Button statueButton; // ������Ƭ��ȡ��ʯ��ť
    public Button closeButton; // �رս���İ�ť

    [Header("��Ƭ��Դ")]
    public Sprite[] attributeSprites; // ���Կ�Ƭ��ͼƬ
    public Sprite coinSprite; // ��ҿ�Ƭ��ͼƬ
    public GameObject starEffectPrefab; //������ЧԤ����
    private List<GameObject> cards = new List<GameObject>(); // ������ɵĿ�Ƭ
    private bool isCardSelected = false; // ����Ƿ��Ѿ�ѡ��Ƭ
    public Sprite backFaceSprite;

    private List<Vector2> cardPositions = new List<Vector2>
    {
        new Vector2(-500, 0),  // ���
        new Vector2(0, 0),     // �м�
        new Vector2(500, 0)    // �Ҳ�
    };
    public List<CardData> attributeCardDataList = new List<CardData>();

    void Start()
    {
        // �󶨵���¼�
        statueButton.onClick.AddListener(StartCardDraw);
        closeButton.onClick.AddListener(CloseCardDrawUI);

        // ȷ����ʼ״̬�����ص�
        maskPanel.SetActive(false);
        carddeco.SetActive(false);
        closeButton.gameObject.SetActive(false); //�رհ�ťĬ������

        attributeCardDataList.Add(new CardData(attributeSprites[0], "+2 ����"));
        attributeCardDataList.Add(new CardData(attributeSprites[1], "+2 ����"));
        attributeCardDataList.Add(new CardData(attributeSprites[2], "+2 ����"));
        attributeCardDataList.Add(new CardData(attributeSprites[3], "+2 ����"));
        attributeCardDataList.Add(new CardData(attributeSprites[4], "+2 ����"));
    }

    /// <summary>
    /// ��ʼ�鿨����
    /// </summary>
    void StartCardDraw()
    {
        // ��ʾ�ɰ�
        maskPanel.SetActive(true);
        carddeco.SetActive(true);

        // �������ſ�Ƭ
        GenerateCards();

        closeButton.gameObject.SetActive(false);
    }

    /// <summary>
    /// �������ſ�Ƭ���������Կ� + һ�Ž�ҿ���
    /// </summary>
    void GenerateCards()
    {
        // ���֮ǰ�Ŀ�Ƭ
        foreach (var card in cards)
        {
            Destroy(card);
        }
        cards.Clear();

        // �����ȡ�������Կ�
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

        // ���Ͻ�ҿ�
        List<(Sprite, string)> cardData = new List<(Sprite, string)>
        {
            (selectedAttributes[0].image, selectedAttributes[0].effectText),
            (selectedAttributes[1].image, selectedAttributes[1].effectText),
            (coinSprite, GetRandomGoldValue())
        };

        // **�������˳��**���ý�ҿ�����̶���ĳ��λ��
        Shuffle(cardData);

        // ���δ������ſ�Ƭ�������õ��̶�λ��
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
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]); // ����λ��
        }
    }


    /// <summary>
    /// �������ſ�Ƭ
    /// </summary>
    GameObject CreateCard(Sprite cardSprite, string effectText, Vector2 position)
    {
        // ������Ϣ����� cardPrefab �Ƿ�Ϊ��
        if (cardPrefab == null)
        {
            Debug.LogError("cardPrefab Ϊ�գ��޷����ɿ�Ƭ��");
            return null;
        }

        // ������Ϣ����� cardParent �Ƿ�Ϊ��
        if (cardParent == null)
        {
            Debug.LogError("cardParent Ϊ�գ��޷����ɿ�Ƭ��");
            return null;
        }

        GameObject card = Instantiate(cardPrefab, cardParent);

        Debug.Log($"��Ƭ {card.name} λ��: {card.transform.position}, �ߴ�: {card.GetComponent<RectTransform>().sizeDelta}");


        // ������Ϣ��ȷ�Ͽ�Ƭ�Ƿ�ɹ�����
        if (card == null)
        {
            Debug.LogError("��Ƭ����ʧ�ܣ�");
            return null;
        }


        // ��ʼ����Ƭ
        Card cardComponent = card.GetComponent<Card>();
        if (cardComponent == null)
        {
            Debug.LogError("cardPrefab ȱ�� Card �����");
            return null;
        }
        cardComponent.SetFaces(cardSprite, backFaceSprite);
        cardComponent.Initialize(this, effectText);

        RectTransform rectTransform = card.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = position;
        rectTransform.localScale = Vector3.one;



        cards.Add(card);

        // ������Ϣ��ȷ�Ͽ�Ƭ�ɹ���ӵ��б�
        Debug.Log("�ɹ�������Ƭ��" + card.name);

        return card;
    }

    /// <summary>
    /// ����������Լӳɣ�ʾ����+2������+2������
    /// </summary>
    string GetRandomCardEffect()
    {
        string[] effects = { "+2 ����", "+2 ����", "+2 ����", "+2 ����", "+2 �ٶ�" };
        return effects[Random.Range(0, effects.Length)];
    }

    /// <summary>
    /// ���������ҽ���
    /// </summary>
    string GetRandomGoldValue()
    {
        int goldAmount = Random.Range(10, 50);
        return $"+{goldAmount} ���";
    }

    /// <summary>
    /// ����Ƭ�����
    /// </summary>
    public void OnCardSelected(Card selectedCard)
    {
        Debug.Log("��Ƭѡ�У�" + selectedCard.gameObject.name);

        // ֹͣ���п�Ƭ�ĵ�����ܣ���ֹ�ظ����
        foreach (var card in cards)
        {
            card.GetComponent<Button>().interactable = false;
        }

        // **��������������Ƭ**
        foreach (var card in cards)
        {
            if (card != selectedCard.gameObject)
            {
                card.SetActive(false); // **����δѡ�еĿ�Ƭ**
            }
        }

        // �ÿ�Ƭ�ƶ������벢��ת
        selectedCard.MoveToCenter(centerPosition.position);
    }

    public void EnableCloseButton()
    {
        closeButton.gameObject.SetActive(true); // ��Ƭ��ת��ɺ�ż���رհ�ť
    }

    /// <summary>
    /// �رճ鿨UI
    /// </summary>
    void CloseCardDrawUI()
    {
        // **�������п�Ƭ**
        foreach (var card in cards)
        {
            Destroy(card);
        }
        cards.Clear(); // **����б���ֹ�ظ�����**

        maskPanel.SetActive(false);
        carddeco.SetActive(false);

        // **���عرհ�ť**
        closeButton.gameObject.SetActive(false);

        isCardSelected = false;
    }
}
