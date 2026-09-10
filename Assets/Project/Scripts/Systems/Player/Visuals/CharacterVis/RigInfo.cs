using UnityEngine;
using TinyInspector;
using FIMSpace.FProceduralAnimation;

// Summary
// Script used for storing and accessing components on the visualized rig

namespace VIAW.Systems.Player
{
    public class RigInfo : MonoBehaviour
    {
        [BoxGroup("Scene Refs")]
        public Transform followerConstraint;
        [BoxGroup("Scene Refs")]
        public Transform handRoot;
        [BoxGroup("Scene Refs/Components")]
        public Animator characterAnimator;
        [BoxGroup("Scene Refs/Components")]
        public _PlayerAnimation playerAnimation;
        [BoxGroup("Scene Refs/Components")]
        public LegsAnimator legsAnimator;
        [BoxGroup("Scene Refs/Components")]
        public MeshManager meshManager;

        [BoxGroup("Controls")]
        public bool localPlayer = false;
    }
}
