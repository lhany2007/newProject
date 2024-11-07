using UnityEngine;
using UnityEngine.UI;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject monsterPrefab;
    public float spawnRadius = 10f; // �÷��̾� �ֺ��� ���� ���� �ݰ�
    public float spawnInterval = 1f; // ���ο� ���͸� �����ϴ� ����(��)
    public int maxMonsters = 20; // ���ÿ� ������ �� �ִ� �ִ� ���� ��
    public float monsterMaxHP = 100f; // �� ������ �ִ� ü��

    public Transform player;

    Transform spawnParent;

    float timeSinceLastSpawn = 0f; // ������ ���� ���� ��� �ð�
    int monstersSpawned = 0; // ���� ������ ���� ��

    void Start()
    {
        // �÷��̾��� Ʈ�������� ã�� ������ ����
        player = GameObject.FindGameObjectWithTag("Player").transform;
        spawnParent = GetComponent<Transform>(); // �θ� ���� ��ũ��Ʈ�� �پ��ִ� ������Ʈ�� ����
    }

    void Update()
    {
        // ��� �ð��� ��� ����
        timeSinceLastSpawn += Time.deltaTime;

        // ���� ������ ���� ���� �ִ� ���� ���� �������� ���� ��� ���� ����
        if (timeSinceLastSpawn >= spawnInterval && monstersSpawned < maxMonsters)
        {
            SpawnMonster();
            timeSinceLastSpawn = 0f; // ���� �� �ð� �ʱ�ȭ
        }
    }

    void SpawnMonster()
    {
        // �÷��̾� �ֺ��� ���� ��ġ�� ����Ͽ� ���� ��ġ ����
        Vector3 spawnPosition = player.position + new Vector3(
            Random.Range(-spawnRadius, spawnRadius),
            Random.Range(-spawnRadius, spawnRadius),
            0f
        );

        // ���� �������� ���� ��ġ�� �ν��Ͻ�ȭ
        GameObject monster = Instantiate(monsterPrefab, spawnPosition, Quaternion.identity, spawnParent);

        // ���Ϳ� HP �����̴� �߰�
        AddHPSlider(monster);

        monstersSpawned++;
    }

    void AddHPSlider(GameObject monster)
    {
        // HP �����̴��� �� GameObject ����
        GameObject sliderGO = new GameObject("HP Slider");

        // ������Ʈ �߰�
        Canvas canvas = sliderGO.AddComponent<Canvas>();
        Slider slider = sliderGO.AddComponent<Slider>();
        MonsterHealth monsterHealth = monster.GetComponent<MonsterHealth>();

        if (monsterHealth != null)
        {
            sliderGO.transform.SetParent(monster.transform); // �����̴��� ������ �ڽ����� ����
            sliderGO.transform.localPosition = Vector3.up * 1.5f; // �����̴��� ���� ���� ��ġ

            canvas.renderMode = RenderMode.WorldSpace; // ���� �������� ������
            canvas.sortingOrder = 10; // �����̴��� ���� ���� ǥ�õǵ��� ����

            slider.fillRect.gameObject.GetComponent<Image>().color = Color.red; // ä��� ������ �������� ����
            slider.maxValue = monsterHealth.maxHP; // �����̴� �ִ� ���� ������ �ִ� HP�� ����
            slider.value = monsterHealth.maxHP; // �����̴��� �ִ� HP�� �ʱ�ȭ

            monsterHealth.slider = slider; // �����̴� ���� ����
        }
        else
        {
            Destroy(sliderGO); // MonsterHealth ������Ʈ�� ���� ��� �����̴��� ����
        }
    }
}
