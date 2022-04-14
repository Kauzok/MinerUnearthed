using UnityEngine;

namespace DiggerPlugin {
    public class DireseekerController : MonoBehaviour
    {
        public ParticleSystem burstFlame;
        public ParticleSystem rageFlame;

        public void StartRageMode()
        {
            if (rageFlame) rageFlame.Play();
        }

        public void FlameBurst()
        {
            if (burstFlame) burstFlame.Play();
        }
    }
}
