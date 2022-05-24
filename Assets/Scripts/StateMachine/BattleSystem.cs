using System;
using System.Collections;
using System.Diagnostics.SymbolStore;
using Cinemachine;
using TMPro;
using UnityEngine;

namespace MossaGames.StateMachine
{
    public class BattleSystem : StateMachine
    {
        [SerializeField] private TMP_Text _stateText;
        [SerializeField] private TMP_Text _moveCountText;

        [SerializeField] private GameObject _winPanel;
        [SerializeField] private GameObject _losePanel;
        [SerializeField] private GameObject _mainPanel;
        
        [SerializeField] private GameObject _handObject;
        
        public Action MoveFinished;
        
        private int _moveCount;
        private int _result;
        private TMP_Text _levelText;
        private bool _isNeedToMoveCamera;
        private CinemachineVirtualCamera _camera;
        private Action _onWin;
        private Action _onLose;
        private Vector3 _cameraPoint;
        private bool _isNeedToSetupCamera;
        private bool _called;
        private int _currentLevel;
        private bool _startHandAnimation;
        private GameObject handS;
        private Vector3 handPos;
        private bool called;
        public GameObject WinPanel
        {
            get => _winPanel;
            set => _winPanel = value;
        }

        public GameObject LosePanel
        {
            get => _losePanel;
            set => _losePanel = value;
        }

        public GameObject MainPanel
        {
            get => _mainPanel;
            set => _mainPanel = value;
        }

        public bool IsNeedToMoveCamera
        {
            get => _isNeedToMoveCamera;
            set => _isNeedToMoveCamera = value;
        }

        public void Init(int currentLevel, TMP_Text levelText, CinemachineVirtualCamera camera, Action OnWin, Action OnLose, Vector3 cameraPoint)
        {
            _currentLevel = currentLevel;
            _cameraPoint = cameraPoint;
            _onLose = OnLose;
            _onWin = OnWin;
            _levelText = levelText;
            _camera = camera;
            SetState(new Begin(this));
            MainPanel.SetActive(true);
            _winPanel.SetActive(false);
            _losePanel.SetActive(false);

            if (_currentLevel == 6)
            {
                var hand = Instantiate(_handObject);
                handS = hand.transform.GetChild(0).transform.GetChild(0).gameObject;
                handS.transform.position = new Vector3(-2.3f, 1.55f, -3f);
                _startHandAnimation = true;
                handPos = handS.transform.position;
            }

        }

        private void Update()
        {
            if (_startHandAnimation)
            {
                handS.transform.position = Vector3.Lerp(handS.transform.position, new Vector3(handS.transform.position.x + 1, handS.transform.position.y, handS.transform.position.z), 1 * Time.deltaTime);
                if (!called) StartCoroutine(Wait());
            }
            if (_isNeedToSetupCamera)
            {
                _camera.transform.position = Vector3.Lerp(_camera.transform.position, _cameraPoint, 5 * Time.deltaTime);

                if (!_called)
                {

                    _onWin.Invoke();
                    StartCoroutine(CameraMovementFinished());
                }
            }
        }

        private IEnumerator CameraMovementFinished()
        {
            _called = true;
            print(_cameraPoint);
            yield return new WaitForSeconds(1f);
            _isNeedToSetupCamera = false;
            _called = false;
        }
        public void StartToMoveCamera()
        {
            _isNeedToSetupCamera = true;
        }
        public void GameStart()
        {
            SetState(new PlayerTurn(this));
            MainPanel.SetActive(false);
        }

        public void LevelChanged()
        {
            SetState(new PlayerTurn(this));
            _moveCount = 0;
            _called = false;
            _moveCountText.text = "Move count: " + _moveCount;
            _winPanel.SetActive(false);
            _losePanel.SetActive(false);
            MainPanel.SetActive(false);
            _levelText.enabled = true;
        }

        public void OnBallMove()
        {
            SetState(new BallTurn(this));
        }
    
        public void OnBallStop(int result)
        {
            _result = result;
            SetState(new Calculate(this));
            _moveCount++;
            if (handS != null) handS.SetActive(false);
            _moveCountText.text = "Move count: " + _moveCount;
            
            MoveFinished?.Invoke();
        }

        public void SetStateText(string text)
        {
            _stateText.text = "State: " + text;
        }

        public int Check()
        {
            return _result;
        }

        public void Win()
        {
            _levelText.enabled = false;
            _winPanel.SetActive(true);
        }

        public void Lose()
        {
            _levelText.enabled = false;
            _losePanel.SetActive(true);
        }

        public void DisplayText(bool flag)
        {
            _levelText.enabled = flag;
        }

        private IEnumerator Wait()
        {
            called = true;
            yield return new WaitForSeconds(2f);
            _startHandAnimation = false;
            called = false;
            handS.transform.position = handPos;
            yield return new WaitForSeconds(0.5f);
            _startHandAnimation = true;
        }
    }
}
