using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _20180713._Scripts
{
    public class GameStarter : MonoBehaviour
    {
        public int PlayerCount = 4;
        public int BotCount = 1;
        public int GameLengthSeconds = 5 * 60;
        
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

        private enum ArenaSize
        {
            Small,
            Large,
            Gigantic
        }

        [SerializeField] private ArenaSize arenaSize = ArenaSize.Large;
        private int arenaWidth = 20;
        private int arenaHeight = 20;
        private int shipCornerOffset = 5;

        private readonly List<GameObject> players = new List<GameObject>();
        private readonly List<GameObject> ships = new List<GameObject>();

        private CameraManager cameraManager;

        private GameTimer gameTimer;
        private bool foundTimer;
        private Scoreboard scoreboard;
        private float gameTimeLeft = 5 * 60;
        private bool gameHasEnded = false;

        public bool HasLoaded = false;

        private void Start()
        {
            cameraManager = GameObject.FindWithTag("CameraManager").GetComponent<CameraManager>();
            if (arenaSize == ArenaSize.Large) SetToLargeArena();
            else if (arenaSize == ArenaSize.Small) SetToSmallArena();
            else if (arenaSize == ArenaSize.Gigantic) SetToGiganticArena();

            scoreboard = GameObject.FindWithTag("Scoreboard").GetComponent<Scoreboard>();
            for (var i = 0; i < PlayerCount; i++)
            {
                var playerNameIndex = Random.Range(0, PlayerNames.Count - 1);
                var playerName = PlayerNames[playerNameIndex];
                PlayerNames.RemoveAt(playerNameIndex);
                var playerOrder = i + 1;

                var player = i < PlayerCount - BotCount
                    ? CreatePlayer(playerName, playerOrder)
                    : CreateBot(playerOrder);

                var playerShip = CreateShipAndPlacePlayerAboveShip(player);
                var playerComponent = player.GetComponent<Player>();
                scoreboard.Players.Add(playerComponent);
                players.Add(player);
                ships.Add(playerShip);
            }

            var shipManager = GameObject.FindWithTag("ShipManager").GetComponent<ShipManager>();
            var shipComponents = ships.Select(shipGameObject => shipGameObject.GetComponent<Base>());
            shipManager.Ships.AddRange(shipComponents);

            var gameTimerObject = GameObject.FindWithTag("GameTimer");
            if (gameTimerObject)
            {
                foundTimer = true;
                gameTimer = gameTimerObject.GetComponent<GameTimer>();
                gameTimer.SetTime(GameLengthSeconds);
                gameTimeLeft = GameLengthSeconds;
                GameObject.FindWithTag("EndText").GetComponent<EndText>().SetText("");
            }

            scoreboard.ResetScoreboard();

            //TODO Some bug requires us to deactivate-activate the main camera for it to display stuff properly
            var mainCamera = GameObject.FindWithTag("MainCamera");
            mainCamera.SetActive(false);
            mainCamera.SetActive(true);

            HasLoaded = true;
        }

        void Update()
        {
            if (foundTimer)
            {
                gameTimeLeft = Math.Max(0, gameTimeLeft - Time.deltaTime);
                gameTimer.SetTime(gameTimeLeft);
            }

            if (gameTimeLeft < 0.001 && !gameHasEnded)
            {
                gameHasEnded = true;

                var text = "Winner is " + scoreboard.GetLeaderName() + " with a score of " +
                           scoreboard.GetLeaderScore();
                GameObject.FindWithTag("EndText").GetComponent<EndText>().SetText(text);

                foreach (var block in BlockManager.ActiveBlocks)
                {
                    if (block.IsFree()) block.BlowUp();
                }

                foreach (var ship in ships)
                {
                    var shipComponent = ship.GetComponent<Base>();
                    shipComponent.BlowUpAllBlocksExceptPilot();
                }
            }

            foreach (var player in players)
            {
                var position = player.transform.position;
                if (position.x > arenaWidth || position.x < -arenaWidth)
                {
                    player.transform.position = new Vector3(
                        0,
                        position.y,
                        position.z
                    );
                }

                if (position.y > 1 || position.y < -1)
                {
                    player.transform.position = new Vector3(
                        position.x,
                        0,
                        position.z
                    );
                }

                if (position.z > arenaHeight || position.z < -arenaHeight)
                {
                    player.transform.position = new Vector3(
                        position.x,
                        position.y,
                        0
                    );
                }
            }
        }

        public Vector2 GetArenaDimensions()
        {
            return new Vector2(arenaWidth, arenaHeight);
        }

        private GameObject CreatePlayer(string playerName, int order)
        {
            var player = Instantiate(PlayerTemplate);
            player.name = playerName;
            var playerComponent = player.GetComponent<Player>();
            playerComponent.Name = playerName;
            playerComponent.Order = order;
            return player;
        }

        private GameObject CreateBot(int order)
        {
            var player = CreatePlayer("Anna (" + order + ")", order);
            player.gameObject.AddComponent<Bot>();
            return player;
        }

        private GameObject CreateShipAndPlacePlayerAboveShip(GameObject player)
        {
            var playerShip = Instantiate(BaseTemplate);
            playerShip.name = "Ship " + player.name;
            var playerComponent = player.GetComponent<Player>();
            playerShip.transform.position = GetShipPositionByPlayerOrder(playerComponent.Order);
            player.transform.position = GetShipPositionByPlayerOrder(playerComponent.Order) + Vector3.up;
            var shipComponent = playerShip.GetComponent<Base>();
            var playerShipOwnerComponent = player.GetComponent<ShipOwner>();
            playerShipOwnerComponent.OwnShip = shipComponent;

            var pilotBlockController = playerShip.GetComponentInChildren<PilotBlockController>();
            pilotBlockController.Owner = playerComponent;

            return playerShip;
        }

        private Vector3 GetShipPositionByPlayerOrder(int order)
        {
            if (order == 1) //Top left corner
            {
                return new Vector3(
                    -arenaWidth * .5f + shipCornerOffset,
                    -1,
                    arenaHeight * .5f - shipCornerOffset
                );
            }

            if (order == 4) //Top right corner
            {
                return new Vector3(
                    arenaWidth * .5f - shipCornerOffset,
                    -1,
                    arenaHeight * .5f - shipCornerOffset
                );
            }

            if (order == 2) //Bottom right corner
            {
                return new Vector3(
                    arenaWidth * .5f - shipCornerOffset,
                    -1,
                    -arenaHeight * .5f + shipCornerOffset
                );
            }

            if (order == 3) //Bottom left corner
            {
                return new Vector3(
                    -arenaWidth * .5f + shipCornerOffset,
                    -1,
                    -arenaHeight * .5f + shipCornerOffset
                );
            }

            return Vector3.zero;
        }

        private void SetToSmallArena()
        {
            arenaWidth = 30;
            arenaHeight = 15;
            shipCornerOffset = 4;
            cameraManager.SetToSmallArena();
        }

        private void SetToLargeArena()
        {
            arenaWidth = 40;
            arenaHeight = 20;
            shipCornerOffset = 5;
            cameraManager.SetToLargeArena();
        }
        
        private void SetToGiganticArena()
        {
            arenaWidth = 80;
            arenaHeight = 40;
            shipCornerOffset = 10;
            cameraManager.SetToGiganticArena();
        }
    }
}