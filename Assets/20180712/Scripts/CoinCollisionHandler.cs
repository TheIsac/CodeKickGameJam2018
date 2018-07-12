using UnityEngine;

namespace _20180712.Scripts
{
    public class CoinCollisionHandler : MonoBehaviour
    {
        private void OnCollisionEnter(Collision other)
        {
            var scoreHolder = other.collider.GetComponent<ScoreHolder>();
            scoreHolder.IncreaseScoreBy(1);
        }
    }
}