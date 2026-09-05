using UnityEngine;
using Dissonance;
using TinyInspector;

namespace VIAW.Async.Preferences
{
    public class MicrophoneSettings : MonoBehaviour
    {
        const string PrefsKey = "SelectedMicrophone";

        [BoxGroup("Scene Ref")]
        [SerializeField] private DissonanceComms comms;
    
        void Awake()
        {
            if(comms == null) {
                comms = FindObjectOfType<DissonanceComms>();
            }
        }
    
        void Start()
        {
            Apply(Load());
        }
    
        public static string Load()
        {
            return PlayerPrefs.GetString(PrefsKey, string.Empty);
        }
    
        public static void Save(string micName)
        {
            PlayerPrefs.SetString(PrefsKey, micName ?? string.Empty);
            PlayerPrefs.Save();
        }
    
        public void Apply(string micName)
        {
            if (comms == null)
                return;
    
            comms.MicrophoneName = string.IsNullOrEmpty(micName) ? null : micName;
        }
    
        public void SetMicrophone(string micName)
        {
            Apply(micName);
            Save(micName);
        }
    } 
}
