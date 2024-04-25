using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MusicLeap {

    [RequireComponent(typeof(AudioPeer))]
    public class SpectrumWave : MonoBehaviour {

        public MusicPlayer player;

        AudioPeer audioPeer;
        int numBar;
        int numBarTotal;

        public GameObject barPrefab;
        GameObject[] bars;

        float[] curValues;  // Current value from audioPeer
        float[] barValues;  // Current height of bar
        float[] barSpeeds;  // Current speed of bar

        float radius = 5;
        float maxScale = 10;
        float maxVolume = 10f;
        float minVolume = 0.001f;
        int multiple = 2;
        int divisor = 2;

        float dw;

        // Iniialize bar's gameObjects
        void Start() {
            audioPeer = GetComponent<AudioPeer>();
            numBar = audioPeer.numSample / divisor;
            numBarTotal = numBar * multiple;
            bars = new GameObject[numBarTotal];

            curValues = new float[numBar];
            barValues = new float[numBar];
            barSpeeds = new float[numBar];

            float da = 2 * Mathf.PI / numBarTotal;
            dw = radius * da;

            for (int i = 0; i < numBarTotal; i++) {

                float a = i * da;
                float x = Mathf.Sin(a) * radius;
                float z = Mathf.Cos(a) * radius;

                GameObject bar = (GameObject) Instantiate(barPrefab);
                bar.name = "SampleCube" + i;
                bar.transform.parent = this.transform;
                bar.transform.localPosition = new Vector3(-x, 0, z);
                bar.transform.localScale = new Vector3(dw, normValue(), dw);
                bar.transform.localRotation = Quaternion.LookRotation(bar.transform.forward, transform.up);
                bars[i] = bar;
            }
        }

        void Update() {
            if (player.isPlaying) {
                transform.Rotate(0, 1, 0);
                GetFromAudio();
                ApplySpeed();
                UpdateBars();
            }
            if (player.isStoping) {
                Reset();
            }
        }

        // Stop rotating and reset bar height to zero
        void Reset() {
            for (int i = 0; i < numBarTotal; i++) {
                bars[i].transform.localScale = new Vector3(dw, normValue(), dw);
            }
            for (int i = 0; i < numBar; i++) {
                curValues[i] = 0;
                barValues[i] = 0;
                barSpeeds[i] = 0;
            }
        }

        // Gaussion kernel
        float[] kernel = {
            102f / 684,
            95f  / 684,
            77f  / 684,
            55f  / 684,
            34f  / 684,
            18f  / 684,
            8f   / 684,
            3f   / 684,
            1f   / 684,
        };

        // Get frequency data from audioPeer and apply Guassion convolution
        void GetFromAudio() {
            for (int i = 0; i < numBar; i++) {
                curValues[i] = 0;
                for(int j = 0; j < 8; j++) {
                    curValues[i] +=
                        Mathf.Sqrt(audioPeer.samples[mod(i+j, numBar)]) * kernel[j];
                }
                for(int j = 1; j < 8; j++) {
                    curValues[i] +=
                        Mathf.Sqrt(audioPeer.samples[mod(i+j, numBar)]) * kernel[j];
                }
            }
        }

        // Avoid glitching
        void ApplySpeed() {
            float speed0    = 0.0005f;  // Initial speed
            float speedRate = 1.2f;     // Speed increasement factor
            float elastic   = 0.005f;   // Elastic factor

            // Apply speed
            for (int i = 0; i < numBar; i++) {
                if (curValues[i] > barValues[i]) {
                    barValues[i] = curValues[i];
                    barSpeeds[i] = speed0;
                } else {
                    if ( barSpeeds[i] < 0 ) {
                        barSpeeds[i] = speed0;
                    }
                    barValues[i] -= barSpeeds[i];
                    barSpeeds[i] *= speedRate;
                }
            }

            // Apply elastic
            for (int i = 0; i < numBar; i++) {
                barSpeeds[i] -= (barValues[mod(i+1, numBar)] - barValues[i]) * elastic;
                barSpeeds[i] -= (barValues[mod(i-1, numBar)] - barValues[i]) * elastic;
            }

        }

        // Set bar heights to bar's gameObject
        void UpdateBars() {
            for (int i = 0; i < numBarTotal; i++) {
                bars[i].transform.localScale =
                    new Vector3(dw, normValue(barValues[i % numBar]), dw);
            }
        }

        float normValue(float value = 0) {
            return Mathf.Clamp(value, minVolume, maxVolume) * maxScale;
        }

        int mod(int x, int m) {
            return (x % m + m) % m;
        }
    }
}
