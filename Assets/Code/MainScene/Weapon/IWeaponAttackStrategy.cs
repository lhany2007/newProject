using UnityEngine;

public interface IWeaponAttackStrategy
{
    void Initialize(PolygonCollider2D weaponCollider, WeaponData.Stats weaponStats, WeaponData.WeaponInfo weaponInfo);
    void TransformCollider(int shapeIndex);
    void ApplyDamage(Collider2D monsterCollider, float weaponDamage);
}
