using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlashCandy
{
    public class GameLoopManager : MonoBehaviour
    {
        private bool _continueGame;
        private bool _pauseGameplay = false;

        private Board _board;


        [SerializeField] 
        private GameObject _boardGO;

        [SerializeField] 
        private GameObject _boardObjectPrefab;
        
        [SerializeField] 
        private int mapSize;

        [SerializeField]
        private float worldSize;

        [SerializeField]
        private BoardObject normalCandy;

        [SerializeField]
        private RectTransform _gameplayRect;

        // Start is called before the first frame update
        void Start()
        {
            SwapDetector.Init(Vector2.one * worldSize, Vector2Int.one * mapSize, _boardGO, _gameplayRect);
            StartCoroutine(GameLoop());
        }

        IEnumerator GameLoop()
        {
            yield return null;

            yield return StartCoroutine(InitializeBoard());
            int automaticActionCount = 0;
            _continueGame = true;
            while (_continueGame)
            {
                EnsurePossibleMoves();

                yield return StartCoroutine(ApplyGravity());
                bool automaticActionsAvaliable = CheckAutomaticActions();
                if (automaticActionsAvaliable)
                {
                    automaticActionCount++;
                    yield return StartCoroutine(PreformAutomaticActions());
                }
                else
                {
                    automaticActionCount = 0;
                    yield return StartCoroutine(HumanMove());
                }

                DisplayReaction(automaticActionCount);

            }


        }

        private void EnsurePossibleMoves()
        {

        }
        bool CheckAutomaticActions()
        {

            return false;
        }

        IEnumerator InitializeBoard()
        {
            _board = new Board(_boardGO, _boardObjectPrefab);

            _board.Initialize(mapSize, normalCandy, worldSize);



            yield return null;
        }
        IEnumerator ApplyGravity()
        {

            yield return null;
        }
        IEnumerator PreformAutomaticActions()
        {

            yield return null;
        }
        IEnumerator HumanMove()
        {

            int prevTouchCount = 0;
            while (true)
            {
                if (!_pauseGameplay)
                {
                    if (Input.GetMouseButtonDown(0) || (Input.touchCount == 1 && Input.touchCount != prevTouchCount))
                    {
                        prevTouchCount = Input.touchCount;

                        Coroutine swapDetection = StartCoroutine(SwapDetector.DetectSwap());

                        while (!_pauseGameplay && !SwapDetector.swapDetectionComplete)
                        {
                            yield return null;
                        }

                        if (_pauseGameplay)
                        {
                            StopCoroutine(swapDetection);
                            
                            break;
                        }

                        if (SwapDetector.swapDetectionComplete)
                        {
                            Vector2Int object1 = SwapDetector.swipeStartCoord;
                            Vector2Int object2 = SwapDetector.swipeStartCoord + SwapDetector.swipeDir;
                            if (object2.x < 0 || object2.x >= mapSize || object2.y < 0 || object2.y >= mapSize)
                            {
                                continue;
                            }

                            if (SwapDetector.swipeDir == Vector2Int.zero)
                            {
                                continue;
                            }
                            else
                            {
                                if (_board.IsSwapAllowed(object1, object2))
                                {
                                    yield return StartCoroutine(_board.PreformSwap(object1, object2));
                                    break;
                                }
                                else
                                {
                                    yield return StartCoroutine(_board.PreformMockSwap(object1, object2));
                                    continue;
                                }
                            }
                        }

                    }
                }
                yield return null;
            }


            
        }
        


        private void DisplayReaction(int automaticActionCount)
        {

        }




    }








}
