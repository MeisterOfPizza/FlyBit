using UnityEngine;
using UnityEngine.EventSystems;

namespace FlyBit.UI
{

    class UISmoothLayoutGroup : UIBehaviour
    {

        #region Editor

        [SerializeField] private RectOffset padding   = new RectOffset();
        [SerializeField] private Vector2    size      = new Vector2(100, 100);
        [SerializeField] private float      spacing   = 0f;
        [SerializeField] private Direction  direction = Direction.Up;

        [Space]
        [SerializeField] private float travelSpeed = 3f;

        #endregion

        #region Private variables

        private int  childCount;
        private bool isDirty;

        #endregion

        #region Enums

        private enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }

        #endregion

        protected override void Awake()
        {
            base.Awake();

            childCount = transform.childCount;
        }

        private void Update()
        {
            if (isDirty)
            {
                UpdateChildren();
            }
        }

#pragma warning disable CS0114 // Member hides inherited member; missing override keyword
        private void OnValidate()
#pragma warning restore CS0114 // Member hides inherited member; missing override keyword
        {
            if (Application.isPlaying)
            {
                isDirty = true;
            }
        }

        private void OnTransformChildrenChanged()
        {
            if (transform.childCount != childCount)
            {
                // The child count was changed:
                childCount = transform.childCount;
                isDirty    = childCount > 0;
            }
        }

        private void UpdateChildren()
        {
            bool placingDone = true;

            for (int i = 0; i < childCount; i++)
            {
                Transform child  = transform.GetChild(i);
                Vector3   target = GetChildTargetPosition(i);

                child.localPosition = Vector3.Lerp(child.localPosition, target, travelSpeed * Time.deltaTime);

                if (placingDone)
                {
                    placingDone = Mathf.Abs(child.localPosition.magnitude - target.magnitude) < 0.01f;
                }
            }

            isDirty = !placingDone;
        }

        private Vector3 GetChildTargetPosition(int index)
        {
            switch (direction)
            {
                case Direction.Up:
                    return new Vector3(padding.horizontal, spacing * index + size.y * index + padding.bottom);
                case Direction.Down:
                    return new Vector3(padding.horizontal, -spacing * index - size.y * index - padding.top);
                case Direction.Left:
                    return new Vector3(-spacing * index - size.x * index - padding.right, padding.vertical);
                case Direction.Right:
                    return new Vector3(spacing * index + size.x * index + padding.left, padding.vertical);
                default:
                    return Vector3.zero;
            }
        }

    }

}
