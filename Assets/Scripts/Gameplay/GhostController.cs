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
        public MazeCell SpawnCell { get; }
        public GhostState State { get; internal set; }
        public bool Frightened { get; internal set; }
        public bool IsEdible => Frightened && (State == GhostState.Roaming || State == GhostState.Exiting);
        public bool IsDangerous => !Frightened && (State == GhostState.Roaming || State == GhostState.Exiting);
        public Vector2 Position => State == GhostState.Eaten ? ReturnPosition : Motor.Position;
        public GridDirection Direction => State == GhostState.Eaten ? ReturnHeading : Motor.Direction;

        internal MazeCell GateCell, ExitCell;
        internal System.Random Random;
        internal Vector2 ReturnStart, ReturnPosition;
        internal double ReturnDistance, ReturnTravelled;
        internal GridDirection ReturnHeading;

        internal GhostAgent(int index, MazeNavigation navigation, MazeCell spawn)
        {
            Index = index;
            SpawnCell = spawn;
            Motor = new GridMotor(navigation, spawn, GhostController.NormalSpeed);
            int column = index < 2 ? 13 : 14;
            bool topExit = index % 2 == 0;
            GateCell = new MazeCell(topExit ? 12 : 16, column);
            ExitCell = new MazeCell(topExit ? 11 : 17, column);
        }
    }

    // Living ghosts use adjacent-cell tweens. Only a Dead ghost leaves the
    // corridor graph, travelling directly back to its own spawn through walls.
    public sealed class GhostController
    {
        public const float NormalSpeed = 4.5f;
        public const float SpiritSpeed = NormalSpeed * .5f;
        private static readonly GridDirection[] DecisionOrder =
            { GridDirection.Up, GridDirection.Left, GridDirection.Down, GridDirection.Right };
        private readonly MazeNavigation corridors;
        private readonly MazeNavigation house;
        private readonly GhostAgent[] ghosts = new GhostAgent[4];
        private readonly int seed;
        private Vector2 playerPosition;
        private double frightRemaining;

        public IReadOnlyList<GhostAgent> Ghosts { get; }
        public float FrightRemaining => (float)frightRemaining;
        public bool Recovering => frightRemaining > 0 && frightRemaining <= 3;
        public bool IsRecovering => Recovering;

        public GhostController(LevelMapLayout layout, MazeNavigation playerNavigation, int seed = 173)
        {
            if (layout == null) throw new ArgumentNullException(nameof(layout));
            if (playerNavigation == null) throw new ArgumentNullException(nameof(playerNavigation));
            if (playerNavigation.Rows != layout.Rows || playerNavigation.Columns != layout.Columns)
                throw new ArgumentException("Player and ghost navigation must use the same level dimensions.", nameof(playerNavigation));
            this.seed = seed;
            corridors = new MazeNavigation(layout, allowTunnelWrap: false);
            house = new MazeNavigation(layout, allowHouseGates: true, allowTunnelWrap: false);
            for (int index = 0; index < ghosts.Length; index++)
            {
                var ghost = new GhostAgent(index, house, new MazeCell(14, 12 + index));
                if (!house.IsWalkable(ghost.GateCell) || !corridors.IsWalkable(ghost.ExitCell))
                    throw new ArgumentException("The supplied maze must contain the assigned guardian house exits.", nameof(layout));
                ghost.Motor.Arrived += cell => Arrived(ghost);
                ghosts[index] = ghost;
            }
            Ghosts = Array.AsReadOnly(ghosts);
            Reset();
        }

        public void Reset(int round = 1)
        {
            if (round < 1) throw new ArgumentOutOfRangeException(nameof(round));
            frightRemaining = 0;
            playerPosition = corridors.WorldPosition(new MazeCell(1, 1));
            foreach (var ghost in ghosts)
            {
                // Independent streams make simultaneous actors insensitive to
                // the order in which a rendered frame visits their decisions.
                ghost.Random = new System.Random(unchecked(seed + ghost.Index * 104729));
                ghost.State = GhostState.Exiting;
                ghost.Frightened = false;
                ghost.ReturnDistance = ghost.ReturnTravelled = 0;
                ghost.Motor.Reset(ghost.SpawnCell);
                ghost.Motor.Enabled = true;
                ghost.Motor.Speed = NormalSpeed;
                ChooseExitDirection(ghost);
            }
        }

        public void Tick(float deltaSeconds, Vector2 actualPlayerPosition)
        {
            if (deltaSeconds < 0 || float.IsNaN(deltaSeconds) || float.IsInfinity(deltaSeconds))
                throw new ArgumentOutOfRangeException(nameof(deltaSeconds));
            if (float.IsNaN(actualPlayerPosition.x) || float.IsNaN(actualPlayerPosition.y)
                || float.IsInfinity(actualPlayerPosition.x) || float.IsInfinity(actualPlayerPosition.y))
                throw new ArgumentOutOfRangeException(nameof(actualPlayerPosition));
            playerPosition = actualPlayerPosition;
            double remaining = deltaSeconds;
            while (remaining > 0)
            {
                double step = frightRemaining > 0 ? Math.Min(remaining, frightRemaining) : remaining;
                foreach (var ghost in ghosts) Advance(ghost, step);
                remaining -= step;
                if (frightRemaining <= 0) continue;
                frightRemaining = Math.Max(0, frightRemaining - step);
                if (frightRemaining > 0) continue;
                foreach (var ghost in ghosts)
                {
                    if (ghost.State != GhostState.Eaten) ghost.Frightened = false;
                    UpdateSpeed(ghost);
                }
            }
        }

        public void Tick(float deltaSeconds, MazeCell playerCell, GridDirection playerDirection)
            => Tick(deltaSeconds, corridors.WorldPosition(playerCell));

        public void ActivatePower(float seconds)
        {
            if (seconds <= 0 || float.IsNaN(seconds) || float.IsInfinity(seconds))
                throw new ArgumentOutOfRangeException(nameof(seconds));
            frightRemaining = seconds;
            foreach (var ghost in ghosts)
            {
                if (ghost.State == GhostState.Eaten) continue;
                ghost.Frightened = true;
                UpdateSpeed(ghost);
            }
        }

        public bool Eat(int index, bool frightenedDuringStep = false)
        {
            if (index < 0 || index >= ghosts.Length) return false;
            var ghost = ghosts[index];
            bool alive = ghost.State == GhostState.Exiting || ghost.State == GhostState.Roaming;
            if (!ghost.IsEdible && !(frightenedDuringStep && alive)) return false;
            ghost.ReturnStart = ghost.ReturnPosition = ghost.Position;
            Vector2 difference = house.WorldPosition(ghost.SpawnCell) - ghost.ReturnStart;
            ghost.ReturnDistance = difference.magnitude;
            ghost.ReturnTravelled = 0;
            ghost.ReturnHeading = Mathf.Abs(difference.x) >= Mathf.Abs(difference.y)
                ? (difference.x >= 0 ? GridDirection.Right : GridDirection.Left)
                : (difference.y >= 0 ? GridDirection.Up : GridDirection.Down);
            ghost.State = GhostState.Eaten;
            ghost.Frightened = false;
            ghost.Motor.Enabled = false;
            UpdateSpeed(ghost);
            return true;
        }

        private void Advance(GhostAgent ghost, double seconds)
        {
            double remaining = seconds;
            if (ghost.State == GhostState.Eaten)
            {
                double secondsToHome = Math.Max(0, ghost.ReturnDistance - ghost.ReturnTravelled) / SpiritSpeed;
                double used = Math.Min(remaining, secondsToHome);
                ghost.ReturnTravelled += used * SpiritSpeed;
                float fraction = ghost.ReturnDistance <= 0 ? 1 : (float)(ghost.ReturnTravelled / ghost.ReturnDistance);
                ghost.ReturnPosition = Vector2.Lerp(ghost.ReturnStart, house.WorldPosition(ghost.SpawnCell), fraction);
                remaining -= used;
                if (used < secondsToHome) return;
                Revive(ghost);
            }
            if (remaining <= 0 || !ghost.Motor.Enabled) return;
            UpdateSpeed(ghost);
            if (ghost.Motor.IsAtCenter && !CanMove(ghost, ghost.Motor.BufferedDirection)) Arrived(ghost);
            ghost.Motor.Tick((float)remaining);
        }

        private void Revive(GhostAgent ghost)
        {
            ghost.Motor.Reset(ghost.SpawnCell);
            ghost.State = GhostState.Exiting;
            ghost.Frightened = frightRemaining > 0;
            ghost.Motor.Enabled = true;
            UpdateSpeed(ghost);
            ChooseExitDirection(ghost);
        }

        private void Arrived(GhostAgent ghost)
        {
            if (ghost.State == GhostState.Exiting)
            {
                ChooseExitDirection(ghost);
                return;
            }
            if (ghost.State == GhostState.Roaming) ChooseRoamingDirection(ghost);
        }

        private void ChooseExitDirection(GhostAgent ghost)
        {
            if (ghost.Motor.CurrentCell == ghost.ExitCell)
            {
                ghost.State = GhostState.Roaming;
                ChooseRoamingDirection(ghost);
                return;
            }
            MazeCell start = ghost.Motor.CurrentCell;
            var choices = LegalDirections(ghost);
            var pending = new Queue<MazeCell>();
            var firstSteps = new Dictionary<MazeCell, GridDirection> { [start] = GridDirection.None };
            foreach (var direction in choices)
            {
                house.TryStep(start, direction, out MazeCell next);
                if (next == ghost.ExitCell) { ghost.Motor.QueueDirection(direction); return; }
                firstSteps.Add(next, direction);
                pending.Enqueue(next);
            }
            while (pending.Count > 0)
            {
                MazeCell cell = pending.Dequeue();
                foreach (var direction in DecisionOrder)
                {
                    if (!house.TryStep(cell, direction, out MazeCell next) || !InDepartureRegion(ghost, next)
                        || firstSteps.ContainsKey(next)) continue;
                    if (next == ghost.ExitCell) { ghost.Motor.QueueDirection(firstSteps[cell]); return; }
                    firstSteps.Add(next, firstSteps[cell]);
                    pending.Enqueue(next);
                }
            }
            throw new InvalidOperationException("No guardian departure route from " + start + " to " + ghost.ExitCell + ".");
        }

        private void ChooseRoamingDirection(GhostAgent ghost)
        {
            var choices = LegalDirections(ghost);
            ghost.Motor.QueueDirection(GhostDecision(ghost, choices));
        }

        internal List<GridDirection> LegalDirections(GhostAgent ghost)
        {
            var choices = new List<GridDirection>(4);
            GridDirection reverse = MazeNavigation.Opposite(ghost.Motor.Direction);
            foreach (var direction in DecisionOrder)
                if (CanMove(ghost, direction) && direction != reverse) choices.Add(direction);
            if (choices.Count == 0 && CanMove(ghost, reverse)) choices.Add(reverse);
            if (choices.Count == 0)
                throw new InvalidOperationException("A living guardian has no legal exit at " + ghost.Motor.CurrentCell + ".");
            return choices;
        }

        internal GridDirection GhostDecision(GhostAgent ghost, List<GridDirection> choices)
            => choices[ghost.Random.Next(choices.Count)];

        private bool CanMove(GhostAgent ghost, GridDirection direction)
        {
            if (ghost.State == GhostState.Exiting)
                return house.TryStep(ghost.Motor.CurrentCell, direction, out MazeCell next) && InDepartureRegion(ghost, next);
            return corridors.TryStep(ghost.Motor.CurrentCell, direction, out _);
        }

        private static bool InDepartureRegion(GhostAgent ghost, MazeCell cell)
            => cell == ghost.GateCell || cell == ghost.ExitCell
                || cell.Row >= 13 && cell.Row <= 15 && cell.Column >= 11 && cell.Column <= 16;

        private static void UpdateSpeed(GhostAgent ghost)
            => ghost.Motor.Speed = ghost.State == GhostState.Eaten || ghost.Frightened ? SpiritSpeed : NormalSpeed;
    }
}
