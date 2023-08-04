using UnityEngine;

namespace Game.Components.UI
{
    public class LoadingAnimation : MonoBehaviour
    {
        [SerializeField] private float speed = 50f;

        private void Update()
        {
            transform.Rotate(Vector3.forward, speed * Time.deltaTime);
        }
    }
}