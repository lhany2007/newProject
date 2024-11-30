using UnityEngine;
using System.Collections.Generic;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; private set; }

    public WeaponData weaponData;

    public Dictionary<string, IWeaponAttackStrategy> WeaponStrategyDictionary;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        InitializeDictionary();
    }

    private void InitializeDictionary()
    {
        WeaponStrategyDictionary = new Dictionary<string, IWeaponAttackStrategy>
        {
            { Weapons.Sword, new SwordAttackStrategy() }
        };
    }

    public IWeaponAttackStrategy GetWeaponStrategy(string weaponName)
    {
        if (!WeaponStrategyDictionary.TryGetValue(weaponName, out IWeaponAttackStrategy weaponStrategy))
        {
            throw new System.Exception($"Weapon{weaponName}�� ã�� �� ����");
        }
        return weaponStrategy;
    }

    // ������ ���� ��ȯ
    public WeaponData.Stats GetWeaponStats(string weaponName)
    {
        if (!weaponData.WeaponDictionary.TryGetValue(weaponName, out WeaponData.WeaponInfo weapon))
        {
            throw new System.Exception($"Weapon{weaponName}�� ã�� �� ����");
        }
        return new WeaponData.Stats(weapon.damage, weapon.speed);
    }

    public WeaponData.WeaponInfo GetWeaponInfo(string weaponName)
    {
        if (!weaponData.WeaponDictionary.TryGetValue(weaponName, out WeaponData.WeaponInfo weapon))
        {
            throw new System.Exception($"Weapon{weaponName}�� ã�� �� ����");
        }
        return weapon;
    }
}