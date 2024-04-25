using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MusicLeap {

    public class UiHover : MonoBehaviour {

        Vector3 defaultScale;
        Vector3 hoverScale;

        void Start() {
            defaultScale = transform.localScale;
            hoverScale = defaultScale * 2;
        }

        public void Hover() {
            transform.localScale = hoverScale;
        }

        public void UnHover() {
            transform.localScale = defaultScale;
        }
    }
}
