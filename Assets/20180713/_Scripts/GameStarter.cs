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
        public ArenaSize SizeOfArena = ArenaSize.Large;

        public bool UseScoreboard = true;
        public bool StartImmediately = true;

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

        public enum ArenaSize
        {
            Small,
            Large,
            Gigantic
        }

        private int arenaWidth = 20;
        private int arenaHeight = 20;
        private int shipCornerOffset = 5;

        public List<GameObject> Players = new List<GameObject>();
        public List<GameObject> Ships = new List<GameObject>();

        private CameraManager cameraManager;
        private ShipManager shipManager;
        private GameTimer gameTimer;
        private Scoreboard scoreboard;

        private bool foundTimer;
        private float gameTimeLeft = 5 * 60;
        private bool gameHasEnded = false;

        public bool HasLoaded = false;

        private void Start()
        {
            if (StartImmediately)
            {
                SetupGameWithCurrentSettings();
                StartGame(Players);
            }
        }

        void Update()
        {
            if (foundTimer) UpdateGameTimer();

            var shouldEndGame = gameTimeLeft < 0.001 && !gameHasEnded;
            if (shouldEndGame) EndGame();

            MakeSurePlayersAreInsideArena();
        }

        public void SetupGameWithCurrentSettings()
        {
            SetupGame(SizeOfArena, PlayerCount, BotCount, PlayerNames);
        }

        private void SetupGame(ArenaSize arenaSize, int playerCount, int botCount, List<string> playerNames)
        {
            cameraManager = GameObject.FindWithTag("CameraManager").GetComponent<CameraManager>();
            shipManager = GameObject.FindWithTag("ShipManager").GetComponent<ShipManager>();

            ResetArenaSize(arenaSize);

            var availablePlayerNames = playerNames.ToList();
            var players = new List<GameObject>();
            var playerShips = new List<GameObject>();
            for (var i = 0; i < playerCount; i++)
            {
                var playerNameIndex = Random.Range(0, availablePlayerNames.Count - 1);
                var playerName = availablePlayerNames[playerNameIndex];
                availablePlayerNames.RemoveAt(playerNameIndex);
                var playerOrder = i + 1;

                var player = i < playerCount - botCount
                    ? CreatePlayer(playerName, playerOrder)
                    : CreateBot(playerOrder);

                var playerShip = CreateShipAndPlacePlayerAboveShip(player);
                players.Add(player);
                playerShips.Add(playerShip);
            }

            ResetShips(playerShips);
            ResetPlayers(players);
            HasLoaded = true;
        }

        private void StartGame(List<GameObject> players)
        {
            if (UseScoreboard)
            {
                scoreboard = GameObject.FindWithTag("Scoreboard").GetComponent<Scoreboard>();

                foreach (var player in players)
                {
                    var playerComponent = player.GetComponent<Player>();
                    scoreboard.Players.Add(playerComponent);
                }

                scoreboard.ResetScoreboard();
            }

            var gameTimerObject = GameObject.FindWithTag("GameTimer");
            if (gameTimerObject)
            {
                foundTimer = true;
                gameTimer = gameTimerObject.GetComponent<GameTimer>();
                gameTimer.SetTime(GameLengthSeconds);
                gameTimeLeft = GameLengthSeconds;
                GameObject.FindWithTag("EndText").GetComponent<EndText>().SetText("");
            }

            //TODO Some bug requires us to deactivate-activate the main camera for it to display stuff properly
            var mainCamera = GameObject.FindWithTag("MainCamera");
            mainCamera.SetActive(false);
            mainCamera.SetActive(true);
        }

        private void ResetShips(List<GameObject> ships)
        {
            if (Ships != null)
            {
                Ships.ForEach(Destroy);
                Ships.Clear();
                Ships.AddRange(ships);
            }
            else
            {
                Ships = ships;
            }

            shipManager.Ships.Clear();
            var shipComponents = ships.Select(s => s.GetComponent<Base>());
            shipManager.Ships.AddRange(shipComponents);
        }

        private void ResetPlayers(List<GameObject> players)
        {
            if (Players != null)
            {
                Players.ForEach(Destroy);
                Players.Clear();
                Players.AddRange(players);
            }
            else
            {
                Players = players;
            }
        }

        private void ResetArenaSize(ArenaSize size)
        {
            if (size == ArenaSize.Large) SetToLargeArena();
            else if (size == ArenaSize.Small) SetToSmallArena();
            else if (size == ArenaSize.Gigantic) SetToGiganticArena();
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
            shipComponent.SetOwner(player);
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

        private void UpdateGameTimer()
        {
            gameTimeLeft = Math.Max(0, gameTimeLeft - Time.deltaTime);
            gameTimer.SetTime(gameTimeLeft);
        }

        private void EndGame()
        {
            gameHasEnded = true;

            var text = "Winner is " + scoreboard.GetLeaderName() + " with a score of " +
                       scoreboard.GetLeaderScore();
            GameObject.FindWithTag("EndText").GetComponent<EndText>().SetText(text);

            foreach (var block in BlockManager.ActiveBlocks)
            {
                if (block.IsFree()) block.BlowUp();
            }

            foreach (var ship in Ships)
            {
                var shipComponent = ship.GetComponent<Base>();
                shipComponent.BlowUpAllBlocksExceptPilot();
            }
        }

        private void MakeSurePlayersAreInsideArena()
        {
            foreach (var player in Players)
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
    }
}