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

        private static Camera gameplayCam;
        private static Camera mainCam;

        public static void Init(Vector2 worldMapSize, Vector2Int mapSize, GameObject boardGO, RectTransform boardHolder, Camera gameplayCamera, Camera mainCamera)
        {
            gameplayCam = gameplayCamera;
            mainCam = mainCamera;
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
            yield return null;
            if (RectTransformUtility.RectangleContainsScreenPoint(_boardHolder,pointerPos))
            {
                yield return null;
                Vector2 viewPortPos;
                Vector2 localPoint = pointerPos - (Vector2)_boardHolder.position;
                viewPortPos = localPoint / _boardHolder.rect.size;
                viewPortPos += Vector2.one * 0.5f;
                Ray ray = gameplayCam.ViewportPointToRay(viewPortPos);
                Physics.Raycast(ray, out RaycastHit hitInfo, 40f); //40f is a random value that ensures all objects can be hit
                tempSwipeStartCord = hitInfo.transform.GetComponent<CandyManager>().BoardPosition;
            }
            else
            {
                yield return null;
                swapDetectionComplete = true; 

                yield break;
            }


            do
            {
                yield return null;
                pointerPos = GetPointerPos();
                distanceSwiped = pointerPos - originalPointerPos;
            }
            while (IsPointerDown() && distanceSwiped.sqrMagnitude < Mathf.Pow(mainCam.pixelWidth * minSwipeOnViewport,2));
            Vector2Int swipeDirection = Vector2Int.zero;
            if (distanceSwiped.sqrMagnitude > Mathf.Pow(mainCam.pixelWidth * minSwipeOnViewport, 2))
            {
                
                if (Mathf.Abs(distanceSwiped.x) > Mathf.Abs(distanceSwiped.y))
                {
                    swipeDirection.x = Mathf.RoundToInt(Mathf.Sign(distanceSwiped.x));
                }
                else
                {
                    swipeDirection.y = Mathf.RoundToInt(Mathf.Sign(distanceSwiped.y));
                }


            }
            swapDetectionComplete = true;
            swipeStartCoord = tempSwipeStartCord;
            swipeDir = swipeDirection;

        }

        private static Vector2 CalculateViewportPos(Vector2 pixelPos)
        {
            Rect gameplayBoardRect;
            Vector2 gameplayBoardPixelPos = new Vector2(Screen.width, Screen.height) / 2 + _boardHolder.anchoredPosition;
            Vector2 gameplayBoardPixelSize = _boardHolder.rect.size;

            gameplayBoardRect = new Rect(gameplayBoardPixelPos, gameplayBoardPixelSize);
            Vector2 viewportPos = pixelPos - gameplayBoardRect.position;
            viewportPos = new Vector2(viewportPos.x / gameplayBoardRect.width, viewportPos.y / gameplayBoardRect.height) + Vector2.one / 2;
            return viewportPos;
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
