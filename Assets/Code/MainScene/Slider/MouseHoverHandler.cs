using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static UIManager;

public class MouseHoverHandler : MonoBehaviour
{
    public TextMeshProUGUI PlayerCurrentHP;
    public TextMeshProUGUI PlayerCurrentEXP;

    public RectTransform TargetHPButtonRectTransform;
    public RectTransform TargetEXPButtonRectTransform;

    public Button HPButton;

    public Vector3 HPTextFixedPosition = new Vector3(152.00f, 982.00f, 0);

    private PlayerStats playerStats;

    Vector2 HPLocalMousePos;
    Vector2 EXPLocalMousePos;

    bool isHPButtonClickedl = false;

    UIManager uiManager;
    SliderManager sliderManager;

    void Start()
    {
        uiManager = UIManager.Instance;

        if (PlayerCurrentEXP != null)
        {
            PlayerCurrentEXP.gameObject.SetActive(false);
        }
        if (PlayerCurrentHP != null)
        {
            PlayerCurrentHP.gameObject.SetActive(false);
        }
        playerStats = PlayerStats.Instance;
        HPButton.onClick.AddListener(OnClick);
    }

    void OnClick()
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
        // HP와 EXP 각각에 대해 로컬 좌표로 변환
        RectTransformUtility.ScreenPointToLocalPointInRectangle(TargetHPButtonRectTransform, Input.mousePosition, null, out HPLocalMousePos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(TargetEXPButtonRectTransform, Input.mousePosition, null, out EXPLocalMousePos);

        // HP 텍스트 처리
        if (EventSystem.current.IsPointerOverGameObject() &&
            TargetHPButtonRectTransform.rect.Contains(HPLocalMousePos) &&
            !isHPButtonClickedl)
        {
            PlayerCurrentHP.gameObject.SetActive(true);
            PlayerCurrentHP.transform.position = Input.mousePosition + new Vector3(0, 10, 0);
            PlayerCurrentHP.text = $"{playerStats.currentHP:F2}";
            // 혹은 PlayerCurrentHP.text = System.Math.Truncate(playerStats.currentHP).ToString();
        }
        else if (!EventSystem.current.IsPointerOverGameObject() &&
            !TargetHPButtonRectTransform.rect.Contains(HPLocalMousePos) &&
            !isHPButtonClickedl)
        {
            PlayerCurrentHP.gameObject.SetActive(false);
        }

        // EXP 텍스트 처리
        if (EventSystem.current.IsPointerOverGameObject() && TargetEXPButtonRectTransform.rect.Contains(EXPLocalMousePos))
        {
            PlayerCurrentEXP.gameObject.SetActive(true);
            PlayerCurrentEXP.transform.position = Input.mousePosition + new Vector3(0, 10, 0);

            Slider xpSlider = uiManager.sliderManager.SliderDictionary["XP"];
            // 현재 경험치 퍼센트
            PlayerCurrentEXP.text = GetExperiencePercentage(xpSlider);
        }
        else
        {
            PlayerCurrentEXP.gameObject.SetActive(false);
        }

        if (isHPButtonClickedl)
        {
            PlayerCurrentHP.text = $"{playerStats.currentHP:F2}";
        }
    }
    private string GetExperiencePercentage(Slider xpSlider)
    {
        return xpSlider.value > 0 ? $"{(xpSlider.value / PlayerStats.Instance.XpSliderMaxValue) * 100:F2}%" : "0%";
    }
}