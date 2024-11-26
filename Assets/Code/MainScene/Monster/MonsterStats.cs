public class MonsterStats
{
    public struct Stats
    {
        public float MaxHP { get; }
        public float Speed { get; }
        public float Damage { get; }

        public Stats(float maxHP, float speed, float damage)
        {
            MaxHP = maxHP;
            Speed = speed;
            Damage = damage;
        }
    }
}