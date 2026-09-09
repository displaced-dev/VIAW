using System;
using UnityEngine;

// Summary
// Event Called for ending the round
// TODO: Need to update the script to support which team won the round

namespace VIAW.Data.Events
{
    [CreateAssetMenu(fileName = "RoundOver", menuName = "ScriptableObjects/Events/Level/RoundOver", order = 1)]
    public class RoundOverEventSO : ScriptableObject
    {
        public event Action OnRoundOver;

        public void RaiseEvent()
        {
            OnRoundOver?.Invoke();
        }
    }
}