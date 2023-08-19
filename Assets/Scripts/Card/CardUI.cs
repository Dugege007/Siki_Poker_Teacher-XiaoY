using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// �˽ű���Ҫ���ص��˿�����
public class CardUI : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler
{
    private Card card;

    private Color darkColor = new Color(0.6f, 0.6f, 0.6f, 1);
    private Color lightColor = new Color(1, 1, 1, 1);

    private Image image;

    private bool isUp;

    private bool isSelect = false;
    public bool IsSelect
    {
        get => isSelect;
        set
        {
            isSelect = value;
            if (isSelect)
            {
                image.color = darkColor;
            }
            else
            {
                image.color = lightColor;
            }
        }
    }

    private void Start()
    {
        card = CardManager.GetCard(gameObject.name);
        image = GetComponent<Image>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            GameManager.isPress = false;

            if (IsSelect)
            {
                IsSelect = false;

                if (isUp)
                {
                    transform.localPosition -= Vector3.up * 10;
                    isUp = false;
                    if (GameManager.selectedCard.Contains(card))
                        GameManager.selectedCard.Remove(card);
                }
                else
                {
                    transform.localPosition += Vector3.up * 10;
                    isUp = true;
                    if (!GameManager.selectedCard.Contains(card))
                        GameManager.selectedCard.Add(card);
                }
            }
        }
    }

    // private void OnMouseDown() �˷����� UI ����Ч
    // ����ʹ�����½ӿ�

    public void OnPointerDown(PointerEventData eventData)
    {
        GameManager.isPress = true;

        if (IsSelect)
            IsSelect = false;
        else
            IsSelect = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GameManager.isPress)
        {
            if (IsSelect)
                IsSelect = false;
            else
                IsSelect = true;
        }

        // ���̧���ʱ��������κεط������Խ����̧��ķ���д�� Update() ������
    }
}
