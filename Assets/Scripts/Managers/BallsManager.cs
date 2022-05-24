using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using MossaGames.ObjectModel;
using MossaGames.Powers;
using MossaGames.StateMachine;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MossaGames.Managers
{
    public class BallsManager : MonoBehaviour
    {
        [SerializeField] private BallSetting[] _ballSettings;
    
        [SerializeField] private List<GameObject> _confetties;
        [SerializeField] private List<GameObject> _brokenParts;

        [SerializeField] private List<Ball> _balls = new List<Ball>();
        [SerializeField] private List<GameObject> _ballsObject = new List<GameObject>();

        private GridManager _gridManager;
        private Transform _ballsParent;
        private Transform _confettiesParent;
        private List<Material> _ballsMaterials;
        private List<GameObject> _ballsPrefab;
        private Dictionary<Vector2, int> _cells = new Dictionary<Vector2, int>();
        private Action _onBallMove;
        private Action<int> _onBallStop;
        private MonoBehaviour _coroutiner;
        private int _moves;
        private int _targetValue;
        private float _intensity;

        public List<GameObject> Confetties
        {
            get => _confetties;
            set => _confetties = value;
        }

        public List<GameObject> BrokenParts
        {
            get => _brokenParts;
            set => _brokenParts = value;
        }

        public List<GameObject> BallsObject
        {
            get => _ballsObject;
            set => _ballsObject = value;
        }

        public Transform BallsParent
        {
            get => _ballsParent;
            set => _ballsParent = value;
        }

        public void Init(bool isInfinity, GridManager gridManager, Transform parent, List<Material> ballsMaterials, List<GameObject> ballsPrefab,
            Transform confettiesParent, Action onBallMove, Action<int> onBallStop, MonoBehaviour coroutiner, int targetValue, float intensity)
        {
            Clear();
            _intensity = intensity;
            _targetValue = targetValue;
            _coroutiner = coroutiner;
            _onBallMove = onBallMove;
            _onBallStop = onBallStop;
            // cells 
            _gridManager = gridManager;
            BallsParent = parent;
            _ballsMaterials = ballsMaterials;
            _ballsPrefab = ballsPrefab;
            _confettiesParent = confettiesParent;
            _moves = 0;     
            if (isInfinity)
                GenerateBallsINF();
            else
                GenerateBalls();
            
        }

        public void Clear()
        {
            _balls.Clear();
            _cells.Clear(); 
        }
        public void DestroyAllObjects(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                Destroy(parent.GetChild(i).gameObject);
            }
        }
        
        public void Remove(Ball ball)
        {
            _balls.Remove(ball);
        }
        public int Check(Ball ball, int result)
        {
            var position = ball.Position;
            foreach (var varBall in _balls)
            {
                if (varBall.Value == ball.Value)
                {
                    if (varBall.Position.x - 1 == ball.Position.x && varBall.Position.z == ball.Position.z)
                    {
                        print("Right");
                        _coroutiner.StartCoroutine(StartMove(ball, 1));
                        //ball.MoveToDirection(3);
                        return 2;
                    } 
                    if (varBall.Position.x + 1 == ball.Position.x && varBall.Position.z == ball.Position.z)
                    {
                        print("Left");
                    
                        _coroutiner.StartCoroutine(StartMove(ball, 3));
                        //ball.MoveToDirection(1);                 
                        return 2;
                    } 
                    if (varBall.Position.z + 1 == ball.Position.z && varBall.Position.x == ball.Position.x)
                    {
                        print("Down");
                    
                        _coroutiner.StartCoroutine(StartMove(ball, 2));
                        //ball.MoveToDirection(2);           
                        return 2;
                    } 
                    if (varBall.Position.z - 1 == ball.Position.z && varBall.Position.x == ball.Position.x)
                    {
                        print("Up   ");
                    
                        _coroutiner.StartCoroutine(StartMove(ball, 0));
                        //ball.MoveToDirection(0);           
                        return 2;
                    } 
                }
                
            }

            if (result == 1 && _targetValue <= ball.Value) return 1;
            if (result == 1 && _targetValue != ball.Value) return 0;
            return 3;
        }
        private IEnumerator StartMove(Ball ball, int direction)
        {
            yield return new WaitForSeconds(0.1f);
            ball.MoveToDirection(direction);   
        }
        private void GenerateBalls()
        {
            for (int i = 0; i < _ballSettings.Length; i++)
            {
                if (_ballSettings[i].SpawnMove != _moves) continue;
                GenerateSingleBall(i);
            }
        }
        private void GenerateBallsINF()
        {
            for (int i = 0; i < _ballSettings.Length; i++)
            {
                var width = _gridManager.Width;
                var height = _gridManager.Height;
            
                var x = -(width/2) + _ballSettings[i].Row;
                var z = (height/2) - _ballSettings[i].Column;
                
                if (_cells.ContainsKey(new Vector2(x, z)))
                {
                    i--;
                    continue;
                }
                else
                {
                    var value = _ballSettings[i].Value;
                    var index = -1;

                    while (value != 1)
                    {
                        value /= 2;
                        index++;
                    }
                    var ball = Instantiate(_ballsPrefab[index], BallsParent);

                    var ballComponent = ball.GetComponent<Ball>();

                    ballComponent.Merged += GenerateBall;
                    ballComponent.StartMove += _onBallMove;
                    ballComponent.StopMove += _onBallStop;

                    ballComponent.Init(new Vector3(x, 1f, z), _ballSettings[i].Value, _ballsMaterials, index, this, _confettiesParent, _intensity);

                    _balls.Add(ballComponent);
                    _cells.Add(new Vector2(x, z), ballComponent.ID);
                }
            }
        }
        private void GenerateBall(int id)
        {
            foreach (var cell in _cells)
            {
                if (cell.Value == id)
                {
                    _cells.Remove(cell.Key);
                
                    break;
                }
            }
        
            var width = _gridManager.Width;
            var height = _gridManager.Height;
        
            var x = Random.Range(-(width/2), width/2);
            var z = Random.Range(-(height / 2), height / 2);

            if (_cells.ContainsKey(new Vector2(x, z)))
            {
                GenerateBall(id);
            }
            else
            {
                int randInt = Random.Range(0, 100);
                int value = 2;
                int index = 0;
                if (randInt >= 0 && randInt < 50)
                {
                    value = 2;
                    index = 0;
                }
                else if (randInt >= 50 && randInt < 75)
                {
                    value = 4;
                    index = 1;
                }
                else if (randInt >= 75 && randInt < 88)
                {
                    value = 8;
                    index = 2;
                }
                else if (randInt >= 88 && randInt < 94)
                {
                    value = 16;
                    index = 3;
                } 
                else if (randInt >= 94 && randInt < 97)
                {
                    value = 32;
                    index = 4;
                }
                else if (randInt >= 97 && randInt < 100)
                {
                    value = 64;
                    index = 5;
                }


                var ball = Instantiate(_ballsPrefab[index], BallsParent);

                var ballComponent = ball.GetComponent<Ball>();
                ballComponent.Init(new Vector3(x, 1f, z), _ballSettings[id].Value, _ballsMaterials, index, this, _confettiesParent, _intensity);
                _balls.Add(ballComponent);

                _cells.Add(new Vector2(x, z), ballComponent.ID);
            
                ballComponent.StartMove += _onBallMove;
                ballComponent.StopMove += _onBallStop;
                ballComponent.Merged += GenerateBall;
            }
        }
        private void OnBallStop(int id)
        {
            _moves++;

            for (int i = 0; i < _ballSettings.Length; i++)
            {
                if (_ballSettings[i].SpawnMove == _moves)
                {
                    GenerateSingleBall(i);
                }
            }
        }

        private void GenerateSingleBall(int i)
        {
            var width = _gridManager.Width;
            var height = _gridManager.Height;
            
            var x = -(width/2) + _ballSettings[i].Row;
            var z = (height/2) - _ballSettings[i].Column -1;

            var value = _ballSettings[i].Value;
            var index = -1;

            if (value % 2 != 0)
                throw new Exception($"wrong value {value}");
            while (value != 1)
            {
                value /= 2;
                index++;
            }
            
            var ball = Instantiate(_ballsPrefab[index], BallsParent);
            BallsObject.Add(ball);
            
            var ballComponent = ball.GetComponentInChildren<Ball>();

            ballComponent.Init(new Vector3(x, .1f, z), _ballSettings[i].Value, _ballsMaterials, index, this, _confettiesParent, _intensity);

            _balls.Add(ballComponent);
            // callbacks
            ballComponent.StartMove += _onBallMove;
            ballComponent.StopMove += _onBallStop;
            ballComponent.StopMove += OnBallStop;
        }
    }

    [System.Serializable]
    public class BallSetting
    {
        public int Row;
        public int Column;
        public int Value;
        public int SpawnMove;
    }
}