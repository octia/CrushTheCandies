using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlashCandy.UI
{
    [ExecuteAlways]
    public class CanvasSizeAdjuster : MonoBehaviour
    {
        RectTransform _rt;
        // Start is called before the first frame update
        void Start()
        {
            _rt = GetComponent<RectTransform>();
            _rt.sizeDelta = new Vector2(0, _rt.rect.width);
        }

#if UNITY_EDITOR
        private void Update()
        {
            _rt.sizeDelta = new Vector2(0, _rt.rect.width);
        }
#endif
    }

}
