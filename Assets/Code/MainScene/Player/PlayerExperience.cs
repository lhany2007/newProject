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
    public int LevelUpThreshold = 3; // 경험치의 최대값이 증가하는 주기
    public List<float> ExpGrowthRate = new List<float> { 3, 6, 9, 12, 15, 18, 21 }; // 각 경험치 오브의 경험치 증가량 리스트

    const float START_EXP = 0f;
    const int EXP_MAX_VALUE_INCREASE = 2; // 일정 레벨 이후 경험치 최대값 증가량
    const string EXP_TAG = "Exp";

    int lastCheckLevel = 0; // 마지막으로 확인한 레벨 (경험치 최대값 증가 체크용)

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


    // 경험치 오브를 수집하는 함수
    public void CollectExpOrb(GameObject orb, int orbType)
    {
        // 경험치 슬라이더 업데이트
        UpdateExpSlider(orbType);

        // 오브를 다시 풀로 반환
        ExpOrbSpawner.Instance.CollectExpOrb(orb, orbType);
    }

    // 경험치 슬라이더를 업데이트하는 함수
    void UpdateExpSlider(int currentExpIndex)
    {
        float newExp = ExpSlider.value + ExpGrowthRate[currentExpIndex]; // 새로운 경험치 계산

        if (newExp >= ExpMaxValue)
        {
            CurrentLevel += 1;
            ExpSlider.value = newExp - ExpMaxValue; // 남은 경험치를 다시 설정
        }
        else
        {
            ExpSlider.value = newExp; // 새로운 경험치 설정
        }

        if (CurrentLevel % LevelUpThreshold == 0 && CurrentLevel != lastCheckLevel)
        {
            ExpMaxValue += EXP_MAX_VALUE_INCREASE; // 경험치 최대값 증가
            ExpSlider.maxValue = ExpMaxValue; // 슬라이더의 최대값 갱신
            lastCheckLevel = CurrentLevel; // 마지막 체크 레벨 갱신
        }

        UpdateUI(); // UI 업데이트
    }

    // UI를 업데이트하는 함수
    void UpdateUI()
    {
        if (LevelText != null)
        {
            LevelText.text = $"{CurrentLevel}"; // 레벨 텍스트 업데이트
        }
            
        if (CurrentExpText != null)
        {
            CurrentExpText.text = $"{ExpSlider.value:F1} / {ExpMaxValue}"; // 현재 경험치 텍스트 업데이트
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(EXP_TAG)) // 경험치 오브와 충돌한 경우
        {
            // 오브의 이름에서 타입을 추출하여 수집
            int orbType = int.Parse(other.gameObject.name.Split('_')[1]);
            CollectExpOrb(other.gameObject, orbType);
        }
    }
}
