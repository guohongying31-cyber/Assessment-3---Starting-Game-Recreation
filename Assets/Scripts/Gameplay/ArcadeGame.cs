using UnityEngine;
using UnityEngine.SceneManagement;

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
        [SerializeField] private bool talismanMode;
        [SerializeField] private GameObject wardPrefab;
        [SerializeField] private GameObject lifeIndicatorPrefab;
        public MazeNavigation Navigation { get; private set; }
        public GridMotor Player { get; private set; }
        public GhostDirector Ghosts { get; private set; }
        public PickupLedger Pickups { get; private set; }
        public ArcadePhase Phase { get; private set; }
        public int Score { get; private set; }
        public int Lives { get; private set; }
        public int Round { get; private set; }
        public float ReadyTime { get; private set; }
        public CherryController Cherry { get; private set; }
        public bool ExtraLifeGranted { get; private set; }
        public int GhostCombo { get; private set; }
        public double ElapsedSeconds { get; private set; }
        public bool TalismanMode => talismanMode;
        public TalismanWard Ward { get; } = new TalismanWard();
        public int BestScore => Mathf.Max(Score, PlayerPrefs.GetInt(BestKey, 0));
        private string BestKey => talismanMode ? "SpiritTrail.TrialBest" : "SpiritTrail.ClassicBest";
        private ActorSprite playerView;
        private PacStudentController playerController;
        private PlayerParticles particles;
        private readonly ActorSprite[] ghostViews = new ActorSprite[4];
        private readonly Vector2[] previousGhosts = new Vector2[4];
        private readonly bool[] edibleBeforeStep = new bool[4];
        private GameObject wardView;
        private Animator[] pickupAnimators;
        private float aspect, deathTime;
        private ArcadePhase beforePause;
        private readonly MazeCell playerStart = new MazeCell(1, 1);

        private void Start()
        {
            Navigation = new MazeNavigation(generator.GeneratedLayout, new MazeCell(1, 1));
            pickupAnimators = generator.GeneratedLevel.GetComponentsInChildren<Animator>(true);
            playerView = Instantiate(playerPrefab).AddComponent<ActorSprite>();
            playerView.name = "Cultivator_Player";
            playerController = playerView.gameObject.AddComponent<PacStudentController>();
            playerController.Initialize(Navigation, playerStart);
            Player = playerController.Motor;
            Player.Arrived += Arrive;
            particles = playerView.gameObject.AddComponent<PlayerParticles>();
            var moteRenderer = generator.TileAt(1,2).GetComponentInChildren<SpriteRenderer>();
            particles.Initialize(moteRenderer.sprite, moteRenderer.sharedMaterial);
            Player.Blocked += direction => { audioPlayer.Effect("Wall"); particles.Impact(Player.Position,direction); };
            hud.SetLifeSprite(lifeIndicatorPrefab.GetComponentInChildren<SpriteRenderer>().sprite);
            Ghosts = new GhostDirector(generator.GeneratedLayout, Navigation);
            for(int i=0; i<4; i++)
            {
                ghostViews[i] = Instantiate(ghostPrefabs[i]).AddComponent<ActorSprite>();
                ghostViews[i].name = "Guardian_" + i;
                GhostNumberLabel.Create(ghostViews[i].transform, i + 1);
            }
            Cherry = gameObject.AddComponent<CherryController>();
            Cherry.Initialize(fruitPrefab, Navigation.Rows, Navigation.Columns, levelCamera);
            if(talismanMode)
            {
                wardView = Instantiate(wardPrefab);
                wardView.name = "Active_TalismanWard";
                wardView.transform.localScale *= 2.1f;
                foreach(var renderer in wardView.GetComponentsInChildren<SpriteRenderer>())
                { renderer.sortingOrder = 9; renderer.color = new Color(1,1,1,.6f); }
            }
            NewGame();
        }

        public void NewGame()
        {
            Score = 0; Lives = 3; Round = 1; ExtraLifeGranted = false;
            ElapsedSeconds = 0;
            Cherry.ResetClock();
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
            playerController.ResetLife(playerStart);
            Player.Enabled = false;
            Ghosts.Reset(Round);
            GhostCombo = 0;
            particles.Clear();
            Ward.Reset();
            Phase = ArcadePhase.Ready; ReadyTime = 2.4f;
            audioPlayer.Music("Intro", true);
        }
        private void Collected(MazeCell cell, int category)
        {
            generator.TileAt(cell.Row, cell.Column).SetActive(false);
            if(category == 6)
            {
                GhostCombo = 0;
                Ghosts.ActivatePower(Mathf.Max(2, 6 - (Round-1)*.4f));
            }
        }
        private void Arrive(MazeCell cell)
        {
            AddScore(Pickups.Consume(cell));
            if (Pickups.Remaining != 0) return;
            Phase = ArcadePhase.RoundClear; Player.Enabled = false;
            RecordBest();
            audioPlayer.Music(null);
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
            playerController.InputEnabled = active;
            playerView.transform.position = Player.Position;
            playerView.Present(Player.Direction == GridDirection.None ? GridDirection.Right : Player.Direction, active && Player.IsMoving || Phase == ArcadePhase.Dying,
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
            if(wardView)
            {
                wardView.SetActive(Ward.Active);
                wardView.transform.position = Player.Position;
                wardView.GetComponent<Animator>().speed = active ? 1 : 0;
            }
            if(active) audioPlayer.Music(returning ? "Dead" : Ghosts.FrightRemaining > 0 ? "Scared" : "Normal");
            audioPlayer.Moving(active && Player.IsMoving, Pickups.Contains(Player.TargetCell));
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
                float step = Mathf.Min(remaining, 1f/120);
                if(Phase == ArcadePhase.Playing)
                {
                    if(Ghosts.FrightRemaining > 0) step = Mathf.Min(step,Ghosts.FrightRemaining);
                    if(Ward.Active) step = Mathf.Min(step,Ward.Remaining);
                }
                remaining -= step;
                if(Phase == ArcadePhase.Ready)
                {
                    Cherry.Tick(step);
                    ReadyTime -= step;
                    if(ReadyTime <= 0) { Phase = ArcadePhase.Playing; Player.Enabled = true; }
                }
                else if(Phase == ArcadePhase.Dying)
                {
                    particles.Tick(step,Player.Position,false);
                    deathTime -= step;
                    if(deathTime <= 0)
                    {
                        if(Lives == 0) { Phase = ArcadePhase.GameOver; RecordBest(); audioPlayer.Music(null); }
                        else ResetActors();
                    }
                }
                else TickPlaying(step);
            }
        }
        private void TickPlaying(float step)
        {
            Vector2 previousPlayer = Player.Position;
            for(int i=0; i<4; i++)
            {
                previousGhosts[i] = Ghosts.Ghosts[i].Motor.Position;
                edibleBeforeStep[i] = Ghosts.Ghosts[i].IsEdible;
            }
            bool protectedDuringStep = Ward.Active;
            Cherry.Tick(step);
            Ward.Tick(step);
            Player.Tick(step);
            particles.Tick(step,Player.Position,Player.IsMoving);
            if(Phase != ArcadePhase.Playing) return;
            bool fruitInside = Cherry.Position.x >= -.5f && Cherry.Position.x <= Navigation.Columns-.5f
                && Cherry.Position.y <= .5f && Cherry.Position.y >= -Navigation.Rows+.5f;
            if(Cherry.Active && fruitInside && MazeContact.Crossed(previousPlayer,Player.Position,Cherry.PreviousPosition,Cherry.Position,Navigation.Columns) && Cherry.Collect())
            { AddScore(100); audioPlayer.Effect("Fruit"); }
            Ghosts.Tick(step,Player.CurrentCell,Player.Direction);
            for(int i=0; i<4; i++)
            {
                var ghost = Ghosts.Ghosts[i];
                if(!MazeContact.Crossed(previousPlayer,Player.Position,previousGhosts[i],ghost.Motor.Position,Navigation.Columns)) continue;
                if(Ghosts.Eat(i, edibleBeforeStep[i]))
                {
                    AddScore(200 << Mathf.Min(GhostCombo,3)); GhostCombo++; audioPlayer.Effect("Ghost");
                }
                else if(ghost.IsDangerous && !protectedDuringStep) { LoseLife(); break; }
            }
        }
        private void LoseLife()
        {
            Lives--; Phase = ArcadePhase.Dying; deathTime = 1.8f;
            Player.Enabled = false;
            particles.Death(Player.Position);
            audioPlayer.Music(null); audioPlayer.Effect("Death");
        }
        private void ReadInput()
        {
            if(Input.GetKeyDown(KeyCode.M) && Phase != ArcadePhase.Playing && Phase != ArcadePhase.Dying) ReturnToMenu();
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)) TogglePause();
            if (Phase == ArcadePhase.RoundClear && Input.GetKeyDown(KeyCode.Return)) NextRound();
            if (Phase == ArcadePhase.GameOver && Input.GetKeyDown(KeyCode.Return)) NewGame();
            if (Phase != ArcadePhase.Ready && Phase != ArcadePhase.Playing) return;
            if(Input.GetKeyDown(KeyCode.Space)) TryActivateWard();
        }
        public bool TryActivateWard()
        {
            if(!talismanMode || Phase != ArcadePhase.Playing || !Ward.TryActivate()) return false;
            audioPlayer.Effect("Ghost");
            return true;
        }
        public void ReturnToMenu() { RecordBest(); SceneManager.LoadScene("StartScene"); }
        private void RecordBest()
        {
            if(Score <= PlayerPrefs.GetInt(BestKey,0)) return;
            PlayerPrefs.SetInt(BestKey,Score); PlayerPrefs.Save();
        }
        private void OnApplicationQuit() => RecordBest();
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
