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
        /// Call this method before destroying the GameObject
        /// </summary>
        public abstract void OnDestroyed(BoardSlot boardSlot);

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
