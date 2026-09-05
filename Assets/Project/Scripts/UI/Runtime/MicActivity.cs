using System;
using Dissonance.Audio.Capture;
using Evo.UI;
using NAudio.Wave;
using UnityEngine;
using TinyInspector;

namespace VIAW.UI
{
    public class MicActivity : MonoBehaviour, IMicrophoneSubscriber
    {
        [BoxGroup("Scene Refs")]
        [SerializeField] private BasicMicrophoneCapture microphone;
        [BoxGroup("Scene Refs")]
        [SerializeField] private ProgressBar progressBar;

        [BoxGroup("Config")]
        [SerializeField] private float gain = 8f;

        private float level;

        void Start()
        {
            if(microphone == null) { microphone = FindObjectOfType<BasicMicrophoneCapture>(); }
            if(microphone != null) { microphone.Subscribe(this); }
        }

        void OnDestroy()
        {
            if(microphone != null) { microphone.Unsubscribe(this); }
        }

        public void ReceiveMicrophoneData(ArraySegment<float> buffer, WaveFormat format)
        {
            if(buffer.Array == null || buffer.Count == 0) { return; }

            float sum = 0f;
            for(int i = 0; i < buffer.Count; i++)
            {
                float s = buffer.Array[buffer.Offset + i];
                sum += s * s;
            }
            float rms = Mathf.Sqrt(sum / buffer.Count);

            level = Mathf.Clamp01(rms * gain) * 100f;
        }

        public void Reset() => level = 0f;

        void Update()
        {
            if(progressBar != null) { progressBar.SetValue(level); }
        }
    }
}
