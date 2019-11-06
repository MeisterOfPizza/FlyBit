using FlyBit.Controllers;
using UnityEngine;

namespace FlyBit.PowerUps
{

    class DubblePointsPowerUp : PowerUp
    {

        #region Editor

        [Header("Values")]
        [SerializeField] private float dubblePointsDuration = 30f;

        #endregion

        protected override void Activate()
        {
            PlayerController.Singleton.AddPlayerEffect(PlayerController.PlayerEffect.DubblePoints, dubblePointsDuration);

            ScoreController.Singleton.AddStatRecordValue(ScoreController.StatRecordType.DubblePointsPowerUpsTaken, 1);
        }

    }

}
