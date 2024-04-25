using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Leap;
using Leap.Unity;

namespace MusicLeap {

    public class VolumeSlicer : MonoBehaviour {

        public Detector detector;

        [Tooltip("The hand model to watch. Set automatically if detector is on a hand.")]
        public HandModelBase handModel = null;

        public MusicController controller;
        public UiHover sliderHandle;

        public float scale = 1f;

        bool active = false;
        float currHeight;
        float currVolume;

        private void Awake(){
          detector.OnActivate.RemoveListener(Activate); //avoid double subscription
          detector.OnActivate.AddListener(Activate);
          detector.OnDeactivate.RemoveListener(Deactivate); //avoid double subscription
          detector.OnDeactivate.AddListener(Deactivate);
        }

        void Update() {
            if ( active ) {
                SetVolume();
            }
        }

        // Activate when hand pitch
        private void Activate() {
            active = true;
            sliderHandle.Hover();
            currHeight = GetHeight();
            currVolume = controller.volume;
        }

        private void Deactivate() {
            active = false;
            sliderHandle.UnHover();
        }

        private void SetVolume() {
            float height = GetHeight();
            float volume = (height - currHeight) * scale + currVolume;
            controller.SetVolume(volume);
        }

        private float GetHeight() {
            Hand hand;
            if(handModel != null){
              hand = handModel.GetLeapHand();
              if(hand != null){
                return hand.PalmPosition.y;
              }
            }
            return 0;
        }
    }
}
