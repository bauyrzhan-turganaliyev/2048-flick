using MossaGames.ObjectModel;
using MossaGames.Powers;
using MossaGames.StateMachine;
using UnityEngine;

namespace MossaGames.Managers
{
    public class MouseManager : MonoBehaviour
    {
        [SerializeField] private LineRenderer _lineRenderer;

        private BattleSystem _battleSystem;
    
        private Vector3 _startPoint;
        private Vector3 _finishPoint;

        private bool _isMouseDown;
        private int _type;
        
        private Ball _currentMovingBall;
        private Bomb _currentMovingBomb;
        private MultiBall _currentMovingMultiBall;


        public Vector3 worldPosition;
        Plane plane = new Plane(Vector3.up, 0);

        public void Init(BattleSystem battleSystem)
        {
            _battleSystem = battleSystem;
            _lineRenderer.positionCount = 2;

        }
        private void Update()
        {
            if (_battleSystem.State.GetType() != typeof(PlayerTurn))
                return;
        
            if (Input.GetMouseButtonDown (0)) {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
 
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    if(hit.collider.gameObject.TryGetComponent<Ball>(out Ball ball))
                    {
                        _currentMovingBall = ball;
                        _startPoint = ball.BallPrefab.transform.position;
                        _isMouseDown = true;
                        _type = 1;

                    }else if(hit.collider.gameObject.TryGetComponent<Bomb>(out Bomb bomb))
                    {
                        _currentMovingBomb = bomb;
                        _startPoint = bomb.Position;
                        _isMouseDown = true;
                        _type = 2;
                    }else if(hit.collider.gameObject.TryGetComponent<MultiBall>(out MultiBall multiBall))
                    {
                        _currentMovingMultiBall = multiBall;
                        _startPoint = multiBall.Position;
                        _isMouseDown = true;
                        _type = 3;
                    }
                    else
                    {
                        _type = 0;
                        _isMouseDown = false;
                    }
                }    
            }
            if (Input.GetMouseButton(0) && _isMouseDown)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitData;
                if (Physics.Raycast(ray, out hitData, Mathf.Infinity))
                {
                    worldPosition = hitData.point;
                }
                _lineRenderer.SetPosition(0, new Vector3(_startPoint.x, 1f, _startPoint.z));
                _lineRenderer.SetPosition(1, new Vector3(worldPosition.x, 1f, worldPosition.z));
            }


            if (Input.GetMouseButtonUp(0) && _isMouseDown)
            {
                float distance;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (plane.Raycast(ray, out distance))
                {
                    worldPosition = ray.GetPoint(distance);
                }

                _finishPoint = new Vector3(worldPosition.x, 1, worldPosition.z);
            
                _lineRenderer.SetPosition(0, new Vector3(0, 0f, 0));
                _lineRenderer.SetPosition(1, new Vector3(0, 0f, 0));

            
                StartCalculation();

                if (_type == 1) _currentMovingBall = new Ball();
                else if (_type == 2) _currentMovingBomb = new Bomb();
                else if (_type == 3) _currentMovingMultiBall = new MultiBall();
                _isMouseDown = false;
                _type = 0;
            }
        }
    
    
        private void StartCalculation()
        {
            var direction = _finishPoint - _startPoint;
            Debug.DrawRay(_finishPoint, direction, Color.black);

            float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            var direct = 0;
        
            if ((angle >= -45 && angle <= 0) || (angle >= 0 && angle <= 45))
            {
                direct = 0;
            } else if ((angle >= 45 && angle <= 90) || (angle >= 90 && angle <= 135))
            {
                direct = 1;
            } else if ((angle >= 135 && angle <= 180) || (angle >= -180 && angle <= -135))
            {
                direct = 2;
            } else if ((angle >= -135 && angle <= -90) || (angle >= -90 && angle <= -45))
            {
                direct = 3;
            }
            if (_type == 1) _currentMovingBall.MoveToDirection(direct);
            else if (_type == 2) _currentMovingBomb.MoveToDirection(direct);
            else if (_type == 3) _currentMovingMultiBall.MoveToDirection(direct);
        }
    }
}
