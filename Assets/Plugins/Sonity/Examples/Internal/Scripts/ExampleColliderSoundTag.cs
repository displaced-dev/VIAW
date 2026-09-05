// Created by Victor Engström
// Copyright 2026 Sonigon AB
// http://www.sonity.org/

using UnityEngine;
using Sonity;

namespace ExampleSonity {

    [AddComponentMenu("")]
    public class ExampleColliderSoundTag : MonoBehaviour {

        public SoundTag soundTagIndoor;
        public SoundTag soundTagOutdoor;

        private AudioListener cachedAudioListener;
        private Collider cachedCollider;

        void Start() {
#if UNITY_6000_4_OR_NEWER || SONITY_DLL_RUNTIME
            // Unity 6000.4 removes FindFirstObjectByType
            cachedAudioListener = UnityEngine.Object.FindAnyObjectByType<AudioListener>();
#elif UNITY_2022_3_OR_NEWER
            // FindFirstObjectByType is slower than FindAnyObjectByType but is more consistent
            cachedAudioListener = UnityEngine.Object.FindFirstObjectByType<AudioListener>();
#else
            cachedAudioListener = UnityEngine.Object.FindObjectOfType<AudioListener>();
#endif
            cachedCollider = GetComponent<Collider>();
        }

        void Update() {
            if (cachedCollider.bounds.Contains(cachedAudioListener.GetComponent<Transform>().position)) {
                // Is Indoor
                SoundManager.Instance.SetGlobalSoundTagAtIndex(0, soundTagIndoor);
            } else {
                // Is Outdoor
                SoundManager.Instance.SetGlobalSoundTagAtIndex(0, soundTagOutdoor);
            }
        }
    }
}
