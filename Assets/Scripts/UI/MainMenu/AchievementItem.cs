using Survivor.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Survivor.UI
{
    /// <summary>
    /// 成就从data动态生成
    /// </summary>
    public class AchievementItem : MonoBehaviour
    {
        public Image icon;
        public Text descText;
        public Image statusImage;

        public void SetData(AchievementData data, string desc, bool isCompleted) {
            icon.sprite = data.icon;
            descText.text = desc;
            statusImage.sprite = isCompleted ? data.completedIcon : data.incompleteIcon;
        }
    }
}
