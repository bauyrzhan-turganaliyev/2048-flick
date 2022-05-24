using System.Collections;

namespace MossaGames.StateMachine
{
    public class PlayerTurn : State
    {
        public PlayerTurn(BattleSystem battleSystem) : base(battleSystem)
        {
        }

        public override IEnumerator Start()
        {
            BattleSystem.SetStateText("Player Turn...");
            BattleSystem.DisplayText(true);
            yield break;
        }

        public override IEnumerator Move()
        {
            BattleSystem.SetState(new Calculate(BattleSystem));
            yield break;
        
        }
    }
}

