using UnityEngine;
using VIAW.Data.Events;
using TinyInspector;

// Summary
// Stores a plethora of typical game state information such as round ending, and other information, with the ability to act on that data
// I.e the round ends, what needs to happen? -> Players are teleported back to their spawns with their respective hp reset

namespace VIAW.Systems.Player
{
    public class PlayerGameStateManager : MonoBehaviour
    {
        [BoxGroup("Events")]
        [SerializeField] private GameOverEventSO gameOver;
        [BoxGroup("Events")]
        [SerializeField] private RoundOverEventSO roundOver;

        private void OnEnable() {
            if(gameOver != null) {
                gameOver.OnGameOver += GameEnded;
            }
            if(roundOver != null) {
                roundOver.OnRoundOver += RoundCompleted;
            }
        }

        private void OnDisable() {
            if(gameOver != null) {
                gameOver.OnGameOver -= GameEnded;
            }
            if(roundOver != null) {
                roundOver.OnRoundOver -= RoundCompleted;
            }
        }

        private void GameEnded(){ }
        private void RoundCompleted() { }
    }
}
