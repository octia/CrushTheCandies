using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlashCandy
{
    public class BoardSlot
    {

        private CandyManager _candyManager;

        private BoardObject _boardObj;
        private Vector2Int _position;
        private int _variant;
        private Board _board;

        public CandyManager candyManager { get => _candyManager; }

        public BoardObject boardObject { get => _boardObj; }

        public int variant { get => _variant; }
        
        public Vector2Int position { get => _position; set => _position = value; }




        public BoardSlot(CandyManager candyManager)
        {
            _candyManager = candyManager;
        }



        public BoardSlot Init(Board board, Vector2Int pos, BoardObject boardObj, int variant)
        {
            _board = board;
            _position = pos;
            _boardObj = boardObj;
            _variant = variant;
            _boardObj.SetupGameObject(this);
            return this;
        }




    }
}