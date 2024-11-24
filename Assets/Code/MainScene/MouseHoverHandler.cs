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
        // HP와 EXP 각각에 대해 로컬 좌표로 변환
        RectTransformUtility.ScreenPointToLocalPointInRectangle(TargetHPButtonRectTransform, Input.mousePosition, null, out HPLocalMousePos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(TargetXPButtonRectTransform, Input.mousePosition, null, out XPLocalMousePos);

        // UI 위에 마우스를 올렸을 때, text를 표시하는 위치
        Vector3 targetPosition = Input.mousePosition + new Vector3(0, 10, 0);

        // UI 요소 위에 포인터가 있는지 여부
        bool isPointerOverUI = EventSystem.current.IsPointerOverGameObject();
        // 마우스가 HP 버튼의 RectTransform 영역 내에 있는지 여부
        bool isMouseOverHPButton = TargetHPButtonRectTransform.rect.Contains(HPLocalMousePos);
        bool isMouseOverHXButton = TargetHPButtonRectTransform.rect.Contains(XPLocalMousePos);

        // 플레이어의 현재 체력
        string HP = $"{PlayerStats.Instance.currentHP:F2}";

        // HP 텍스트 처리
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

        // XP 텍스트 처리
        if (isPointerOverUI && isMouseOverHXButton)
        {
            TextManagerActive(playerCurrentXP, true);
            TextManagerMove(playerCurrentXP, targetPosition);
            // 현재 경험치 퍼센트
            TextManagerUpdate(playerCurrentXP, GetExperiencePercentage(UIManager.Instance.sliderManager.SliderDictionary["XP"]));
        }
        else if (!isPointerOverUI && !isMouseOverHXButton)
        {
            TextManagerActive(playerCurrentXP, false);
        }

        // 고정된 텍스트에 플레이어의 현제 체력을 업데이트
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