using UnityEngine;

namespace ShanHaiSpiritTrail
{
    [RequireComponent(typeof(Animator))]
    public sealed class ActorSprite : MonoBehaviour
    {
        private Animator actorAnimator;
        private string lastState;
        private void Awake()
        {
            actorAnimator = GetComponent<Animator>();
            actorAnimator.updateMode = AnimatorUpdateMode.Normal;
            actorAnimator.SetBool("Showcase", false);
        }

        public void Present(GridDirection direction, bool animate, string form = "Walking")
        {
            string state = form == "Dead" || form == "Recovering" ? form
                : form + (direction == GridDirection.None ? "Left" : direction.ToString());
            if (lastState != state)
            {
                actorAnimator.Play(state, 0, 0);
                actorAnimator.Update(0);
                lastState = state;
            }
            actorAnimator.speed = animate ? 1 : 0;
        }
    }
}
