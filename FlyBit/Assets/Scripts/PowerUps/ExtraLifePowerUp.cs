using FlyBit.Controllers;

namespace FlyBit.PowerUps
{

    sealed class ExtraLifePowerUp : PowerUp
    {

        protected override void Activate()
        {
            PlayerController.Singleton.GiveLife();

            ScoreController.Singleton.AddStatRecordValue(ScoreController.StatRecordType.ExtraLifePowerUpsTaken, 1);
        }

        private void OnDisable()
        {
            if (!isActivated)
            {
                // The power up was not taken.
                ScoreController.Singleton.AddStatRecordValue(ScoreController.StatRecordType.ExtraLifePowerUpsMissed, 1);
            }
        }

    }

}
