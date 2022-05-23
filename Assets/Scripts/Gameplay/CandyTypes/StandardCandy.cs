using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SlashCandy
{
    [CreateAssetMenu(menuName = "BoardObjects/StandardCandy")]
    public class StandardCandy : BoardObject
    {
        [Serializable]
        public struct CandyData
        {
            public Mesh mesh;
            public Color color;

        }
        [SerializeField]
        private List<CandyData> possibleCandies;



        protected override int GetVariantCount() => possibleCandies.Count;
        public override IEnumerator DestroyObject(BoardSlot boardSlot)
        {
            yield return boardSlot.candyManager.StartCoroutine(boardSlot.candyManager.PlayDestroyAnimation());
        }
        
        public override void SetupGameObject(BoardSlot boardSlot)
        {
            base.SetupGameObject(boardSlot);
            int variant = boardSlot.variant;

            if (variant < 0 || variant >= possibleCandies.Count)
            {
                Debug.LogError(string.Format("Incorrect variant: {0} at position ({1},{2}).", variant, boardSlot.position.x, boardSlot.position.y));
                return;
            }

            boardSlot.candyManager.SetCandyData(possibleCandies[variant], boardSlot.position);
            
        }



    }
}