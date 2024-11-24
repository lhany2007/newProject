using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MouseHoverHandler : MonoBehaviour
{
    public RectTransform TargetHPButtonRectTransform;
    public RectTransform TargetXPButtonRectTransform;

    public Button HPButton;

    public Vector3 HPTextFixedPosition;

    private Vector2 HPLocalMousePos;
    private Vector2 XPLocalMousePos;

    private const string playerCurrentHP = "PlayerCurrentHP";
    private const string playerCurrentXP = "PlayerCurrentXP";

    private bool isHPButtonClickedl = false;

    private void Start()
    {
        HPTextFixedPosition = new Vector3(152.00f, 982.00f, 0);

        TextManagerActive(playerCurrentHP, false);
        TextManagerActive(playerCurrentXP, false);

        HPButton.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        isHPButtonClickedl = !isHPButtonClickedl;

        if (isHPButtonClickedl)
        {
            TextManagerActive(playerCurrentHP, isHPButtonClickedl);
            TextManagerMove(playerCurrentHP, HPTextFixedPosition);
        }
        else if (!isHPButtonClickedl)
        {
            TextManagerActive(playerCurrentHP, isHPButtonClickedl);
        }
    }

    private void TextManagerActive(string name, bool isTextActive)
    {
        UIManager.Instance.textManager.SetTextActive(name, isTextActive);
    }
    private void TextManagerMove(string name, Vector3 pos)
    {
        UIManager.Instance.textManager.MoveText(name, pos);
    }
    private void TextManagerUpdate(string name, string text)
    {
        UIManager.Instance.textManager.TextUpdate(name, text);
    }

    private void Update()
    {
        // HP�� EXP ������ ���� ���� ��ǥ�� ��ȯ
        RectTransformUtility.ScreenPointToLocalPointInRectangle(TargetHPButtonRectTransform, Input.mousePosition, null, out HPLocalMousePos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(TargetXPButtonRectTransform, Input.mousePosition, null, out XPLocalMousePos);

        // UI ���� ���콺�� �÷��� ��, text�� ǥ���ϴ� ��ġ
        Vector3 targetPosition = Input.mousePosition + new Vector3(0, 10, 0);

        // UI ��� ���� �����Ͱ� �ִ��� ����
        bool isPointerOverUI = EventSystem.current.IsPointerOverGameObject();
        // ���콺�� HP ��ư�� RectTransform ���� ���� �ִ��� ����
        bool isMouseOverHPButton = TargetHPButtonRectTransform.rect.Contains(HPLocalMousePos);
        bool isMouseOverHXButton = TargetHPButtonRectTransform.rect.Contains(XPLocalMousePos);

        // �÷��̾��� ���� ü��
        string HP = $"{PlayerStats.Instance.currentHP:F2}";

        // HP �ؽ�Ʈ ó��
        if (isPointerOverUI && isMouseOverHPButton && !isHPButtonClickedl)
        {
            TextManagerActive(playerCurrentHP, true);
            TextManagerMove(playerCurrentHP, targetPosition);
            TextManagerUpdate(playerCurrentHP, HP);
        }
        else if (!isPointerOverUI && !isMouseOverHPButton && !isHPButtonClickedl)
        {
            TextManagerActive(playerCurrentHP, false);
        }

        // XP �ؽ�Ʈ ó��
        if (isPointerOverUI && isMouseOverHXButton)
        {
            TextManagerActive(playerCurrentXP, true);
            TextManagerMove(playerCurrentXP, targetPosition);
            // ���� ����ġ �ۼ�Ʈ
            TextManagerUpdate(playerCurrentXP, GetExperiencePercentage(UIManager.Instance.sliderManager.SliderDictionary["XP"]));
        }
        else if (!isPointerOverUI && !isMouseOverHXButton)
        {
            TextManagerActive(playerCurrentXP, false);
        }

        // ������ �ؽ�Ʈ�� �÷��̾��� ���� ü���� ������Ʈ
        if (isHPButtonClickedl)
        {
            TextManagerUpdate(playerCurrentHP, HP);
        }
    }
    private string GetExperiencePercentage(Slider xpSlider)
    {
        return xpSlider.value > 0 ? $"{(PlayerStats.Instance.CurrentXP / PlayerStats.Instance.XpSliderMaxValue) * 100:F2}%" : "0%";
    }
}