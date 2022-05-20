using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

namespace SlashCandy
{
    public class Board
    {
        /// <summary>
        /// Board Singleton.
        /// </summary>
        public static Board Instance
        {
            get;
            private set;
        }

        public Vector2Int _size;
        public BoardSlot[,] _candyWall;

        private GameObject _boardGO;
        private GameObject _baseBoardObjectPrefab;

        private const float swapTime = 0.4f;

        public Board(GameObject boardGO, GameObject baseBoardObjectPrefab)
        {
            _boardGO = boardGO;
            _baseBoardObjectPrefab = baseBoardObjectPrefab;
        }




        /// <summary>
        /// Initialize the map, and instantinate the board objects in world space.
        /// </summary>
        /// <param name="size">Size of the map. to be replaced with Vector2Int when scriptable board layout system is created.</param>
        /// <param name="toFillWith">The object type to fill the board with, to be replaced when scriptable board layout system is created.</param>
        /// <param name="_boardGO"></param>
        public void Initialize(int size, BoardObject toFillWith, float worldSize)
        {
            _size = new Vector2Int(size, size);
            if (Instance != null)
            {
                Debug.LogError("Board instance already exists!");
                return;
            }
            Instance = this;

            _candyWall = new BoardSlot[_size.x, _size.y];
            for (int x = 0; x < _size.x; x++)
            {
                for (int y = 0; y < _size.y; y++)
                {

                    GameObject instantinated = GameObject.Instantiate(_baseBoardObjectPrefab, _boardGO.transform);

                    _candyWall[x, y] = new BoardSlot(instantinated.GetComponent<CandyManager>());
                    
                    // Create the candy 
                    // TODO: Read slot data from a scriptableobject(?)
                    _candyWall[x, y].Init(this, new Vector2Int(x, y), toFillWith, toFillWith.RandomVariant());


                    // set positions between 0 and 1
                    Vector2 worldPos = new Vector2(((float)x) / (_size.x-1), ((float)y) / (_size.y-1));

                    // set and apply world position
                    worldPos = new Vector2(Mathf.Lerp(-worldSize / 2, worldSize / 2, worldPos.x), Mathf.Lerp(-worldSize / 2, worldSize / 2, worldPos.y));
                    instantinated.transform.position = worldPos;

                    instantinated.name = string.Format("Candy ({0},{1})", x, y);

                }
            }
        }

        public bool IsSwapAllowed(Vector2Int obj1, Vector2Int obj2)
        {

            BoardSlot slot1 = _candyWall[obj1.x, obj1.y];
            BoardSlot slot2 = _candyWall[obj2.x, obj2.y];

            bool slot1Swappable = IsRowOfTwoNearby(obj2.x, obj2.y, slot1.variant);
            bool slot2Swappable = IsRowOfTwoNearby(obj1.x, obj1.y, slot2.variant);



            return false;

        }

        public IEnumerator PreformSwap(Vector2Int obj1, Vector2Int obj2)
        {
            BoardSlot slot1 = _candyWall[obj1.x, obj1.y];
            BoardSlot slot2 = _candyWall[obj2.x, obj2.y];

            SwapPositions(slot1, slot2);
            yield return new WaitForSeconds(swapTime);


        }

        public IEnumerator PreformMockSwap(Vector2Int obj1, Vector2Int obj2)
        {
            BoardSlot slot1 = _candyWall[obj1.x, obj1.y];
            BoardSlot slot2 = _candyWall[obj2.x, obj2.y];

            SwapPositions(slot1, slot2);
            yield return new WaitForSeconds(swapTime);
            yield return null; 
            SwapPositions(slot1, slot2);
            yield return new WaitForSeconds(swapTime);
        }

        // TODO: replace with an animation(?)
        private void SwapPositions(BoardSlot obj1, BoardSlot obj2)
        {
            Vector3 pos1 = obj1.candyManager.transform.position;
            Vector3 pos2 = obj2.candyManager.transform.position;

            obj1.candyManager.SwapToPosition(pos2, swapTime, 1);
            obj2.candyManager.SwapToPosition(pos1, swapTime, -1);
        }


        private bool IsRowOfTwoNearby(int x, int y, int variant)
        {
            return false;


        }




    }

}

