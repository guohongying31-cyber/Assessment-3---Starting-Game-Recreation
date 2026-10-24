using UnityEngine;

namespace ShanHaiSpiritTrail
{
    [RequireComponent(typeof(Animator))]
    public sealed class ActorSprite : MonoBehaviour
    {
        private Animator actorAnimator;
        private string lastState;
        private float clipDuration;
        private void Awake()
        {
            actorAnimator = GetComponent<Animator>();
            actorAnimator.updateMode = AnimatorUpdateMode.Normal;
            actorAnimator.SetBool("Showcase", false);
        }

        public void Present(GridDirection direction, bool animate, string form = "Walking", float cycleSeconds = 0)
        {
            string state = form == "Dead" || form == "Recovering" ? form
                : form + (direction == GridDirection.None ? "Left" : direction.ToString());
            if (lastState != state)
            {
                actorAnimator.Play(state, 0, 0);
                actorAnimator.Update(0);
                lastState = state;
                var clips = actorAnimator.GetCurrentAnimatorClipInfo(0);
                clipDuration = clips.Length > 0 ? clips[0].clip.length : 0;
            }
            // The death audio and particles last 1.8 seconds; scale the original
            // animation cycle to the same duration without editing shared clips.
            actorAnimator.speed = !animate ? 0 : cycleSeconds > 0 && clipDuration > 0
                ? clipDuration / cycleSeconds : 1;
        }
    }
}
