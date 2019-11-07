using FlyBit.Controllers;
using UnityEngine;

#pragma warning disable 0649

namespace FlyBit.PowerUps
{

    sealed class InfiniteFuelPowerUp : PowerUp
    {

        #region Editor

        [Header("References")]
        [SerializeField] private Sprite uiPlayerEffectIcon;

        [Header("Values")]
        [SerializeField] private float infiniteFuelDuration = 3f;

        #endregion

        protected override void Activate()
        {
            PlayerEffectsController.Singleton.AddPlayerEffect(PlayerEffect.InfiniteFuel, infiniteFuelDuration);
            PlayerEffectsController.Singleton.AddUIPlayerEffect(PlayerEffect.InfiniteFuel, uiPlayerEffectIcon, infiniteFuelDuration);

            ScoreController.Singleton.AddStatRecordValue(ScoreController.StatRecordType.InfiniteFuelPowerUpsTaken, 1);
        }

    }

}
