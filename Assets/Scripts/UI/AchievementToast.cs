using DG.Tweening;
using Survivor.Data;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace Survivor.UI
{
    /// <summary>
    /// 成就弹窗
    /// </summary>
    public class AchievementToast : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Image icon;
        [SerializeField] private Text titleText;
        [SerializeField] private Text descText;
        [SerializeField] private CanvasGroup canvasGroup;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        public void Initialize(AchievementData achievement)
        {
            string localizedName = LocalizationSettings.StringDatabase.GetLocalizedString("UIText", achievement.nameKey);
            string localizedDesc = LocalizationSettings.StringDatabase.GetLocalizedString("UIText", achievement.descKey);

            if (icon != null) icon.sprite = achievement.icon;
            if (titleText != null) titleText.text = localizedName;
            if (descText != null) descText.text = localizedDesc;

            // 显示动画
            canvasGroup.alpha = 0;
            gameObject.SetActive(true);

            Sequence seq = DOTween.Sequence();
            seq.Append(canvasGroup.DOFade(1, 0.3f));
            seq.AppendInterval(2f);
            seq.Append(canvasGroup.DOFade(0, 0.3f));
            seq.OnComplete(() => Destroy(gameObject));
        }
    }
}
