using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace _20180713._Scripts
{
    public class Scoreboard : MonoBehaviour
    {
        public List<Player> Players;
        public Text ScoreTemplate;

        private List<Text> scoreTexts;

        void Awake()
        {
            ResetScoreboard();
        }

        public void ResetScoreboard()
        {
            scoreTexts = new List<Text>();
            for (var i = 0; i < Players.Count; i++)
            {
                var text = Instantiate(ScoreTemplate);
                text.transform.SetParent(transform);
                text.transform.position = transform.position + Vector3.right * 200 * i;
                scoreTexts.Add(text);
            }
        }

        public string GetLeaderName()
        {
            var leaderPlayer = Players[0];
            var leaderScore = -1f;
            foreach (var player in Players)
            {
                var playerScore = player.GetScore();
                if (playerScore > leaderScore || leaderScore < 0)
                {
                    leaderPlayer = player;
                    leaderScore = playerScore;
                }
            }

            return leaderPlayer.Name;
        }

        public float GetLeaderScore()
        {
            return Players.Max(p => p.GetScore());
        }

        void Update()
        {
            for (var i = 0; i < Players.Count; i++)
            {
                var player = Players[i];

                scoreTexts[i].text = player.Name + " " + player.GetScore();
            }
        }
    }
}