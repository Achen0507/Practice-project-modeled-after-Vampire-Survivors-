using Survivor.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Survivor.MainMenu
{
    public class EnhanceItem : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Transform levelBlocksContainer;
        [SerializeField] private GameObject blockPrefab;

        private int maxLevel;

        public void SetData(UpgradeData data, int level) {
            icon.sprite = data.icon;
            maxLevel = data.maxLevel;
            CreateBlocks(level);
        }

        private void CreateBlocks(int level) {
            foreach (Transform child in levelBlocksContainer)
                Destroy(child.gameObject);

            for (int i = 0; i < maxLevel; i++)
            {
                GameObject block = Instantiate(blockPrefab, levelBlocksContainer);
                Image img = block.GetComponent<Image>();
                img.color = (i < level) ? Color.white : Color.gray;
            }
        }
    }
}
