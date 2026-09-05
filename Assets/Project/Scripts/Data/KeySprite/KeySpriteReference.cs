using System.Collections.Generic;
using UnityEngine;
using TinyInspector;
using VIAW.UI;

namespace VIAW.Data
{
    [CreateAssetMenu(fileName = "KeySpriteReference", menuName = "ScriptableObjects/Input/Key Sprite Reference")]
    public class KeySpriteReference : ScriptableObject
    {
        [System.Serializable]
        public class KeySpritePair
        {
            public string controlPath;
            public Sprite sprite;
        }

        [System.Serializable]
        public class PathAlias
        {
            public string from;
            public string to;
        }

        [BoxGroup("Keyboard")]
        [SerializeField] private List<KeySpritePair> keyboardSprites = new();
        [BoxGroup("Keyboard")]
        [SerializeField] private Sprite defaultKeyboardSprite;

        [BoxGroup("Mouse")]
        [SerializeField] private List<KeySpritePair> mouseSprites = new();
        [BoxGroup("Mouse")]
        [SerializeField] private Sprite defaultMouseSprite;

        [BoxGroup("Gamepad")]
        [SerializeField] private List<KeySpritePair> gamepadSprites = new();
        [BoxGroup("Gamepad")]
        [SerializeField] private Sprite defaultGamepadSprite;

        [BoxGroup("Aliases")]
        [SerializeField] private List<PathAlias> aliases = new()
        {
            new PathAlias { from = "/shift", to = "/leftshift" },
            new PathAlias { from = "/ctrl", to = "/leftctrl" },
            new PathAlias { from = "/alt", to = "/leftalt" },
            new PathAlias { from = "/meta", to = "/leftmeta" },
            new PathAlias { from = "/command", to = "/leftcommand" },

            new PathAlias { from = "/gamepad/", to = "/xinputcontrollerwindows/" },
            new PathAlias { from = "/xinputcontroller/", to = "/xinputcontrollerwindows/" },
            new PathAlias { from = "/dualshockgamepad/", to = "/xinputcontrollerwindows/" },
            new PathAlias { from = "/switchprocontroller/", to = "/xinputcontrollerwindows/" },
        };

        private Dictionary<string, Sprite> cache;

        private void OnEnable() => RebuildCache();
        private void OnValidate() => RebuildCache();

        public Sprite GetSprite(string controlPath)
        {
            if(string.IsNullOrEmpty(controlPath)){
                return ControllerTabsManager.IsUsingKeyboard() ? defaultKeyboardSprite : defaultGamepadSprite;
            }

            if(cache == null){
                RebuildCache();
            }

            string path = Normalize(controlPath);

            if(cache.TryGetValue(path, out Sprite sprite)){
                return sprite;
            }

            return GetDefaultSprite(path);
        }

        public IEnumerable<string> GetAllPaths()
        {
            if(cache == null){
                RebuildCache();
            }

            return cache.Keys;
        }

        public void RebuildCache()
        {
            cache = new Dictionary<string, Sprite>();

            AddToCache(keyboardSprites);
            AddToCache(mouseSprites);
            AddToCache(gamepadSprites);
        }

        private void AddToCache(List<KeySpritePair> pairs)
        {
            foreach(KeySpritePair pair in pairs)
            {
                if(string.IsNullOrEmpty(pair.controlPath) || pair.sprite == null){
                    continue;
                }

                string path = Normalize(pair.controlPath);

                if(cache.ContainsKey(path)){
                    Debug.LogWarning($"KeySpriteReference [{name}]: Duplicate path '{path}', keeping the first one.", this);
                }
                else
                    cache.Add(path, pair.sprite);
            }
        }

        private string Normalize(string path)
        {
            path = path.Replace("<", "").Replace(">", "").ToLowerInvariant();

            if(!path.StartsWith("/")){
                path = "/" + path;
            }

            foreach(PathAlias alias in aliases)
            {
                if(string.IsNullOrEmpty(alias.from) || string.IsNullOrEmpty(alias.to)){
                    continue;
                }

                path = path.Replace(alias.from.ToLowerInvariant(), alias.to.ToLowerInvariant());
            }

            return path;
        }

        private Sprite GetDefaultSprite(string path)
        {
            if(path.Contains("keyboard")) { return defaultKeyboardSprite; }
            if(path.Contains("mouse")) { return defaultMouseSprite; }
            return defaultGamepadSprite;
        }
    }
}