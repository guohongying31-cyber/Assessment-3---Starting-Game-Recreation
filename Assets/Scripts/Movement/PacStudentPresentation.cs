using UnityEngine;

namespace ShanHaiSpiritTrail
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PacStudentPatrol), typeof(Animator), typeof(AudioSource))]
    public sealed class PacStudentPresentation : MonoBehaviour
    {
        private static readonly int Showcase = Animator.StringToHash("Showcase");
        private static readonly int Right = Animator.StringToHash("WalkingRight");
        private static readonly int Down = Animator.StringToHash("WalkingDown");
        private static readonly int Left = Animator.StringToHash("WalkingLeft");
        private static readonly int Up = Animator.StringToHash("WalkingUp");

        private PacStudentPatrol patrol;
        private Animator animator;
        private AudioSource movementAudio;
        private bool started;

        private void Awake()
        {
            patrol = GetComponent<PacStudentPatrol>();
            animator = GetComponent<Animator>();
            movementAudio = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            patrol.DirectionChanged += FaceDirection;
            if (started) FaceDirection(patrol.Direction);
        }

        private void Start()
        {
            started = true;
            animator.SetBool(Showcase, false);
            FaceDirection(patrol.Direction);
        }

        private void FaceDirection(Vector3 direction)
        {
            if (!started) return;
            int state = direction.x > 0 ? Right : direction.x < 0 ? Left : direction.y < 0 ? Down : Up;
            animator.Play(state, 0, 0);
            // Evaluate the new sprite on the same frame as the corner turn.
            animator.Update(0);
        }

        private void Update()
        {
            // Set playback speed before Unity evaluates this frame's animation.
            bool moving = patrol.isActiveAndEnabled && Time.timeScale > 0;
            animator.speed = moving ? 1 : 0;
            if (moving && !movementAudio.isPlaying) movementAudio.Play();
            else if (!moving && movementAudio.isPlaying) movementAudio.Stop();
        }

        private void OnDisable()
        {
            patrol.DirectionChanged -= FaceDirection;
            movementAudio.Stop();
            animator.speed = 0;
        }
    }
}
