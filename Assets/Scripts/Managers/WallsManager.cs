using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace MossaGames.Managers
{
    public class WallsManager : MonoBehaviour
    {
        [SerializeField] private GameObject _hWall;
        [SerializeField] private GameObject _vWall;
        [SerializeField] private WallSetting[] _wallSettings;

        [SerializeField] private List<GameObject> _walls = new List<GameObject>();

        private GridManager _gridManager;
        private Transform _parent;
    
        public void Init(GridManager gridManager, Transform parent)
        {
            _gridManager = gridManager;
            _parent = parent;
        
            GenerateWalls();
        }

        public void Clear()
        {
        }
        public void DestroyAllObjects(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                Destroy(parent.GetChild(i).gameObject);
            }
        }

        private void GenerateWalls()
        {
            for (int i = 0; i < _wallSettings.Length; i++)
            {
                var width = _gridManager.Width;
                var height = _gridManager.Height;
            
                var x = -(width/2) + _wallSettings[i].Row;
                var z = (height/2) - _wallSettings[i].Column - 1;
                if (_wallSettings[i].Direction == WallSetting.DirectionSetting.Left)
                {
                    var wall = Instantiate(_vWall, _parent);
                    _walls.Add(wall);
                    wall.transform.position = new Vector3(x - 0.5f, .1f, z);
                } 
                else if (_wallSettings[i].Direction == WallSetting.DirectionSetting.Right)
                {
                    var wall = Instantiate(_vWall, _parent);
                    _walls.Add(wall);
                    wall.transform.position = new Vector3(x + 0.5f, .1f, z);
                }
                else if (_wallSettings[i].Direction == WallSetting.DirectionSetting.Top)
                {
                    var wall = Instantiate(_hWall, _parent);
                    _walls.Add(wall);
                    wall.transform.position = new Vector3(x, .1f, z + 0.5f);
                }
                else if (_wallSettings[i].Direction == WallSetting.DirectionSetting.Bottom)
                {
                    var wall = Instantiate(_hWall, _parent);
                    _walls.Add(wall);
                    wall.transform.position = new Vector3(x, .1f, z - 0.5f);
                }
            }
        }
    
    }

    [System.Serializable]
    public class WallSetting
    {
        public int Row;
        public int Column;
        public DirectionSetting Direction;
        public enum DirectionSetting{ Left, Right, Top, Bottom }
    }
}