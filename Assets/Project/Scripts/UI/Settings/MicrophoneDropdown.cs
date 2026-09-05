using System.Collections.Generic;
using VIAW.Async.Preferences;
using UnityEngine;
using Evo.UI;
using TMPro;
using TinyInspector;

namespace VIAW.UI
{
    public class MicrophoneDropdown : MonoBehaviour
    {
        [BoxGroup("Scene Refs")]
        [SerializeField] private Dropdown dropdown;
        [BoxGroup("Scene Refs")]
        [SerializeField] private MicrophoneSettings micSettings;
        [BoxGroup("Scene Refs")]
        [SerializeField] private TMP_Text fullNameText;

        const string DefaultLabel = "System Default";

        void Awake()
        {
            if(dropdown == null) {
                dropdown = GetComponent<Dropdown>();
            }
            if(micSettings == null) {
                micSettings = FindObjectOfType<MicrophoneSettings>();
            }  
        }

        void Start()
        {
            Populate();
            dropdown.onItemSelected.AddListener(OnMicSelected);
        }

        void OnDestroy()
        {
            if(dropdown != null) {
                dropdown.onItemSelected.RemoveListener(OnMicSelected);
            }
        }

        public void Populate()
        {
            dropdown.ClearAllItems();

            var labels = new List<string> { DefaultLabel };
            labels.AddRange(Microphone.devices);
            dropdown.AddItems(labels.ToArray());

            string saved = MicrophoneSettings.Load();
            int index = string.IsNullOrEmpty(saved) ? 0 : Mathf.Max(0, labels.IndexOf(saved));
            dropdown.SelectItem(index, false); 
        }

        void OnMicSelected(int index)
        {
            string micName = index == 0 ? string.Empty : dropdown.items[index].label;
            micSettings.SetMicrophone(micName);
            fullNameText.text = micName;
        }
    }
}
