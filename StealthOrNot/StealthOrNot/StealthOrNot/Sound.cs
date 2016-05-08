using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;

namespace StealthOrNot
{
    public class Sound
    {
        private const string BaseFolder = @"Sounds\";

        private SoundEffect sound;

        private List<SoundEffectInstance> soundInstances;

        public Vector2 Position;

        private bool isLooped;

        public Sound(float soundDistance, Vector2 startPosition, float maxVolume = 1.0f)
        {
            SoundDistance = soundDistance;
            MaxVolume = maxVolume;
            soundInstances = new List<SoundEffectInstance>();
            isLooped = false;
            Position = startPosition;
        }

        public void Load(string asset)
        {
            sound = Main.content.Load<SoundEffect>(BaseFolder + asset);
        }

        public float SoundDistance { set; get; }

        public float MaxVolume { get; set; }

        public bool IsLooped
        {
            get
            {
                return isLooped;
            }
            set
            {
                isLooped = value;
            }
        }

        public void Update(Vector2 newPosition)
        {
            Position = newPosition;

            for (int i = 0; i < soundInstances.Count; i++)
            {
                if (soundInstances[i].State == SoundState.Stopped)
                {
                    soundInstances.Remove(soundInstances[i]);
                    i--;
                }
                else
                {
                    CalculateSoundEffectInstancePanAndVolume(soundInstances[i]);
                }
            }
        }

        private void CalculateSoundEffectInstancePanAndVolume(SoundEffectInstance soundEffectInstance)
        {
            if (!Main.MusicIsMuted)
            {
                float distance = Vector2.Distance(Main.EarPosition, Position);

                if (distance <= SoundDistance)
                {
                    soundEffectInstance.Volume = MathHelper.Lerp(MaxVolume, 0.05f, distance / SoundDistance);
                    soundEffectInstance.Pan = (Position.X - Main.EarPosition.X) / SoundDistance;
                }
                else
                {
                    soundEffectInstance.Volume = 0;
                }
            }
            else
            {
                soundEffectInstance.Volume = 0;
            }
        }

        public void Play()
        {
            if (!Main.MusicIsMuted)
            {
                float distance = Vector2.Distance(Main.EarPosition, Position);
                SoundEffectInstance instance = sound.CreateInstance();
                CalculateSoundEffectInstancePanAndVolume(instance);
                instance.IsLooped = isLooped;
                instance.Play();
                soundInstances.Add(instance);
            }
        }
    }
}