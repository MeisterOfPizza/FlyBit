using FlyBit.Controllers;

namespace FlyBit.PowerUps
{

    sealed class ExtraLifePowerUp : PowerUp
    {

        protected override void Activate()
        {
            PlayerController.Singleton.GiveLife();
        }

    }

}
