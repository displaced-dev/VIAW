using System;
using UnityEngine;
using TMPro;
using TinyInspector;
using VIAW.Async.Preferences;

namespace VIAW.UI
{
    public class UsernameSetupController : MonoBehaviour
    {
        [BoxGroup("Scene Refs")]
        [SerializeField] private TMP_InputField usernameInput;
        [BoxGroup("Scene Refs")]
        [SerializeField] private TMP_Text errorText;
        [BoxGroup("Scene Refs")]
        [SerializeField] private TextAsset profanityList; 

        [BoxGroup("Helpers")]
        public bool AllowedToUseName;

        private const int MinLength = 5;
        private const int MaxLength = 15;

        private string[] bannedWords = Array.Empty<string>();

        private void Awake()
        {
            LoadData();

            if(profanityList != null)
            {
                bannedWords = profanityList.text.Split(
                    new[] { '\r', '\n' },
                    StringSplitOptions.RemoveEmptyEntries);
            }
        }

        private void OnEnable()
        {
            usernameInput.onValueChanged.AddListener(Validate);
            Validate(usernameInput.text);
        }

        private void OnDisable()
        {
            usernameInput.onValueChanged.RemoveListener(Validate);
        }

        private void Validate(string username)
        {
            AllowedToUseName = false;

            if(string.IsNullOrEmpty(username))
            {
                errorText.text = "Must have a username to continue";
                return;
            }

            if(string.IsNullOrWhiteSpace(username))
            {
                errorText.text = "Cannot be spaces only";
                return;
            }

            if(username.Length > MaxLength)
            {
                errorText.text = $"Max of {MaxLength} characters";
                return;
            }

            if(username.Length < MinLength)
            {
                errorText.text = $"Minimum of {MinLength} characters";
                return;
            }

            foreach(string word in bannedWords)
            {
                if(username.IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    errorText.text = "Username might contain profanity";
                    return;
                }
            }

            errorText.text = string.Empty;
            AllowedToUseName = true;
        }

        public void SaveData(){
            if(AllowedToUseName) {
                PlayerPreferences.Instance.SetString(PlayerPreferences.Keys.Username, usernameInput.text);
            }
        }

        private void LoadData() {
            if(PlayerPreferences.Instance != null) {
                if(PlayerPreferences.Instance.HasKey(PlayerPreferences.Keys.Username)) {
                    usernameInput.text = PlayerPreferences.Instance.GetString(PlayerPreferences.Keys.Username);
                }
            }
        }   
    }
}