using FlyBit.Controllers;

namespace FlyBit.PowerUps
{

    class InvertPowerUp : PowerUp
    {

        protected override void Activate()
        {
            EffectsController.Singleton.ToggleInvertEffect();
        }

    }

}
