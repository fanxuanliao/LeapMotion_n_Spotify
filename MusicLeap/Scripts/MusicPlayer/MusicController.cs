using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;

namespace MusicLeap {

    [RequireComponent(typeof(MusicPlayer))]
    [RequireComponent(typeof(MusicPlaylist))]
    public class MusicController : MonoBehaviour {

        MusicPlayer player;
        MusicPlaylist playlist;

        public GameObject uiCanvas;
        Text uiTitleText;
        Text uiIdxText;
        Text uiShuffleText;
        Text uiRepeatText;
        UiHover uiShuffleHover;
        UiHover uiRepeatHover;
        Slider uiVolumeSlider;
        GameObject uiConfirmQuit;

        bool isQuiting = false;

        public float volume {
            get {
                return player.volume;
            }
        }

        void Start() {
            player   = GetComponent<MusicPlayer>();
            playlist = GetComponent<MusicPlaylist>();

            uiTitleText    = uiCanvas.transform.Find("Title").GetComponent<Text>();
            uiIdxText      = uiCanvas.transform.Find("Idx").GetComponent<Text>();
            uiShuffleText  = uiCanvas.transform.Find("Shuffle").GetComponent<Text>();
            uiRepeatText   = uiCanvas.transform.Find("Repeat").GetComponent<Text>();
            uiShuffleHover = uiCanvas.transform.Find("Shuffle").GetComponent<UiHover>();
            uiRepeatHover  = uiCanvas.transform.Find("Repeat").GetComponent<UiHover>();
            uiVolumeSlider = uiCanvas.transform.Find("Volume").GetComponent<Slider>();
            uiConfirmQuit  = uiCanvas.transform.Find("ConfirmQuit").gameObject;
        }

        void Update() {
            _UpdateUi();
        }

        // Send info to UI
        void _UpdateUi() {
            if ( !player.isStoping ) {
                uiTitleText.text = player.clipName;
                uiIdxText.text = playlist.idxName;
            } else {
                uiTitleText.text = "";
                uiIdxText.text = "";
            }

            if (playlist.isShuffle) {
                uiShuffleHover.Hover();
            } else {
                uiShuffleHover.UnHover();
            }
            uiShuffleText.text = playlist.shuffleName;

            if (playlist.isRepeat) {
                uiRepeatHover.Hover();
            } else {
                uiRepeatHover.UnHover();
            }
            uiRepeatText.text = playlist.repeatName;

            uiVolumeSlider.value = player.volume;
        }

        public void Up() {
            if (!uiConfirmQuit.activeInHierarchy) {
                Pause();
                Quit();
            } else {
                Stop();
                ConfirmQuit();
            }
        }

        public void Down() {
            if (!uiConfirmQuit.activeInHierarchy) {
                PlayPause();
            } else {
                CancelQuit();
            }
        }

        public void PlayPause() {
            if ( player.isPlaying ) {
                Pause();
            } else {
                Play();
            }
        }

        public void Play() {
            Debug.Log("MusicController: Play!");
            if ( player.isStoping ) {
                player._Play();
            } else {
                player._UnPause();
            }
        }

        public void Stop() {
            Debug.Log("MusicController: Stop!");
            player._Stop();
            player._Clear();
        }

        public void Pause() {
            Debug.Log("MusicController: Pause!");
            player._Pause();
        }

        public void Next() {
            Debug.Log("MusicController: Next!");
            player._NextPrev(true);
        }

        public void Prev() {
            Debug.Log("MusicController: Prev!");
            player._NextPrev(false);
        }

        public void ToggleShuffle() {
            playlist.ToggleShuffle();
            Debug.Log($"MusicPlayer: {playlist.shuffleName}!");
        }

        public void ToggleRepeat() {
            playlist.ToggleRepeat();
            Debug.Log($"MusicPlayer: {playlist.repeatName}!");
        }

        public void SetVolume(float val) {
            player.volume = Mathf.Clamp(val, 0f, 1f);
            Debug.Log($"MusicPlayer: Set Volume {player.volume}!");
        }

        public void Quit() {
            uiConfirmQuit.SetActive(true);
        }

        public void CancelQuit() {
            uiConfirmQuit.SetActive(false);
        }

        public void ConfirmQuit() {
            Debug.Log($"MusicPlayer: Quit!");
            uiConfirmQuit.SetActive(false);
            Application.Quit();
        }
    }
}
