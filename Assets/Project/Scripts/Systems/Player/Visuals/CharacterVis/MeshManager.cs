using UnityEngine;

namespace VIAW.Systems.Player
{
    public class MeshManager : MonoBehaviour
    {
        public bool disableMeshes;
        
        private void Update()
        {
            foreach(var renderer in GetComponentsInChildren<Renderer>(true))
            {
                renderer.enabled = !disableMeshes;
            }
        }
    }
}