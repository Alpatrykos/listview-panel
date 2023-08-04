using System;
using System.Diagnostics.CodeAnalysis;
using Game.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Components.UI
{
    public sealed class ListItem : MonoBehaviour
    {
        [SerializeField] [NotNull] private SpriteContainer gemSprites;
        [SerializeField] [NotNull] private Image gemImage;
        [SerializeField] [NotNull] private GameObject glow;

        [SerializeField] [NotNull] private TextMeshProUGUI description;
        [SerializeField] [NotNull] private TextMeshProUGUI number;

        private DataItem _dataItem;

        public void Set(int count, DataItem dataItem)
        {
            gameObject.SetActive(true);
            _dataItem = dataItem;

            glow.SetActive(_dataItem.Special);
            gemImage.sprite = GetGemSpriteByItemCategory(dataItem.Category);
            description.text = dataItem.Description;
            number.text = (count + 1).ToString();
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }

        private Sprite GetGemSpriteByItemCategory(DataItem.CategoryType category)
        {
            return category switch
            {
                DataItem.CategoryType.RED => gemSprites.items[0],
                DataItem.CategoryType.BLUE => gemSprites.items[1],
                DataItem.CategoryType.GREEN => gemSprites.items[2],
                _ => throw new ArgumentException($"Category {category} is not supported.")
            };
        }
    }
}