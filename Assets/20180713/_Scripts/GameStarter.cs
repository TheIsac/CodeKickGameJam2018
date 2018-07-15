using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _20180713._Scripts
{
    public class GameStarter : MonoBehaviour
    {
        public int PlayerCount = 2;

        public int ArenaWidth = 20;
        public int ArenaHeight = 20;
        public int BaseCornerOffset = 5;

        public List<string> PlayerNames = new List<string>
        {
            "Isac",
            "Gabriel",
            "David",
            "Gustav",
            "Heimer",
            "August"
        };

        public GameObject BaseTemplate;
        public GameObject PlayerTemplate;

        private readonly List<GameObject> players = new List<GameObject>();
        private readonly List<GameObject> ships = new List<GameObject>();

        public void Start()
        {
            var scoreboard = GameObject.FindWithTag("Scoreboard");
            var scoreboardComponent = scoreboard.GetComponent<Scoreboard>();
            for (var i = 0; i < PlayerCount; i++)
            {
                var playerNameIndex = Random.Range(0, PlayerNames.Count - 1);
                var playerName = PlayerNames[playerNameIndex];
                PlayerNames.RemoveAt(playerNameIndex);
                var playerOrder = i + 1;
                var player = CreatePlayer(playerName, playerOrder);
                var playerShip = CreateShipAndPlacePlayerAboveShip(player);
                var playerComponent = player.GetComponent<Player>();
                scoreboardComponent.Players.Add(playerComponent);
                players.Add(player);
                ships.Add(playerShip);
            }

            var shipManager = GameObject.FindWithTag("ShipManager").GetComponent<ShipManager>();
            var shipComponents = ships.Select(shipGameObject => shipGameObject.GetComponent<Base>());
            shipManager.Ships.AddRange(shipComponents);

            scoreboardComponent.ResetScoreboard();
        }

        private GameObject CreatePlayer(string playerName, int order)
        {
            var player = Instantiate(PlayerTemplate);
            var playerComponent = player.GetComponent<Player>();
            playerComponent.Name = playerName;
            playerComponent.Order = order;
            return player;
        }

        private GameObject CreateShipAndPlacePlayerAboveShip(GameObject player)
        {
            var playerShip = Instantiate(BaseTemplate);
            var playerComponent = player.GetComponent<Player>();
            playerShip.transform.position = GetShipPositionByPlayerOrder(playerComponent.Order);
            player.transform.position = GetShipPositionByPlayerOrder(playerComponent.Order) + Vector3.up;
            var shipComponent = playerShip.GetComponent<Base>();
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