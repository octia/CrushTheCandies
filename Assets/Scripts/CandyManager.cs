using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace SlashCandy
{
    public class CandyManager : MonoBehaviour
    {

        GameObject _instantinatedPrefab;
        public Vector2Int BoardPosition
        {
            get { return _position; }
        }
        private Vector2Int _position;

        private Coroutine _swapPosCoroutine;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public GameObject SetupWithPrefab(GameObject prefab)
        {
            _instantinatedPrefab = Instantiate(prefab, transform);
            _instantinatedPrefab.transform.localPosition = Vector3.zero;
            return _instantinatedPrefab;
        }

        public void SetCandyData(StandardCandy.CandyData candyData, Vector2Int pos)
        {
            _position = pos;
            _instantinatedPrefab.GetComponent<MeshFilter>().mesh = candyData.mesh;

            _instantinatedPrefab.GetComponent<MeshRenderer>().material.color = candyData.color;
        }


        public void SwapToPosition(Vector3 newPos, float time, int rotateDir)
        {
            _swapPosCoroutine = StartCoroutine(RotateToPos(newPos, time, rotateDir));
        }


        private IEnumerator RotateToPos(Vector3 newPos, float swapTime, int rotateDir)
        {
            Vector3 oldPos = transform.position;
            Vector3 midPos = (oldPos + newPos) / 2;
            float timer = 0;
            do
            {
                yield return null;
                float delta = Time.deltaTime;
                if (timer + delta > swapTime)
                {
                    delta = swapTime - timer;
                }
                timer += delta;
                transform.RotateAround(midPos, Vector3.Cross(newPos- oldPos, Vector3.forward), rotateDir * delta * 180f / swapTime);
                

            }
            while (timer < swapTime);


        }

    }
}