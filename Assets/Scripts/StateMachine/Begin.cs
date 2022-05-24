using System.Collections;
using System.Threading;
using UnityEngine;

namespace MossaGames.StateMachine
{
    public class Begin : State
    {
        public Begin(BattleSystem battleSystem) : base(battleSystem)
        {
        }

        public override IEnumerator Start()
        {
            BattleSystem.SetStateText("Loading level...");
            BattleSystem.DisplayText(false);

            yield return new WaitWhile(() => true);
        
            BattleSystem.SetState(new PlayerTurn(BattleSystem));
        }
    }
}

