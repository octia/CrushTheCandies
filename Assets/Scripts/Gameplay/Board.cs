using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SlashCandy.Audio;
using Random = UnityEngine.Random;

namespace SlashCandy
{
    public class Board
    {


        public Vector2Int _size;
        protected BoardSlot[,] _objectBoard;

        private GameObject _boardGO;
        private GameObject _baseBoardObjectPrefab;
        private Vector2 _worldSize;
        public const int minLineLength = 3;
        private const float swapTime = 0.4f;

        private BoardObject _toFillMapWith;


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
        public void Initialize(Vector2Int size, BoardObject toFillWith, Vector2 worldSize)
        {
            _size = size;
            _worldSize = worldSize;

            _objectBoard = new BoardSlot[_size.x, _size.y];
            _toFillMapWith = toFillWith;

            int[,] variantWall = new int[_size.x, _size.y];
            for (int x = 0; x < _size.x; x++)
            {
                for (int y = 0; y < _size.x; y++)
                {
                    variantWall[x, y] = _toFillMapWith.RandomVariant();
                }
            }
            int k = 0;

            do
            {
                k++;
                int sameVariantCount;
                int prevVariant;

                // check columns
                for (int x = 0; x < _size.x; x++)
                {
                    sameVariantCount = 1;
                    prevVariant = -1;
                    for (int y = 0; y < _size.y; y++)
                    {
                        if (variantWall[x, y] == prevVariant)
                        {
                            sameVariantCount++;
                        }
                        else
                        {
                            prevVariant = variantWall[x, y];
                            sameVariantCount = 1;
                        }
                        if (sameVariantCount >= minLineLength)
                        {
                            int rand = Random.Range(0, minLineLength);
                            variantWall[x, y - rand] = _toFillMapWith.RandomVariant();
                            x--;
                            break;
                        }
                    }
                }


                // check rows
                for (int y = 0; y < _size.y; y++)
                {
                    sameVariantCount = 1;
                    prevVariant = -1;
                    for (int x = 0; x < _size.x; x++)
                    {
                        if (variantWall[x, y] == prevVariant)
                        {
                            sameVariantCount++;
                        }
                        else
                        {
                            prevVariant = variantWall[x, y];
                            sameVariantCount = 1;
                        }

                        if (sameVariantCount >= minLineLength)
                        {
                            int rand = Random.Range(0, minLineLength);
                            variantWall[x-rand,y] = _toFillMapWith.RandomVariant();
                            y--;
                            break;
                        }
                    }
                }
            } while (CheckRowsForMultiples(variantWall) && k < 10);
            for (int x = 0; x < _size.x; x++)
            {
                for (int y = 0; y < _size.y; y++)
                {
                    CreateNewGOAt(x, y, _toFillMapWith, variantWall[x, y]);

                }
            }

        }

        public HashSet<Vector2Int> CheckForObjToDestroy()
        {
            HashSet<Vector2Int> toReturn = new HashSet<Vector2Int>();
            int[,] variantWall = new int[_size.x, _size.y];
            for (int x = 0; x < _size.x; x++)
            {
                for (int y = 0; y < _size.x; y++)
                {
                    variantWall[x, y] = _objectBoard[x, y].variant;
                }
            }

            int sameVariantCount;
            int prevVariant;

            // check columns
            for (int x = 0; x < _size.x; x++)
            {
                sameVariantCount = 1;
                prevVariant = -1;
                for (int y = 0; y < _size.y; y++)
                {
                    if (variantWall[x, y] == prevVariant)
                    {
                        sameVariantCount++;
                        if (y + 1 == _size.y)
                        {
                            if (sameVariantCount >= minLineLength)
                            {
                                for (int i = 0; i < sameVariantCount; i++)
                                {
                                    toReturn.Add(new Vector2Int(x, y - i));
                                }
                            }
                        }
                    }
                    else
                    {
                        if (sameVariantCount >= minLineLength)
                        {
                            for (int i = 1; i <= sameVariantCount; i++)
                            {
                                toReturn.Add(new Vector2Int(x, y - i));
                            }
                        }
                        prevVariant = variantWall[x, y];
                        sameVariantCount = 1;
                    }
                }

            }


            // check rows
            for (int y = 0; y < _size.y; y++)
            {
                sameVariantCount = 1;
                prevVariant = -1;
                for (int x = 0; x < _size.x; x++)
                {
                    if (variantWall[x, y] == prevVariant)
                    {
                        sameVariantCount++;
                        if (x + 1 == _size.x)
                        {
                            if (sameVariantCount >= minLineLength)
                            {
                                for (int i = 0; i < sameVariantCount; i++)
                                {
                                    toReturn.Add(new Vector2Int(x - i, y));
                                }
                            }
                        }
                    }
                    else
                    {
                        if (sameVariantCount >= minLineLength)
                        {
                            for (int i = 1; i <= sameVariantCount; i++)
                            {
                                toReturn.Add(new Vector2Int(x-i, y));
                            }
                        }
                        prevVariant = variantWall[x, y];
                        sameVariantCount = 1;
                    }

                }
            }



            return toReturn;
        }

        public IEnumerator RefillAndApplyGravity()
        {


            List<BoardSlot> objectsToGravitate = new List<BoardSlot>();

            // Gravity on _candyWall
            for (int x = 0; x < _size.x; x++)
            {
                for (int y = 0; y < _size.y; y++)
                {
                    if (_objectBoard[x, y] == null)
                    {
                        for (int newYoffset = 0; newYoffset < _size.y - y; newYoffset++)
                        {
                            if (_objectBoard[x, y + newYoffset] != null)
                            {
                                objectsToGravitate.Add(_objectBoard[x, y + newYoffset]);
                                SetObjectPositionOnBoard(x, y, y + newYoffset);
                                break;
                            }
                        }
                    }
                }
            }

            // Refill objects
            for (int x = 0; x < _size.x; x++)
            {
                int missingObjectsInColumn = 0;
                for (int y = 0; y < _size.y; y++)
                {
                    if (_objectBoard[x, y] == null)
                    {
                        if (missingObjectsInColumn == 0)
                        {
                            missingObjectsInColumn = (_size.y) - y;
                        }
                        CreateNewGOAt(x, y, _toFillMapWith, _toFillMapWith.RandomVariant());
                        _objectBoard[x, y].candyManager.transform.position = BoardToWorldPos(new Vector2Int(x, y + missingObjectsInColumn));
                        objectsToGravitate.Add(_objectBoard[x, y]);
                    }
                }
            }


            // Gravity on GameObjects
            float maxFallTime = 0;
            foreach (var obj in objectsToGravitate)
            {

                Vector3 targetObjPosition = obj.candyManager.transform.position;
                float fallTime = obj.candyManager.FallToPosition(targetObjPosition, BoardToWorldPos(obj.position));
                obj.candyManager.SetNewBoardPosition(obj.position);
                if (fallTime > maxFallTime)
                {
                    maxFallTime = fallTime;
                }
            }
            yield return new WaitForSeconds(maxFallTime);

        }

        private Vector3 BoardToWorldPos(Vector2Int boardPos)
        {
            // translate position to range between 0 and 1
            Vector2 worldPos = new Vector2(((float)boardPos.x) / (_size.x - 1), ((float)boardPos.y) / (_size.y - 1));

            // scale translated position to worldSpace using worldsSize of the board
            worldPos = new Vector2(Mathf.Lerp(-_worldSize.x / 2, _worldSize.x / 2, worldPos.x), Mathf.LerpUnclamped(-_worldSize.y / 2, _worldSize.y / 2, worldPos.y));
            return worldPos;
        }


        private void SetObjectPositionOnBoard(int targetX, int targetY, int sourceY)
        {
            _objectBoard[targetX, targetY] = _objectBoard[targetX, sourceY];
            _objectBoard[targetX, sourceY] = null;

            _objectBoard[targetX, targetY].position = new Vector2Int(targetX, targetY);
        }

        public bool IsSwapAllowed(Vector2Int obj1, Vector2Int obj2)
        {
            BoardSlot slot1 = _objectBoard[obj1.x, obj1.y];
            BoardSlot slot2 = _objectBoard[obj2.x, obj2.y];
            //Debug.Log(string.Format("Variants {0} {1}", slot1.variant, slot2.variant));
            if (slot1.variant == slot2.variant)
            {
                return false;
            }

            int[,] variantWall = new int[_size.x, _size.y];
            for (int x = 0; x < _size.x; x++)
            {
                for (int y = 0; y < _size.x; y++)
                {
                    variantWall[x, y] = _objectBoard[x, y].variant;
                }
            }
            variantWall[slot1.position.x, slot1.position.y] = slot2.variant;
            variantWall[slot2.position.x, slot2.position.y] = slot1.variant;

            // We can assume there were no triplets before the swap.
            return CheckRowsForMultiples(variantWall);
    
        }

        public IEnumerator PreformSwap(Vector2Int obj1, Vector2Int obj2)
        {
            BoardSlot slot1 = _objectBoard[obj1.x, obj1.y];
            BoardSlot slot2 = _objectBoard[obj2.x, obj2.y];

            SwapGOPositions(slot1, slot2);
            yield return new WaitForSeconds(swapTime);
            _objectBoard[obj1.x, obj1.y] = slot2;
            _objectBoard[obj2.x, obj2.y] = slot1;
            slot1.position = obj2;
            slot2.position = obj1;
            _objectBoard[obj1.x, obj1.y].candyManager.SetNewBoardPosition(obj1);
            _objectBoard[obj2.x, obj2.y].candyManager.SetNewBoardPosition(obj2);

        }

        public IEnumerator PreformMockSwap(Vector2Int obj1, Vector2Int obj2)
        {
            BoardSlot slot1 = _objectBoard[obj1.x, obj1.y];
            BoardSlot slot2 = _objectBoard[obj2.x, obj2.y];

            SwapGOPositions(slot1, slot2);
            yield return new WaitForSeconds(swapTime);
            yield return null; 
            SwapGOPositions(slot1, slot2);
            yield return new WaitForSeconds(swapTime);
        }

        public IEnumerator DestroyObjectAt(Vector2Int pos)
        {
            BoardSlot toDestroy = _objectBoard[pos.x, pos.y];
            yield return toDestroy.boardObject.DestroyObject(toDestroy);
            _objectBoard[pos.x, pos.y] = null;

        }


        // TODO: replace with an animation(?)
        private void SwapGOPositions(BoardSlot obj1, BoardSlot obj2)
        {
            AudioSystem.PlayOnCandySwap();
            Vector3 pos1 = obj1.candyManager.transform.position;
            Vector3 pos2 = obj2.candyManager.transform.position;

            obj1.candyManager.SwapToPosition(pos2, swapTime, 1);
            obj2.candyManager.SwapToPosition(pos1, swapTime, -1);
        }


        // IMPORTANT TODO: Consolidate functions searching for multiples
        private bool CheckRowsForMultiples(int[,] variants)
        {
            int sameVariantCount;
            int prevVariant;
            if (variants == null)
            {
                variants = new int[_size.x, _size.y];
                for (int x = 0; x < _size.x; x++)
                {
                    for (int y = 0; y < _size.x; y++)
                    {
                        variants[x, y] = _objectBoard[x, y].variant;
                    }
                }
            }

            // check columns
            for (int x = 0; x < _size.x; x++)
            {
                sameVariantCount = 1;
                prevVariant = -1;
                for (int y = 0; y < _size.y; y++)
                {
                    if (variants[x,y] == prevVariant)
                    {
                        sameVariantCount++;
                    }
                    else
                    {
                        prevVariant = variants[x, y];
                        sameVariantCount = 1;
                    }
                    if (sameVariantCount >= minLineLength)
                    {
                        return true;
                    }
                }
            }


            // check rows
            for (int y = 0; y < _size.y; y++)
            {
                sameVariantCount = 1;
                prevVariant = -1;
                for (int x = 0; x < _size.x; x++)
                {
                    if (variants[x, y] == prevVariant)
                    {
                        sameVariantCount++;
                    }
                    else
                    {
                        prevVariant = variants[x, y];
                        sameVariantCount = 1;
                    }

                    if (sameVariantCount >= minLineLength)
                    {
                        return true;
                    }
                }
            }

            // no variant series found
            return false;
        }

       

        private void CreateNewGOAt(int x, int y, BoardObject toFillWith, int variant)
        {
            GameObject instantinated = GameObject.Instantiate(_baseBoardObjectPrefab, _boardGO.transform);

            _objectBoard[x, y] = new BoardSlot(instantinated.GetComponent<CandyManager>());

            // Create the candy 
            // TODO: Read slot data from a scriptableobject(?)
            _objectBoard[x, y].Init(this, new Vector2Int(x, y), toFillWith, variant);


            // set positions between 0 and 1

            Vector3 worldPos = BoardToWorldPos(new Vector2Int(x, y));
            instantinated.transform.position = worldPos;


        }


        private bool ValidateBoardPos(Vector2Int pos)
        {
            if (pos.x >= 0 && pos.x < _size.x && pos.y >= 0 && pos.y < _size.y)
            {
                return true;
            }
            else
            {
                return false;
            }
        }



    }

}

