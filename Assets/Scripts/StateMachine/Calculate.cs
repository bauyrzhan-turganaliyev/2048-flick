using System.Collections;

namespace MossaGames.StateMachine
{
    public class Calculate : State
    {
        public Calculate(BattleSystem battleSystem) : base(battleSystem)
        {
        }

        public override IEnumerator Start()
        {
            var result = BattleSystem.Check();

            switch (result)
            {
                case 0:
                    BattleSystem.SetState(new Lost(BattleSystem));
                    break;
                case 1:
                    BattleSystem.SetState(new Won(BattleSystem));
                    break;
                case 2:
                    BattleSystem.SetState(new BallTurn(BattleSystem));
                    break;
                case 3:
                    BattleSystem.SetState(new PlayerTurn(BattleSystem));
                    break;
            }
        
            yield break;
        }
    }
}






