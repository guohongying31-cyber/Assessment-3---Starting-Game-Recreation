using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShanHaiSpiritTrail
{
    public enum GhostState { Waiting, Exiting, Roaming, Eaten }

    public sealed class GhostAgent
    {
        public int Index { get; }
        public GridMotor Motor { get; }
        public GhostState State { get; internal set; }
        public bool Frightened { get; internal set; }
        public bool IsEdible => Frightened && (State == GhostState.Roaming || State == GhostState.Exiting);
        public bool IsDangerous => !Frightened && (State == GhostState.Roaming || State == GhostState.Exiting);
        public float ReleaseRemaining => (float)WaitRemaining;
        public float DeadTimeRemaining => (float)DeadRemaining;

        internal double WaitRemaining;
        internal double DeadRemaining;
        internal GhostState StateBeforeDeath;

        internal GhostAgent(int index, MazeNavigation navigation, MazeCell spawn)
        {
            Index = index;
            Motor = new GridMotor(navigation, spawn, 4.4f);
        }
    }

    // Decisions use the same corridor graph as the player. Only release paths
    // cross the house gates; no physics moves these actors.
    public sealed class GhostDirector
    {
        private static readonly GridDirection[] DecisionOrder =
            { GridDirection.Up, GridDirection.Left, GridDirection.Down, GridDirection.Right };
        private static readonly double[] ModeDurations = { 7, 20, 7, 20, 5, 20, 5 };
        private readonly MazeNavigation playerNavigation;
        private readonly MazeNavigation houseNavigation;
        private readonly GhostAgent[] ghosts = new GhostAgent[4];
        private readonly MazeCell[] spawnCells = new MazeCell[4];
        private readonly MazeCell[] scatterCells = new MazeCell[4];
        private readonly MazeCell exitCell;
        private readonly MazeCell homeCell;
        private System.Random random;
        private MazeCell playerCell;
        private GridDirection playerDirection;
        private double frightRemaining;
        private double modeRemaining;
        private int modeIndex;
        private float normalSpeed;

        public IReadOnlyList<GhostAgent> Ghosts { get; }
        public float FrightRemaining => (float)frightRemaining;
        public bool Recovering => frightRemaining > 0 && frightRemaining <= 3;
        public bool IsRecovering => Recovering;
        public bool IsScattering => modeIndex < ModeDurations.Length && modeIndex % 2 == 0;
        public float ModeTimeRemaining => (float)modeRemaining;

        public GhostDirector(LevelMapLayout layout, MazeNavigation playerNavigation)
        {
            if (layout == null) throw new ArgumentNullException(nameof(layout));
            this.playerNavigation = playerNavigation ?? throw new ArgumentNullException(nameof(playerNavigation));
            houseNavigation = new MazeNavigation(layout, allowHouseGates: true);
            exitCell = Nearest(playerNavigation, 11, 13);
            homeCell = Nearest(houseNavigation, 14, 13);
            spawnCells[0] = Nearest(houseNavigation, 14, 12);
            spawnCells[1] = homeCell;
            spawnCells[2] = Nearest(houseNavigation, 14, 14);
            spawnCells[3] = Nearest(houseNavigation, 14, 15);
            scatterCells[0] = Nearest(playerNavigation, 1, layout.Columns - 2);
            scatterCells[1] = Nearest(playerNavigation, 1, 1);
            scatterCells[2] = Nearest(playerNavigation, layout.Rows - 2, layout.Columns - 2);
            scatterCells[3] = Nearest(playerNavigation, layout.Rows - 2, 1);

            for (int index = 0; index < ghosts.Length; index++)
            {
                var ghost = new GhostAgent(index, houseNavigation, spawnCells[index]);
                ghost.Motor.Arrived += cell => Arrived(ghost, cell);
                ghosts[index] = ghost;
            }
            Ghosts = Array.AsReadOnly(ghosts);
            Reset(1);
        }

        public void Reset(int round)
        {
            if (round < 1) throw new ArgumentOutOfRangeException(nameof(round));
            random = new System.Random(unchecked(173 + round * 31));
            normalSpeed = Mathf.Min(6f, 4.4f + 0.15f * (round - 1));
            frightRemaining = 0;
            modeIndex = 0;
            modeRemaining = ModeDurations[0];
            playerCell = Nearest(playerNavigation, 17, 13);
            playerDirection = GridDirection.Left;
            foreach (GhostAgent ghost in ghosts)
            {
                ghost.Motor.Reset(spawnCells[ghost.Index]);
                ghost.Motor.Speed = normalSpeed;
                ghost.Frightened = false;
                ghost.DeadRemaining = 0;
                ghost.WaitRemaining = ghost.Index * 2;
                ghost.State = GhostState.Waiting;
                ghost.Motor.Enabled = ghost.State != GhostState.Waiting;
                if (ghost.State == GhostState.Roaming) ChooseRoamingDirection(ghost);
            }
        }

        public void Tick(float deltaSeconds, MazeCell currentPlayerCell, GridDirection currentPlayerDirection)
        {
            if (deltaSeconds < 0 || float.IsNaN(deltaSeconds) || float.IsInfinity(deltaSeconds))
                throw new ArgumentOutOfRangeException(nameof(deltaSeconds));
            playerCell = playerNavigation.IsWalkable(currentPlayerCell)
                ? currentPlayerCell : Nearest(playerNavigation, currentPlayerCell.Row, currentPlayerCell.Column);
            playerDirection = currentPlayerDirection;
            double remaining = deltaSeconds;
            while (remaining > 0)
            {
                // Split at timer boundaries so a long frame cannot skip a mode
                // change or accidentally advance the chase timer while frightened.
                double step = Math.Min(remaining, frightRemaining > 0 ? frightRemaining : modeRemaining);
                foreach (GhostAgent ghost in ghosts)
                {
                    if (ghost.State == GhostState.Waiting && ghost.WaitRemaining > 0)
                        step = Math.Min(step, ghost.WaitRemaining);
                    if (ghost.State == GhostState.Eaten && ghost.DeadRemaining > 0)
                        step = Math.Min(step, ghost.DeadRemaining);
                }

                bool wasFrightened = frightRemaining > 0;
                foreach (GhostAgent ghost in ghosts) Advance(ghost, (float)step);
                remaining -= step;
                if (wasFrightened)
                {
                    frightRemaining = Math.Max(0, frightRemaining - step);
                    if (frightRemaining == 0)
                        foreach (GhostAgent ghost in ghosts)
                        {
                            ghost.Frightened = false;
                            UpdateSpeed(ghost);
                        }
                }
                else
                {
                    modeRemaining -= step;
                    if (modeRemaining <= 0)
                    {
                        modeIndex++;
                        modeRemaining = modeIndex < ModeDurations.Length
                            ? ModeDurations[modeIndex] : double.PositiveInfinity;
                        foreach (GhostAgent ghost in ghosts) ReverseRoaming(ghost);
                    }
                }
            }
        }

        public void ActivatePower(float seconds)
        {
            if (seconds <= 0 || float.IsNaN(seconds) || float.IsInfinity(seconds))
                throw new ArgumentOutOfRangeException(nameof(seconds));
            frightRemaining = seconds;
            foreach (GhostAgent ghost in ghosts)
            {
                if (ghost.State == GhostState.Eaten) continue;
                ghost.Frightened = true;
                UpdateSpeed(ghost);
                ReverseRoaming(ghost);
            }
        }

        public bool Eat(int index, bool frightenedDuringStep = false)
        {
            if (index < 0 || index >= ghosts.Length) return false;
            GhostAgent ghost = ghosts[index];
            bool roaming = ghost.State == GhostState.Roaming || ghost.State == GhostState.Exiting;
            // The shared simulation may end exactly at the power expiry boundary.
            // Contact during that bounded step still uses its active power state.
            if (!ghost.IsEdible && !(frightenedDuringStep && roaming)) return false;
            ghost.StateBeforeDeath = ghost.State;
            ghost.State = GhostState.Eaten;
            ghost.Frightened = false;
            ghost.DeadRemaining = 3;
            ghost.Motor.Enabled = false;
            UpdateSpeed(ghost);
            return true;
        }

        private void Advance(GhostAgent ghost, float deltaSeconds)
        {
            if (ghost.State == GhostState.Eaten)
            {
                ghost.DeadRemaining = Math.Max(0, ghost.DeadRemaining - deltaSeconds);
                if (ghost.DeadRemaining <= 0)
                {
                    ghost.State = ghost.StateBeforeDeath;
                    ghost.Frightened = frightRemaining > 0;
                    ghost.Motor.Enabled = true;
                    UpdateSpeed(ghost);
                    if (ghost.Motor.IsAtCenter)
                    {
                        if (ghost.State == GhostState.Exiting) ChooseExitDirection(ghost);
                        else ChooseRoamingDirection(ghost);
                    }
                }
                return;
            }
            if (ghost.State == GhostState.Waiting)
            {
                ghost.WaitRemaining = Math.Max(0, ghost.WaitRemaining - deltaSeconds);
                if (ghost.WaitRemaining <= 0)
                {
                    ghost.State = GhostState.Exiting;
                    ghost.Motor.Enabled = true;
                    ChooseExitDirection(ghost);
                }
                return;
            }
            UpdateSpeed(ghost);
            ghost.Motor.Tick(deltaSeconds);
        }

        private void Arrived(GhostAgent ghost, MazeCell cell)
        {
            switch (ghost.State)
            {
                case GhostState.Roaming: ChooseRoamingDirection(ghost); break;
                case GhostState.Exiting: ChooseExitDirection(ghost); break;
            }
        }

        private void ChooseExitDirection(GhostAgent ghost)
        {
            if (ghost.Motor.CurrentCell == exitCell)
            {
                ghost.State = GhostState.Roaming;
                ChooseRoamingDirection(ghost);
            }
            else ghost.Motor.QueueDirection(ShortestDirection(houseNavigation, ghost.Motor.CurrentCell, exitCell));
        }

        private void ChooseRoamingDirection(GhostAgent ghost)
        {
            MazeCell cell = ghost.Motor.CurrentCell;
            GridDirection reverse = MazeNavigation.Opposite(ghost.Motor.Direction);
            var choices = new List<GridDirection>(4);
            foreach (GridDirection direction in DecisionOrder)
                if (direction != reverse && playerNavigation.TryStep(cell, direction, out _))
                    choices.Add(direction);
            if (choices.Count == 0 && playerNavigation.TryStep(cell, reverse, out _)) choices.Add(reverse);
            if (choices.Count == 0)
            {
                ghost.Motor.QueueDirection(GridDirection.None);
                return;
            }
            if (ghost.Frightened)
            {
                ghost.Motor.QueueDirection(choices[random.Next(choices.Count)]);
                return;
            }

            MazeCell target = IsScattering ? scatterCells[ghost.Index] : ChaseTarget(ghost);
            GridDirection best = choices[0];
            int bestDistance = int.MaxValue;
            foreach (GridDirection direction in choices)
            {
                playerNavigation.TryStep(cell, direction, out MazeCell next);
                int distance = SquaredDistance(next, target);
                if (distance >= bestDistance) continue;
                bestDistance = distance;
                best = direction;
            }
            ghost.Motor.QueueDirection(best);
        }

        private MazeCell ChaseTarget(GhostAgent ghost)
        {
            Vector2 ahead = MazeNavigation.StepVector(playerDirection);
            switch (ghost.Index)
            {
                case 0: return playerCell;
                case 1:
                    return Nearest(playerNavigation, playerCell.Row - (int)ahead.y * 4,
                        playerCell.Column + (int)ahead.x * 4);
                case 2:
                    MazeCell first = ghosts[0].Motor.CurrentCell;
                    int row = playerCell.Row - (int)ahead.y * 2;
                    int column = playerCell.Column + (int)ahead.x * 2;
                    return Nearest(playerNavigation, row * 2 - first.Row, column * 2 - first.Column);
                default:
                    return SquaredDistance(ghost.Motor.CurrentCell, playerCell) >= 64
                        ? playerCell : scatterCells[ghost.Index];
            }
        }

        private void ReverseRoaming(GhostAgent ghost)
        {
            if (ghost.State != GhostState.Roaming || ghost.Motor.Direction == GridDirection.None) return;
            GridDirection reverse = MazeNavigation.Opposite(ghost.Motor.Direction);
            if (!ghost.Motor.IsAtCenter || playerNavigation.TryStep(ghost.Motor.CurrentCell, reverse, out _))
                ghost.Motor.QueueDirection(reverse);
        }

        private void UpdateSpeed(GhostAgent ghost)
            => ghost.Motor.Speed = ghost.State == GhostState.Eaten || ghost.Frightened ? 2.8f : normalSpeed;

        private static GridDirection ShortestDirection(MazeNavigation navigation, MazeCell from, MazeCell target)
        {
            if (from == target) return GridDirection.None;
            var pending = new Queue<MazeCell>();
            var firstSteps = new Dictionary<MazeCell, GridDirection> { [from] = GridDirection.None };
            pending.Enqueue(from);
            while (pending.Count > 0)
            {
                MazeCell cell = pending.Dequeue();
                foreach (GridDirection direction in DecisionOrder)
                {
                    if (!navigation.TryStep(cell, direction, out MazeCell next) || firstSteps.ContainsKey(next)) continue;
                    GridDirection firstStep = cell == from ? direction : firstSteps[cell];
                    if (next == target) return firstStep;
                    firstSteps.Add(next, firstStep);
                    pending.Enqueue(next);
                }
            }
            throw new InvalidOperationException($"No guardian route from {from} to {target}.");
        }

        private static MazeCell Nearest(MazeNavigation navigation, int row, int column)
            => navigation.NearestCell(new Vector2(Mathf.Clamp(column, 0, navigation.Columns - 1),
                -Mathf.Clamp(row, 0, navigation.Rows - 1)));

        private static int SquaredDistance(MazeCell first, MazeCell second)
        {
            int row = first.Row - second.Row;
            int column = first.Column - second.Column;
            return row * row + column * column;
        }
    }
}
