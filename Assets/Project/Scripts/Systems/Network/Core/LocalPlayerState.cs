using UnityEngine;
using VIAW.Systems.Player;
using TinyInspector;
using PurrNet;

namespace VIAW.Systems.Network
{
    public class LocalPlayerState : NetworkIdentity
    {
        [BoxGroup("Scene References")]
        public VIAW.Systems.Player.Player playerObject;

        [BoxGroup("Debug")]
        [SerializeField] private bool Host;
        [BoxGroup("Debug")]
        [SerializeField] private bool DedicatedServer;

        public bool isLocalPlayer = false;

        protected override void OnSpawned()
        {
            if(Host)
            {
                isLocalPlayer = isOwner;
            }

            if(DedicatedServer)
            {
                isLocalPlayer = isOwner && !isServer;
            }

            playerObject.Initialize();
        }
    }
}
