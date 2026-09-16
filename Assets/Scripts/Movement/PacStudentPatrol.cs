using System;
using UnityEngine;

namespace ShanHaiSpiritTrail
{
    [DisallowMultipleComponent]
    public sealed class PacStudentPatrol : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] private float unitsPerSecond = 2.5f;

        // Fixed world-space route; changing the generated map must not relocate this patrol.
        private static readonly Vector3[] Waypoints =
        {
            new Vector3(1, -1, 0),
            new Vector3(6, -1, 0),
            new Vector3(6, -5, 0),
            new Vector3(1, -5, 0)
        };

        private LinearPositionTween segment;
        private int segmentIndex;

        public float UnitsPerSecond => unitsPerSecond;
        public Vector3 Direction { get; private set; }
        public int CompletedLaps { get; private set; }
        public event Action<Vector3> DirectionChanged;

        private void Awake()
        {
            if (unitsPerSecond <= 0 || float.IsNaN(unitsPerSecond) || float.IsInfinity(unitsPerSecond))
            {
                Debug.LogError("Patrol speed must be a positive finite number.", this);
                enabled = false;
                return;
            }

            transform.position = Waypoints[0];
            BeginSegment();
        }

        private void Update()
        {
            double remaining = Time.deltaTime;
            while (remaining > 0)
            {
                transform.position = segment.Advance(remaining, out remaining);
                if (!segment.IsComplete) break;

                segmentIndex = (segmentIndex + 1) % Waypoints.Length;
                if (segmentIndex == 0) CompletedLaps++;
                BeginSegment();
                // Carry unused time into the next segment, including an exact corner turn.
            }
        }

        private void BeginSegment()
        {
            Vector3 start = Waypoints[segmentIndex];
            Vector3 end = Waypoints[(segmentIndex + 1) % Waypoints.Length];
            double duration = Vector3.Distance(start, end) / (double)unitsPerSecond;
            segment = new LinearPositionTween(start, end, duration);
            Direction = (end - start).normalized;
            DirectionChanged?.Invoke(Direction);
        }

        private void OnValidate()
        {
            if (unitsPerSecond < 0.1f || float.IsNaN(unitsPerSecond) || float.IsInfinity(unitsPerSecond))
                unitsPerSecond = 2.5f;
        }
    }
}
