using System;

namespace Snake
{
    [Serializable]
    public struct PlayerScore
    {
        public string Name { get; set; }
        public int Score { get; set; }

        public PlayerScore(string name, int score)
            :this()
        {
            Name = name;
            Score = score;
        }
    }
}
