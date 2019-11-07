using FlyBit.Controllers;
using UnityEngine;

#pragma warning disable 0649

namespace FlyBit.PowerUps
{

    class DubblePointsPowerUp : PowerUp
    {

        #region Editor

        [Header("References")]
        [SerializeField] private Sprite uiPlayerEffectIcon;

        [Header("Values")]
        [SerializeField] private float dubblePointsDuration = 30f;

        #endregion

        protected override void Activate()
        {
            PlayerEffectsController.Singleton.AddPlayerEffect(PlayerEffect.DubblePoints, dubblePointsDuration);
            PlayerEffectsController.Singleton.AddUIPlayerEffect(PlayerEffect.DubblePoints, uiPlayerEffectIcon, dubblePointsDuration);

            ScoreController.Singleton.AddStatRecordValue(ScoreController.StatRecordType.DubblePointsPowerUpsTaken, 1);
        }

    }

}
