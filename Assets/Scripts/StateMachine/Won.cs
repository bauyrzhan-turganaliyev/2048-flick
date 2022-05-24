using System.Collections;

namespace MossaGames.StateMachine
{
    public class Won : State
    {
        public Won(BattleSystem battleSystem) : base(battleSystem)
        {
        }
    
        public override IEnumerator Start()
        {
            BattleSystem.SetStateText("Level completed");
            BattleSystem.Win();
            BattleSystem.SetState(new CameraMovement(BattleSystem));
            yield break;
        
        }
    }
}
