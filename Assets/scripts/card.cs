using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler

{
    private CardDrawManager manager;
    private string effectText;


    [Header("��Ƭ UI Ԫ��")]
    [SerializeField] private Image backFace;  // ��Ƭ����
    [SerializeField] private Image frontFace; // ��Ƭ����
    [SerializeField] private TextMeshProUGUI attributeText; // ��ת����ʾ������

    [Header("��������")]
    public float scaleFactor = 1.1f; // �����ͣʱ�ķŴ���
    public float scaleDuration = 0.2f; // �Ŵ󶯻���ʱ��
    private Vector3 originalScale; // ��¼��Ƭԭʼ��С
    private bool isSelected = false; // ��¼�Ƿ���ѡ��

    public GameObject starEffectPrefab; //������ЧԤ����
    private GameObject activeStarEffect; // ��¼��ǰ�����Ӷ���

    public void Initialize(CardDrawManager cardManager, string text)
    {
        manager = cardManager;
        effectText = text;

        //��¼��Ƭ��ʼ��С
        originalScale = transform.localScale;

        // ��ȡ������Ч
        starEffectPrefab = manager.starEffectPrefab;

        Debug.Log("Card ��ʼ��ʱ starEffectPrefab: " + (starEffectPrefab != null ? "����" : "Ϊ�գ�"));

        // ��������¼�
        GetComponent<Button>().onClick.AddListener(() => OnPointerClick(null));
    }

    public void SetFaces(Sprite front, Sprite back)
    {
        frontFace.sprite = front;
        backFace.sprite = back;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isSelected) return; // ��ֹ�ظ����
        isSelected = true;


        Debug.Log("��Ƭ�������" + gameObject.name);

        // **ѡ�п�Ƭ�������������ӣ���ֹ�������ƶ�**
        if (activeStarEffect != null)
        {
            Destroy(activeStarEffect);
            activeStarEffect = null;
        }

        // ֪ͨ CardDrawManager ѡ���˿�Ƭ
        manager.OnCardSelected(this);
    }

    // **�����ͣʱ���Ŵ�Ƭ & ��������**
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isSelected) return; // ����Ѿ�ѡ�У��򲻷Ŵ�
        if (activeStarEffect != null) return; // ��������Ѿ����ڣ������ظ�����

        LeanTween.scale(gameObject, originalScale * scaleFactor, scaleDuration).setEaseOutQuad();
        SpawnStarEffect(); // �����ǹ�����
    }

    // **����ƿ�ʱ���ָ���Ƭ��С**
    public void OnPointerExit(PointerEventData eventData)
    {
        if (isSelected) return; // ����Ѿ�ѡ�У�����С
        LeanTween.scale(gameObject, originalScale, scaleDuration).setEaseOutQuad();

        // ֹͣ����Ч��
        if (activeStarEffect != null)
        {
            ParticleSystem ps = activeStarEffect.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Stop(); // ����ֹͣ���Ӳ���
            }
            Destroy(activeStarEffect, 0.3f); // 0.3 ������٣���Ҫ��̫��
            activeStarEffect = null; // ������ã���ֹ�ظ�����
        }
    }

    // **�����ǹ�����**
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
            Debug.LogError("starEffectPrefab Ϊ�գ����� CardDrawManager �Ƿ���ȷ��ֵ��");
        }
    }



    // **�ÿ�Ƭ�ƶ�������**
    public void MoveToCenter(Vector3 centerPosition)
    {
        LeanTween.move(gameObject, centerPosition, 0.5f).setOnComplete(() =>
        {
            FlipCard(); // **�ƶ���ɺ�ת**
        });
    }

    // **��Ƭ��ת**
    public void FlipCard()
    {
        LeanTween.rotateY(gameObject, 90, 0.3f).setOnComplete(() =>
        {
            backFace.gameObject.SetActive(false);
            frontFace.gameObject.SetActive(true);
            attributeText.text = effectText;

            LeanTween.rotateY(gameObject, 0, 0.3f).setOnComplete(() =>
            {
                // ֪ͨ CardDrawManager ���ùرհ�ť
                manager.EnableCloseButton();
            });
        });
    }
}

