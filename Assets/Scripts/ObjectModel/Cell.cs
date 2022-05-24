using UnityEngine;

namespace MossaGames.ObjectModel
{
    public class Cell : MonoBehaviour
    {
        [SerializeField] private GameObject _cellPrefab;

        public GameObject CellPrefab
        {
            get => _cellPrefab;
            set => _cellPrefab = value;
        }
    }
}
