using System.Collections.Generic;
using UnityEngine;

namespace MossaGames.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/BallsPrefabsSO", order = 1)]
    public class BallsPrefabsSO : ScriptableObject
    {
        public List<GameObject> BallsPrefab;
    }
}


