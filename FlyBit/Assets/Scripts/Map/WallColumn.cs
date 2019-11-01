using FlyBit.Controllers;
using System;
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
        [SerializeField] private float screenHeight = 5f;
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
            Vector2 topTarget    = new Vector2(transform.position.x, open ? PlayerController.Singleton.transform.position.y + screenHeight : topWall.transform.position.y);
            Vector2 bottomTarget = new Vector2(transform.position.x, open ? PlayerController.Singleton.transform.position.y - screenHeight : bottomWall.transform.position.y);

            if (!open)
            {
                topWall.transform.localPosition    = Vector2.zero;
                bottomWall.transform.localPosition = Vector2.zero;
            }

            while (time > 0f)
            {
                time -= Time.deltaTime;

                topWall.transform.position    = Vector3.Lerp(topWall.transform.position, topTarget, moveWallTime * screenHeight * Time.deltaTime);
                bottomWall.transform.position = Vector3.Lerp(bottomWall.transform.position, bottomTarget, moveWallTime * screenHeight * Time.deltaTime);

                yield return new WaitForEndOfFrame();
            }

            while (transform.position.x > PlayerController.Singleton.transform.position.x - MapController.Singleton.PlayerSeeRadius)
            {
                yield return new WaitForEndOfFrame();
            }
        }

    }

}
