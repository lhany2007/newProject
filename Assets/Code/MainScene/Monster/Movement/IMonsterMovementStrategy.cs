using UnityEngine;

public interface IMonsterMovementStrategy
{
    void Initialize(GameObject monster, MonsterStats.Stats stats);
    void Move();
    void OnCollisionEnter2D(Collision2D collision);
    void Stop();
}