using UnityEngine;
using UnityEngine.SceneManagement;

namespace ShanHaiSpiritTrail
{
    public enum ArcadePhase { Ready, Playing, Paused, Dying, GameOver }

    [DefaultExecutionOrder(100)]
    public sealed class ArcadeGame : MonoBehaviour
    {
        private const float DeathDuration = 1.8f;
        private const double TimerTolerance = .0000001;
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
        public GhostController Ghosts { get; private set; }
        public PickupLedger Pickups { get; private set; }
        public ArcadePhase Phase { get; private set; }
        public int Score { get; private set; }
        public int Lives { get; private set; }
        public float ReadyTime => (float)readyRemaining;
        public float ResultRemaining => (float)resultRemaining;
        public CherryController Cherry { get; private set; }
        public double ElapsedSeconds { get; private set; }
        public bool TalismanMode => talismanMode;
        public TalismanWard Ward { get; } = new TalismanWard();
        public int BestScore => Mathf.Max(Score, LevelRecord.Score(talismanMode));
        private ActorSprite playerView;
        private PacStudentController playerController;
        private PlayerParticles particles;
        private readonly ActorSprite[] ghostViews = new ActorSprite[4];
        private readonly Vector2[] previousGhosts = new Vector2[4];
        private readonly bool[] edibleBeforeStep = new bool[4];
        private GameObject wardView;
        private SpriteRenderer[] wardRenderers;
        private Animator[] pickupAnimators;
        private float aspect;
        private double readyRemaining, deathRemaining, resultRemaining;
        private bool loadingMenu;
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
            Ghosts = new GhostController(generator.GeneratedLayout, Navigation);
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
                wardRenderers = wardView.GetComponentsInChildren<SpriteRenderer>();
                foreach(var renderer in wardRenderers)
                { renderer.sortingOrder = 9; renderer.color = new Color(1,1,1,.6f); }
            }
            NewGame();
        }

        public void NewGame()
        {
            Score = 0; Lives = 3;
            ElapsedSeconds = 0;
            loadingMenu = false;
            deathRemaining = resultRemaining = 0;
            Cherry.ResetClock();
            audioPlayer.Pause(false);
            PausePickups(false);
            ResetLevel();
            readyRemaining = 4;
            SetPhase(ArcadePhase.Ready);
            audioPlayer.Music("Intro", true);
        }
        private void ResetLevel()
        {
            // Collected objects are destroyed. A fresh run needs fresh objects,
            // not an attempt to reactivate references to the previous pickups.
            if (Pickups != null)
            {
                generator.GenerateLevel();
                pickupAnimators = generator.GeneratedLevel.GetComponentsInChildren<Animator>(true);
            }
            Pickups = new PickupLedger(Navigation);
            Pickups.CollectedPickup += Collected;
            ResetActors();
        }
        private void ResetActors()
        {
            Player.Speed = 5;
            playerController.ResetLife(playerStart);
            playerController.InputEnabled = false;
            Player.Enabled = false;
            Ghosts.Reset(1);
            particles.Clear();
            Ward.Reset();
        }
        private void Collected(MazeCell cell, int category)
        {
            var pickup = generator.TileAt(cell.Row, cell.Column);
            if (pickup) Destroy(pickup);
            if(category == 6)
            {
                Ghosts.ActivatePower(10);
            }
        }
        private void Arrive(MazeCell cell)
        {
            AddScore(Pickups.Consume(cell));
            if (Pickups.Remaining != 0) return;
            FinishGame();
        }
        private void AddScore(int points)
        {
            Score += points;
        }
        private void Update()
        {
            if (Player == null || loadingMenu) return;
            ReadInput();
            if (loadingMenu) return;
            Advance(Time.deltaTime);
            if (!loadingMenu) Present();
        }
        public void Present()
        {
            bool active = Phase == ArcadePhase.Playing;
            playerController.InputEnabled = active;
            playerView.transform.position = Player.Position;
            playerView.Present(Player.Direction == GridDirection.None ? GridDirection.Right : Player.Direction, active && Player.IsMoving || Phase == ArcadePhase.Dying,
                Phase == ArcadePhase.Dying || Phase == ArcadePhase.GameOver && Lives == 0 ? "Dead" : "Walking",
                Phase == ArcadePhase.Dying ? DeathDuration : 0);
            bool returning = false;
            for(int i=0; i<ghostViews.Length; i++)
            {
                var ghost = Ghosts.Ghosts[i];
                ghostViews[i].transform.position = ghost.Position;
                string form = ghost.State == GhostState.Eaten ? "Dead" : ghost.Frightened
                    ? (Ghosts.Recovering ? "Recovering" : "Scared") : "Walking";
                ghostViews[i].Present(ghost.Direction, active, form);
                returning |= ghost.State == GhostState.Eaten;
            }
            if(wardView)
            {
                wardView.SetActive(Ward.Active);
                wardView.transform.position = Player.Position;
                wardView.GetComponent<Animator>().speed = active ? 1 : 0;
                float opacity = Ward.Remaining < .5f ? .35f + .4f * Mathf.Abs(Mathf.Sin(Ward.Remaining * 24)) : .65f;
                Color tint = Ward.BlockedContact ? ArcadeHud.Gold : Color.white;
                tint.a = opacity;
                foreach (var renderer in wardRenderers) renderer.color = tint;
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
            double remaining = deltaTime;
            while(remaining > 0 && Phase != ArcadePhase.Paused && !loadingMenu)
            {
                if (Phase == ArcadePhase.Ready)
                {
                    float step = (float)System.Math.Min(remaining, readyRemaining);
                    Cherry.Tick(step);
                    remaining = System.Math.Max(0, remaining - step);
                    readyRemaining = System.Math.Max(0, readyRemaining - step);
                    if (readyRemaining <= TimerTolerance)
                    {
                        readyRemaining = 0;
                        BeginPlaying();
                    }
                }
                else if (Phase == ArcadePhase.Dying)
                {
                    float step = (float)System.Math.Min(remaining, deathRemaining);
                    Cherry.Tick(step);
                    particles.Tick(step, Player.Position, false);
                    ElapsedSeconds += step;
                    remaining = System.Math.Max(0, remaining - step);
                    deathRemaining = System.Math.Max(0, deathRemaining - step);
                    if (deathRemaining <= TimerTolerance)
                    {
                        deathRemaining = 0;
                        if (Lives == 0) FinishGame();
                        else { ResetActors(); BeginPlaying(); }
                    }
                }
                else if (Phase == ArcadePhase.GameOver)
                {
                    float step = (float)System.Math.Min(remaining, resultRemaining);
                    remaining = System.Math.Max(0, remaining - step);
                    resultRemaining = System.Math.Max(0, resultRemaining - step);
                    if (resultRemaining <= TimerTolerance)
                    {
                        resultRemaining = 0;
                        LoadStartScene();
                        return;
                    }
                }
                else
                {
                    float step = (float)System.Math.Min(remaining, 1d / 120);
                    if (Ghosts.FrightRemaining > 0) step = Mathf.Min(step, Ghosts.FrightRemaining);
                    if (Ward.Active) step = Mathf.Min(step, Ward.Remaining);
                    remaining = System.Math.Max(0, remaining - step);
                    ElapsedSeconds += step;
                    TickPlaying(step);
                }
            }
        }
        private void TickPlaying(float step)
        {
            Vector2 previousPlayer = Player.Position;
            for(int i=0; i<4; i++)
            {
                previousGhosts[i] = Ghosts.Ghosts[i].Position;
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
            if(Cherry.Active && fruitInside && MazeContact.CrossedFruit(previousPlayer,Player.Position,Cherry.PreviousPosition,Cherry.Position,Navigation.Columns) && Cherry.Collect())
            { AddScore(100); audioPlayer.Effect("Fruit"); }
            Ghosts.Tick(step,Player.Position);
            for(int i=0; i<4; i++)
            {
                var ghost = Ghosts.Ghosts[i];
                if(!MazeContact.Crossed(previousPlayer,Player.Position,previousGhosts[i],ghost.Position,Navigation.Columns)) continue;
                if(Ghosts.Eat(i, edibleBeforeStep[i]))
                {
                    AddScore(300); audioPlayer.Effect("Ghost");
                }
                else if(ghost.IsDangerous)
                {
                    if (!protectedDuringStep) { LoseLife(); break; }
                    if (Ward.RegisterBlockedContact()) audioPlayer.Effect("Wall");
                }
            }
        }
        private void LoseLife()
        {
            Lives--;
            SetPhase(ArcadePhase.Dying);
            deathRemaining = DeathDuration;
            particles.Death(Player.Position);
            playerView.Present(Player.Direction, true, "Dead", DeathDuration);
            audioPlayer.Moving(false);
            audioPlayer.Music(null); audioPlayer.Effect("Death");
        }
        private void ReadInput()
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)) TogglePause();
            if (Phase != ArcadePhase.Playing) return;
            if(Input.GetKeyDown(KeyCode.Space)) TryActivateWard();
        }
        public bool TryActivateWard()
        {
            if(!talismanMode || Phase != ArcadePhase.Playing || !Ward.TryActivate()) return false;
            audioPlayer.Effect("Ghost");
            return true;
        }
        private void SetPhase(ArcadePhase phase)
        {
            Phase = phase;
            Player.Enabled = phase == ArcadePhase.Playing;
            playerController.InputEnabled = phase == ArcadePhase.Playing;
        }

        private void BeginPlaying()
        {
            SetPhase(ArcadePhase.Playing);
            audioPlayer.Music("Normal");
        }

        private void FinishGame()
        {
            if (Phase == ArcadePhase.GameOver) return;
            SetPhase(ArcadePhase.GameOver);
            resultRemaining = 3;
            foreach (var ghost in Ghosts.Ghosts) ghost.Motor.Enabled = false;
            PausePickups(true);
            audioPlayer.Moving(false);
            audioPlayer.Music(null);
            LevelRecord.Submit(talismanMode, Score, ElapsedSeconds);
            hud.Draw(this);
        }

        public void ReturnToMenu()
        {
            if (Phase != ArcadePhase.GameOver && !loadingMenu) LoadStartScene();
        }

        private void LoadStartScene()
        {
            if (loadingMenu) return;
            loadingMenu = true;
            Player.Enabled = playerController.InputEnabled = false;
            SceneManager.LoadScene("StartScene");
        }

        public void TogglePause()
        {
            if (loadingMenu) return;
            if (Phase == ArcadePhase.Paused) { SetPhase(beforePause); audioPlayer.Pause(false); PausePickups(false); }
            else if (Phase == ArcadePhase.Playing || Phase == ArcadePhase.Ready)
            { beforePause = Phase; SetPhase(ArcadePhase.Paused); audioPlayer.Pause(true); PausePickups(true); }
        }
        private void PausePickups(bool pause)
        {
            foreach(var pickup in pickupAnimators) if (pickup) pickup.speed = pause ? 0 : 1;
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
