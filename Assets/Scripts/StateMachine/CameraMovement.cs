using System.Collections;
using UnityEngine;

namespace MossaGames.StateMachine
{
    public class CameraMovement : State
    {
        public CameraMovement(BattleSystem battleSystem) : base(battleSystem)
        {
        }
    
        public override IEnumerator Start()
        {
            yield return new WaitForSeconds(1f);
            BattleSystem.SetStateText("Camera moving..");
            BattleSystem.StartToMoveCamera();
            yield break;
        
        }
    }
}
