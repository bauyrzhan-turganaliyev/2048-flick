using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using MossaGames.Managers;
using MossaGames.ObjectModel;
using UnityEngine;

namespace MossaGames.Powers
{
    public class Bomb : MonoBehaviour
    {
        
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Vector3 _position;
        [SerializeField] private int _force;
        [SerializeField] private int _speed;
        
        public Action StartMove;
        public Action<int> StopMove;

        private BallsManager _ballsManager;
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

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent<Ball>(out Ball ball))
            {
                _isNeedToMove = false;
            
                _rigidbody.velocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;

                StopMove?.Invoke(_powersManager.CheckBomb(this));
                Destroy(gameObject);
            }
            else if (!other.gameObject.CompareTag("Untagged"))
            {
                _isNeedToMove = false;
            
                _rigidbody.velocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
            
                var wallPos = other.gameObject.transform.position;
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
                
                StopMove?.Invoke(_powersManager.CheckBomb(this));
                Destroy(gameObject);
            }
        }
    }
}