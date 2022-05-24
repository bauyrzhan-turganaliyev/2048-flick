using UnityEngine;

namespace MossaGames.Managers
{
    public class LevelReference : MonoBehaviour
    {
    
        [SerializeField] private GameObject _levelPrefab;
    
        [SerializeField] private BallsManager _ballsManager;
        [SerializeField] private WallsManager _wallsManager;
        [SerializeField] private PowersManager _powersManager;

        [SerializeField] private int _gridHeight;
        [SerializeField] private int _gridWidth;

        [SerializeField] private bool _isInfinity;
        [SerializeField] private int _targetValue;
        [SerializeField] private Vector3 _cameraOffset;

        private Transform _ballsParent;
        private Transform _wallsParent;
        private Transform _powersParent;
        private Transform _cellsParent;

        public int GridWidth
        {
            get => _gridWidth;
            set => _gridWidth = value;
        }

        public int GridHeight
        {
            get => _gridHeight;
            set => _gridHeight = value;
        }

        public BallsManager BallsManager
        {
            get => _ballsManager;
            set => _ballsManager = value;
        }
    
        public GameObject LevelPrefab
        {
            get => _levelPrefab;
            set => _levelPrefab = value;
        }

        public bool IsInfinity
        {
            get => _isInfinity;
            set => _isInfinity = value;
        }

        public int TargetValue
        {
            get => _targetValue;
            set => _targetValue = value;
        }

        public WallsManager WallsManager
        {
            get => _wallsManager;
            set => _wallsManager = value;
        }

        public PowersManager PowersManager
        {
            get => _powersManager;
            set => _powersManager = value;
        }

        public Vector3 CameraOffset
        {
            get => _cameraOffset;
            set => _cameraOffset = value;
        }

        public Transform BallsParent
        {
            get => _ballsParent;
            set => _ballsParent = value;
        }

        public Transform WallsParent
        {
            get => _wallsParent;
            set => _wallsParent = value;
        }

        public Transform PowersParent
        {
            get => _powersParent;
            set => _powersParent = value;
        }

        public Transform CellsParent
        {
            get => _cellsParent;
            set => _cellsParent = value;
        }
    }
}
