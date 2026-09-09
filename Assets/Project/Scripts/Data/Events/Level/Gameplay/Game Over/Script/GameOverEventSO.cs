using System;
using UnityEngine;

namespace VIAW.Data.Events
{
    [CreateAssetMenu(fileName = "GameOver", menuName = "ScriptableObjects/Events/Level/GameOver", order = 2)]
    public class GameOverEventSO : ScriptableObject
    {
        public event Action OnGameOver;

        public void RaiseEvent()
        {
            OnGameOver?.Invoke();
        }
    }
}