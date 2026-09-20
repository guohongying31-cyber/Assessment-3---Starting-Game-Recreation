using System;
using UnityEngine;

namespace ShanHaiSpiritTrail
{
    // Movement is a sequence of one-tile linear tweens. Decisions happen at
    // tile centres; reversing can happen anywhere along the current segment.
    public sealed class GridMotor
    {
        private readonly MazeNavigation navigation;
        private MazeCell segmentStart;
        private MazeCell segmentEnd;
        private double progress;
        private float speed;
        private bool traversing;
        private int resetVersion;
        private GridDirection bufferedDirection;
        private GridDirection lastBlockedDirection;

        public event Action<MazeCell> Arrived;
        public event Action<GridDirection> DirectionChanged;
        public event Action<GridDirection> Blocked;

        // CurrentCell is the last centre actually reached, including during a reversal.
        public MazeCell CurrentCell { get; private set; }
        public MazeCell TargetCell => traversing ? segmentEnd : CurrentCell;
        public GridDirection Direction { get; private set; }
        public GridDirection BufferedDirection => bufferedDirection;
        public Vector2 Position { get; private set; }
        public bool Enabled { get; set; } = true;
        public bool IsMoving => Enabled && traversing;
        public bool IsAtCenter => !traversing;

        public float Speed
        {
            get => speed;
            set
            {
                if (value <= 0 || float.IsNaN(value) || float.IsInfinity(value))
                    throw new ArgumentOutOfRangeException(nameof(value), "Speed must be finite and positive.");
                speed = value;
            }
        }

        public GridMotor(MazeNavigation navigation, MazeCell start, float speed)
        {
            this.navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
            Speed = speed;
            Reset(start);
        }

        public void Reset(MazeCell cell, GridDirection direction = GridDirection.None)
        {
            if (!navigation.IsWalkable(cell))
                throw new ArgumentException("The motor must start on a reachable corridor.", nameof(cell));
            ValidateDirection(direction);
            resetVersion++;
            CurrentCell = segmentStart = segmentEnd = cell;
            Direction = bufferedDirection = direction;
            lastBlockedDirection = GridDirection.None;
            progress = 0;
            traversing = false;
            Position = navigation.WorldPosition(cell);
        }

        public void QueueDirection(GridDirection direction)
        {
            ValidateDirection(direction);
            if (bufferedDirection != direction) lastBlockedDirection = GridDirection.None;
            bufferedDirection = direction;
            if (Enabled && traversing && direction == MazeNavigation.Opposite(Direction))
                ReverseSegment();
        }

        public void Tick(float deltaSeconds)
        {
            if (deltaSeconds < 0 || float.IsNaN(deltaSeconds) || float.IsInfinity(deltaSeconds))
                throw new ArgumentOutOfRangeException(nameof(deltaSeconds));
            if (!Enabled || deltaSeconds == 0) return;

            int version = resetVersion;
            double remaining = deltaSeconds;
            while (Enabled && remaining > 0)
            {
                if (traversing && bufferedDirection == MazeNavigation.Opposite(Direction))
                    ReverseSegment();
                if (resetVersion != version || !Enabled) return;

                if (!traversing && !BeginSegment()) return;
                if (resetVersion != version || !Enabled) return;

                double secondsToCenter = (1 - progress) / Speed;
                if (remaining < secondsToCenter)
                {
                    progress += remaining * Speed;
                    UpdatePosition();
                    return;
                }

                remaining -= secondsToCenter;
                CurrentCell = segmentEnd;
                Position = navigation.WorldPosition(CurrentCell);
                traversing = false;
                progress = 0;
                Arrived?.Invoke(CurrentCell);
                // A collection or collision callback can stop or reset a round.
                if (resetVersion != version || !Enabled) return;
            }
        }

        private bool BeginSegment()
        {
            GridDirection nextDirection = bufferedDirection;
            if (!navigation.TryStep(CurrentCell, nextDirection, out MazeCell next))
            {
                nextDirection = Direction;
                if (!navigation.TryStep(CurrentCell, nextDirection, out next))
                {
                    GridDirection blocked = bufferedDirection != GridDirection.None ? bufferedDirection : Direction;
                    if (blocked != GridDirection.None && blocked != lastBlockedDirection)
                    {
                        lastBlockedDirection = blocked;
                        Blocked?.Invoke(blocked);
                    }
                    return false;
                }
            }

            segmentStart = CurrentCell;
            segmentEnd = next;
            progress = 0;
            traversing = true;
            lastBlockedDirection = GridDirection.None;
            if (Direction != nextDirection)
            {
                Direction = nextDirection;
                DirectionChanged?.Invoke(Direction);
            }
            return true;
        }

        private void ReverseSegment()
        {
            MazeCell previousStart = segmentStart;
            segmentStart = segmentEnd;
            segmentEnd = previousStart;
            progress = 1 - progress;
            Direction = MazeNavigation.Opposite(Direction);
            UpdatePosition();
            DirectionChanged?.Invoke(Direction);
        }

        private void UpdatePosition()
        {
            Vector2 start = navigation.WorldPosition(segmentStart);
            Vector2 destination = start + MazeNavigation.StepVector(Direction);
            Vector2 position = Vector2.Lerp(start, destination, (float)progress);
            // Wrap at the tunnel boundary without tweening across the whole maze.
            if (position.x < -0.5f) position.x += navigation.Columns;
            else if (position.x >= navigation.Columns - 0.5f) position.x -= navigation.Columns;
            Position = position;
        }

        private static void ValidateDirection(GridDirection direction)
        {
            if (direction < GridDirection.None || direction > GridDirection.Right)
                throw new ArgumentOutOfRangeException(nameof(direction));
        }
    }
}
