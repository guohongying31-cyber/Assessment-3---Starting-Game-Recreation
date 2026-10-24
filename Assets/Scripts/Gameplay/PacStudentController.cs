using UnityEngine;

namespace ShanHaiSpiritTrail
{
    // New keys replace lastInput. The motor evaluates it only when the current
    // one-cell tween ends, falling back to currentInput at blocked turns.
    public sealed class PacStudentController : MonoBehaviour
    {
        [SerializeField] private GridDirection lastInput;
        [SerializeField] private GridDirection currentInput;
        public GridDirection LastInput => lastInput;
        public GridDirection CurrentInput => currentInput;
        public GridMotor Motor { get; private set; }
        public bool InputEnabled { get; set; }

        public void Initialize(MazeNavigation navigation, MazeCell start)
        {
            Motor = new GridMotor(navigation, start, 5);
            Motor.DirectionChanged += direction => currentInput = direction;
            ResetLife(start);
        }

        public void ResetLife(MazeCell start)
        {
            lastInput = currentInput = GridDirection.None;
            Motor.Reset(start);
        }

        public void SubmitDirection(GridDirection direction)
        {
            if (!InputEnabled || direction == GridDirection.None) return;
            lastInput = direction;
            Motor.QueueDirection(lastInput);
        }

        private void Update()
        {
            if (!InputEnabled || Motor == null) return;
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) SubmitDirection(GridDirection.Up);
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) SubmitDirection(GridDirection.Left);
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) SubmitDirection(GridDirection.Down);
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) SubmitDirection(GridDirection.Right);
        }
    }
}
