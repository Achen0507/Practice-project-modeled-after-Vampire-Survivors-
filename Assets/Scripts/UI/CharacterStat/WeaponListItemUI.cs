using UnityEngine;
using UnityEngine.UI;

namespace Survivor.UI
{
    /// <summary>
    /// 局内武器等级和ui图标动态获取
    /// </summary>
    public class WeaponListItemUI : MonoBehaviour
    {
        [Header("图标")]
        [SerializeField] private Image iconImage;

        [Header("等级方块")]
        [SerializeField] private Transform levelBlocksContainer;
        [SerializeField] private GameObject blockPrefab;  

        public void Initialize(Sprite icon, int currentLevel)
        {
            if (iconImage != null && icon != null)
                iconImage.sprite = icon;

            CreateBlocks(currentLevel);
        }

        private void CreateBlocks(int count)
        {
            foreach (Transform child in levelBlocksContainer)
            {
                Destroy(child.gameObject);
            }

            // 根据等级创建对应数量的方块
            for (int i = 0; i < count; i++)
            {
                GameObject block = Instantiate(blockPrefab, levelBlocksContainer);
                Image img = block.GetComponent<Image>();
                if (img != null)
                {
                    img.enabled = true;  // 强制启用
                }
            }
        }

        /// <summary>
        /// 升级时更新方块数量
        /// </summary>
        public void SetLevel(int level)
        {
            CreateBlocks(level);
        }
    }
}