using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TopScrollingGame.Creatures.Enemies;

namespace TopScrollingGame
{
    public enum WaveBonus
    {
        AttackSpeed,
        Damage,
        FlamingCats
    }

    public enum NegativeWaveBonus
    {
        Regeneration,
        None
    }

    internal static class WavesSystem
    {
        private const int DamageBonusIncreasePerWave = 2;
        private const float AttackSpeedBonusIncreasePerWave = 0.5f;
        public static bool BossBeaten;

        public static List<Creature> Creatures { get; set; }

        public static int CurrentWave { get; set; }

        public static WaveBonus CurrentWaveBonus { get; set; }

        public static NegativeWaveBonus CurrentNegativeWaveBonus { get; set; }

        public static void Initialize()
        {
            Creatures = new List<Creature>();
            Vector2 playerStartingPos = new Vector2((Main.playingAreaX + Main.playingAreaWidth) / 2, Main.baseScreenSize.Y - 180);
            WavesSystem.Creatures.Add(new Hero(playerStartingPos));
            Main.heroRef = WavesSystem.Creatures.Find(cr => cr is Hero) as Hero;
            Creatures[0].Load();
            CurrentWave = 0;
            CurrentNegativeWaveBonus = NegativeWaveBonus.None;
            BossBeaten = false;
        }

        private static void ResetWaveBonusStats()
        {
            Main.heroRef.Damage = Hero.startingDamage;
            Main.heroRef.AttackSpeed = Hero.startingAttackSpeed;
            Main.heroRef.ProjectileDebuff = EffectType.None;
        }

        private static void ApplyBonus()
        {
            string message = "Wave Bonus: ";

            switch (CurrentWaveBonus)
            {
                case WaveBonus.AttackSpeed:
                    Main.heroRef.AttackSpeed += CurrentWave * AttackSpeedBonusIncreasePerWave;
                    message += "Increased Attack Speed";
                    break;

                case WaveBonus.Damage:
                    Main.heroRef.Damage += CurrentWave * DamageBonusIncreasePerWave;
                    message += "Increased Damage";
                    break;

                case WaveBonus.FlamingCats:
                    Main.heroRef.ProjectileDebuff = EffectType.Burning;
                    message += "Flaming Cats";
                    break;

                default:
                    break;
            }

            GUI.AddMessage(message, 1000, Color.CornflowerBlue);
        }

        private static void SetWaveBonus()
        {
            Random rand = new Random();
            var waveBonusesCount = Enum.GetNames(typeof(WaveBonus)).Length;
            CurrentWaveBonus = (WaveBonus)rand.Next(waveBonusesCount);
            ApplyBonus();
        }

        private static void ApplyNegativeBonus()
        {

        }

        private static void SetNegativeWaveBonus()
        {
            Random rand = new Random();
            var waveBonusesCount = Enum.GetNames(typeof(NegativeWaveBonus)).Length - 1;
            CurrentNegativeWaveBonus = (NegativeWaveBonus)rand.Next(waveBonusesCount);

            string message = "Negative Wave Bonus: ";

            switch (CurrentNegativeWaveBonus)
            {
                case NegativeWaveBonus.Regeneration:
                    message += "Enemy Regeneration!";
                    break;
            }

            GUI.AddMessage(message, 1000, Color.DimGray);
        }

        public static void StartWave(int index)
        {
            Main.InitializeRoom();
            ResetWaveBonusStats();
            GUI.AddMessage("Wave " + index, 1000, Color.White);
            CurrentWave = index;
            List<Creature> newWaveEnemies = GetCurrentWaveEnemies();
            newWaveEnemies.Add(Creatures.First(cr => cr is Hero));
            Creatures = newWaveEnemies;
            SetWaveBonus();

            if (CurrentWave >= 5)
            {
                SetNegativeWaveBonus();
            }
            else
            {
                CurrentNegativeWaveBonus = NegativeWaveBonus.None;
            }

            if (CurrentWave == 10)
            {
                Main.ChangeMusic(Main.bossMusic);
            }
        }

        private static void CheckForNewWave(GameTime gameTime)
        {
            if (Creatures.Count == 1 && CurrentWave > 0)
            {
                if (CurrentWave < 10)
                {
                    CurrentWave++;
                    Main.heroRef.MaxHealth += 10;
                    StartWave(CurrentWave);
                }
                else
                {
                    if (!BossBeaten)
                    {
                        GUI.AddMessage("You won! Well done!", 3000, Color.RosyBrown); 
                    }

                    BossBeaten = true;
                }
            }
        }

        private static void UpdateWaveEffects()
        {
            if (CurrentNegativeWaveBonus == NegativeWaveBonus.Regeneration)
            {
                var enemies =
                from cr in Creatures
                where cr is Enemy && cr.Health < cr.MaxHealth
                select cr;

                enemies.ForEach(en => en.Regenerate());
            }
        }

        public static void UpdateWaves(GameTime gameTime)
        {
            UpdateWaveEffects();
            Creatures.ForEach(cr => cr.Update(gameTime));
            CheckForNewWave(gameTime);
        }

        private static List<Creature> GetCurrentWaveEnemies()
        {
            Random rand = new Random();
            List<int> AvailablePositions = new List<int>() { 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000 };
            List<Creature> ReturnedCreatures = new List<Creature>();

            switch (CurrentWave)
            {
                case 1:

                    ReturnedCreatures.Add(new RangedEnemy(new Vector2(300, -100), EnemyType.Scorpion));
                    ReturnedCreatures.Add(new RangedEnemy(new Vector2(600, -100), EnemyType.Scorpion));
                    ReturnedCreatures.Add(new RangedEnemy(new Vector2(900, -100), EnemyType.Scorpion));

                    break;

                case 2:

                    ReturnedCreatures.Add(new RangedEnemy(new Vector2(350, -100), EnemyType.Scorpion));
                    ReturnedCreatures.Add(new MeleeEnemy(new Vector2(500, -100), EnemyType.Skeleton));
                    ReturnedCreatures.Add(new MeleeEnemy(new Vector2(650, -100), EnemyType.Skeleton));
                    ReturnedCreatures.Add(new RangedEnemy(new Vector2(800, -100), EnemyType.Scorpion));

                    break;

                case 3:

                    ReturnedCreatures.Add(new RangedEnemy(new Vector2(150, -100), EnemyType.Scorpion));
                    ReturnedCreatures.Add(new RangedEnemy(new Vector2(300, -100), EnemyType.Scorpion));
                    ReturnedCreatures.Add(new MeleeEnemy(new Vector2(450, -100), EnemyType.Skeleton));
                    ReturnedCreatures.Add(new MeleeEnemy(new Vector2(600, -100), EnemyType.Skeleton));
                    ReturnedCreatures.Add(new RangedEnemy(new Vector2(750, -100), EnemyType.Scorpion));
                    ReturnedCreatures.Add(new RangedEnemy(new Vector2(900, -100), EnemyType.Scorpion));

                    break;

                case 4:

                    ReturnedCreatures.Add(new RangedEnemy(new Vector2(300, -100), EnemyType.AxeMaster));
                    ReturnedCreatures.Add(new MeleeEnemy(new Vector2(450, -100), EnemyType.Skeleton));
                    ReturnedCreatures.Add(new MeleeEnemy(new Vector2(600, -100), EnemyType.Skeleton));
                    ReturnedCreatures.Add(new RangedEnemy(new Vector2(750, -100), EnemyType.AxeMaster));

                    break;

                case 5:

                    ReturnedCreatures.Add(new MeleeEnemy(new Vector2(150, -100), EnemyType.Skeleton));
                    ReturnedCreatures.Add(new RangedEnemy(new Vector2(300, -100), EnemyType.Scorpion));
                    ReturnedCreatures.Add(new MeleeEnemy(new Vector2(450, -100), EnemyType.Skeleton));
                    ReturnedCreatures.Add(new MeleeEnemy(new Vector2(600, -100), EnemyType.Skeleton));
                    ReturnedCreatures.Add(new RangedEnemy(new Vector2(750, -100), EnemyType.Scorpion));
                    ReturnedCreatures.Add(new MeleeEnemy(new Vector2(900, -100), EnemyType.Skeleton));

                    break;

                case 6:
                    ReturnedCreatures.Add(new MeleeEnemy(new Vector2(150, -200), EnemyType.Skeleton));
                    ReturnedCreatures.Add(new MeleeEnemy(new Vector2(150, -100), EnemyType.Skeleton));
                    ReturnedCreatures.Add(new RangedEnemy(new Vector2(300, -100), EnemyType.AxeMaster));
                    ReturnedCreatures.Add(new RangedEnemy(new Vector2(450, -100), EnemyType.Scorpion));
                    ReturnedCreatures.Add(new RangedEnemy(new Vector2(600, -100), EnemyType.Scorpion));
                    ReturnedCreatures.Add(new RangedEnemy(new Vector2(750, -100), EnemyType.AxeMaster));
                    ReturnedCreatures.Add(new MeleeEnemy(new Vector2(900, -100), EnemyType.Skeleton));
                    ReturnedCreatures.Add(new MeleeEnemy(new Vector2(900, -200), EnemyType.Skeleton));

                    break;

                case 7:
                    ReturnedCreatures.Add(new MeleeEnemy(new Vector2(150, -200), EnemyType.Skeleton));
                    ReturnedCreatures.Add(new RangedEnemy(new Vector2(150, -100), EnemyType.Scorpion));
                    ReturnedCreatures.Add(new RangedEnemy(new Vector2(300, -100), EnemyType.Scorpion));
                    ReturnedCreatures.Add(new MeleeEnemy(new Vector2(450, -100), EnemyType.Thief));
                    ReturnedCreatures.Add(new MeleeEnemy(new Vector2(600, -100), EnemyType.Thief));
                    ReturnedCreatures.Add(new RangedEnemy(new Vector2(750, -100), EnemyType.Scorpion));
                    ReturnedCreatures.Add(new RangedEnemy(new Vector2(900, -100), EnemyType.Scorpion));
                    ReturnedCreatures.Add(new MeleeEnemy(new Vector2(900, -200), EnemyType.Skeleton));

                    break;

                case 8:
                    ReturnedCreatures.Add(new MeleeEnemy(new Vector2(150, -200), EnemyType.Skeleton));
                    ReturnedCreatures.Add(new MeleeEnemy(new Vector2(150, -100), EnemyType.Skeleton));
                    ReturnedCreatures.Add(new MeleeEnemy(new Vector2(300, -300), EnemyType.Thief));
                    ReturnedCreatures.Add(new RangedEnemy(new Vector2(300, -100), EnemyType.AxeMaster));
                    ReturnedCreatures.Add(new RangedEnemy(new Vector2(450, -100), EnemyType.Scorpion));
                    ReturnedCreatures.Add(new RangedEnemy(new Vector2(600, -100), EnemyType.Scorpion));
                    ReturnedCreatures.Add(new RangedEnemy(new Vector2(750, -100), EnemyType.AxeMaster));
                    ReturnedCreatures.Add(new MeleeEnemy(new Vector2(750, -300), EnemyType.Thief));
                    ReturnedCreatures.Add(new MeleeEnemy(new Vector2(900, -100), EnemyType.Skeleton));
                    ReturnedCreatures.Add(new MeleeEnemy(new Vector2(900, -200), EnemyType.Skeleton));

                    break;

                case 9:
                    ReturnedCreatures.Add(new MeleeEnemy(new Vector2(300, -400), EnemyType.Thief));
                    ReturnedCreatures.Add(new RangedEnemy(new Vector2(300, -100), EnemyType.AxeMaster));
                    ReturnedCreatures.Add(new MeleeEnemy(new Vector2(450, -100), EnemyType.Thief));
                    ReturnedCreatures.Add(new MeleeEnemy(new Vector2(750, -400), EnemyType.Thief));
                    ReturnedCreatures.Add(new RangedEnemy(new Vector2(750, -100), EnemyType.AxeMaster));
                    ReturnedCreatures.Add(new MeleeEnemy(new Vector2(900, -100), EnemyType.Thief));

                    break;

                case 10:
                    ReturnedCreatures.Add(new Boss(new Vector2(Main.playingAreaX + Main.playingAreaWidth / 2, 180)));
                    Main.background.texture = Scripts.LoadTexture(@"Sprites\Background\Myst");
                    GUI.castleGate = Scripts.LoadTexture(@"Sprites\GUI\MystyCastle");
                    break;

                default:

                    for (int i = 0; i < 4; i++)
                    {
                        int randomPosition = AvailablePositions[rand.Next(AvailablePositions.Count)];
                        AvailablePositions.Remove(randomPosition);
                        ReturnedCreatures.Add(new RangedEnemy(new Vector2(randomPosition, 0), EnemyType.AxeMaster));
                    }

                    break;
            }
            return ReturnedCreatures;
        }
    }
}