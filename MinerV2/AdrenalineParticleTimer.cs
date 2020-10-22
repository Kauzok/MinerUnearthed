using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace MinerPlugin {

    public class AdrenalineParticleTimer : MonoBehaviour {

        private float maxEmission = 400, adrenViewMax = MinerPlugin.adrenalineCap;
        private float minSpeed = 1f, maxSpeed = 5f;
        private float minSpread = 0.35f, maxSpread = 0.47f;
        private float minRadius = 0.15f, maxRadius = 0.34f;

        private Color maxColor = new Color(1, 0.6f, 0f), minColor = new Color(0.6f, 0f, 0f);

        private float comboDuration = 5;

        public ParticleSystem _fireParticles;

        private EmissionModule _emisModule;
        private MainModule _mainModule;
        private ShapeModule _shapeModule;
        private bool modulesInitialized;

        private float _adrenalineCount = 0;
        private float _comboTimer = 0;

        public void init(GameObject particleSystemObject) {

            _fireParticles = particleSystemObject.GetComponent<ParticleSystem>();

        }

        public void updateAdrenaline(float adrenAmount, bool refreshCombo = false) {

            _adrenalineCount = adrenAmount;

            if (refreshCombo) {
                _comboTimer = 4.5f; //+ magicianearrings stacks + 1
                _fireParticles.Play();
            }
        }

        void Start() {

            _emisModule = _fireParticles.emission;
            _mainModule = _fireParticles.main;
            _shapeModule = _fireParticles.shape;
        }

        void Update() {

            _comboTimer -= Time.deltaTime;

            //lerps between min value and max value based on our lerp value, ranged 0-1

            if (!modulesInitialized) {

                modulesInitialized = true;
            }

            float comboLerp = _comboTimer / comboDuration;

            _mainModule.startColor = Color.Lerp(minColor, maxColor, comboLerp);

            float adrenalineLerp = _adrenalineCount / adrenViewMax;

            _emisModule.rateOverTime = Mathf.Lerp(0, maxEmission, adrenalineLerp);
            _mainModule.startSpeed = Mathf.Lerp(minSpeed, maxSpeed, adrenalineLerp);
            _shapeModule.randomDirectionAmount = Mathf.Lerp(minSpread, maxSpread, adrenalineLerp);
            _shapeModule.radius = Mathf.Lerp(minRadius, maxRadius, adrenalineLerp);
        }

    } 
}
