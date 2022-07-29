using EntityStates;
using RoR2;
using UnityEngine;
using KinematicCharacterController;
using R2API;

namespace EntityStates.Digger
{
    public class ToTheStarsClassicScepter : ToTheStarsClassic
    {
        public override void FireStar(BulletAttack bulletAttack, Vector3 forwardDirection)
        {
            //add stun
            bulletAttack.damageType |= DamageType.Stun1s;

            //Fire initial center shot
            bulletAttack.aimVector = Vector3.down;
            bulletAttack.Fire();

            //Fire the edges of the star
            forwardDirection.y = 0f;
            forwardDirection.Normalize();

            Vector3 origin = bulletAttack.origin;

            //Outer
            int edges = 5;
            float radiansPerRotation = 2f * Mathf.PI / edges;
            float distanceFromCenter = 8f;

            for (int i = 0; i < edges; i++)
            {
                Vector3 currentForward = forwardDirection;
                Vector3 newForward = currentForward;
                if (i != 0) //Rotate Vector laterally
                {
                    float cos = Mathf.Cos(i * radiansPerRotation);
                    float sin = Mathf.Sin(i * radiansPerRotation);

                    float x2 = currentForward.x * cos - currentForward.z * sin;
                    float z2 = currentForward.x * sin + currentForward.z * cos;
                    newForward.x = x2;
                    newForward.z = z2;
                }

                Vector3 shotOrigin = origin + distanceFromCenter * newForward;
                bulletAttack.origin = shotOrigin;
                bulletAttack.procChainMask = default;
                bulletAttack.Fire();
            }

            //Inner
            edges = 4;
            float offset = Mathf.PI * 0.25f;
            radiansPerRotation = 2f * Mathf.PI / edges;
            distanceFromCenter = 12f;

            for (int i = 0; i < edges; i++)
            {
                Vector3 currentForward = forwardDirection;
                Vector3 newForward = Vector3.zero;

                float cos = Mathf.Cos(i * radiansPerRotation + offset);
                float sin = Mathf.Sin(i * radiansPerRotation + offset);

                float x2 = currentForward.x * cos - currentForward.z * sin;
                float z2 = currentForward.x * sin + currentForward.z * cos;
                newForward.x = x2;
                newForward.z = z2;

                Vector3 shotOrigin = origin + distanceFromCenter * newForward;
                bulletAttack.origin = shotOrigin;
                bulletAttack.procChainMask = default;
                bulletAttack.Fire();
            }
        }
    }
}
