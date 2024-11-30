using UnityEngine;

public class SwordAttackStrategy : IWeaponAttackStrategy
{
    private WeaponData.Stats stats;
    private PolygonCollider2D swordCollider;
    private WeaponData.WeaponInfo swordInfo;

    public void Initialize(PolygonCollider2D weaponCollider, WeaponData.Stats weaponStats, WeaponData.WeaponInfo weaponInfo)
    {
        swordCollider = weaponCollider;
        stats = weaponStats;
        swordInfo = weaponInfo;
    }

    public void TransformCollider(int shapeIndex)
    {
        Vector2[] Vector2ColliderArray = swordInfo.ColliderShape[shapeIndex].array;
        swordCollider.SetPath(0, Vector2ColliderArray);
    }

    public void ApplyDamage(Collider2D monsterCollider, float weaponDamage)
    {
        MonsterHealth monsterHealth = monsterCollider.GetComponent<MonsterHealth>();
        if (monsterHealth == null)
        {
            throw new System.NullReferenceException("monsterHealth is null");
        }
        monsterHealth.TakeDamage(weaponDamage);
    }
}
