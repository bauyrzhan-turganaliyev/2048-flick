using System;
using MossaGames.Managers;
using MossaGames.ObjectModel;
using UnityEngine;

namespace MossaGames.Powers
{
    public class MultiBall : MonoBehaviour
    {
        [SerializeField] private Animator _ballAnimator;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Vector3 _position;
        [SerializeField] private int _force;
        [SerializeField] private int _speed;
        
        public Action StartMove;
        public Action<int> StopMove;

        private Action<Bomb> _checkBomb;
        private bool _isNeedToMove;
        private bool _isNeedToRotate;
        private int _direction;
        private int _indexSO;
        private PowersManager _powersManager;

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

        public Animator BallAnimator
        {
            get => _ballAnimator;
            set => _ballAnimator = value;
        }


        public void Init(Vector3 position, PowersManager powersManager)
        {
            _powersManager = powersManager;
            Position = position;
            transform.position = Position;
        }
        public void MoveToDirection(int direction)
        {
            _direction = direction;
            _isNeedToMove = true;
            print("Moving");
        
            StartMove?.Invoke();
        }

        private void Update()
        {
            if (!_isNeedToMove) return;
            switch (_direction)
            {
                case 0:
                    RigidbodyObject.AddForce(new Vector3(0, 0, _force * Time.deltaTime * _speed));
                    break;
                case 1:
                    RigidbodyObject.AddForce(new Vector3(_force * Time.deltaTime * _speed, 0, 0));
                    break;
                case 2:
                    RigidbodyObject.AddForce(new Vector3(0, 0, -_force * Time.deltaTime * _speed));
                    break;
                case 3:
                    RigidbodyObject.AddForce(new Vector3(-_force * Time.deltaTime* _speed, 0, 0));
                    break;
            }
        }
        
        private void OnCollisionEnter(Collision collision)
        {
            Ball ball = new Ball();
            bool _isBall = false;
            if (collision.gameObject.TryGetComponent<Ball>(out Ball ballComp))
            {
                ball = ballComp;
                _isBall = true;
            }
            if (_isNeedToMove && _isBall)
                {
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
                
                    ball.Value *= 2;
                    //ball.ValueText.text = ball.Value.ToString();
                
                    ball.RigidbodyObject.constraints = RigidbodyConstraints.None;
                    ball.RotateToNormal();

                    ball.IndexSo++;
                    ball.BallMaterial.material = ball.BallsMaterials[ball.IndexSo];
                
                    GameObject confetty = Instantiate(ball.BallsManager.Confetties[ball.IndexSo], ball.ConfettiesParent);
                    confetty.transform.position = transform.position;
                    
                    StopMove?.Invoke(ball.BallsManager.Check(ball, 0));
                
                    Destroy(gameObject);
                }

            if (!_isBall && collision.gameObject.CompareTag($"Wall"))
            {
                _isNeedToMove = false;
            
                _rigidbody.velocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
            
                var wallPos = collision.gameObject.transform.position;
                switch (_direction)
                {
                    case 0:
                        print("Top");
                        transform.position = new Vector3(wallPos.x, transform.position.y, wallPos.z - 0.5f);

                        Position = transform.position;
                        break;
                    case 1:
                    
                        print("Right");
                        transform.position = new Vector3(wallPos.x - 0.5f, transform.position.y, wallPos.z);
                    
                        Position = transform.position;
                        break;
                    case 2:
                        print("Bottom");
                        transform.position = new Vector3(wallPos.x, transform.position.y, wallPos.z + 0.5f);
                    
                        Position = transform.position;
                        break;
                    case 3:
                        print("Left");
                        transform.position = new Vector3(wallPos.x + 0.5f, transform.position.y, wallPos.z);
                    
                        Position = transform.position;
                        break;
                }
                StopMove?.Invoke(3);
            } 
            else if (!_isBall && collision.gameObject.CompareTag($"StoneWall"))
            {
                _isNeedToMove = false;
            
                _rigidbody.velocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
            
                var wallPos = collision.gameObject.transform.position;
                switch (_direction)
                {
                    case 0:
                        print("Top");
                        transform.position = new Vector3(wallPos.x, transform.position.y, wallPos.z - 1f);

                        Position = transform.position;
                        break;
                    case 1:
                    
                        print("Right");
                        transform.position = new Vector3(wallPos.x - 1f, transform.position.y, wallPos.z);
                    
                        Position = transform.position;
                        break;
                    case 2:
                        print("Bottom");
                        transform.position = new Vector3(wallPos.x, transform.position.y, wallPos.z + 1f);
                    
                        Position = transform.position;
                        break;
                    case 3:
                        print("Left");
                        transform.position = new Vector3(wallPos.x + 1f, transform.position.y, wallPos.z);
                    
                        Position = transform.position;
                        break;
                }

                StopMove?.Invoke(3);
            } 
            else if (!_isBall && collision.gameObject.CompareTag($"IceWall"))
            {
                _isNeedToMove = false;
            
                _rigidbody.velocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
            
                var wallPos = collision.gameObject.transform.position;
                switch (_direction)
                {
                    case 0:
                        print("Top");
                        transform.position = new Vector3(wallPos.x, transform.position.y, wallPos.z - 1f);

                        Position = transform.position;
                        break;
                    case 1:
                    
                        print("Right");
                        transform.position = new Vector3(wallPos.x - 1f, transform.position.y, wallPos.z);
                    
                        Position = transform.position;
                        break;
                    case 2:
                        print("Bottom");
                        transform.position = new Vector3(wallPos.x, transform.position.y, wallPos.z + 1f);
                    
                        Position = transform.position;
                        break;
                    case 3:
                        print("Left");
                        transform.position = new Vector3(wallPos.x + 1f, transform.position.y, wallPos.z);
                    
                        Position = transform.position;
                        break;
                }
                StopMove?.Invoke(3);
            }
            
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Rotator"))
            {
                var powerRotation = other.gameObject.transform.rotation;
                var powerPosition = other.gameObject.transform.position;

                var rotationY = powerRotation.eulerAngles.y;
                print(rotationY);
                print(_direction);
                switch (_direction)
                {
                    case 0:
                        switch (rotationY)
                        {
                            case 0:
                                _rigidbody.velocity = Vector3.zero;
                                _rigidbody.angularVelocity = Vector3.zero;
                                _direction = 1;
                                transform.position = new Vector3(powerPosition.x, transform.position.y,
                                    powerPosition.z);
                                break;                        
                            case 90:
                                _rigidbody.velocity = Vector3.zero;
                                _rigidbody.angularVelocity = Vector3.zero;
                                _direction = 3;
                                transform.position = new Vector3(powerPosition.x, transform.position.y,
                                    powerPosition.z);
                                break;
                            case 180:
                                _isNeedToMove = false;
                                _rigidbody.velocity = Vector3.zero;
                                _rigidbody.angularVelocity = Vector3.zero;
                                transform.position = new Vector3(powerPosition.x, transform.position.y,
                                    powerPosition.z - 1);

                                _isNeedToRotate = true;
                                
                                StopMove?.Invoke(3);
                                break;
                            case 270:
                                _isNeedToMove = false;
                                _rigidbody.velocity = Vector3.zero;
                                _rigidbody.angularVelocity = Vector3.zero;
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
                                _rigidbody.velocity = Vector3.zero;
                                _rigidbody.angularVelocity = Vector3.zero;
                                _direction = 2;
                                transform.position = new Vector3(powerPosition.x, transform.position.y,
                                    powerPosition.z);
                                break;
                            case 180:
                                _rigidbody.velocity = Vector3.zero;
                                _rigidbody.angularVelocity = Vector3.zero;
                                _direction = 0;
                                transform.position = new Vector3(powerPosition.x, transform.position.y,
                                    powerPosition.z);
                                break;
                            case 270:
                                _isNeedToMove = false;
                                _rigidbody.velocity = Vector3.zero;
                                _rigidbody.angularVelocity = Vector3.zero;
                                transform.position = new Vector3(powerPosition.x - 1, transform.position.y,
                                    powerPosition.z);
                                
                                _isNeedToRotate = true;
                                
                                StopMove?.Invoke(3);
                                break;
                            case 0:
                                _isNeedToMove = false;
                                _rigidbody.velocity = Vector3.zero;
                                _rigidbody.angularVelocity = Vector3.zero;
                                transform.position = new Vector3(powerPosition.x - 1, transform.position.y,
                                    powerPosition.z);
                                
                                _isNeedToRotate = true;
                                
                                StopMove?.Invoke(3);
                                break;
                        }
                        break;
                    case 2:
                        switch (rotationY)
                        {
                            case 180:
                                _rigidbody.velocity = Vector3.zero;
                                _rigidbody.angularVelocity = Vector3.zero;
                                _direction = 3;
                                transform.position = new Vector3(powerPosition.x, transform.position.y,
                                    powerPosition.z);
                                break;
                            case 270:
                                _rigidbody.velocity = Vector3.zero;
                                _rigidbody.angularVelocity = Vector3.zero;
                                _direction = 1;
                                transform.position = new Vector3(powerPosition.x, transform.position.y,
                                    powerPosition.z);
                                break;
                            case 0:
                                _isNeedToMove = false;
                                _rigidbody.velocity = Vector3.zero;
                                _rigidbody.angularVelocity = Vector3.zero;
                                transform.position = new Vector3(powerPosition.x, transform.position.y,
                                    powerPosition.z + 1);

                                _isNeedToRotate = true;
                                
                                StopMove?.Invoke(3);
                                break;
                            case 90:
                                _isNeedToMove = false;
                                _rigidbody.velocity = Vector3.zero;
                                _rigidbody.angularVelocity = Vector3.zero;
                                transform.position = new Vector3(powerPosition.x, transform.position.y,
                                    powerPosition.z + 1);
                                
                                _isNeedToRotate = true;
                                StopMove?.Invoke(3);
                                break;
                            
                        }
                        break;
                    case 3:
                        switch (rotationY)
                        {
                            case 270:
                                print("EEE");
                                _rigidbody.velocity = Vector3.zero;
                                _rigidbody.angularVelocity = Vector3.zero;
                                _direction = 0;
                                transform.position = new Vector3(powerPosition.x, transform.position.y,
                                    powerPosition.z);
                                break;
                            case 0:
                                _rigidbody.velocity = Vector3.zero;
                                _rigidbody.angularVelocity = Vector3.zero;
                                _direction = 2;
                                transform.position = new Vector3(powerPosition.x, transform.position.y,
                                    powerPosition.z);
                                break;
                            case 90:
                                _isNeedToMove = false;
                                _rigidbody.velocity = Vector3.zero;
                                _rigidbody.angularVelocity = Vector3.zero;
                                _isNeedToRotate = true;
                                transform.position = new Vector3(powerPosition.x + 1, transform.position.y,
                                    powerPosition.z);
                                
                                StopMove?.Invoke(3);
                                break;
                            case 180:
                                _isNeedToMove = false;
                                _rigidbody.velocity = Vector3.zero;
                                _rigidbody.angularVelocity = Vector3.zero;
                                _isNeedToRotate = true;
                                transform.position = new Vector3(powerPosition.x + 1, transform.position.y,
                                    powerPosition.z);
                                
                                StopMove?.Invoke(3);
                                break;
                            
                        }
                        break;
                }
            }
        }
    }
}