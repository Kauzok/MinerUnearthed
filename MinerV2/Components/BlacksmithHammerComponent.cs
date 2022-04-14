using System;
using RoR2;
using UnityEngine;
using System.Security;
using System.Security.Permissions;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace DiggerPlugin {
    public class BlacksmithHammerComponent : MonoBehaviour
    {
        public static event Action<bool> HammerGetEvent = delegate { };

        private void Awake()
        {
            InvokeRepeating("Sex", 0.5f, 0.5f);
        }

        private void Sex()
        {
            Collider[] array = Physics.OverlapSphere(transform.position, 2.5f, LayerIndex.defaultLayer.mask);
            for (int i = 0; i < array.Length; i++)
            {
                CharacterBody component = array[i].GetComponent<CharacterBody>();
                if (component)
                {
                    if (component.baseNameToken == "MINER_NAME")
                    {
                        HammerGetEvent?.Invoke(true);
                        Destroy(this.gameObject);
                    }
                }
            }
        }
    }
}