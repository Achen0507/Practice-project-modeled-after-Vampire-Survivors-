using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Survivor.UI
{
    /// <summary>
    /// 伤害数字
    /// </summary>
    public class DamageText : MonoBehaviour
    {
        private Text text;

        private void Awake()
        {
            text = GetComponent<Text>();
        }

        public void Initialize(int damage, bool isCrit = false)
        {
            float randomScale;

            if (isCrit)
            {
                text.text = damage.ToString();
                text.color = new Color(1f, 0.6f, 0f); 
                randomScale = Random.Range(1.3f, 1.8f);
            }
            else
            {
                text.text = damage.ToString();
                // 根据伤害值渐变颜色
                float t = Mathf.Clamp01(damage / 80f);
                text.color = Color.Lerp(Color.white, Color.red, t);
                randomScale = Random.Range(0.9f, 1.3f);
            }

            Vector2 randomOffset = Random.insideUnitCircle * 40f;
            transform.localPosition += (Vector3)randomOffset;
            transform.localScale = Vector3.one * randomScale;

            // 弹跳 + 上升
            Sequence seq = DOTween.Sequence();
            seq.Append(transform.DOScale(randomScale * 1.3f, 0.1f));
            seq.Append(transform.DOScale(randomScale * 0.6f, 0.2f));
            seq.Join(transform.DOLocalMoveY(transform.localPosition.y + 60f, 0.6f));
            seq.Join(text.DOFade(0, 0.6f));
            seq.OnComplete(() => {
                Destroy(gameObject);
            });
        }
    }
}