using UnityEngine;

namespace ShanHaiSpiritTrail
{
    public enum ArcadePhase { Ready, Playing, Paused, RoundClear }

    [DefaultExecutionOrder(100)]
    public sealed class ArcadeGame : MonoBehaviour
    {
        [SerializeField] private LevelGenerator generator;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Camera levelCamera;
        [SerializeField] private ArcadeAudio audioPlayer;
        [SerializeField] private ArcadeHud hud;
        public MazeNavigation Navigation { get; private set; }
        public GridMotor Player { get; private set; }
        public PickupLedger Pickups { get; private set; }
        public ArcadePhase Phase { get; private set; }
        public int Score { get; private set; }
        public int Round { get; private set; } = 1;
        public float ReadyTime { get; private set; }
        private ActorSprite playerView;
        private float aspect;
        private ArcadePhase beforePause;
        private float lastPickupTime = -1;

        private void Start()
        {
            Navigation = new MazeNavigation(generator.GeneratedLayout, new MazeCell(1, 1));
            playerView = Instantiate(playerPrefab).AddComponent<ActorSprite>();
            playerView.name = "Cultivator_Player";
            Player = new GridMotor(Navigation, new MazeCell(17, 13), 5f);
            Player.Arrived += Arrive;
            Player.Blocked += _ => audioPlayer.Effect("Wall");
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
            Pickups.CollectedPickup += (cell, category) =>
            {
                generator.TileAt(cell.Row, cell.Column).SetActive(false);
                lastPickupTime = Time.time;
                audioPlayer.Effect("Pellet");
            };
            Player.Reset(new MazeCell(17, 13));
            Player.QueueDirection(GridDirection.Left);
            Phase = ArcadePhase.Ready;
            ReadyTime = 2.4f;
            audioPlayer.Music("Intro");
        }

        private void Arrive(MazeCell cell)
        {
            Score += Pickups.Consume(cell);
            if (Pickups.Remaining != 0) return;
            Phase = ArcadePhase.RoundClear;
            Player.Enabled = false;
            audioPlayer.Music(null);
        }

        private void Update()
        {
            if (Player == null) return;
            ReadInput();
            Advance(Time.deltaTime);
            playerView.transform.position = Player.Position;
            playerView.Present(Player.Direction, Phase == ArcadePhase.Playing && Player.IsMoving);
            audioPlayer.Moving(Phase == ArcadePhase.Playing && Player.IsMoving && Time.time - lastPickupTime > .15f);
            FitCamera();
            hud.Draw(this);
        }

        public void Advance(float deltaTime)
        {
            if (Phase == ArcadePhase.Ready)
            {
                ReadyTime -= deltaTime;
                if (ReadyTime <= 0) { Phase = ArcadePhase.Playing; Player.Enabled = true; audioPlayer.Music("Normal"); }
            }
            else if (Phase == ArcadePhase.Playing) Player.Tick(deltaTime);
        }

        private void ReadInput()
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)) TogglePause();
            if (Phase == ArcadePhase.RoundClear && Input.GetKeyDown(KeyCode.Return)) NextRound();
            if (Phase != ArcadePhase.Ready && Phase != ArcadePhase.Playing) return;
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) Player.QueueDirection(GridDirection.Up);
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) Player.QueueDirection(GridDirection.Left);
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) Player.QueueDirection(GridDirection.Down);
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) Player.QueueDirection(GridDirection.Right);
        }

        public void TogglePause()
        {
            if (Phase == ArcadePhase.Paused) { Phase = beforePause; audioPlayer.Pause(false); }
            else if (Phase == ArcadePhase.Playing || Phase == ArcadePhase.Ready)
            { beforePause = Phase; Phase = ArcadePhase.Paused; audioPlayer.Pause(true); }
        }
        public void NextRound()
        {
            if (Phase != ArcadePhase.RoundClear) return;
            Round++;
            StartRound();
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
