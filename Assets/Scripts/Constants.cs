using UnityEngine;

public static class Constants
{
    public static class Layers
    {
        public static readonly int PlayerLayer = LayerMask.NameToLayer("Player");
        public static readonly int EnemyLayer = LayerMask.NameToLayer("Enemy");
        public static readonly int WallLayer = LayerMask.NameToLayer("Wall");
        public static readonly int PlayerBulletLayer = LayerMask.NameToLayer("PlayerBullet");
        public static readonly int EnemyBulletLayer = LayerMask.NameToLayer("EnemyBullet");
    }

    
}
