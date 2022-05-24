using System.Collections;

namespace MossaGames.StateMachine
{
    public class Lost : State
    {
        public Lost(BattleSystem battleSystem) : base(battleSystem)
        {
        }

        public override IEnumerator Start()
        {
            BattleSystem.SetStateText("Level failed");
            BattleSystem.Lose();
            yield break;
        
        }
    }
}
