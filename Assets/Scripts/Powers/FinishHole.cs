using System;
using MossaGames.ObjectModel;
using TMPro;
using UnityEngine;

namespace MossaGames.Powers
{
    public class FinishHole : MonoBehaviour
    {

        [SerializeField] private TMP_Text _targetValueText;

        public void Init(int targetValue)
        {
            _targetValueText.text = targetValue.ToString();
        }
    }
}