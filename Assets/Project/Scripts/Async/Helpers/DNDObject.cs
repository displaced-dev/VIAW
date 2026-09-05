using UnityEngine;

// Summary
// A script that will push the attached object into the Do Not Destroy
// Use this instead of the internal pass of whatever singleton to allow for better discovery and root cause analysis

namespace VIAW.Async.Helpers
{
    public class DNDObject : MonoBehaviour
    {
        private void Awake() {
            DontDestroyOnLoad(this);
        }
    }
}
