using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("��������� ������")]
    public Image buttonImage;              // ������ �� Image ������
    public Sprite normalImage;
    public Sprite hoverImage;

    [Header("���")]
    public Image backgroundImage;          // ������ �� Image ������� ����
    public Sprite hoverBackground;

    public void OnPointerEnter(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();

        if (buttonImage != null)
        {
            buttonImage.sprite = hoverImage;
        }

        if (backgroundImage != null && hoverBackground != null)
            backgroundImage.sprite = hoverBackground;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();

        if (buttonImage != null)
            buttonImage.sprite = normalImage;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
