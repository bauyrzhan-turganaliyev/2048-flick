using System;
using System.Collections.Generic;
using MossaGames.ObjectModel;
using MossaGames.Powers;
using UnityEngine;

namespace MossaGames.Managers
{
    public class PowersManager : MonoBehaviour
    {
        [SerializeField] private GameObject _rotator;
        [SerializeField] private GameObject _iceWall;
        [SerializeField] private GameObject _stoneWall;
        [SerializeField] private GameObject _woodenBox;
        [SerializeField] private GameObject _bomb;
        [SerializeField] private GameObject _multiBall;
        [SerializeField] private GameObject _finishHole;
        [SerializeField] private ParticleSystem _boomParticle;
        [SerializeField] private GameObject _poofParticle;
        [SerializeField] private PowerSetting[] _powerSettings;
        [SerializeField] private List<GameObject> _powers = new List<GameObject>();

        
        private Dictionary<Vector3, GameObject> _cells = new Dictionary<Vector3, GameObject>();
        private List<Vector3> _toRemoveCoors = new List<Vector3>();
        private Transform _powersParent;
        private GridManager _gridManager;

        private float _width;
        private float _height;
        private Action<int> _onBallStop;
        private Action _onBallMove;
        private MonoBehaviour _corutiner;
        private List<Material> _ballsMaterials;
        private BallsManager _ballsManager;
        private Transform _confettiesParent;
        private float _intensity;
        private int _targetValue;

        public void Init(GridManager gridManager,Transform powersParent, Action onBallMove, Action<int> onBallStop,
            MonoBehaviour corutiner, List<Material> ballsMaterials, BallsManager ballsManager, Transform confettiesParent, float intensity, int targetValue)
        {
            _targetValue = targetValue;
            _intensity = intensity;
            _confettiesParent = confettiesParent;
            _ballsManager = ballsManager;
            _ballsMaterials = ballsMaterials;
            _corutiner = corutiner;
            _onBallMove = onBallMove;
            _onBallStop = onBallStop;
            _gridManager = gridManager;
            _powersParent = powersParent;
            
            _width = _gridManager.Width;
            _height = _gridManager.Height;
            
            GeneratePowers();
        }        
        public void Clear()
        {
            _cells.Clear();
            _toRemoveCoors.Clear();
        }
        public void DestroyAllObjects(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                Destroy(parent.GetChild(i).gameObject);
            }
        }
        public int CheckBomb(Bomb bomb)
        {
            var boomParticle = Instantiate(_boomParticle, _confettiesParent);
            boomParticle.transform.position = bomb.Position;
            boomParticle.Play();
            var position = bomb.Position;
            for (int i = 3; i >= 1; i--)
            {
                foreach (var cell in _cells)
                {
                    print(cell.Value.gameObject);
                    if (Mathf.Abs(cell.Key.x - position.x) <= i && Mathf.Abs(cell.Key.z - position.z) <= i)
                    {
                        if (cell.Value.TryGetComponent<IceWall>(out IceWall iceWall))
                        {
                            iceWall.Destroy();
                            RemovePower(cell.Key);
                        }

                        if (cell.Value.TryGetComponent<WoodenBox>(out WoodenBox woodenBox))
                        {
                            var poofParticle = Instantiate(_poofParticle, _confettiesParent);
                            _poofParticle.transform.position = woodenBox.transform.position;
                            woodenBox.Destroy();
                            RemovePower(cell.Key);
                        }
                    }
                }
            }

            foreach (var coor in _toRemoveCoors)
            {
                _cells.Remove(coor);
            }
            _toRemoveCoors.Clear();
            return 3;
        }

        private void RemovePower(Vector3 pos)
        {
            _toRemoveCoors.Add(pos);
        }        
        private void RemovePowerAndDelete(Vector3 pos)
        {
            _cells.Remove(pos);
        }

        private void GeneratePowers()
        {
            for (int i = 0; i < _powerSettings.Length; i++)
            {
                switch (_powerSettings[i].Power)
                {
                    case PowerSetting.PowerObject.Rotator:
                        SpawnRotator(i);
                        break;
                    case PowerSetting.PowerObject.IceWall:
                        SpawnIceWall(i);
                        break;
                    case PowerSetting.PowerObject.StoneWall:
                        SpawnStoneWall(i);
                        break;
                    case PowerSetting.PowerObject.WoodenBox:
                        SpawnWoodenBox(i);
                        break;
                    case PowerSetting.PowerObject.Bomb:
                        SpawnBomb(i);
                        break;                    
                    case PowerSetting.PowerObject.MultiBall:
                        SpawnMultiBall(i);
                        break;     
                    case PowerSetting.PowerObject.FinishHole:
                        SpawnFinishHole(i);
                        break;
                }
            }
        }

        private void SpawnFinishHole(int i)
        {
            var x = -(_width / 2) + _powerSettings[i].Row;
            var z = (_height / 2) - _powerSettings[i].Column-1;
            
            var power = Instantiate(_finishHole, _powersParent);
            var powerComponent = power.GetComponent<FinishHole>();
            powerComponent.Init(_targetValue);
            _powers.Add(power);
            _cells.Add(new Vector3(x, 0.6f, z), power);
            power.transform.position = new Vector3(x, 0.6f, z);
        }

        private void SpawnMultiBall(int i)
        {
            var x = -(_width / 2) + _powerSettings[i].Row;
            var z = (_height / 2) - _powerSettings[i].Column - 1;
            
            var power = Instantiate(_multiBall, _powersParent);
            _powers.Add(power);
            power.transform.position = new Vector3(x, 1, z);
            var multiball = power.GetComponent<Ball>();
            multiball.Init(new Vector3(x, 1, z), -1, _ballsMaterials, -1, _ballsManager, _confettiesParent, _intensity);
            multiball.StartMove += _onBallMove;
            multiball.StopMove += _onBallStop;
        }
        private void SpawnBomb(int i)
        {
            var x = -(_width / 2) + _powerSettings[i].Row;
            var z = (_height / 2) - _powerSettings[i].Column -1;
            
            var power = Instantiate(_bomb, _powersParent);
            _powers.Add(power);
            var bomb = power.GetComponent<Bomb>();
            
            bomb.StartMove += _onBallMove;
            bomb.StopMove += _onBallStop;
            bomb.Init(new Vector3(x, .5f, z), this);
        }
        private void SpawnWoodenBox(int i)
        {
            var x = -(_width / 2) + _powerSettings[i].Row;
            var z = (_height / 2) - _powerSettings[i].Column-1;
            
            var power = Instantiate(_woodenBox, _powersParent);
            var woodenBoxComponent = power.GetComponent<WoodenBox>();
            woodenBoxComponent.Init(_powersParent, RemovePower);
            _powers.Add(power);
            _cells.Add(new Vector3(x, 0.6f, z), power);
            power.transform.position = new Vector3(x, 0.6f, z);
        }
        private void SpawnStoneWall(int i)
        {
            var x = -(_width / 2) + _powerSettings[i].Row;
            var z = (_height / 2) - _powerSettings[i].Column-1;
            
            var power = Instantiate(_stoneWall, _powersParent);
            _powers.Add(power);
            power.transform.position = new Vector3(x, 0, z);
        }
        private void SpawnIceWall(int i)
        {
            var x = -(_width / 2) + _powerSettings[i].Row;
            var z = (_height / 2) - _powerSettings[i].Column-1;
            
            var power = Instantiate(_iceWall, _powersParent);
            _powers.Add(power);
            var iceWallC = power.GetComponent<IceWall>();
            iceWallC.Init(_corutiner, _powersParent, RemovePower, RemovePowerAndDelete);
            _cells.Add(new Vector3(x, 0.1f, z), power);
            power.transform.position = new Vector3(x, 0.1f, z);
        }
        private void SpawnRotator(int i)
        {
            var x = -(_width / 2) + _powerSettings[i].Row;
            var z = (_height / 2) - _powerSettings[i].Column-1;
            
            if (_powerSettings[i].Direction == PowerSetting.DirectionSetting.LeftTop)
            {
                var power = Instantiate(_rotator, _powersParent);
                _powers.Add(power);
                print(x + " " + z);
                
                power.transform.position = new Vector3(x - 0.05f, 0.1f, z + 0.05f);
            } 
            else if (_powerSettings[i].Direction == PowerSetting.DirectionSetting.RightTop)
            {
                var power = Instantiate(_rotator, _powersParent);
                
                _powers.Add(power);
                print(x + " " + z);
                
                power.transform.Rotate(0, 90, 0);
                power.transform.position = new Vector3(x + 0.05f, 0.1f, z + 0.05f);
            }           
            else if (_powerSettings[i].Direction == PowerSetting.DirectionSetting.RightBottom)
            {
                var power = Instantiate(_rotator, _powersParent);
                _powers.Add(power);
                
                print(x + " " + z);
                
                power.transform.Rotate(0, 180, 0);
                power.transform.position = new Vector3(x + 0.05f, 0.1f, z - 0.05f);
            }
            else if (_powerSettings[i].Direction == PowerSetting.DirectionSetting.LeftBottom)
            {
                var power = Instantiate(_rotator, _powersParent);
                _powers.Add(power);
                
                print(x + " " + z);
                
                power.transform.Rotate(0, 270, 0);
                power.transform.position = new Vector3(x - 0.05f, 0.1f, z - 0.05f);
            }
        }
    }
    
    [System.Serializable]
    public class PowerSetting
    {
        public int Row;
        public int Column;
        public PowerObject Power;
        public enum PowerObject{ Rotator, IceWall, StoneWall, WoodenBox, Bomb, MultiBall, FinishHole }
        public DirectionSetting Direction;
        public enum DirectionSetting{Left, Right, Top, Bottom, LeftTop, RightTop, LeftBottom, RightBottom, Center }
    }
}
