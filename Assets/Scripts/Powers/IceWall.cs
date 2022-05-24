using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace MossaGames.Powers
{
    public class IceWall : MonoBehaviour
    {
        [SerializeField] private GameObject _iceBrokenParts;
        private MonoBehaviour _corutinner;
        private Transform _parent;
        private Action<Vector3> _removeIce;
        private Action<Vector3> _removeIceAndDelete;

        public void Init(MonoBehaviour corutinner, Transform parent, Action<Vector3> RemoveIce, Action<Vector3> RemoveIceAndDelete)
        {
            _removeIce = RemoveIce;
            _parent = parent;
            _corutinner = corutinner;
            _removeIceAndDelete = RemoveIceAndDelete;
        }
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Ball"))
            {
                _removeIceAndDelete.Invoke(transform.position);
                Destroy();
            }
        }

        public async void Destroy()
        {
            
            var parts = Instantiate(_iceBrokenParts, _parent);
            parts.transform.position = gameObject.transform.position;
            gameObject.SetActive(false);
            if (gameObject == null) return;
            
            if (parts == null) return;
            if (parts.transform == null) return;
            for (int i = 0; i < parts.transform.childCount; i++)
            {
                if (gameObject == null) return;
                if (parts.transform.GetChild(i).gameObject == null) return;
                await WaitForDestroy(parts.transform.GetChild(i).gameObject);
            }
        }

        private async Task WaitForDestroy(GameObject part)
        {
            var end = Time.time + 0.1f;
            while (Time.time < end)
            {
                if (part != null) part.SetActive(false);
                await Task.Yield();
            }
        }
        

    }
}
