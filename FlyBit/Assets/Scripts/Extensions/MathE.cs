using UnityEngine;

namespace FlyBit.Extensions
{

    static class MathE
    {

        #region Color

        public static Color Invert(this Color color)
        {
            return new Color(1f - color.r, 1f - color.g, 1f - color.b, color.a);
        }

        #endregion

    }

}
