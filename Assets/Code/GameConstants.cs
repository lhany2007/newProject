using UnityEngine;

public static class GameConstants
{
    public enum Tags
    {
        Player,
        Monster,
        XP
    }
    public enum Layers
    {
        Player,
        Monster
    }

    public enum AnimationParams
    {
        [Header("BatMovement")]
        IsAngering,
        IsDashed,

        [Header("SlimeMovement")]
        IsMoving
    }
}