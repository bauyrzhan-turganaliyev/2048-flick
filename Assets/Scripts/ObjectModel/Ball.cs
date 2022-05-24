using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.NiceVibrations;
using MossaGames.Managers;
using MossaGames.Powers;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace MossaGames.ObjectModel
{
    public class Ball : MonoBehaviour
    {
        private static int index;
        [SerializeField] private GameObject _ballPrefab;
        [SerializeField] private GameObject _ballPartsPrefab;

        [SerializeField] private SkinnedMeshRenderer _ballMaterial;
        [SerializeField] private SkinnedMeshRenderer[] _eyesMaterials;

        [SerializeField] private Animator _ballAnimator;

        //[SerializeField] private TMP_Text _valueText;
        //[SerializeField] private Canvas _canvas;
        [SerializeField] private Vector3 _position;
        [SerializeField] private int _value;
        [SerializeField] private int _force;
        [SerializeField] private int _speed;
        [SerializeField] private float _rotationDamp;
        [SerializeField] private bool _isMultiBall;
        [SerializeField] private bool _isJellyBall;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Transform _transform;

        public Action<int> Merged;
        public Action<int> MergedWithValue;
        public Action StartMove;
        public Action<int> StopMove;

        private BallsManager _ballsManager;
        private int _id;
        private SphereCollider[] _colliders;
        private Transform _confettiesParent;
        private bool _isNeedToMove;
        [SerializeField] private bool _isNeedToRotate;
        private int _direction;
        private bool _isWrongCollision;
        private bool _isFinished;
        private int _indexSO;

        private List<Material> _ballsMaterials;
        private float _intensity;
        
        public GameObject BallPrefab
        {
            get => _ballPrefab;
            set => _ballPrefab = value;
        }
        public int Value
        {
            get => _value;
            set => _value = value;
        }
        
        public int ID
        {
            get => _id;
            set => _id = value;
        }
        public Vector3 Position
        {
            get => _position;
            set => _position = value;
        }
        public Rigidbody RigidbodyObject
        {
            get => _rigidbody;
            set => _rigidbody = value;
        }
        public int IndexSo
        {
            get => _indexSO;
            set => _indexSO = value;
        }
        public List<Material> BallsMaterials
        {
            get => _ballsMaterials;
            set => _ballsMaterials = value;
        }
        public SkinnedMeshRenderer BallMaterial
        {
            get => _ballMaterial;
            set => _ballMaterial = value;
        }
        public Animator BallAnimator
        {
            get => _ballAnimator;
            set => _ballAnimator = value;
        }
        public BallsManager BallsManager
        {
            get => _ballsManager;
            set => _ballsManager = value;
        }
        public Transform ConfettiesParent
        {
            get => _confettiesParent;
            set => _confettiesParent = value;
        }
        public bool IsMultiBall
        {
            get => _isMultiBall;
            set => _isMultiBall = value;
        }
        public GameObject BallPartsPrefab
        {
            get => _ballPartsPrefab;
            set => _ballPartsPrefab = value;
        }
        public bool IsJellyBall
        {
            get => _isJellyBall;
            set => _isJellyBall = value;
        }
        public SkinnedMeshRenderer[] EyesMaterials
        {
            get => _eyesMaterials;
            set => _eyesMaterials = value;
        }

        public Transform TransformObject
        {
            get => _transform;
            set
            {
                _transform = value;
                Position = value.position;
            }
        }

        public bool IsNeedToMove
        {
            get => _isNeedToMove;
            set => _isNeedToMove = value;
        }


        public void Init(Vector3 position, int value, List<Material> ballsMaterials, int indexSO, BallsManager ballsManager, Transform confettiesParent, float intesity)
        {
            _id = index;
            _intensity = intesity;
            _indexSO = indexSO;
            //_ballAnimator.enabled = false;
            BallsManager = ballsManager;
            _colliders = _ballPrefab.GetComponents<SphereCollider>();

            _isNeedToMove = true;
            _direction = 5;
            _confettiesParent = confettiesParent;
            BallsMaterials = ballsMaterials;
        
            Position = position;
            _ballPrefab.transform.position = Position;
            _value = value;
            //ValueText.text = _value.ToString();
            //_canvas.transform.position = new Vector3(_ballPrefab.transform.position.x, _ballPrefab.transform.position.y + 0.51f, _ballPrefab.transform.position.z);
            index++;
            
            var _idleBlinking = Animator.StringToHash("idle_blinking");
            var _idlelookAround = Animator.StringToHash("idle_lookaround");
            
            var randInt = Random.Range(0, 100);
            if (randInt < 50) BallAnimator.CrossFade(_idleBlinking, 5);
            else BallAnimator.CrossFade(_idlelookAround, 5);
        }
        public void MoveToDirection(int direction)
        {
            _direction = direction;
            IsNeedToMove = true;
            
            switch (_direction)
            {
                case 0:
                    BallAnimator.Play("swipe_B");
                    break;
                case 1:
                    BallAnimator.Play("swipe_R");
                    break;
                case 2:
                    BallAnimator.Play("swipe_T");
                    break;
                case 3:
                    BallAnimator.Play("swipe_L");
                    break;
            }
            StartMove?.Invoke();
        }

        private void Update()
        {
            if (_isNeedToRotate)
            {
                Quaternion currentRot = transform.rotation;
                Quaternion targerRot = Quaternion.Euler(new Vector3());
                Quaternion smoothRot = Quaternion.Slerp(currentRot, targerRot, Time.deltaTime / _rotationDamp);

                transform.rotation = smoothRot;

                StartCoroutine(StopRotation());
            }
            if (!IsNeedToMove) return;
            switch (_direction)
            {
                case 0:
                    _transform.Translate(new Vector3(0, 0,  Time.deltaTime * _speed));
                    //RigidbodyObject.AddForce(new Vector3(0, 0, _force * Time.deltaTime * _speed));
                    _ballAnimator.Play("swipe_T");
                    break;
                case 1:
                    _transform.Translate(new Vector3(Time.deltaTime * _speed, 0, 0));
                    //RigidbodyObject.AddForce(new Vector3(_force * Time.deltaTime * _speed, 0, 0));
                    _ballAnimator.Play("swipe_R");
                    break;
                case 2:
                    _transform.Translate(new Vector3(0, 0, -Time.deltaTime * _speed));
                    //RigidbodyObject.AddForce(new Vector3(0, 0, -_force * Time.deltaTime * _speed));
                    _ballAnimator.Play("swipe_B");
                    break;
                case 3:
                    _transform.Translate(new Vector3(-Time.deltaTime * _speed, 0, 0));
                    //RigidbodyObject.AddForce(new Vector3(-_force * Time.deltaTime* _speed, 0, 0));
                    _ballAnimator.Play("swipe_L");
                    break;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!collision.gameObject.CompareTag("Untagged"))
            {
                MMVibrationManager.TransientHaptic(_intensity, 0.7f, true, this);
            }
            Ball ball = new Ball();
            bool _isBall = false;
            if (collision.gameObject.TryGetComponent<Ball>(out Ball ballComp))
            {
                ball = ballComp;
                _isBall = true;
            }
            /*if (_isBall && ball.Value == _value && !_isMultiBall && !ball.IsMultiBall)
            {
                if (_isNeedToMove)
                {
                    StopBall();
                    switch (_direction)
                        {
                            case 0:
                                ball.BallAnimator.Play("merge_B");
                                break;
                            case 1:
                                ball.BallAnimator.Play("merge_R");
                                break;
                            case 2:
                                ball.BallAnimator.Play("merge_T");
                                break;
                            case 3:
                                ball.BallAnimator.Play("merge_L");
                                break;
                        }

                    ball.RigidbodyObject.constraints = RigidbodyConstraints.FreezeAll;
                    
                    ball.transform.position = ball.Position;
                    ball.Position = ball.TransformObject.position;
                    
                    ball.Value += _value;
                
                    ball.RigidbodyObject.constraints = RigidbodyConstraints.None;
                    ball.RotateToNormal();

                    ball.IndexSo++;
                    ball.BallMaterial.material = ball.BallsMaterials[ball.IndexSo];
                    foreach (var eye in ball.EyesMaterials)
                    {
                        eye.material = ball.BallsMaterials[ball.IndexSo];
                    }
                
                    GameObject confetty = Instantiate(BallsManager.Confetties[ball.IndexSo], _confettiesParent);
                    confetty.transform.position = transform.position;
                
                    _ballsManager.Remove(this);


                    Merged?.Invoke(_id);
                    MergedWithValue?.Invoke(ball.Value);
                    StopMove?.Invoke(ball.BallsManager.Check(ball, 0));
                
                    Destroy(gameObject);
                }
            }*/
            if (_isBall)
            {
                if (_value == ball.Value)
                {
                    if (!_isNeedToMove || _direction == 5) return;

                    print("Value is equal");
                    ball.IsNeedToMove = false;
                    ball.RigidbodyObject.constraints = RigidbodyConstraints.FreezeAll;
                
                    ball.TransformObject.position = ball.Position;
                    ball.Position = ball.TransformObject.position;
                    
                    ball.RigidbodyObject.constraints = RigidbodyConstraints.None;
                    
                    ball.RotateToNormal();
                    switch (_direction)
                    {
                        case 0:
                            ball.BallAnimator.Play("merge_B");
                            break;
                        case 1:
                            ball.BallAnimator.Play("merge_R");
                            break;
                        case 2:
                            ball.BallAnimator.Play("merge_T");
                            break;
                        case 3:
                            ball.BallAnimator.Play("merge_L");
                            break;
                    }
                    
                    ball.IndexSo++;
                    ball.BallMaterial.material = ball.BallsMaterials[ball.IndexSo];
                    foreach (var eye in ball.EyesMaterials)
                    {
                        eye.material = ball.BallsMaterials[ball.IndexSo];
                    }
                    
                    GameObject confetty = Instantiate(ball.BallsManager.Confetties[ball.IndexSo], _confettiesParent);
                    confetty.transform.position = transform.position;

                    _direction = 5;
                    ball.Value += _value;
                    
                    Merged?.Invoke(ball.ID);
                    MergedWithValue?.Invoke(ball.Value);
                    StopMove?.Invoke(BallsManager.Check(ball, 0));
                    
                    _ballsManager.Remove(this);
                    Destroy(gameObject);
                }
                
                else if (_isMultiBall && _direction != 5)
                {
                    if (!_isMultiBall) return;

                    _isNeedToMove = false;
                    ball.IsNeedToMove = false;
                    
                    ball.RigidbodyObject.constraints = RigidbodyConstraints.FreezeAll;
                    ball.RigidbodyObject.constraints = RigidbodyConstraints.None;
                    RigidbodyObject.constraints = RigidbodyConstraints.FreezeAll;
                    RigidbodyObject.constraints = RigidbodyConstraints.None;
                    switch (_direction)
                    {
                        case 0:
                            _ballAnimator.Play("merge_B");
                            break;
                        case 1:
                            _ballAnimator.Play("merge_R");
                            break;
                        case 2:
                            _ballAnimator.Play("merge_T");
                            break;
                        case 3:
                            _ballAnimator.Play("merge_L");
                            break;
                    }

                    print("you are multi ball");
                    _isMultiBall = false;
                    ball.IsMultiBall = false;
                    ball.transform.position = ball.Position;
                    ball.Position = ball.Position;

                    ball.Value *= 2;
                    ball.IndexSo++;
                    
                    ball.BallMaterial.material = ball.BallsMaterials[ball.IndexSo];
                    foreach (var eye in ball.EyesMaterials)
                    {
                        eye.material = ball.BallsMaterials[ball.IndexSo];
                    }

                    
                    GameObject confetty = Instantiate(ball.BallsManager.Confetties[ball.IndexSo], _confettiesParent);
                    confetty.transform.position = transform.position;

                    BallsManager.Remove(this);
                    Merged?.Invoke(ball.ID);
                    MergedWithValue?.Invoke(ball.Value);
                    StopMove?.Invoke(BallsManager.Check(ball, 0));
                    
                    Destroy(gameObject);
                }
                else if (ball.IsMultiBall)
                {
                    if (_isMultiBall || _direction == 5) return;

                    _isNeedToMove = false;
                    ball.IsNeedToMove = false;
                    
                    ball.RigidbodyObject.constraints = RigidbodyConstraints.FreezeAll;
                    ball.RigidbodyObject.constraints = RigidbodyConstraints.None;
                    RigidbodyObject.constraints = RigidbodyConstraints.FreezeAll;
                    RigidbodyObject.constraints = RigidbodyConstraints.None;
                    switch (_direction)
                    {
                        case 0:
                            _ballAnimator.Play("merge_B");
                            break;
                        case 1:
                            _ballAnimator.Play("merge_R");
                            break;
                        case 2:
                            _ballAnimator.Play("merge_T");
                            break;
                        case 3:
                            _ballAnimator.Play("merge_L");
                            break;
                    }

                    print("he is multi ball");
                    transform.position = ball.Position;
                    Position = transform.position;

                    _value *= 2;
                    _indexSO++;
                    
                    BallMaterial.material = BallsMaterials[IndexSo];
                    foreach (var eye in EyesMaterials)
                    {
                        eye.material = BallsMaterials[IndexSo];
                    }

                    
                    GameObject confetty = Instantiate(BallsManager.Confetties[IndexSo], _confettiesParent);
                    confetty.transform.position = transform.position;

                    ball.BallsManager.Remove(this);
                    
                    Merged?.Invoke(_id);
                    StopMove?.Invoke(BallsManager.Check(this, 0));

                    Destroy(ball.gameObject);
                }
                else if (_value != ball.Value && !ball.IsMultiBall && !_isMultiBall)
                {
                    if (!_isNeedToMove) return;
                    
                    print("Value is not equal");
                    _isNeedToMove = false; 
                    _rigidbody.velocity = Vector3.zero;
                    _rigidbody.angularVelocity = Vector3.zero;

                    ball.RigidbodyObject.velocity = Vector3.zero;
                    ball.RigidbodyObject.angularVelocity = Vector3.zero;

                    _isWrongCollision = true;
                    _isNeedToMove = false;

                    if (_direction == 0) _direction = 2;
                    else if (_direction == 1) _direction = 3;
                    else if (_direction == 2) _direction = 0;
                    else if (_direction == 3) _direction = 1;
                
                    switch (_direction)
                    {
                        case 0:
                            RigidbodyObject.AddForce(new Vector3(0, 0, 18.5f * Time.deltaTime * _force ));

                            ball.RigidbodyObject.velocity = Vector3.zero;
                            ball.RigidbodyObject.angularVelocity = Vector3.zero;
                        
                            ball.RigidbodyObject.AddForce(new Vector3(0, 0, -18.5f * Time.deltaTime * _force));
                            break;
                        case 1:
                            RigidbodyObject.AddForce(new Vector3(18.5f * Time.deltaTime * _force , 0, 0));
                        
                            ball.RigidbodyObject.velocity = Vector3.zero;
                            ball.RigidbodyObject.angularVelocity = Vector3.zero;
                        
                            ball.RigidbodyObject.AddForce(new Vector3(-18.5f * Time.deltaTime * _force , 0, 0));
                            break;
                        case 2:
                            RigidbodyObject.AddForce(new Vector3(0, 0, Time.deltaTime * -18.5f * _force ));
                        
                            ball.RigidbodyObject.velocity = Vector3.zero;
                            ball.RigidbodyObject.angularVelocity = Vector3.zero;

                            ball.RigidbodyObject.AddForce(new Vector3(0, 0, Time.deltaTime * 18.5f * _force ));
                            break;
                        case 3:
                            RigidbodyObject.AddForce(new Vector3(Time.deltaTime * -18.5f * _force, 0, 0));
                        
                            ball.RigidbodyObject.velocity = Vector3.zero;
                            ball.RigidbodyObject.angularVelocity = Vector3.zero;
                            
                            ball.RigidbodyObject.AddForce(new Vector3(Time.deltaTime * 18.5f * _force, 0, 0));
                            break;
                    }

                    StopMove?.Invoke(0);
                    StartCoroutine(BreakBalls(ball));
                }
            }
            
            else if (!_isBall && collision.gameObject.CompareTag("Barer"))
            {
                if (!IsNeedToMove) return;

                _isWrongCollision = true;
                IsNeedToMove = false;
            
                _rigidbody.velocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
                
                StopMove?.Invoke(0);
                
                StartCoroutine(BreakBalls());
            }
            else if (!_isBall && collision.gameObject.CompareTag($"StoneWall"))
            {
                IsNeedToMove = false;
            
                _rigidbody.velocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
            
                var bombPos = collision.gameObject.transform.position;
                switch (_direction)
                {
                    case 0:
                        print("Top");
                        TransformObject.position = new Vector3(bombPos.x, TransformObject.position.y, bombPos.z - 1f);
                        break;
                    case 1:
                    
                        print("Right");
                        TransformObject.position = new Vector3(bombPos.x - 1f, TransformObject.position.y, bombPos.z);
                        break;
                    case 2:
                        print("Bottom");
                        TransformObject.position = new Vector3(bombPos.x, TransformObject.position.y, bombPos.z + 1f);
                        break;
                    case 3:
                        print("Left");
                        TransformObject.position = new Vector3(bombPos.x + 1f, TransformObject.position.y, bombPos.z);
                        break;
                }

                Position = TransformObject.position;
                RotateToNormal();

                StopMove?.Invoke(BallsManager.Check(this, 0));
            } 
            else if (!_isBall && collision.gameObject.CompareTag($"IceWall"))
            {
                IsNeedToMove = false;
            
                _rigidbody.velocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
            
                var bombPos = collision.gameObject.transform.position;
                switch (_direction)
                {
                    case 0:
                        print("Top");
                        TransformObject.position = new Vector3(bombPos.x, TransformObject.position.y, bombPos.z - 1f);
                        break;
                    case 1:
                    
                        print("Right");
                        TransformObject.position = new Vector3(bombPos.x - 1f, TransformObject.position.y, bombPos.z);
                        break;
                    case 2:
                        print("Bottom");
                        TransformObject.position = new Vector3(bombPos.x, TransformObject.position.y, bombPos.z + 1f);
                        break;
                    case 3:
                        print("Left");
                        TransformObject.position = new Vector3(bombPos.x + 1f, TransformObject.position.y, bombPos.z);
                        break;
                }

                Position = TransformObject.position;
                RotateToNormal();
            
            
                StopMove?.Invoke(BallsManager.Check(this, 0));
            }
            else if (!_isBall && collision.gameObject.CompareTag($"WoodenBox"))
            {
                IsNeedToMove = false;
            
                _rigidbody.velocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
            
                var bombPos = collision.gameObject.transform.position;
                switch (_direction)
                {
                    case 0:
                        print("Top");
                        TransformObject.position = new Vector3(bombPos.x, TransformObject.position.y, bombPos.z - 1f);
                        break;
                    case 1:
                        print("Right");
                        TransformObject.position = new Vector3(bombPos.x - 1f, TransformObject.position.y, bombPos.z);
                        break;
                    case 2:
                        print("Bottom");
                        TransformObject.position = new Vector3(bombPos.x, TransformObject.position.y, bombPos.z + 1f);
                        break;
                    case 3:
                        print("Left");
                        TransformObject.position = new Vector3(bombPos.x + 1f, TransformObject.position.y, bombPos.z);
                        break;
                }

                Position = TransformObject.position;
                RotateToNormal();
                
                StopMove?.Invoke(BallsManager.Check(this, 0));
            }
            else if (collision.gameObject.CompareTag($"Wall"))
            {
                var bombPos = collision.gameObject.transform.position;
                
                _isNeedToMove = false;
                _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
                
                switch (_direction)
                {
                    case 0:
                        transform.position = new Vector3(bombPos.x, transform.position.y, bombPos.z - 0.5f);
                        break;
                    case 1:
                        var x = bombPos.x - 0.5f;
                        transform.position = new Vector3(bombPos.x - 0.5f, transform.position.y, bombPos.z);
                        break;
                    case 2:
                        transform.position = new Vector3(bombPos.x, transform.position.y, bombPos.z + 0.5f);
                        break;
                    case 3:
                        transform.position = new Vector3(bombPos.x + 0.5f, transform.position.y, bombPos.z);
                        break;
                }

                Position = transform.position;
                _rigidbody.constraints = RigidbodyConstraints.None;
                StopMove?.Invoke(BallsManager.Check(this, 0));
            } 
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag($"Bomb"))
            {
                if (!IsNeedToMove) return;
                IsNeedToMove = false;
            
                _rigidbody.velocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
            
                var bombPos = other.gameObject.transform.position;
                switch (_direction)
                {
                    case 0:
                        print("Top");
                        TransformObject.position = new Vector3(bombPos.x, TransformObject.position.y, bombPos.z - 1f);
                        Position = _transform.position;
                        break;
                    case 1:
                    
                        print("Right");
                        TransformObject.position = new Vector3(bombPos.x - 1f, TransformObject.position.y, bombPos.z);
                        Position = _transform.position;
                        break;
                    case 2:
                        print("Bottom");
                        TransformObject.position = new Vector3(bombPos.x, TransformObject.position.y, bombPos.z + 1f);
                        Position = _transform.position;
                        break;
                    case 3:
                        print("Left");
                        TransformObject.position = new Vector3(bombPos.x + 1f, TransformObject.position.y, bombPos.z);
                        Position = _transform.position;
                        break;
                }

                RotateToNormal();
                
                StopMove?.Invoke(3);
            } 
            else if (other.gameObject.CompareTag($"FinishHole"))
            {
                IsNeedToMove = false;
            
                _rigidbody.velocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
            
                BallAnimator.Play("Exit");
                
                var finishHolePosition = other.gameObject.transform.position;

                TransformObject.position = finishHolePosition;
                _isFinished = true;
                StartCoroutine(SetInvisible());
                StopMove?.Invoke(BallsManager.Check(this, 1));
            }
            else if (other.gameObject.CompareTag("Rotator"))
             {
                 _isNeedToMove = false;
                 //SetupRotator(other.gameObject.transform);
                 var powerRotation = other.gameObject.transform.rotation;
                 var powerPosition = SetupRotator(other.gameObject.transform);
                 
                 _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
                 
                 var rotationY = powerRotation.eulerAngles.y;
                 switch (_direction)
                 {
                     case 0:
                         switch (rotationY)
                         {
                             case 0:
                                 print("TOP");
                                 transform.position = new Vector3(powerPosition.x, transform.position.y,
                                     powerPosition.z);
                                 _direction = 1;
                                 break;                        
                             case 90:
                                 print(powerPosition);
                                 transform.position = new Vector3(powerPosition.x, transform.position.y,
                                     powerPosition.z);
                                 _direction = 3;
                                 break;
                             case 180:
                                 print(powerPosition);
                                 transform.position = new Vector3(powerPosition.x, transform.position.y,
                                     powerPosition.z - 1);
                                 
                                 StopMove?.Invoke(3);
                                 break;
                             case 270:
                                 print(powerPosition);
                                 transform.position = new Vector3(powerPosition.x, transform.position.y,
                                     powerPosition.z - 1);
                                 _isNeedToRotate = true;
                                 
                                 StopMove?.Invoke(3);
                                 break;
                         }
                         break;
                     case 1:
                         switch (rotationY)
                         {
                             case 90:
                                 print(powerPosition);
                                 transform.position = new Vector3(powerPosition.x, transform.position.y,
                                     powerPosition.z);
                                 
                                 _direction = 2;
                                 break;
                             case 180:
                                 print(powerPosition);
                                 _transform.position = new Vector3(powerPosition.x, _transform.position.y,
                                     powerPosition.z);
                                 _direction = 0;
                                 break;
                             case 270:
                                 print(powerPosition);
                                 transform.position = new Vector3(powerPosition.x - 1, transform.position.y,
                                     powerPosition.z);
                                 
                                 StopMove?.Invoke(3);
                                 break;
                             case 0:
                                 print(powerPosition);
                                 transform.position = new Vector3(powerPosition.x - 1, transform.position.y,
                                     powerPosition.z);

                                 StopMove?.Invoke(3);
                                 break;
                         }
                         break;
                     case 2:
                         switch (rotationY)
                         {
                             case 180:
                                 print(powerPosition);
                                 transform.position = new Vector3(powerPosition.x, transform.position.y,
                                     powerPosition.z);
                                 _direction = 3;
                                 IsNeedToMove = true;
                                 break;
                             case 270:
                                 print(powerPosition);
                                 transform.position = new Vector3(powerPosition.x, transform.position.y,
                                     powerPosition.z);
                                 _direction = 1;
                                 break;
                             case 0:
                                 transform.position = new Vector3(powerPosition.x, transform.position.y,
                                     powerPosition.z + 1);
 
                                 StopMove?.Invoke(3);
                                 break;
                             case 90:
                                 transform.position = new Vector3(powerPosition.x, transform.position.y,
                                     powerPosition.z + 1);
                                    
                                 StopMove?.Invoke(3);
                                 break;
                             
                         }
                         break;
                     case 3:
                         switch (rotationY)
                         {
                             case 270:
                                 transform.position = new Vector3(powerPosition.x, transform.position.y,
                                     powerPosition.z);
                                 _direction = 0;
                                 IsNeedToMove = true;
                                 break;
                             case 0:
                                 transform.position = new Vector3(powerPosition.x, transform.position.y,
                                     powerPosition.z);
                                 _direction = 2;
                                 break;
                             case 90:
                                 transform.position = new Vector3(powerPosition.x + 1, transform.position.y,
                                     powerPosition.z);
                                 StopMove?.Invoke(3);
                                 break;
                             case 180:
                                 transform.position = new Vector3(powerPosition.x + 1, transform.position.y,
                                     powerPosition.z);
                                 StopMove?.Invoke(3);
                                 break;
                             
                         }
                         break;
                 }

                 _isNeedToMove = true;
                 _rigidbody.constraints = RigidbodyConstraints.None;
             }
            
        }

        private Vector3 SetupRotator(Transform transformRotator)
        {
            var transfromRotation = transformRotator.rotation.eulerAngles.y;
            var pos = transformRotator.position;
            switch (transfromRotation)
            {
                case 0:
                    pos += new Vector3(-0.05f, 0, 0.05f);
                    break;
                case 90:
                    pos += new Vector3(-0.05f, 0, -0.05f);
                    break;
                case 180:
                    pos += new Vector3(-0.05f, 0, 0.05f);
                    break;
                case 270:
                    pos += new Vector3(0.05f, 0, 0.05f);
                    break;
            }

            return pos;
        }

        private IEnumerator BreakBalls(Ball ball)
        {
            yield return new WaitForSeconds(0.4f);
            var parts = Instantiate(BallsManager.BrokenParts[_indexSO], BallsManager.BallsParent);
            parts.transform.position = TransformObject.position;
            parts.transform.rotation = TransformObject.rotation;
            
            var partsSecond = Instantiate(BallsManager.BrokenParts[ball.IndexSo], BallsManager.BallsParent);
            if (ball != null)
            {
                partsSecond.transform.position = ball.transform.position;
                partsSecond.transform.rotation = ball.transform.rotation;
            }

            BallsManager.BallsObject.Add(parts);
            BallsManager.BallsObject.Add(partsSecond);
            
            Destroy(gameObject);
            if (ball != null) Destroy(ball.gameObject);
        
        }
        
        private IEnumerator BreakBalls()
        {
            yield return new WaitForSeconds(0);
            var parts = Instantiate(BallsManager.BrokenParts[_indexSO], BallsManager.BallsParent);
            parts.transform.position = TransformObject.position;
            parts.transform.rotation = TransformObject.rotation;
            

        
            BallsManager.BallsObject.Add(parts);
            
            Destroy(gameObject);
        
        }
        public void RotateToNormal()
        {
            _isNeedToRotate = true;
        }
        public IEnumerator SetInvisible()
        {
            yield return new WaitForSeconds(0.7f);
        }

        private void StopBall()
        {
            IsNeedToMove = false;
            
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }

        private IEnumerator StopRotation()
        {
            yield return new WaitForSeconds(0.1f);
            
            _isNeedToRotate = false;
            transform.position = Position;
        }
    }
}
