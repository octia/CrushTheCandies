using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlashCandy
{
    [CreateAssetMenu(menuName = "BoardObjects/Wall")]
    public class Wall : BoardObject
    {
        

        public override void OnDestroyed(BoardSlot boardSlot)
        {
            Debug.LogError("Wall cannot be destroyed during gameplay!");
        }
        protected override int GetVariantCount() => 1;


    }
}