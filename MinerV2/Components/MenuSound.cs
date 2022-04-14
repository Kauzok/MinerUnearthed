using RoR2;
using UnityEngine;
using System.Security;
using System.Security.Permissions;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace DiggerPlugin {
    public class MenuSound : MonoBehaviour
    {
        private uint playID;

        private void OnEnable() {

            this.playID = Util.PlaySound(Sounds.Select, base.gameObject);
        }

        private void OnDestroy()
        {
            AkSoundEngine.StopPlayingID(this.playID);
        }
    }
}