using System;
using System.Collections.Generic;

namespace ShanHaiSpiritTrail
{
    // Clockwise traversal of the supplied maze's outer corridor, including its
    // top, bottom and side wall indentations. The portal stubs are not in this loop.
    public sealed class ClockwisePatrol
    {
        private static readonly GridDirection[] Directions =
            { GridDirection.Up, GridDirection.Left, GridDirection.Down, GridDirection.Right };
        private readonly MazeNavigation navigation;
        private readonly Dictionary<MazeCell, GridDirection> clockwise = new Dictionary<MazeCell, GridDirection>();
        public IReadOnlyList<MazeCell> Route { get; }

        private readonly struct DirectedCell : IEquatable<DirectedCell>
        {
            public MazeCell Cell { get; }
            public GridDirection Incoming { get; }
            public DirectedCell(MazeCell cell, GridDirection incoming) { Cell = cell; Incoming = incoming; }
            public bool Equals(DirectedCell other) => Cell == other.Cell && Incoming == other.Incoming;
            public override bool Equals(object other) => other is DirectedCell cell && Equals(cell);
            public override int GetHashCode() => unchecked(Cell.GetHashCode() * 397 ^ (int)Incoming);
        }

        public ClockwisePatrol(MazeNavigation navigation)
        {
            this.navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
            if (navigation.Rows != 29 || navigation.Columns != 28)
                throw new ArgumentException("The clockwise patrol requires the supplied 29-by-28 maze.", nameof(navigation));
            var vertices = new[]
            {
                new MazeCell(1,1), new MazeCell(1,12), new MazeCell(5,12), new MazeCell(5,15),
                new MazeCell(1,15), new MazeCell(1,26), new MazeCell(8,26), new MazeCell(8,21),
                new MazeCell(20,21), new MazeCell(20,26), new MazeCell(27,26), new MazeCell(27,15),
                new MazeCell(23,15), new MazeCell(23,12), new MazeCell(27,12), new MazeCell(27,1),
                new MazeCell(20,1), new MazeCell(20,6), new MazeCell(8,6), new MazeCell(8,1),
                new MazeCell(1,1)
            };
            var cells = new List<MazeCell>(138);
            for (int index = 0; index < vertices.Length - 1; index++)
            {
                MazeCell cell = vertices[index], target = vertices[index + 1];
                int rowStep = Math.Sign(target.Row - cell.Row);
                int columnStep = Math.Sign(target.Column - cell.Column);
                if (rowStep != 0 && columnStep != 0)
                    throw new InvalidOperationException("Clockwise patrol segments must follow the grid.");
                GridDirection direction = rowStep < 0 ? GridDirection.Up : rowStep > 0 ? GridDirection.Down
                    : columnStep < 0 ? GridDirection.Left : GridDirection.Right;
                while (cell != target)
                {
                    var expected = new MazeCell(cell.Row + rowStep, cell.Column + columnStep);
                    if (!navigation.TryStep(cell, direction, out MazeCell next) || next != expected || clockwise.ContainsKey(cell))
                        throw new ArgumentException("The maze does not support the clockwise outer route at " + cell + ".", nameof(navigation));
                    cells.Add(cell);
                    clockwise.Add(cell, direction);
                    cell = next;
                }
            }
            if (cells.Count != 138)
                throw new InvalidOperationException("The clockwise patrol must contain 138 unique cells.");
            Route = cells.AsReadOnly();
        }

        public GridDirection Choose(MazeCell cell, GridDirection incoming, IReadOnlyList<GridDirection> legalChoices)
        {
            if (legalChoices == null || legalChoices.Count == 0)
                throw new ArgumentException("A patrol decision requires at least one legal direction.", nameof(legalChoices));
            if (clockwise.TryGetValue(cell, out GridDirection next) && Contains(legalChoices, next)) return next;

            // A frightened ghost can finish facing against the perimeter flow.
            // Search (cell, incoming direction) states so joining the loop never
            // requires an illegal reversal, including at intermediate junctions.
            var start = new DirectedCell(cell, incoming);
            var pending = new Queue<DirectedCell>();
            var firstSteps = new Dictionary<DirectedCell, GridDirection> { [start] = GridDirection.None };
            pending.Enqueue(start);
            while (pending.Count > 0)
            {
                DirectedCell state = pending.Dequeue();
                IReadOnlyList<GridDirection> choices = state.Equals(start) ? legalChoices : LegalDirections(state);
                GridDirection first = firstSteps[state];
                if (first != GridDirection.None && clockwise.TryGetValue(state.Cell, out next) && Contains(choices, next))
                    return first;
                foreach (var direction in choices)
                {
                    navigation.TryStep(state.Cell, direction, out MazeCell nextCell);
                    var successor = new DirectedCell(nextCell, direction);
                    if (firstSteps.ContainsKey(successor)) continue;
                    firstSteps.Add(successor, first == GridDirection.None ? direction : first);
                    pending.Enqueue(successor);
                }
            }
            throw new InvalidOperationException("No legal clockwise joining route exists from " + cell + ".");
        }

        private IReadOnlyList<GridDirection> LegalDirections(DirectedCell state)
        {
            var result = new List<GridDirection>(4);
            GridDirection reverse = MazeNavigation.Opposite(state.Incoming);
            foreach (var direction in Directions)
                if (direction != reverse && navigation.TryStep(state.Cell, direction, out _)) result.Add(direction);
            if (result.Count == 0 && navigation.TryStep(state.Cell, reverse, out _)) result.Add(reverse);
            return result;
        }

        private static bool Contains(IReadOnlyList<GridDirection> directions, GridDirection desired)
        {
            for (int index = 0; index < directions.Count; index++) if (directions[index] == desired) return true;
            return false;
        }
    }
}
