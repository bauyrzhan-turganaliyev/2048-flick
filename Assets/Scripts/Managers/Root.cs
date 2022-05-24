using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using MossaGames.ObjectModel;
using MossaGames.ScriptableObjects;
using MossaGames.StateMachine;
using TMPro;
using UnityEngine;

namespace MossaGames.Managers
{
    public class Root : MonoBehaviour
    {
        [SerializeField] private GridManager _gridManager;
        [SerializeField] private MouseManager _mouseManager;
        [SerializeField] private LevelManagerSO _levelManagerSO;
        [SerializeField] private BallsPrefabsSO _ballsPrefabsSO;
        [SerializeField] private BattleSystem _battleSystem;
    
        [SerializeField] private int _currentLevelInspector;

        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private TMP_Text _levelModeText;

        [SerializeField] private Transform _parent;
        [SerializeField] private Transform _confettiesParent;
        
        [SerializeField] private CinemachineVirtualCamera _camera;

        [SerializeField] private float _vibrationIntensity;
        [SerializeField] private int _cellSize;

        //[SerializeField] private List<Texture> _ballsMTextures;
        [SerializeField] private List<Material> _ballsMaterials;
        private Transform _levelParent;
        public Action LevelChangedAction;

        private int _currentLevel = 0;
        private GameObject level;
        private Vector3 _cameraOffset;
    
        private BallsManager _ballsManager;
        private WallsManager _wallsManager;
        private PowersManager _powersManager;
    
        private int _gridHeight;
        private int _gridWidth;

        private bool _isInfinity;
        private int _targetValue;
        private LevelReference _levelReference;

        private void Start()
        {
            Application.targetFrameRate = 60;
            _currentLevel = PlayerPrefs.GetInt("level");
            //_currentLevel = _currentLevelInspector;
            _camera.transform.position = _levelManagerSO.Levels[_currentLevel].CameraOffset;
            InitLevel();

            LevelChangedAction += _battleSystem.LevelChanged;
            TinySauce.OnGameStarted(_currentLevel.ToString());
        }

        private void InitLevel()
        {
            LevelClear();

            if (_currentLevel >= _levelManagerSO.Levels.Count) _currentLevel = 0;
            level = Instantiate(_levelManagerSO.Levels[_currentLevel].LevelPrefab);
            _levelReference = _levelManagerSO.Levels[_currentLevel];
            GetLevelReferences(_levelManagerSO.Levels[_currentLevel]);
            
            var ballsParent = Instantiate(_parent, _levelParent);
            var cellsParent = Instantiate(_parent, _levelParent);
            var wallsParent = Instantiate(_parent, _levelParent);
            var powersParent = Instantiate(_parent, _levelParent);

            _levelManagerSO.Levels[_currentLevel].BallsParent = ballsParent;
            _levelManagerSO.Levels[_currentLevel].CellsParent = cellsParent;
            _levelManagerSO.Levels[_currentLevel].WallsParent = wallsParent;
            _levelManagerSO.Levels[_currentLevel].PowersParent = powersParent;
            
            _gridManager.Init(_gridHeight, _gridWidth, cellsParent);
            _ballsManager.Init(_isInfinity, _gridManager, ballsParent, _ballsMaterials, _ballsPrefabsSO.BallsPrefab, 
                _confettiesParent, _battleSystem.OnBallMove, _battleSystem.OnBallStop, this, _targetValue, _vibrationIntensity);
            _wallsManager.Init(_gridManager, wallsParent);
            _powersManager.Init(_gridManager, powersParent, _battleSystem.OnBallMove, _battleSystem.OnBallStop, this, _ballsMaterials, _ballsManager, _confettiesParent, _vibrationIntensity, _targetValue);
            _mouseManager.Init(_battleSystem);
            _battleSystem.Init(_currentLevel, _levelText, _camera, OnClickNext, OnClickRetry, _levelManagerSO.Levels[_currentLevel].CameraOffset);
            
            LevelChangedAction?.Invoke();
        }

        private void LevelClear()
        {
            if (level != null)
            {
                Destroy(level);
                
                _gridManager.Clear();
                _ballsManager.Clear();
                _wallsManager.Clear();
                _powersManager.Clear();
                
                for (int i = 0; i < _confettiesParent.transform.childCount; i++)
                {
                   Destroy(_confettiesParent.transform.GetChild(i).gameObject);
                }
            }
        }
        private void GetLevelReferences(LevelReference levelReference)
        {
            _levelText.text = "Level " +  (_currentLevel + 1);
            
            _isInfinity = levelReference.IsInfinity;
            _targetValue = levelReference.TargetValue;

            if (_isInfinity) _levelModeText.text = "Endless mode";
            else _levelModeText.text = "Goal: get ball with value: " + _targetValue;

            _ballsManager = levelReference.BallsManager;
            _wallsManager = levelReference.WallsManager;
            _powersManager = levelReference.PowersManager;
        
            _gridHeight = levelReference.GridHeight;
            _gridWidth = levelReference.GridWidth;
            
            _cameraOffset = levelReference.CameraOffset;

        }

        public void OnClickNext()
        {
            _currentLevel++;
            _levelReference.BallsParent.transform.position -= new Vector3(9, 0, 0);
            _levelReference.CellsParent.transform.position -= new Vector3(9, 0, 0);
            _levelReference.WallsParent.transform.position -= new Vector3(9, 0, 0);
            _levelReference.PowersParent.transform.position -= new Vector3(9, 0, 0);
            _camera.transform.position -= new Vector3(9, 0, 0);
            StartCoroutine(Wait(_levelReference));
            
            TinySauce.OnGameFinished(true, 1f, _currentLevel.ToString());

            InitLevel();
        }
        
        public void OnClickRetry()
        { 
            TinySauce.OnGameFinished(false, 1f, _currentLevel.ToString());

            WaitForDestroy(_levelReference);
            InitLevel();
        }
        private void WaitForDestroy(LevelReference levelReference)
        {
            var lll = levelReference;
            Destroy( lll.CellsParent.gameObject);
            Destroy( lll.BallsParent.gameObject);
            Destroy( lll.WallsParent.gameObject);
            Destroy( lll.PowersParent.gameObject);
            /*lll.CellsParent.gameObject.SetActive(false);
            lll.BallsParent.gameObject.SetActive(false);
            lll.WallsParent.gameObject.SetActive(false);
            lll.PowersParent.gameObject.SetActive(false);*/
        }
        private IEnumerator Wait(LevelReference levelReference)
        {
            var lll = levelReference;
            yield return new WaitForSeconds(0.5f);
            Destroy( lll.CellsParent.gameObject);
            Destroy( lll.BallsParent.gameObject);
            Destroy( lll.WallsParent.gameObject);
            Destroy( lll.PowersParent.gameObject);
            /*lll.CellsParent.gameObject.SetActive(false);
            lll.BallsParent.gameObject.SetActive(false);
            lll.WallsParent.gameObject.SetActive(false);
            lll.PowersParent.gameObject.SetActive(false);*/
        }

        private void OnApplicationQuit()
        {
            PlayerPrefs.SetInt("level", _currentLevel);
        }
    }
}
