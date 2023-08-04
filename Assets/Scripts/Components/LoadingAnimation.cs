using System;
using UnityEngine;

namespace Components
{
    public class LoadingAnimation : MonoBehaviour
    {
        [SerializeField]
        private float speed = 50f;
        private void Update()
        {
            transform.Rotate(Vector3.forward, speed * Time.deltaTime);
            
        }
    }
}