using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlashCandy
{
    public abstract class BoardObject : ScriptableObject
    {
        public bool isSwappable;
        public bool isDestroyable;

        [SerializeField]
        protected GameObject basePrefab;

        [HideInInspector]
        public int variantCount => GetVariantCount();

        protected abstract int GetVariantCount();

        /// <summary>
        /// Call this method before destroying/reusing the BoardSlot for a different object.
        /// </summary>
        /// <param name="boardSlot">The boardSlot that is being destroyed.</param>
        /// <returns>Finishes when all animations/sounds/etc finish.</returns>
        public virtual IEnumerator DestroyObject(BoardSlot boardSlot)
        {
            yield break;
        }


        /// <summary>
        /// Set up a GameObject to fit the type of BoardObject
        /// </summary>
        public virtual void SetupGameObject(BoardSlot boardSlot)
        {

            boardSlot.candyManager.SetupWithPrefab(basePrefab);
            
        }

        public virtual int RandomVariant()
        {
            int result = Random.Range(0, variantCount);
            
            return result;
        }




    }

}
