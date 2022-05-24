using System.Collections.Generic;
using MossaGames.Managers;
using UnityEngine;

namespace MossaGames.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/LevelManagerSO", order = 1)]
    public class LevelManagerSO : ScriptableObject
    {
        public List<LevelReference> Levels;
    }
}
