using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace MusicLeap {

    [RequireComponent(typeof(MusicLoader))]
    public class MusicPlaylist : MonoBehaviour {

        MusicLoader loader;

        int idx;
        int[] clipIdxs;  // Array of clip indices
        public string idxName {
            get {
                return $"{idx+1} / {clipIdxs.Length}";
            }
        }

        bool _shuffle = false;
        public bool isShuffle {
            get {
                return _shuffle;
            }
            private set {
                _shuffle = value;
            }
        }
        public string shuffleName {
            get {
                return isShuffle ? "Shuffle On" : "Shuffle Off";
            }
        }

        bool _repeat = false;
        public bool isRepeat {
            get {
                return _repeat;
            }
            private set {
                _repeat = value;
            }
        }
        public string repeatName {
            get {
                return isRepeat ? "Repeat On" : "Repeat Off";
            }
        }

        string[] repeatNames = {
            "Shuffle Off",
            "Shuffle On",
        };

        void Start() {
            loader = GetComponent<MusicLoader>();
        }

        public void ToggleShuffle() {
            isShuffle = !isShuffle;

            if (clipIdxs != null) {
                int oldIdx = clipIdxs[idx];
                ReFill();
                idx = (!isShuffle) ? oldIdx : System.Array.IndexOf(clipIdxs, oldIdx);
            }
        }

        public void ToggleRepeat() {
            isRepeat = !isRepeat;
        }

        // Get a track
        public AudioClip GetClip() {
            if (clipIdxs == null) {
                ReFill();
            }
            return loader.clips[clipIdxs[idx]];
        }

        // Get next/previous track
        // Return true if track available
        public bool NextPrev(bool next) {
            if (clipIdxs == null) {
                ReFill(next);
                return true;
            }
            int n = clipIdxs.Length;

            if (next) {
                idx++;
            } else {
                idx--;
            }

            // Reach end points
            if (idx < 0 || idx >= n) {
                if ( isRepeat ) {
                    ReFill(next);
                } else {
                    return false;
                }
            }
            return true;
        }

        public void Clear() {
            clipIdxs = null;
        }

        public void ReFill(bool next=true) {
            Debug.Log("Re-fill playlist ...");

            // Set playlist in increasing order
            int n = loader.clips.Count;
            clipIdxs = new int[n];
            for (int i = 0; i < n; i++) {
                clipIdxs[i] = i;
            }

            // Shuffle playlist
            if ( isShuffle ) {
                for (int i = 0; i < n; i++) {
                    int r = i + (int)(Random.Range(0, n-i));
                    int value = clipIdxs[r];
                    clipIdxs[r] = clipIdxs[i];
                    clipIdxs[i] = value;
                }
            }

            // Set idx for Next()/Prev()
            if (next) {
                idx = 0;
            } else {
                idx = n-1;
            }
        }

    }
}
