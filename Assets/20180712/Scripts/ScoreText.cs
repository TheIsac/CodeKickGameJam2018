using UnityEngine;
using UnityEngine.UI;

namespace _20180712.Scripts
{
    public class ScoreText : MonoBehaviour
    {
        public ScoreHolder ScoreHolder;

        private Text text;

        void Start()
        {
            text = GetComponent<Text>();
        }

        void Update()
        {
            text.text = "Score: " + ScoreHolder.GetScore();
        }
    }
}