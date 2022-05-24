using UnityEngine;

namespace MossaGames.StateMachine
{
    public abstract class StateMachine : MonoBehaviour
    {
        public State State;

        public void SetState(State state)
        {
            State = state;
            StartCoroutine(State.Start());
        }
    }
}
