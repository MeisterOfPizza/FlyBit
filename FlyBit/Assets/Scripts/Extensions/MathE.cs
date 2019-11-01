using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

        #region Event systems

        public static bool IsPointerOverUIObject(Vector3 position)
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = position;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

            return results.Count > 0;
        }

        #endregion

    }

}
