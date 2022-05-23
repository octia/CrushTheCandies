using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
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
        private Vector2Int mapSize;

        [SerializeField]
        private Vector2 worldSize;

        [SerializeField]
        private BoardObject normalCandy;

        [SerializeField]
        private RectTransform _gameplayRect;

        [SerializeField]
        private Camera gameplayCamera;
        [SerializeField]
        private Camera mainCamera;

        [SerializeField]
        private TMPro.TMP_Text scoreContainer; // todo: move to some sort of ScoreManager class

        private int score = 0; // todo: move to some sort of ScoreManager class

        // Start is called before the first frame update
        void Start()
        {
            SwapDetector.Init(Vector2.one * worldSize, Vector2Int.one * mapSize, _boardGO, _gameplayRect,gameplayCamera, mainCamera);
            StartCoroutine(GameLoop());
        }

        IEnumerator GameLoop()
        {
            yield return null;

            yield return StartCoroutine(InitializeBoard());
            int automaticActionComboCount = 0;
            _continueGame = true;
            while (_continueGame)
            {
                //EnsurePossibleMoves(); // TODO
                HashSet<Vector2Int> multipledObjectsSet = CheckAutomaticActions();

                if (multipledObjectsSet.Count > 0)
                {
                    automaticActionComboCount++;
                    // IncrementAndUpdateScore();
                    score += multipledObjectsSet.Count;
                    scoreContainer.text = score.ToString();
                    yield return StartCoroutine(PreformAutomaticActions(multipledObjectsSet));
                }
                else
                {
                    yield return DisplayReaction(automaticActionComboCount);
                    automaticActionComboCount = 0;
                    yield return StartCoroutine(HumanMove());
                }


                yield return StartCoroutine(RefillObjectsAndApplyGravity());



            }


        }


        private HashSet<Vector2Int> CheckAutomaticActions()
        {
            return _board.CheckForObjToDestroy();
        }

        IEnumerator InitializeBoard()
        {
            _board = new Board(_boardGO, _boardObjectPrefab);

            _board.Initialize(mapSize, normalCandy, worldSize);

            yield return null;
        }

        IEnumerator RefillObjectsAndApplyGravity()
        {
            yield return StartCoroutine(_board.RefillAndApplyGravity());
        }

        IEnumerator PreformAutomaticActions(HashSet<Vector2Int> objectsToDestroy)
        {
            List<Coroutine> destructionCoroutines = new List<Coroutine>(objectsToDestroy.Count);

            // Destroy all objects in parallel, then wait for them all to finish.
            foreach (var item in objectsToDestroy)
            {
                destructionCoroutines.Add(StartCoroutine(_board.DestroyObjectAt(item)));
            }
            for (int i = 0; i < destructionCoroutines.Count; i++)
            {
                yield return destructionCoroutines[i];
            }


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
                            if (object2.x < 0 || object2.x >= mapSize.x || object2.y < 0 || object2.y >= mapSize.y)
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
        


        private IEnumerator DisplayReaction(int automaticActionCount)
        {
            yield break; 
        }




    }








}
