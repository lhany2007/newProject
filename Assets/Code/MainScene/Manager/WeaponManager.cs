using UnityEngine;
using System.Collections.Generic;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance;

    public Dictionary<string, int> WeaponDamageDictionary;

    List<string> WeaponNameList;
    List<int> WeaponDamageList;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        WeaponDamageDictionary = new Dictionary<string, int>();
        WeaponNameList = new List<string> { "Sword" };
        WeaponDamageList = new List<int> { 33 };

        WeaponDamageUpdate();
    }

    public void WeaponDamageUpdate()
    {
        for (int i = 0; i < WeaponNameList.Count; i++)
        {
            WeaponDamageDictionary.Add(WeaponNameList[i], WeaponDamageList[i]);
        }
    }
}
