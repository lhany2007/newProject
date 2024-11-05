using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PlayerExperience : MonoBehaviour
{
    public static PlayerExperience Instance;

    [Header("UI References")]
    public Slider ExpSlider;
    public TextMeshProUGUI LevelText;
    public TextMeshProUGUI CurrentExpText;

    [Header("Experience Settings")]
    public float ExpMaxValue = 30f;
    public int CurrentLevel = 1;
    public int LevelUpThreshold = 3; // ����ġ�� �ִ밪�� �����ϴ� �ֱ�
    public List<float> ExpGrowthRate = new List<float> { 3, 6, 9, 12, 15, 18, 21 }; // �� ����ġ ������ ����ġ ������ ����Ʈ

    const float START_EXP = 0f;
    const int EXP_MAX_VALUE_INCREASE = 2; // ���� ���� ���� ����ġ �ִ밪 ������
    const string EXP_TAG = "Exp";

    int lastCheckLevel = 0; // ���������� Ȯ���� ���� (����ġ �ִ밪 ���� üũ��)

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        ExpSlider.maxValue = ExpMaxValue;
        ExpSlider.value = START_EXP;
        UpdateUI();
    }


    // ����ġ ���긦 �����ϴ� �Լ�
    public void CollectExpOrb(GameObject orb, int orbType)
    {
        // ����ġ �����̴� ������Ʈ
        UpdateExpSlider(orbType);

        // ���긦 �ٽ� Ǯ�� ��ȯ
        ExpOrbSpawner.Instance.CollectExpOrb(orb, orbType);
    }

    // ����ġ �����̴��� ������Ʈ�ϴ� �Լ�
    void UpdateExpSlider(int currentExpIndex)
    {
        float newExp = ExpSlider.value + ExpGrowthRate[currentExpIndex]; // ���ο� ����ġ ���

        if (newExp >= ExpMaxValue)
        {
            CurrentLevel += 1;
            ExpSlider.value = newExp - ExpMaxValue; // ���� ����ġ�� �ٽ� ����
        }
        else
        {
            ExpSlider.value = newExp; // ���ο� ����ġ ����
        }

        if (CurrentLevel % LevelUpThreshold == 0 && CurrentLevel != lastCheckLevel)
        {
            ExpMaxValue += EXP_MAX_VALUE_INCREASE; // ����ġ �ִ밪 ����
            ExpSlider.maxValue = ExpMaxValue; // �����̴��� �ִ밪 ����
            lastCheckLevel = CurrentLevel; // ������ üũ ���� ����
        }

        UpdateUI(); // UI ������Ʈ
    }

    // UI�� ������Ʈ�ϴ� �Լ�
    void UpdateUI()
    {
        if (LevelText != null)
        {
            LevelText.text = $"{CurrentLevel}"; // ���� �ؽ�Ʈ ������Ʈ
        }
            
        if (CurrentExpText != null)
        {
            CurrentExpText.text = $"{ExpSlider.value:F1} / {ExpMaxValue}"; // ���� ����ġ �ؽ�Ʈ ������Ʈ
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(EXP_TAG)) // ����ġ ����� �浹�� ���
        {
            // ������ �̸����� Ÿ���� �����Ͽ� ����
            int orbType = int.Parse(other.gameObject.name.Split('_')[1]);
            CollectExpOrb(other.gameObject, orbType);
        }
    }
}
