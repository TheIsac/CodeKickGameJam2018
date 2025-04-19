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
        public float marginSides = 0f; // Margin from the left and right edges

        private List<Text> scoreTexts;
        private RectTransform rectTransform; // Cache the RectTransform

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>(); // Get the RectTransform
            ResetScoreboard();
        }

        public void ResetScoreboard()
        {
            // Clear existing texts if any
            if (scoreTexts != null)
            {
                foreach (var text in scoreTexts)
                {
                    if (text != null) Destroy(text.gameObject);
                }
            }

            scoreTexts = new List<Text>();
            if (Players == null || Players.Count == 0 || ScoreTemplate == null || rectTransform == null)
            {
                Debug.LogWarning("Scoreboard setup incomplete.");
                return; // Exit if setup is not complete
            }

            var totalWidth = rectTransform.rect.width;
            var availableWidth = totalWidth - 2 * marginSides;
            var playerCount = Players.Count;

            // Ensure ScoreTemplate has a RectTransform
            var templateRect = ScoreTemplate.GetComponent<RectTransform>();
            if (templateRect == null)
            {
                Debug.LogError("ScoreTemplate prefab must have a RectTransform component.");
                return;
            }


            for (var i = 0; i < playerCount; i++)
            {
                var player = Players[i]; // Get the current player
                var text = Instantiate(ScoreTemplate);
                var textRect = text.GetComponent<RectTransform>(); // Get RectTransform of the instance

                textRect.SetParent(transform, false); // Set parent, worldPositionStays = false

                // Set the text color based on player order
                text.color = Player.GetPlayerColor(player.Order);

                float posX;
                if (playerCount == 1)
                {
                    // Center the single score text
                    posX = 0;
                }
                else
                {
                    // Calculate position based on even distribution
                    // Position is calculated from the center, so adjust start position
                    float startX = -availableWidth / 2f;
                    float spacing = availableWidth / (playerCount - 1); // Space between each score
                    posX = startX + i * spacing;
                }

                // Set anchored position - assumes pivot is center (0.5, 0.5)
                textRect.anchoredPosition = new Vector2(posX, 0); // Position along X, centered vertically for now

                scoreTexts.Add(text);
            }
        }

        public string GetLeaderName()
        {
            return GetLeader().Name;
        }

        public Color GetLeaderColor()
        {
            return GetLeader().GetPlayerColor();
        }

        public Player GetLeader()
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

            return leaderPlayer;
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