using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlashCandy
{
    public static class SwapDetector
    {

        public static bool swapDetectionComplete = false;

        public static Vector2Int swipeStartCoord;
        public static Vector2Int swipeDir;


        private static Vector2 _worldMapSize;
        private static Vector2Int _mapSize;
        private static GameObject _boardGO;
        private static RectTransform _boardHolder;

        private const int raycastLayer = 6;
        private const float minSwipeOnViewport = 0.10f;

        private static Camera cam;

        public static void Init(Vector2 worldMapSize, Vector2Int mapSize, GameObject boardGO, RectTransform boardHolder)
        {
            cam = Camera.main;
            _worldMapSize = worldMapSize;
            _mapSize = mapSize;
            _boardGO = boardGO;
            _boardHolder = boardHolder;
        }

        public static IEnumerator DetectSwap()
        {
            swipeStartCoord = Vector2Int.zero;
            swipeDir = Vector2Int.zero;
            swapDetectionComplete = false;
            Vector2 originalPointerPos = GetPointerPos();
            Vector2 pointerPos = originalPointerPos;
            Vector2Int tempSwipeStartCord = Vector2Int.zero;
            Vector2 distanceSwiped;
            do
            {
                pointerPos -= (Vector2)_boardHolder.position;
                if (_boardHolder.rect.Contains(pointerPos))
                {
                    pointerPos = new Vector2(pointerPos.x / cam.pixelWidth, pointerPos.y / cam.pixelHeight) + Vector2.one / 2;

                    Ray ray = cam.ViewportPointToRay(pointerPos);

                    Physics.Raycast(ray, out RaycastHit hitInfo, 40f); //40f is a random value that ensures all objects can be hit
                    tempSwipeStartCord = hitInfo.transform.GetComponent<CandyManager>().BoardPosition;

                }
                yield return null;
                distanceSwiped = GetPointerPos() - originalPointerPos;
            }
            while (IsPointerDown() && distanceSwiped.sqrMagnitude < Mathf.Pow(cam.pixelWidth * minSwipeOnViewport,2));
            Vector2Int swipeDirection = Vector2Int.zero;

            if (Mathf.Abs(distanceSwiped.x) > Mathf.Abs(distanceSwiped.y))
            {
                swipeDirection.x = Mathf.RoundToInt(Mathf.Sign(distanceSwiped.x));
            }
            else
            {
                swipeDirection.y = Mathf.RoundToInt(Mathf.Sign(distanceSwiped.y));
            }
            

            swapDetectionComplete = true;
            swipeStartCoord = tempSwipeStartCord;
            swipeDir = swipeDirection;

        }

        public static bool IsPointerDown()
        {

            return Input.GetMouseButton(0) || (Input.touchCount > 0);
            
        }

        public static Vector2 GetPointerPos()
        {
            if (Input.touchCount > 0)
            {
                return Input.GetTouch(0).position;
            }
            else
            {
                return Input.mousePosition;
            }
        }



    }
}
