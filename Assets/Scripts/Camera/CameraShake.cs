using DG.Tweening;
using UnityEngine;

namespace Survivor.Cam
{
    public class CameraShake : MonoBehaviour
    {
        public static CameraShake Instance;

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }

        /// <summary>
        /// 震动相机
        /// </summary>
        /// <param name="duration">持续时间（秒）</param>
        /// <param name="strength">震动强度</param>
        public void Shake(float duration =0.2f, float strength = 0.2f) {
            if (PlayerPrefs.GetInt("ScreenShake", 1) == 0) return;
            transform.DOShakePosition(duration, strength);
        }
    }
}
