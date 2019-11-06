using FlyBit.Controllers;
using System.Collections;
using UnityEngine;

#pragma warning disable 0649

namespace FlyBit.Map
{

    class WallColumn : MonoBehaviour
    {

        #region Editor

        [Header("References")]
        [SerializeField] private SpriteRenderer topWall;
        [SerializeField] private SpriteRenderer bottomWall;

        [Header("Values")]
        [SerializeField] private float moveWallTime = 1.5f;

        #endregion

        public void Spawn(Vector2 columnPosition, Vector2 topWallPosition, Vector2 bottomWallPosition)
        {
            transform.localPosition = columnPosition;

            topWall.transform.localPosition    = topWallPosition;
            bottomWall.transform.localPosition = bottomWallPosition;
        }

        public void SetColor(Color color)
        {
            topWall.color    = color;
            bottomWall.color = color;
        }

        public void OpenCloseColumn(bool open)
        {
            StartCoroutine("MoveWalls", open);
        }

        private IEnumerator MoveWalls(bool open)
        {
            float   time         = moveWallTime;
            Vector2 topTarget    = new Vector2(0, open ? MapController.Singleton.ScreenHeight - transform.position.y : topWall.transform.localPosition.y);
            Vector2 bottomTarget = new Vector2(0, open ? -(transform.position.y + MapController.Singleton.ScreenHeight) : bottomWall.transform.localPosition.y);

            if (!open)
            {
                topWall.transform.localPosition    = Vector2.zero;
                bottomWall.transform.localPosition = Vector2.zero;
            }

            while (time > 0f)
            {
                time -= Time.deltaTime;

                float speed = 1f - time / moveWallTime;

                topWall.transform.localPosition    = Vector2.Lerp(topWall.transform.localPosition, topTarget, speed);
                bottomWall.transform.localPosition = Vector2.Lerp(bottomWall.transform.localPosition, bottomTarget, speed);

                yield return new WaitForEndOfFrame();
            }
        }

    }

}
