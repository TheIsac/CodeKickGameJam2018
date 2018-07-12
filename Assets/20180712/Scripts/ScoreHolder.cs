using UnityEngine;

namespace _20180712.Scripts
{
    public class ScoreHolder : MonoBehaviour
    {
        private int score;

        public void IncreaseScoreBy(int amount)
        {
            score += amount;
        }

        public int GetScore()
        {
            return score;
        }
    }
}