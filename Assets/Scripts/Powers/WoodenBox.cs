using System;
using System.Threading.Tasks;
using UnityEngine;

namespace MossaGames.Powers
{
    public class WoodenBox : MonoBehaviour
    {
        [SerializeField] private GameObject _woodenBoxBrokenParts;
        private Transform _parent;
        private Action<Vector3> _removeWoodenBox;


        public void Init(Transform parent,  Action<Vector3> RemoveWoodenBox)
        {
            _removeWoodenBox = RemoveWoodenBox;
            _parent = parent;
        }

        public async void Destroy()
        {
            var parts = Instantiate(_woodenBoxBrokenParts, _parent);
            parts.transform.position = gameObject.transform.position;
            gameObject.SetActive(false);
            for (int i = 0; i < parts.transform.childCount; i++)
            {
                await WaitForDestroy(parts.transform.GetChild(i).gameObject);
                
            }
            Destroy(parts);
        }

        private async Task WaitForDestroy(GameObject part)
        {
            var end = Time.time + 0.1f;
            while (Time.time < end)
            {
                part.SetActive(false);
                await Task.Yield();
            }
        }

    }
}