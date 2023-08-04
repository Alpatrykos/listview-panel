using UnityEngine;
using UnityEngine.Serialization;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "ImageContainer", menuName = "Containers/Image", order = 0)]
    public class SpriteContainer : ScriptableObject
    {
        public Sprite[] items;
    }
}