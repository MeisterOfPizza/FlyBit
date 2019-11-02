using FlyBit.Controllers;
using UnityEngine;

namespace FlyBit.PowerUps
{

    sealed class InfiniteFuelPowerUp : PowerUp
    {

        #region Editor

        [Header("Values")]
        [SerializeField] private float powerUpDuration = 3f;

        #endregion

        protected override void Activate()
        {
            PlayerController.Singleton.AddPlayerEffect(PlayerController.PlayerEffect.InfiniteFuel, powerUpDuration);

            ScoreController.Singleton.AddStatRecordValue(ScoreController.StatRecordType.InfiniteFuelPowerUpsTaken, 1);
        }

    }

}
