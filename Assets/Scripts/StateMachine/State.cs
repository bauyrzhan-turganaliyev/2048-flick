using System.Collections;

namespace MossaGames.StateMachine
{
    public abstract class State
    {
        protected BattleSystem BattleSystem;

        public State(BattleSystem battleSystem)
        {
            BattleSystem = battleSystem;
        }
        public virtual IEnumerator Start()
        {
            yield break;
        }
        public virtual IEnumerator Move()
        {
            yield break;
        }    
        public virtual IEnumerator Stop()
        {
            yield break;
        }

    }
}
