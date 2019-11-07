using FlyBit.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#pragma warning disable 0649

namespace FlyBit.Controllers
{

    enum PlayerEffect
    {
        InfiniteFuel,
        DubblePoints,
        Hyperdrive,
        HyperdriveController
    }

    class PlayerEffectsController : Controller<PlayerEffectsController>
    {

        #region Editor

        [Header("References")]
        [SerializeField] private RectTransform uiPlayerEffectsContainer;
        [SerializeField] private UIColorInvert uiPlayerEffectsContainerColorInvert;

        [Space]
        [SerializeField] private GameObject uiPlayerEffectPrefab;

        #endregion

        #region Private variables

        private Dictionary<PlayerEffect, float> playerEffects = new Dictionary<PlayerEffect, float>();

        private Dictionary<PlayerEffect, UIPlayerEffect> uiPlayerEffects = new Dictionary<PlayerEffect, UIPlayerEffect>();

        #endregion

        #region Player effects

        public void AddPlayerEffect(PlayerEffect playerEffect, float duration)
        {
            playerEffects[playerEffect] = duration;
        }

        public void RemovePlayerEffect(PlayerEffect playerEffect)
        {
            playerEffects.Remove(playerEffect);

            RemoveUIPlayerEffect(playerEffect);
        }

        public void UpdatePlayerEffects()
        {
            foreach (var effect in playerEffects.ToList())
            {
                if (effect.Value <= 0)
                {
                    RemovePlayerEffect(effect.Key);
                }
                else
                {
                    playerEffects[effect.Key] -= Time.deltaTime;
                    
                    if (uiPlayerEffects.TryGetValue(effect.Key, out UIPlayerEffect uiPlayerEffect))
                    {
                        uiPlayerEffect.SetDurationText(playerEffects[effect.Key]);
                    }

                    if (effect.Key == PlayerEffect.InfiniteFuel)
                    {
                        ScoreController.Singleton.AddStatRecordValue(ScoreController.StatRecordType.InfiniteFuelDuration, Time.deltaTime);
                    }
                }
            }
        }

        public void ClearPlayerEffects()
        {
            foreach (var playerEffect in playerEffects.Keys)
            {
                RemoveUIPlayerEffect(playerEffect);
            }

            playerEffects.Clear();
            uiPlayerEffects.Clear();
        }

        public bool HasPlayerEffect(PlayerEffect playerEffect)
        {
            return playerEffects.ContainsKey(playerEffect);
        }

        #endregion

        #region UI player effects

        public void AddUIPlayerEffect(PlayerEffect playerEffect, Sprite icon, float duration)
        {
            if (uiPlayerEffects.ContainsKey(playerEffect))
            {
                uiPlayerEffects[playerEffect].SetDurationText(duration);
            }
            else
            {
                var uiPlayerEffect = Instantiate(uiPlayerEffectPrefab, uiPlayerEffectsContainer).GetComponent<UIPlayerEffect>();
                uiPlayerEffect.Initialize(icon, duration);

                uiPlayerEffects.Add(playerEffect, uiPlayerEffect);

                uiPlayerEffectsContainerColorInvert.AddColorOptions(Color.white, uiPlayerEffect.TargetGraphics);
                uiPlayerEffectsContainerColorInvert.SetColor(PlayerController.Singleton.Invert);
            }
        }

        private void RemoveUIPlayerEffect(PlayerEffect playerEffect)
        {
            if (uiPlayerEffects.TryGetValue(playerEffect, out UIPlayerEffect uiPlayerEffect))
            {
                uiPlayerEffects.Remove(playerEffect);

                foreach (var graphic in uiPlayerEffect.TargetGraphics)
                {
                    uiPlayerEffectsContainerColorInvert.RemoveColorOption(graphic);
                }

                Destroy(uiPlayerEffect.gameObject);
            }
        }

        #endregion

    }

}
