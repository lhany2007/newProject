using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<UIManager>();
                if (_instance == null)
                {
                    throw new Exception("UIManager instance is null");
                }
            }
            return _instance;
        }
    }

    public SliderManager sliderManager { get; private set; }
    public TextManager textManager { get; private set; }

    [SerializeField] private List<Slider> sliderList;
    private List<string> sliderNameList;

    [SerializeField] private List<TextMeshProUGUI> TextList;
    private List<string> TextNameList;

    private void Awake()
    {
        InitializeVariable(); // ���� �ʱ�ȭ
        ExceptionHandling(); // ����ó��
    }

    private void Start()
    {
        sliderManager.DisableAllSliders(); // ��� �����̴� Ŭ�� ��Ȱ��ȭ
        InitializeSliderValues(); // �����̴��� �´� Max ���� ���� �� ����
    }

    // ����ó�� �Լ�
    private void ExceptionHandling()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        if (sliderList == null || sliderList.Count == 0)
        {
            throw new NullReferenceException("sliderList is null");
        }
        if (sliderNameList.Count != sliderList.Count)
        {
            Debug.LogError("sliderNameList�� sliderList�� ũ�Ⱑ �ٸ�");
        }
        if (TextList == null || TextList.Count == 0)
        {
            throw new NullReferenceException("TextList is null");
        }
        if (TextNameList.Count != TextList.Count)
        {
            Debug.LogError("TextNameList�� TextList�� ũ�Ⱑ �ٸ�");
        }
    }

    // ���� �ʱ�ȭ �Լ�
    private void InitializeVariable()
    {
        _instance = this;

        sliderManager = new SliderManager();
        textManager = new TextManager();

        sliderNameList = new List<string> { "XP", "HP" };
        TextNameList = new List<string> { "Level", "PlayerCurrentHP", "PlayerCurrentXP" };

        UIAddition();
    }

    // UI���� �� Manager�� Dictionary�� �߰�
    private void UIAddition()
    {
        for (int i = 0; i < sliderList.Count; i++)
        {
            sliderManager.AddSliders(sliderNameList[i], sliderList[i]);
        }
        for (int i = 0; i < TextList.Count; i++)
        {
            textManager.AddTexts(TextNameList[i], TextList[i]);
        }
    }

    // �����̴� ���� �ʱ�ȭ �Լ�
    private void InitializeSliderValues()
    {
        for (int i = 0; i < sliderNameList.Count; i++)
        {
            switch (sliderNameList[i])
            {
                case "HP":
                    sliderManager.InitializeSlider(sliderNameList[i], PlayerStats.Instance.MaxHP, PlayerStats.START_HP_Value);
                    break;
                case "XP":
                    sliderManager.InitializeSlider(sliderNameList[i], PlayerStats.Instance.XpSliderMaxValue, PlayerStats.START_XP_VALUE);
                    break;
                default:
                    throw new NullReferenceException("�߸��� sliderNameList ����");
            }
        }
    }

}

public class SliderManager
{
    public Dictionary<string, Slider> SliderDictionary = new Dictionary<string, Slider>();

    public void DisableAllSliders()
    {
        foreach (var item in SliderDictionary)
        {
            item.Value.interactable = false;
        }
    }

    public void AddSliders(string name, Slider slider)
    {
        SliderDictionary.Add(name, slider);
    }

    public void InitializeSlider(string name, float maxValue, float startValue)
    {
        SliderDictionary[name].maxValue = maxValue;
        SliderDictionary[name].value = startValue;
    }
}

public class TextManager
{
    public Dictionary<string, TextMeshProUGUI> TextDictionary = new Dictionary<string, TextMeshProUGUI>();

    /// <param name="name">text�� �̸�</param>
    /// <param name="text">������ text</param>
    public void TextUpdate(string name, string text)
    {
        TextDictionary[name].text = text;
    }

    public void AddTexts(string name, TextMeshProUGUI text)
    {
        TextDictionary.Add(name, text);
    }

    /// <param name="name">text�� �̸�</param>
    /// <param name="isTextActive">text Ȱ��ȭ ����</param>
    public void SetTextActive(string name, bool isTextActive)
    {
        TextDictionary[name].gameObject.SetActive(isTextActive);
    }

    public void MoveText(string name, Vector3 pos)
    {
        TextDictionary[name].transform.position = pos;
    }
}