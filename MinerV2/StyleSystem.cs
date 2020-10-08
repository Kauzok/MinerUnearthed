using RoR2.UI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using MinerPlugin;
using RoR2;

namespace StyleSystem
{
    public class ExtraHUD : MonoBehaviour
    {
        private void Awake()
        {
            Invoke("InitHUD", 0.25f);
        }

        private void InitHUD()
        {
            HUD self = this.gameObject.GetComponent<HUD>();

            if (self.targetMaster)
            {
                if (self.targetMaster.bodyPrefab == MinerPlugin.MinerPlugin.characterPrefab)
                {
                    GameObject styleMeter = GameObject.Instantiate(Assets.styleMeter, self.transform.Find("MainContainer").Find("MainUIArea").Find("BottomCenterCluster"));
                    styleMeter.GetComponent<RectTransform>().localPosition = Vector3.zero;
                    styleMeter.GetComponent<RectTransform>().anchoredPosition = new Vector3(-128f, 0);
                    styleMeter.GetComponent<RectTransform>().localScale = 1.5f * Vector3.one;

                    StyleMeter temp = styleMeter.AddComponent<StyleMeter>();
                    temp.characterMaster = self.targetMaster;

                    foreach (Image i in temp.GetComponentsInChildren<Image>())
                    {
                        if (i)
                        {
                            if (i.gameObject.name == "StyleRank") temp.styleRankImage = i;
                            if (i.gameObject.name == "StyleText") temp.styleTextImage = i;
                        }
                    }
                }
            }
        }
    }

    public class StyleMeter : MonoBehaviour
    {
        public CharacterMaster characterMaster;
        public CharacterBody characterBody;
        public Image styleRankImage;
        public Image styleTextImage;

        public float styleScore;
        private float maxStyle;
        private float styleDecayRate;
        private float styleDecayGrowth;
        private float styleMaxDecay;
        private float baseStyleGain;

        private Animator anim;
        private string styleString;
        private string oldStyleString;
        private float oldStyleScore;
        private bool isHidden;

        public static event Action<float> OnStyleChanged = delegate { };

        private void Awake()
        {
            anim = GetComponent<Animator>();

            styleScore = 0;
            maxStyle = 100;
            styleDecayRate = 0.24f;
            styleDecayGrowth = 1.25f;
            styleMaxDecay = 4.8f;
            baseStyleGain = 2.1f;

            isHidden = true;
        }

        private void FixedUpdate()
        {
            if (!characterMaster)
            {
                return;
            }

            bool hasPlayer = false;

            if (characterBody != null) hasPlayer = true;

            if (!hasPlayer)
            {
                if (characterMaster.hasBody)
                {
                    if (characterMaster.GetBody() != null)
                    {
                        bool flag = false;
                        if (characterMaster.GetBody().baseNameToken == "MINER_NAME") flag = true;

                        if (!flag)
                        {
                            Destroy(this.gameObject);
                            return;
                        }

                        characterBody = characterMaster.GetBody();

                        if (characterBody.GetComponent<StyleComponent>()) characterBody.GetComponent<StyleComponent>().styleMeter = this;
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }

            StyleDecay();

            string fuckOff = "yeah i know this is lazy hardcoded garbage";
            styleString = "D";

            if (styleScore > 10) styleString = "C";
            if (styleScore > 30) styleString = "B";
            if (styleScore > 50) styleString = "A";
            if (styleScore > 70) styleString = "S";
            if (styleScore > 90) styleString = "SS";
            if (styleScore > 95) styleString = "SSS";

            UpdateStyleMeter(styleString);
        }

        private void StyleDecay()
        {
            styleScore = Mathf.Clamp(styleScore - styleDecayRate * Time.fixedDeltaTime, 0, maxStyle);

            if (characterMaster)
            {
                if (characterMaster.inventory.GetItemCount(ItemIndex.LunarDagger) > 0 && styleScore >= 29) styleScore = 29;
            }

            float growth = styleDecayGrowth;
            if (styleScore > 80) growth *= 1.8f;
            styleDecayRate = Mathf.Clamp(styleDecayRate + styleDecayGrowth * Time.fixedDeltaTime, 0, styleMaxDecay);
        }

        public void AddStyle(float coefficient)
        {
            float gain = baseStyleGain * coefficient;
            float mod = maxStyle / styleScore;
            mod = Mathf.Clamp(mod - 1, 0.4f, 4f);

            if (DifficultyIndex.Hard <= Run.instance.selectedDifficulty) mod *= 1.1f;
            if (DifficultyIndex.Easy >= Run.instance.selectedDifficulty) mod *= 0.5f;

            if (characterMaster)
            {
                if (characterMaster.inventory.GetItemCount(ItemIndex.ShieldOnly) > 0) mod *= 5;
            }

            gain *= mod;

            styleScore = Mathf.Clamp(styleScore + gain, 0, maxStyle);
            styleDecayRate = 0.1f;
        }

        private void UpdateStyleMeter(string tempStyleString)
        {
            if (tempStyleString == "D")
            {
                styleRankImage.sprite = Assets.styleD;
                styleTextImage.sprite = Assets.styleTextD;
            }
            else if (tempStyleString == "C")
            {
                styleRankImage.sprite = Assets.styleC;
                styleTextImage.sprite = Assets.styleTextC;
            }
            else if (tempStyleString == "B")
            {
                styleRankImage.sprite = Assets.styleB;
                styleTextImage.sprite = Assets.styleTextB;
            }
            else if (tempStyleString == "A")
            {
                styleRankImage.sprite = Assets.styleA;
                styleTextImage.sprite = Assets.styleTextA;
            }
            else if (tempStyleString == "S")
            {
                styleRankImage.sprite = Assets.styleS;
                styleTextImage.sprite = Assets.styleTextS;
            }
            else if (tempStyleString == "SS")
            {
                styleRankImage.sprite = Assets.styleSS;
                styleTextImage.sprite = Assets.styleTextSS;
            }
            else if (tempStyleString == "SSS")
            {
                styleRankImage.sprite = Assets.styleSSS;
                styleTextImage.sprite = Assets.styleTextSSS;
            }

            if (tempStyleString != oldStyleString || (styleScore > 0 && isHidden))
            {
                OnStyleChanged(styleScore);

                if (styleScore > 0)
                {
                    isHidden = false;

                    if (styleScore >= oldStyleScore)
                    {
                        anim.SetTrigger("Good");
                    }
                    else
                    {
                        anim.SetTrigger("Bad");
                    }
                }

            }

            if (styleScore <= 0 && !isHidden)
            {
                anim.SetTrigger("Reset");
                isHidden = true;
            }

            oldStyleString = tempStyleString;
            oldStyleScore = styleScore;
        }
    }

    public class StyleComponent : MonoBehaviour
    {
        public StyleMeter styleMeter;

        public void AddStyle(float style)
        {
            if (this.styleMeter) this.styleMeter.AddStyle(style);
        }
    }
}
