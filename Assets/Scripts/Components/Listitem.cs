using System;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Components
{
    public class ListItem : MonoBehaviour
    {
        [SerializeField] private SpriteContainer gemSprites;
        [SerializeField] private Image gemImage;
        [SerializeField] private GameObject glow;

        [SerializeField] private TextMeshProUGUI description;
        [SerializeField] private TextMeshProUGUI number;

        private DataItem _dataItem;

        public void Set(int count, DataItem dataItem)
        {
            gameObject.SetActive(true);
            _dataItem = dataItem;

            glow.SetActive(_dataItem.Special);
            gemImage.sprite = GetGemSpriteByItemCategory(dataItem.Category);
            description.text = dataItem.Description;
            number.text = (count+1).ToString();
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }

        private Sprite GetGemSpriteByItemCategory(DataItem.CategoryType category)
        {
            return category switch
            {
                // explicit pattern matching is better than implicit enum casting in this case
                // enum casting would be ok if CategoryType enum would have specified constant values as such
                // RED = 0, etc.
                DataItem.CategoryType.RED => gemSprites.items[0],
                DataItem.CategoryType.BLUE => gemSprites.items[1],
                DataItem.CategoryType.GREEN => gemSprites.items[2],
                _ => throw new ArgumentException($"Category {category} is not supported.")
            };
        }
    }
}