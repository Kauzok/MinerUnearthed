using RoR2;
using UnityEngine;

namespace EntityStates.Direseeker
{
    public class SpawnState : EntityState
    {
        public static float duration = 2.5f;

        public override void OnEnter()
        {
            base.OnEnter();
            base.GetModelAnimator();

            Util.PlaySound(EntityStates.LemurianBruiserMonster.SpawnState.spawnSoundString, base.gameObject);
            base.PlayAnimation("Body", "Spawn", "Spawn.playbackRate", SpawnState.duration);

            if (EntityStates.LemurianBruiserMonster.SpawnState.spawnEffectPrefab)
            {
                EffectManager.SimpleMuzzleFlash(EntityStates.LemurianBruiserMonster.SpawnState.spawnEffectPrefab, base.gameObject, "SpawnEffectOrigin", false);

                for (int i = 5; i <= 64; i += 2)
                {
                    EffectManager.SpawnEffect(EntityStates.LemurianBruiserMonster.SpawnState.spawnEffectPrefab, new EffectData
                    {
                        origin = base.transform.position + i * base.characterDirection.forward.normalized - 1.8f * Vector3.up,
                        scale = 2.5f
                    }, true);
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= SpawnState.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}
