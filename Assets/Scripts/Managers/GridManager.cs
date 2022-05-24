using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using MossaGames.ObjectModel;
using UnityEngine;

namespace MossaGames.Managers
{
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private Cell _cell;
        [SerializeField] private Cell _topLeftCell;
        [SerializeField] private Cell _topRightCell;
        [SerializeField] private Cell _bottomLeftCell;
        [SerializeField] private Cell _bottomRightCell;
        
        [SerializeField] private GameObject _topBorder;
        [SerializeField] private GameObject _rightBorder;
        [SerializeField] private GameObject _bottomBorder;
        [SerializeField] private GameObject _leftBorder;
        
        [SerializeField] private GameObject _breaker;

        private List<GameObject> _cells = new List<GameObject>();
    
        private float _height;
        private float _width;

        private Transform _parent;

        public float Height
        {
            get => _height;
            set => _height = value;
        }

        public float Width
        {
            get => _width;
            set => _width = value;
        }

        public void Init(int gridHeight, int gridWidth, Transform parent)
        {
            _height = gridHeight;
            _width = gridWidth;
            _parent = parent;
            
            Clear();
            GenerateGrid();
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
        private void GenerateGrid()
        {
            for (float i = -Height/2; i < Height/2; i++)
            {
                for (float j = -Width/2; j < Width/2; j++)
                {
                    if (i == -Height / 2 && j == -Width / 2)
                    {
                        var cell = Instantiate(_bottomLeftCell.CellPrefab, _parent);
                        var breaker = Instantiate(_breaker, _parent);
                        CellSetting(breaker, i-2, j-2);
                        CellSetting(cell, i, j);
                    }                  
                    else if (i == -Height/2 && j + 1 == Width/2)
                    {
                        var cell = Instantiate(_bottomRightCell.CellPrefab, _parent);
                        
                        var breaker2 = Instantiate(_breaker, _parent);
                        breaker2.transform.Rotate(0, 90, 0);
                        CellSetting(breaker2, i-4, j-2);
                        var breaker = Instantiate(_breaker, _parent);
                        CellSetting(breaker, i+2, j+2);
                        CellSetting(cell, i, j);
                    }
                    else if (i + 1 == Height/2 && j == -Width / 2)
                    {
                        var cell = Instantiate(_topLeftCell.CellPrefab, _parent);
                        var breaker = Instantiate(_breaker, _parent);
                        breaker.transform.Rotate(0, 90, 0);
                        CellSetting(breaker, i+4, j+2);
                        CellSetting(cell, i, j);
                    }
                    else if (i + 1 == Height/2 && j + 1 == Width / 2)
                    {
                        var cell = Instantiate(_topRightCell.CellPrefab, _parent);
                        CellSetting(cell, i, j);
                    }
                    else
                    {
                        var cell = Instantiate(_cell.CellPrefab, _parent);
                        CellSetting(cell, i, j);
                    }


                    if (i == -Height / 2 && (j > -Width/2 && j < Width/2 - 1))
                    {
                        GenerateDecorations(_bottomBorder, i, j);
                    }
                    else if (j == -Width / 2 && (i > -Height/2 && i < Height/2 - 1))
                    {
                        GenerateDecorations(_leftBorder, i, j);
                    }
                    else if (i + 1 == Height / 2 && (j > -Width/2 && j < Width/2 - 1))
                    {
                        GenerateDecorations(_topBorder, i, j);
                    }
                    else if (j + 1 == Width / 2 && (i > -Height/2 && i < Height/2 - 1))
                    {
                        GenerateDecorations(_rightBorder, i, j);
                    }
                }
            }
        }

        private void GenerateDecorations(GameObject decor, float i, float j)
        {
            var decorObject = Instantiate(decor, _parent);

            decorObject.transform.position = new Vector3(j, 0.1f, i);
        }

        private void CellSetting(GameObject cell, float i, float j)
        {
            _cells.Add(cell);
            cell.transform.position = new Vector3(j, 0.1f, i);
        }
    }
}
