using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using TopScrollingGame.Creatures.Enemies;

namespace TopScrollingGame
{
    public static class MainHelper
    {
        public const int PowerUpsCount = 3;
        public const int EffectsCount = 5;
        public static Animation fireAnimation;

        public static List<EnemyStat> EnemiesStats { get; set; }

        public static List<Texture2D> PowerUpTextures { get; set; }

        public static void Initialize()
        {
            PowerUpTextures = new List<Texture2D>();
            InitializeEnemyStats();
        }

        public static void Load()
        {
            for (int i = 0; i < EffectsCount; i++)
            {
                PowerUpTextures.Add(Scripts.LoadTexture(@"Sprites\PowerUps\PowerUp_" + i.ToString()));
            }
        }

        private static void InitializeEnemyStats()
        {
            EnemiesStats = new List<EnemyStat>();

            EnemyStat stat;

            //Axe Master
            stat = new EnemyStat();
            stat.MaxHealth = 30;
            stat.Damage = 1;
            stat.Speed = 4;
            stat.Asset = "Fighter";
            stat.Range = 600;
            stat.projectileType = ProjectileType.Axe;
            stat.ProjectileSpeed = 20;
            stat.ProjectileAngleVelocity = 30f;
            stat.projectileDebuff = EffectType.Bleed;
            EnemiesStats.Add(stat);

            //Skeleton
            stat = new EnemyStat();
            stat.MaxHealth = 50;
            stat.Damage = 15;
            stat.Speed = 2;
            stat.Asset = "Skeleton";
            stat.projectileDebuff = EffectType.None;
            EnemiesStats.Add(stat);

            //Scorpion
            stat = new EnemyStat();
            stat.MaxHealth = 20;
            stat.Damage = 7;
            stat.Speed = 6;
            stat.Asset = "Scorpion";
            stat.Range = 400;
            stat.projectileType = ProjectileType.Poison;
            stat.ProjectileSpeed = 12;
            stat.ProjectileAngleVelocity = 0f;
            stat.projectileDebuff = EffectType.None;
            EnemiesStats.Add(stat);

            //Thief
            stat = new EnemyStat();
            stat.MaxHealth = 20;
            stat.Damage = 10;
            stat.Speed = 6;
            stat.Asset = "Thief";
            stat.projectileDebuff = EffectType.None;
            EnemiesStats.Add(stat);

            //Scorpion
            stat = new EnemyStat();
            stat.MaxHealth = 500;
            stat.Damage = 40;
            stat.Speed = 5;
            stat.Asset = "Boss";
            stat.projectileType = ProjectileType.Scythe;
            stat.Range = 5000;
            stat.ProjectileSpeed = 14;
            stat.ProjectileAngleVelocity = 30f;
            stat.projectileDebuff = EffectType.None;
            EnemiesStats.Add(stat);
        }
    }
}