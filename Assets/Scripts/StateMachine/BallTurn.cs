using System.Collections;

namespace MossaGames.StateMachine
{
    public class BallTurn : State
    {
        public BallTurn(BattleSystem battleSystem) : base(battleSystem)
        {
        
        }
    
        public override IEnumerator Start()
        {
            BattleSystem.SetStateText("Ball Turn...");
            yield break;
        
        }
    }
}
