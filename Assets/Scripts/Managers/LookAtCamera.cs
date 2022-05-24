using UnityEngine;

namespace MossaGames.Managers
{
    public class LookAtCamera : MonoBehaviour
    {
        private Camera camera;

        private void Start()
        {
            camera = Camera.main;
        }
        private void Update()
        {
            transform.LookAt(transform.position + camera.transform.rotation * Vector3.back, new Quaternion(camera.transform.rotation.x, camera.transform.rotation.y, camera.transform.rotation.z, 0) * Vector3.up);
        }
    }
}

