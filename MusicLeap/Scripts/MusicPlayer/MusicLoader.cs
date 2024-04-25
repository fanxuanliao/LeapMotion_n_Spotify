using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

namespace MusicLeap {

    public class MusicLoader : MonoBehaviour {

        string audioPath;

        [HideInInspector] public List<AudioClip> clips;

        void Awake() {
            audioPath = Application.dataPath + "/../Audios";
            string[] files = Directory.GetFiles(audioPath, "*.wav");
            Debug.Log(files.Length + " musics in " + audioPath);
            StartCoroutine(LoadAudioFile(files));
        }

        IEnumerator LoadAudioFile(string[] files) {
            foreach ( string file in files ) {
                UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip("file://"+file, AudioType.WAV);
                yield return uwr.SendWebRequest();
                if(uwr.isNetworkError) {
                    Debug.Log(uwr.error + file);
                } else {
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(uwr);
                    clip.name = Path.GetFileNameWithoutExtension(file);
                    clips.Add(clip);
                    Debug.Log("Loaded " + file);
                }
            }
        }
    }
}
