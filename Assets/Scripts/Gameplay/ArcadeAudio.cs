using UnityEngine;

namespace ShanHaiSpiritTrail
{
    public sealed class ArcadeAudio : MonoBehaviour
    {
        [SerializeField] private AudioClip intro, normal, scared, dead, menu;
        [SerializeField] private AudioClip pellet, wall, movement, death, ghost, fruit;
        private AudioSource musicSource, effectSource, footsteps;
        private void Awake()
        {
            musicSource = CreateSource(.35f);
            effectSource = CreateSource(.7f);
            footsteps = CreateSource(.17f);
            footsteps.clip = movement;
            footsteps.loop = true;
        }
        private AudioSource CreateSource(float volume)
        {
            var source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 0;
            source.volume = volume;
            return source;
        }
        public void Music(string mode)
        {
            var clip = mode == "Intro" ? intro : mode == "Scared" ? scared : mode == "Dead" ? dead
                : mode == "Menu" ? menu : mode == "Normal" ? normal : null;
            if (musicSource.clip == clip) return;
            musicSource.Stop();
            musicSource.clip = clip;
            musicSource.loop = mode != "Intro";
            if (clip) musicSource.Play();
        }
        public void Effect(string name)
        {
            var clip = name == "Pellet" ? pellet : name == "Wall" ? wall : name == "Death" ? death
                : name == "Ghost" ? ghost : name == "Fruit" ? fruit : null;
            if (clip) effectSource.PlayOneShot(clip);
        }
        public void Moving(bool moving)
        {
            if (moving && !footsteps.isPlaying) footsteps.Play();
            if (!moving && footsteps.isPlaying) footsteps.Stop();
        }
        public void Pause(bool pause)
        {
            if (pause) { musicSource.Pause(); effectSource.Pause(); footsteps.Pause(); }
            else { musicSource.UnPause(); effectSource.UnPause(); }
        }
    }
}
