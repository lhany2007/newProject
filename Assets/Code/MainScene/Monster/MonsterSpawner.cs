using UnityEngine;
using UnityEngine.UI;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject monsterPrefab;
    public float spawnRadius = 10f; // 플레이어 주변의 몬스터 스폰 반경
    public float spawnInterval = 1f; // 새로운 몬스터를 스폰하는 간격(초)
    public int maxMonsters = 20; // 동시에 존재할 수 있는 최대 몬스터 수
    public float monsterMaxHP = 100f; // 각 몬스터의 최대 체력

    public Transform player;

    Transform spawnParent;

    float timeSinceLastSpawn = 0f; // 마지막 스폰 이후 경과 시간
    int monstersSpawned = 0; // 현재 스폰된 몬스터 수

    void Start()
    {
        // 플레이어의 트랜스폼을 찾아 참조로 설정
        player = GameObject.FindGameObjectWithTag("Player").transform;
        spawnParent = GetComponent<Transform>(); // 부모를 현재 스크립트가 붙어있는 오브젝트로 설정
    }

    void Update()
    {
        // 경과 시간을 계속 증가
        timeSinceLastSpawn += Time.deltaTime;

        // 스폰 간격이 지난 경우와 최대 몬스터 수에 도달하지 않은 경우 몬스터 스폰
        if (timeSinceLastSpawn >= spawnInterval && monstersSpawned < maxMonsters)
        {
            SpawnMonster();
            timeSinceLastSpawn = 0f; // 스폰 후 시간 초기화
        }
    }

    void SpawnMonster()
    {
        // 플레이어 주변의 랜덤 위치를 계산하여 스폰 위치 설정
        Vector3 spawnPosition = player.position + new Vector3(
            Random.Range(-spawnRadius, spawnRadius),
            Random.Range(-spawnRadius, spawnRadius),
            0f
        );

        // 몬스터 프리팹을 스폰 위치에 인스턴스화
        GameObject monster = Instantiate(monsterPrefab, spawnPosition, Quaternion.identity, spawnParent);

        // 몬스터에 HP 슬라이더 추가
        AddHPSlider(monster);

        monstersSpawned++;
    }

    void AddHPSlider(GameObject monster)
    {
        // HP 슬라이더용 새 GameObject 생성
        GameObject sliderGO = new GameObject("HP Slider");

        // 컴포넌트 추가
        Canvas canvas = sliderGO.AddComponent<Canvas>();
        Slider slider = sliderGO.AddComponent<Slider>();
        MonsterHealth monsterHealth = monster.GetComponent<MonsterHealth>();

        if (monsterHealth != null)
        {
            sliderGO.transform.SetParent(monster.transform); // 슬라이더를 몬스터의 자식으로 설정
            sliderGO.transform.localPosition = Vector3.up * 1.5f; // 슬라이더를 몬스터 위에 배치

            canvas.renderMode = RenderMode.WorldSpace; // 월드 공간에서 렌더링
            canvas.sortingOrder = 10; // 슬라이더가 몬스터 위에 표시되도록 설정

            slider.fillRect.gameObject.GetComponent<Image>().color = Color.red; // 채우기 색상을 빨강으로 설정
            slider.maxValue = monsterHealth.maxHP; // 슬라이더 최대 값을 몬스터의 최대 HP로 설정
            slider.value = monsterHealth.maxHP; // 슬라이더를 최대 HP로 초기화

            monsterHealth.slider = slider; // 슬라이더 참조 전달
        }
        else
        {
            Destroy(sliderGO); // MonsterHealth 컴포넌트가 없는 경우 슬라이더를 삭제
        }
    }
}
