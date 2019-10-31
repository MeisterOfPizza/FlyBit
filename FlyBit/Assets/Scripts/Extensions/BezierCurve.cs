using UnityEngine;

namespace FlyBit.Extensions
{

    sealed class BezierCurve
    {

        private Vector2 p0, p1, p2, p3;

        public BezierCurve(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            this.p0 = p0;
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
        }

        public Vector2 GetPoint(float t)
        {
            float delta = 1f - t;

            // P = (1−t)^3 * P1 + 3(1−t)^2 * tP2 + 3(1−t) * t^2 * P3 + t^3 * P4
            return delta * delta * delta * p0 + 3 * (delta * delta) * t * p1 + 3 * delta * (t * t) * p2 + t * t * t * p3;
        }

        public static BezierCurve CreateSlope(Vector2 start, Vector2 end)
        {
            float middleX = (start.x + end.x) / 2f;

            return new BezierCurve(start, new Vector2(middleX, start.y), new Vector2(middleX, end.y), end);
        }

    }

}
