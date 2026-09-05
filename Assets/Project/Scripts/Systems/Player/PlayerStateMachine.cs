using UnityEngine;

namespace VIAW.Systems.Player
{
    public class PlayerStateMachine : MonoBehaviour
    {
        public bool isDead;
        public bool isCinematic;
        
        public bool overrideInput;

        public void FixedUpdate()
        {
            if(isCinematic || isDead)
            {
                overrideInput = true;
            }
            else
            {
                overrideInput = false;
            }
        }
    }
}
