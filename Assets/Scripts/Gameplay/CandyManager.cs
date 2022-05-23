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
        private Vector2Int _position = Vector2Int.one * -1;

        private Coroutine _changePosCoroutine;


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
            SetNewName();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newPos"></param>
        /// <param name="swapTime">Time </param>
        /// <param name="rotateDir">While rotating, go towards the camera, or away from it?</param>
        /// <returns>Time that falling will take</returns>
        public float FallToPosition(Vector3 startPos, Vector3 endPos)
        {
            float fallLength = Vector3.Distance(startPos, endPos);
            float gravityPull = Physics.gravity.y;
            float fallTime = 0.1f * (startPos - endPos).magnitude;//Mathf.Sqrt(fallLength / gravityPull); // S = at^2 => t = sqrt(S/a)
            _changePosCoroutine = StartCoroutine(FallToPos(startPos, endPos, fallTime));

            return fallTime;
        }


        public void SwapToPosition(Vector3 newPos, float time, int rotateDir)
        {
            _changePosCoroutine = StartCoroutine(RotateToPos(newPos, time, rotateDir));
        }

        public void SetNewBoardPosition(Vector2Int newBoardPos)
        {
            _position = newBoardPos;
            SetNewName();
        }

        public IEnumerator PlayDestroyAnimation()
        {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.AddForce(new Vector3(0, 0, -2), ForceMode.VelocityChange);
            Destroy(gameObject, 5);
            yield return new WaitForSeconds(1);
        }


        private void SetNewName()
        {
            gameObject.name = string.Format("Candy ({0},{1})", _position.x, _position.y);
        }


        /// <summary>
        /// Swap position of gameObject, from current to newPos, on a half-circular path.
        /// </summary>
        /// <param name="newPos">Target position in world coordinates. (TODO: change to local?)</param>
        /// <param name="swapTime">Time of the swap.</param>
        /// <param name="rotateDir">Direction of the swap (non-negative starts moving towards camera, negative starts moving away.) Only the sign of the number is taken into account.</param>
        /// <returns></returns>
        private IEnumerator RotateToPos(Vector3 newPos, float swapTime, int rotateDir)
        {
            Vector3 oldPos = transform.position;
            Vector3 midPos = (oldPos + newPos) / 2;
            if (rotateDir == 0)
            {
                rotateDir = 1;
            }
            rotateDir /= Mathf.Abs(rotateDir); // Change rotateDir to either 1 or -1 
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
                transform.RotateAround(midPos, Vector3.Cross(newPos - oldPos, Vector3.forward), rotateDir * delta * 180f / swapTime);


            }
            while (timer < swapTime);


        }

        /// <summary>
        /// Show gameobject falling from startPos to endPos, with set acceleration. 
        /// TODO: When adding immovable objects, change startpos and endpos to a vector3 array.
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <param name="acceleration"></param>
        /// <returns></returns>
        private IEnumerator FallToPos(Vector3 startPos, Vector3 endPos, float travelTime)
        {
            float traveledTime = 0;
            Vector3 travelledDistance = Vector3.zero;
            Vector3 totalDistToTravel = endPos - startPos;
            transform.position = startPos;
            while (traveledTime < travelTime)
            {
                transform.position = Vector3.Lerp(startPos, endPos, traveledTime/travelTime);
                traveledTime += Time.deltaTime;
                yield return null;
                //Debug.Log("StartPos: " + startPos + " Endpos: " + endPos);
                //yield return null;
                //speed += acceleration * Time.deltaTime;
                
                //travelledDistance = startPos - Vector3.MoveTowards(transform.position, endPos, speed);
                //transform.position = startPos + travelledDistance;
                //if (travelledDistance.sqrMagnitude > totalDistToTravel.sqrMagnitude) 
                //{
                //    transform.position = new Vector3(transform.position.x, endPos.y, transform.position.z);
                //    break;
                //}
            }
            transform.position = endPos;
        }
    }
}