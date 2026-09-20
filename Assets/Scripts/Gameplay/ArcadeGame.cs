using UnityEngine;

namespace ShanHaiSpiritTrail
{
    public enum ArcadePhase { Ready, Playing, Paused, Dying, RoundClear, GameOver }

    [DefaultExecutionOrder(100)]
    public sealed class ArcadeGame : MonoBehaviour
    {
        [SerializeField] private LevelGenerator generator;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject[] ghostPrefabs = new GameObject[4];
        [SerializeField] private GameObject fruitPrefab;
        [SerializeField] private Camera levelCamera;
        [SerializeField] private ArcadeAudio audioPlayer;
        [SerializeField] private ArcadeHud hud;
        public MazeNavigation Navigation { get; private set; }
        public GridMotor Player { get; private set; }
        public GhostDirector Ghosts { get; private set; }
        public PickupLedger Pickups { get; private set; }
        public ArcadePhase Phase { get; private set; }
        public int Score { get; private set; }
        public int Lives { get; private set; }
        public int Round { get; private set; }
        public float ReadyTime { get; private set; }
        public float FruitTime { get; private set; }
        public bool ExtraLifeGranted { get; private set; }
        public int GhostCombo { get; private set; }
        private ActorSprite playerView;
        private readonly ActorSprite[] ghostViews = new ActorSprite[4];
        private readonly Vector2[] previousGhosts = new Vector2[4];
        private GameObject fruitView;
        private Animator[] pickupAnimators;
        private float aspect, deathTime, lastPickupTime = -1;
        private ArcadePhase beforePause;
        private readonly MazeCell playerStart = new MazeCell(17, 13);
        private readonly MazeCell fruitCell = new MazeCell(17, 14);

        private void Start()
        {
            Navigation = new MazeNavigation(generator.GeneratedLayout, new MazeCell(1, 1));
            pickupAnimators = generator.GeneratedLevel.GetComponentsInChildren<Animator>(true);
            playerView = Instantiate(playerPrefab).AddComponent<ActorSprite>();
            playerView.name = "Cultivator_Player";
            Player = new GridMotor(Navigation, playerStart, 5f);
            Player.Arrived += Arrive;
            Player.Blocked += _ => audioPlayer.Effect("Wall");
            Ghosts = new GhostDirector(generator.GeneratedLayout, Navigation);
            for(int i=0; i<4; i++)
            {
                ghostViews[i] = Instantiate(ghostPrefabs[i]).AddComponent<ActorSprite>();
                ghostViews[i].name = "Guardian_" + i;
            }
            fruitView = Instantiate(fruitPrefab);
            fruitView.name = "Bonus_CrimsonFruit";
            fruitView.transform.position = Navigation.WorldPosition(fruitCell);
            NewGame();
        }

        public void NewGame()
        {
            Score = 0; Lives = 3; Round = 1; ExtraLifeGranted = false;
            audioPlayer.Pause(false);
            PausePickups(false);
            StartRound();
        }
        private void StartRound()
        {
            Pickups = new PickupLedger(Navigation);
            foreach (var cell in Navigation.Cells)
            {
                int category = Navigation.CategoryAt(cell);
                if (category == 5 || category == 6) generator.TileAt(cell.Row, cell.Column).SetActive(true);
            }
            Pickups.CollectedPickup += Collected;
            ResetActors();
        }
        private void ResetActors()
        {
            Player.Speed = Mathf.Min(6.5f, 5 + (Round-1)*.12f);
            Player.Reset(playerStart);
            Player.Enabled = false;
            Player.QueueDirection(GridDirection.Left);
            Ghosts.Reset(Round);
            GhostCombo = 0; FruitTime = 0;
            Phase = ArcadePhase.Ready; ReadyTime = 2.4f;
            audioPlayer.Music("Intro", true);
        }
        private void Collected(MazeCell cell, int category)
        {
            generator.TileAt(cell.Row, cell.Column).SetActive(false);
            lastPickupTime = Time.time;
            audioPlayer.Effect("Pellet");
            if(category == 6)
            {
                GhostCombo = 0;
                Ghosts.ActivatePower(Mathf.Max(2, 6 - (Round-1)*.4f));
            }
            if(Pickups.Collected == 70 || Pickups.Collected == 170) FruitTime = 10;
        }
        private void Arrive(MazeCell cell)
        {
            AddScore(Pickups.Consume(cell));
            if(FruitTime > 0 && cell == fruitCell)
            {
                FruitTime = 0; AddScore(FruitPoints()); audioPlayer.Effect("Fruit");
            }
            if (Pickups.Remaining != 0) return;
            Phase = ArcadePhase.RoundClear; Player.Enabled = false;
            audioPlayer.Music(null);
        }
        private int FruitPoints()
        {
            int[] points = {100,300,500,500,700,700,1000,1000,2000,2000,3000,3000,5000};
            return points[Mathf.Min(Round-1,points.Length-1)];
        }
        private void AddScore(int points)
        {
            Score += points;
            if(!ExtraLifeGranted && Score >= 10000)
            { Lives++; ExtraLifeGranted = true; }
        }
        private void Update()
        {
            if (Player == null) return;
            ReadInput();
            Advance(Time.deltaTime);
            Present();
        }
        public void Present()
        {
            bool active = Phase == ArcadePhase.Playing;
            playerView.transform.position = Player.Position;
            playerView.Present(Player.Direction, active && Player.IsMoving || Phase == ArcadePhase.Dying,
                Phase == ArcadePhase.Dying || Phase == ArcadePhase.GameOver ? "Dead" : "Walking");
            bool returning = false;
            for(int i=0; i<ghostViews.Length; i++)
            {
                var ghost = Ghosts.Ghosts[i];
                ghostViews[i].transform.position = ghost.Motor.Position;
                string form = ghost.State == GhostState.Eaten ? "Dead" : ghost.Frightened
                    ? (Ghosts.FrightRemaining <= 2 ? "Recovering" : "Scared") : "Walking";
                ghostViews[i].Present(ghost.Motor.Direction, active, form);
                returning |= ghost.State == GhostState.Eaten;
            }
            fruitView.SetActive(FruitTime > 0);
            if(active) audioPlayer.Music(returning ? "Dead" : Ghosts.FrightRemaining > 0 ? "Scared" : "Normal");
            audioPlayer.Moving(active && Player.IsMoving && Time.time - lastPickupTime > .15f);
            FitCamera(); hud.Draw(this);
        }
        public void Advance(float deltaTime)
        {
            if(deltaTime < 0 || float.IsNaN(deltaTime) || float.IsInfinity(deltaTime))
                throw new System.ArgumentOutOfRangeException(nameof(deltaTime));
            // Small shared steps plus swept contact tests keep actor interactions
            // consistent when a rendered frame crosses several cell centres.
            float remaining = deltaTime;
            while(remaining > 0 && Phase != ArcadePhase.Paused && Phase != ArcadePhase.RoundClear && Phase != ArcadePhase.GameOver)
            {
                float step = Mathf.Min(remaining, 1f/120); remaining -= step;
                if(Phase == ArcadePhase.Ready)
                {
                    ReadyTime -= step;
                    if(ReadyTime <= 0) { Phase = ArcadePhase.Playing; Player.Enabled = true; }
                }
                else if(Phase == ArcadePhase.Dying)
                {
                    deathTime -= step;
                    if(deathTime <= 0)
                    {
                        if(Lives == 0) { Phase = ArcadePhase.GameOver; audioPlayer.Music(null); }
                        else ResetActors();
                    }
                }
                else TickPlaying(step);
            }
        }
        private void TickPlaying(float step)
        {
            Vector2 previousPlayer = Player.Position;
            for(int i=0; i<4; i++) previousGhosts[i] = Ghosts.Ghosts[i].Motor.Position;
            FruitTime = Mathf.Max(0, FruitTime-step);
            Player.Tick(step);
            if(Phase != ArcadePhase.Playing) return;
            Ghosts.Tick(step,Player.CurrentCell,Player.Direction);
            for(int i=0; i<4; i++)
            {
                var ghost = Ghosts.Ghosts[i];
                if(!MazeContact.Crossed(previousPlayer,Player.Position,previousGhosts[i],ghost.Motor.Position,Navigation.Columns)) continue;
                if(ghost.IsEdible && Ghosts.Eat(i))
                {
                    AddScore(200 << Mathf.Min(GhostCombo,3)); GhostCombo++; audioPlayer.Effect("Ghost");
                }
                else if(ghost.IsDangerous) { LoseLife(); break; }
            }
        }
        private void LoseLife()
        {
            Lives--; Phase = ArcadePhase.Dying; deathTime = 1.8f;
            Player.Enabled = false; FruitTime = 0;
            audioPlayer.Music(null); audioPlayer.Effect("Death");
        }
        private void ReadInput()
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)) TogglePause();
            if (Phase == ArcadePhase.RoundClear && Input.GetKeyDown(KeyCode.Return)) NextRound();
            if (Phase == ArcadePhase.GameOver && Input.GetKeyDown(KeyCode.Return)) NewGame();
            if (Phase != ArcadePhase.Ready && Phase != ArcadePhase.Playing) return;
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) Player.QueueDirection(GridDirection.Up);
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) Player.QueueDirection(GridDirection.Left);
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) Player.QueueDirection(GridDirection.Down);
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) Player.QueueDirection(GridDirection.Right);
        }
        public void TogglePause()
        {
            if (Phase == ArcadePhase.Paused) { Phase = beforePause; audioPlayer.Pause(false); PausePickups(false); }
            else if (Phase == ArcadePhase.Playing || Phase == ArcadePhase.Ready)
            { beforePause = Phase; Phase = ArcadePhase.Paused; audioPlayer.Pause(true); PausePickups(true); }
        }
        private void PausePickups(bool pause)
        {
            foreach(var pickup in pickupAnimators) pickup.speed = pause ? 0 : 1;
        }
        public void NextRound()
        {
            if (Phase != ArcadePhase.RoundClear) return;
            Round++; StartRound();
        }
        private void OnApplicationFocus(bool focus)
        {
            if (!focus && Player != null && (Phase == ArcadePhase.Playing || Phase == ArcadePhase.Ready)) TogglePause();
        }
        private void FitCamera()
        {
            if (Mathf.Approximately(aspect, levelCamera.aspect)) return;
            aspect = levelCamera.aspect;
            levelCamera.transform.position = new Vector3((generator.GeneratedLayout.Columns - 1) * .5f,
                -(generator.GeneratedLayout.Rows - 1) * .5f, -10);
            levelCamera.orthographicSize = Mathf.Max((generator.GeneratedLayout.Rows + 7) * .5f,
                (generator.GeneratedLayout.Columns + 3) / (2 * aspect));
        }
    }
}
