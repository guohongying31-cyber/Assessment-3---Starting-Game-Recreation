using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShanHaiSpiritTrail
{
    public enum GridDirection { None, Up, Left, Down, Right }

    public readonly struct MazeCell : IEquatable<MazeCell>
    {
        public int Row { get; }
        public int Column { get; }

        public MazeCell(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public bool Equals(MazeCell other) => Row == other.Row && Column == other.Column;
        public override bool Equals(object obj) => obj is MazeCell other && Equals(other);
        public override int GetHashCode() => unchecked((Row * 397) ^ Column);
        public static bool operator ==(MazeCell left, MazeCell right) => left.Equals(right);
        public static bool operator !=(MazeCell left, MazeCell right) => !left.Equals(right);
        public override string ToString() => $"({Row}, {Column})";
    }

    // Empty tiles inside walls and outside the maze are not necessarily corridors.
    // Flood filling from a corridor keeps those disconnected spaces inaccessible.
    public sealed class MazeNavigation
    {
        private static readonly GridDirection[] SearchDirections =
            { GridDirection.Up, GridDirection.Left, GridDirection.Down, GridDirection.Right };
        private readonly LevelMapLayout layout;
        private readonly HashSet<MazeCell> walkable = new HashSet<MazeCell>();

        public int Rows => layout.Rows;
        public int Columns => layout.Columns;
        public int TunnelRow => Rows / 2;
        public IReadOnlyList<MazeCell> Cells { get; }

        public MazeNavigation(LevelMapLayout layout) : this(layout, new MazeCell(1, 1)) { }

        public MazeNavigation(LevelMapLayout layout, MazeCell start)
        {
            this.layout = layout ?? throw new ArgumentNullException(nameof(layout));
            if (!IsCorridor(start))
                throw new ArgumentException("The navigation start must be a corridor tile.", nameof(start));

            var cells = new List<MazeCell>();
            var pending = new Queue<MazeCell>();
            walkable.Add(start);
            pending.Enqueue(start);
            while (pending.Count > 0)
            {
                MazeCell cell = pending.Dequeue();
                cells.Add(cell);
                foreach (GridDirection direction in SearchDirections)
                {
                    MazeCell next = Adjacent(cell, direction);
                    if (IsCorridor(next) && walkable.Add(next)) pending.Enqueue(next);
                }
            }
            Cells = cells.AsReadOnly();
        }

        public bool IsWalkable(MazeCell cell) => walkable.Contains(cell);
        public int CategoryAt(MazeCell cell) => layout.CategoryAt(cell.Row, cell.Column);
        public Vector2 WorldPosition(MazeCell cell) => new Vector2(cell.Column, -cell.Row);

        public bool TryStep(MazeCell cell, GridDirection direction, out MazeCell next)
        {
            next = cell;
            if (direction <= GridDirection.None || direction > GridDirection.Right || !IsWalkable(cell))
                return false;
            MazeCell candidate = Adjacent(cell, direction);
            if (!IsWalkable(candidate)) return false;
            next = candidate;
            return true;
        }

        public MazeCell NearestCell(Vector2 position)
        {
            if (float.IsNaN(position.x) || float.IsNaN(position.y) ||
                float.IsInfinity(position.x) || float.IsInfinity(position.y))
                throw new ArgumentOutOfRangeException(nameof(position));

            MazeCell nearest = Cells[0];
            float nearestDistance = float.PositiveInfinity;
            foreach (MazeCell cell in Cells)
            {
                float distance = (WorldPosition(cell) - position).sqrMagnitude;
                if (distance >= nearestDistance) continue;
                nearestDistance = distance;
                nearest = cell;
            }
            return nearest;
        }

        public static Vector2 StepVector(GridDirection direction)
        {
            switch (direction)
            {
                case GridDirection.Up: return Vector2.up;
                case GridDirection.Left: return Vector2.left;
                case GridDirection.Down: return Vector2.down;
                case GridDirection.Right: return Vector2.right;
                default: return Vector2.zero;
            }
        }

        public static GridDirection Opposite(GridDirection direction)
        {
            switch (direction)
            {
                case GridDirection.Up: return GridDirection.Down;
                case GridDirection.Left: return GridDirection.Right;
                case GridDirection.Down: return GridDirection.Up;
                case GridDirection.Right: return GridDirection.Left;
                default: return GridDirection.None;
            }
        }

        private bool IsCorridor(MazeCell cell)
        {
            if (cell.Row < 0 || cell.Row >= Rows || cell.Column < 0 || cell.Column >= Columns)
                return false;
            int category = CategoryAt(cell);
            return category == 0 || category == 5 || category == 6;
        }

        private MazeCell Adjacent(MazeCell cell, GridDirection direction)
        {
            Vector2 step = StepVector(direction);
            int row = cell.Row - (int)step.y;
            int column = cell.Column + (int)step.x;
            if (row == TunnelRow && cell.Row == TunnelRow)
            {
                if (column < 0) column = Columns - 1;
                else if (column >= Columns) column = 0;
            }
            return new MazeCell(row, column);
        }
    }
}
