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

        #region Public properties

        public bool IsMoving
        {
            get
            {
                return isMoving;
            }
        }

        #endregion

        #region Private variables

        private Action<WallColumn> onCloseCallback;

        private bool isMoving;

        #endregion

        public void Initialize(Action<WallColumn> onCloseCallback)
        {
            //this.onCloseCallback = onCloseCallback;
        }

        public void Spawn(Vector2 position, float spacing)
        {
            transform.position = position;

            topWall.transform.localPosition    = new Vector2(0f, spacing / 2f);
            bottomWall.transform.localPosition = new Vector2(0f, -spacing / 2f);
        }

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

        public void OpenCloseSection(bool open)
        {
            isMoving = true;

            StartCoroutine("MoveWalls", open);
        }

        private IEnumerator MoveWalls(bool open)
        {
            float   time         = moveWallTime;
            Vector2 topTarget    = new Vector2(transform.position.x, open ? PlayerController.Singleton.transform.position.y + screenHeight : transform.position.y);
            Vector2 bottomTarget = new Vector2(transform.position.x, open ? PlayerController.Singleton.transform.position.y - screenHeight : transform.position.y);

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

            if (!open)
            {
                //onCloseCallback.Invoke(this);
            }

            isMoving = false;
        }

    }

}
