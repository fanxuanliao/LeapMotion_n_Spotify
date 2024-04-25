using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace MusicLeap {

    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(MusicPlaylist))]
    public class MusicPlayer : MonoBehaviour {

        AudioSource audioSource;
        MusicPlaylist playlist;

        [HideInInspector] public bool isPlaying = false;
        public bool isStoping {
            get {
                return audioSource.clip == null;
            }
        }

        public float volume {
            get {
                return audioSource.volume;
            }

            set {
                audioSource.volume = value;
            }
        }

        public string clipName {
            get {
                return audioSource.clip.name;
            }
        }

        void Start() {
            audioSource = GetComponent<AudioSource>();
            playlist = GetComponent<MusicPlaylist>();
        }

        void Update() {
            _AutoPlay();
        }

        // Auto play next track
        void _AutoPlay() {
            if ( isPlaying && !audioSource.isPlaying) {
                _NextPrev(true);
            }
        }

        public void _Play() {
            audioSource.clip = playlist.GetClip();
            audioSource.Stop();
            audioSource.Play();
            isPlaying = true;
            Debug.Log("Now playing ... " + clipName);
        }

        public void _Stop() {
            audioSource.Stop();
            isPlaying = false;
        }

        public void _UnPause() {
            audioSource.UnPause();
            isPlaying = true;
            Debug.Log("Now playing ... " + clipName);
        }

        public void _Pause() {
            audioSource.Pause();
            isPlaying = false;
        }

        public void _Clear() {
            audioSource.clip = null;
            playlist.Clear();
        }

        public void _NextPrev(bool next) {
            _Stop();
            bool status = playlist.NextPrev(next);
            if (status) { // Track is available
                _Play();
            } else { // No more track
                _Clear();
            }
        }
    }
}
