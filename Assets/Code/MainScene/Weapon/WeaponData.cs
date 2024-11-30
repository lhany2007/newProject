using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Vector2ArrayWrapper
{
    public Vector2[] array;
}

[CreateAssetMenu(fileName = "WeaponData", menuName = "Game/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Serializable]
    public class WeaponInfo
    {
        public string name;
        public int damage;
        public float speed;
        public List<Vector2ArrayWrapper> ColliderShape;
    }

    public List<WeaponInfo> weapons;

    public Dictionary<string, WeaponInfo> WeaponDictionary { get; private set; }

    private void OnEnable()
    {
        InitializeDictionary();
    }

    private void InitializeDictionary()
    {
        WeaponDictionary = new Dictionary<string, WeaponInfo>();

        foreach (var weapon in weapons)
        {
            WeaponDictionary[weapon.name] = weapon;
        }
    }
    public struct Stats
    {
        public float Damage { get; }
        public float Speed { get; }

        public Stats(float speed, float damage)
        {
            Speed = speed;
            Damage = damage;
        }
    }
}
