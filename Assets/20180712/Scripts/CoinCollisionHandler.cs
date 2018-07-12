using UnityEngine;

namespace _20180712.Scripts
{
    public class CoinCollisionHandler : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D other)
        {
            Destroy(gameObject);
            var scoreHolder = other.gameObject.GetComponent<ScoreHolder>();
            scoreHolder.IncreaseScoreBy(1);
        }
    }
}