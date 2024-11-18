using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Mono.Cecil.Cil;

public class MouseHoverHandler : MonoBehaviour
{
    public TextMeshProUGUI PlayerCurrentHP;
    public TextMeshProUGUI PlayerCurrentEXP;

    public RectTransform TargetHPButtonRectTransform;
    public RectTransform TargetEXPButtonRectTransform;

    public Button HPButton;

    public Vector3 HPTextFixedPosition = new Vector3(152.00f, 982.00f, 0);

    [SerializeField] PlayerHealth playerHealth;
    [SerializeField] PlayerExperience playerExperience;

    Vector2 HPLocalMousePos;
    Vector2 EXPLocalMousePos;

    bool isHPButtonClickedl = false;

    void Start()
    {
        if (PlayerCurrentEXP != null)
        {
            PlayerCurrentEXP.gameObject.SetActive(false);
        }
        if (PlayerCurrentHP != null)
        {
            PlayerCurrentHP.gameObject.SetActive(false);
        }

        HPButton.onClick.AddListener(OnCilck);
    }

    void OnCilck()
    {
        isHPButtonClickedl = !isHPButtonClickedl;

        if (isHPButtonClickedl)
        {
            PlayerCurrentHP.gameObject.SetActive(true);
            PlayerCurrentHP.transform.position = HPTextFixedPosition;
        }
        else if (!isHPButtonClickedl)
        {
            PlayerCurrentHP.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // HP�� EXP ������ ���� ���� ��ǥ�� ��ȯ
        RectTransformUtility.ScreenPointToLocalPointInRectangle(TargetHPButtonRectTransform, Input.mousePosition, null, out HPLocalMousePos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(TargetEXPButtonRectTransform, Input.mousePosition, null, out EXPLocalMousePos);

        // HP �ؽ�Ʈ ó��
        if (EventSystem.current.IsPointerOverGameObject() &&
            TargetHPButtonRectTransform.rect.Contains(HPLocalMousePos) &&
            !isHPButtonClickedl)
        {
            PlayerCurrentHP.gameObject.SetActive(true);
            PlayerCurrentHP.transform.position = Input.mousePosition + new Vector3(0, 10, 0);
            PlayerCurrentHP.text = $"{playerHealth.currentHP:F2}";
            // Ȥ�� PlayerCurrentHP.text = System.Math.Truncate(playerHealth.currentHP).ToString();
        }
        else if (!EventSystem.current.IsPointerOverGameObject() &&
            !TargetHPButtonRectTransform.rect.Contains(HPLocalMousePos) &&
            !isHPButtonClickedl)
        {
            PlayerCurrentHP.gameObject.SetActive(false);
        }

        // EXP �ؽ�Ʈ ó��
        if (EventSystem.current.IsPointerOverGameObject() && TargetEXPButtonRectTransform.rect.Contains(EXPLocalMousePos))
        {
            PlayerCurrentEXP.gameObject.SetActive(true);
            PlayerCurrentEXP.transform.position = Input.mousePosition + new Vector3(0, 10, 0);
            // ���� ����ġ �ۼ�Ʈ
            PlayerCurrentEXP.text = playerExperience.ExpSlider.value > 0 ? $"{(playerExperience.ExpSlider.value / playerExperience.ExpMaxValue) * 100:F2}%" : "0%";
        }
        else
        {
            PlayerCurrentEXP.gameObject.SetActive(false);
        }

        if (isHPButtonClickedl)
        {
            PlayerCurrentHP.text = $"{playerHealth.currentHP:F2}";
        }
    }
}