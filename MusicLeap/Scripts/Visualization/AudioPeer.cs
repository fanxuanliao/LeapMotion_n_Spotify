using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MusicLeap {

    public class AudioPeer : MonoBehaviour {

        [Range(6, 13)]
        public int scale = 8;

        [HideInInspector] public int numSample;

        public float[] samples;

        public AudioSource audioSource;

        void Awake() {
            numSample = 1 << scale;
            samples = new float[numSample];
        }

        void Update() {
            GetSpectrum();
        }

        void GetSpectrum() {
            audioSource.GetSpectrumData(samples, 0, FFTWindow.Blackman);
        }
    }
}
