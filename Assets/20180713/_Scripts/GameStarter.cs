using System.Collections.Generic;
using UnityEngine;

namespace _20180713._Scripts
{
    public class GameStarter : MonoBehaviour
    {
        public int PlayerCount = 2;

        public int ArenaWidth = 20;
        public int ArenaHeight = 20;
        public int BaseCornerOffset = 5;

        public GameObject BaseTemplate;
        public GameObject PlayerTemplate;

        private readonly List<GameObject> players = new List<GameObject>();
        private readonly List<GameObject> ships = new List<GameObject>();

        public void Start()
        {
            for (var i = 0; i < PlayerCount; i++)
            {
                var playerOrder = i + 1;
                var player = CreatePlayer("Player " + playerOrder, playerOrder);
                var playerShip = CreatePlayerBase(player);
                players.Add(player);
                ships.Add(playerShip);
            }
        }

        private GameObject CreatePlayer(string playerName, int order)
        {
            var player = Instantiate(PlayerTemplate);
            var playerComponent = player.GetComponent<Player>();
            playerComponent.Name = playerName;
            playerComponent.Order = order;
            return player;
        }

        private GameObject CreatePlayerBase(GameObject player)
        {
            var playerShip = Instantiate(BaseTemplate);
            var shipComponent = playerShip.GetComponent<Base>();
            var playerComponent = player.GetComponent<Player>();
            playerShip.transform.position = GetShipPositionByPlayerOrder(playerComponent.Order);
            var playerShipOwnerComponent = player.GetComponent<ShipOwner>();
            playerShipOwnerComponent.OwnBase = shipComponent;

            var pilotBlockController = playerShip.GetComponentInChildren<PilotBlockController>();
            pilotBlockController.Owner = playerComponent;

            return playerShip;
        }

        private Vector3 GetShipPositionByPlayerOrder(int order)
        {
            if (order == 1) //Top left corner
            {
                return new Vector3(
                    -ArenaWidth * .5f + BaseCornerOffset,
                    -1,
                    ArenaHeight * .5f - BaseCornerOffset
                );
            }

            if (order == 4) //Top right corner
            {
                return new Vector3(
                    ArenaWidth * .5f - BaseCornerOffset,
                    -1,
                    ArenaHeight * .5f - BaseCornerOffset
                );
            }

            if (order == 2) //Bottom right corner
            {
                return new Vector3(
                    ArenaWidth * .5f - BaseCornerOffset,
                    -1,
                    -ArenaHeight * .5f + BaseCornerOffset
                );
            }

            if (order == 3) //Bottom left corner
            {
                return new Vector3(
                    -ArenaWidth * .5f + BaseCornerOffset,
                    -1,
                    -ArenaHeight * .5f + BaseCornerOffset
                );
            }
            
            return Vector3.zero;
        }
    }
}