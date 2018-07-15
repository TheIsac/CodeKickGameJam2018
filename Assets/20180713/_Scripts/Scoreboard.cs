using System.Collections.Generic;
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
                text.transform.position = transform.position + Vector3.down * 60 * i;
                scoreTexts.Add(text);
            }
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