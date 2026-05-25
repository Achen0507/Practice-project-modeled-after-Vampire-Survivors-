using Survivor.Core;
using Survivor.Data;
using UnityEngine;

namespace Survivor.Combat
{
    public class OrbitWeapon : WeaponBase
    {
        [Header("环绕配置")]
        [SerializeField] private GameObject orbitPrefab;      // 环绕物体预制体
        [SerializeField] private float orbitRadius = 1.5f;      // 环绕半径
        [SerializeField] private float rotationSpeed = 180f;  // 旋转速度（度/秒）

        private GameObject[] orbitObjects;
        private float damageInterval = 0.5f;  // 伤害间隔
        private float[] lastDamageTimes;
        private float currentAngle;

        protected override void Start()
        {
            base.Start();
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
            CreateOrbitObjects();
        }

        private void CreateOrbitObjects()
        {
            int count = GetCurrentShotPattern().projectileCount;
            orbitObjects = new GameObject[count];
            lastDamageTimes = new float[count];

            for (int i = 0; i < count; i++)
            {
                float angle = (360f / count) * i;
                Vector3 pos = GetOrbitPosition(angle);

                orbitObjects[i] = Instantiate(orbitPrefab);
                orbitObjects[i].transform.position = pos;

                // 添加伤害组件
                var dealer = orbitObjects[i].AddComponent<OrbitDamageDealer>();
                dealer.Initialize(this, i, damageTextPrefab, canvasTransform);
            }
        }

        private Vector3 GetOrbitPosition(float angle)
        {
            if (playerTransform == null)
                playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

            float rad = angle * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * orbitRadius;
            return (playerTransform != null ? playerTransform.position : Vector3.zero) + offset;
        }

        protected override void Update()
        {
            if (orbitObjects == null) return;
            if (playerTransform == null) return;

            currentAngle += rotationSpeed * Time.deltaTime;

            for (int i = 0; i < orbitObjects.Length; i++)
            {
                if (orbitObjects[i] == null) continue;

                float angleOffset = (360f / orbitObjects.Length) * i;
                float angle = currentAngle + angleOffset;
                orbitObjects[i].transform.position = GetOrbitPosition(angle);
            }
        }

        public float GetDamage() => GetCurrentDamage();

        public override void Upgrade()
        {
            base.Upgrade();
            RecreateOrbitObjects();
        }

        private void RecreateOrbitObjects()
        {
            // 销毁旧物体
            if (orbitObjects != null)
            {
                foreach (var obj in orbitObjects)
                {
                    if (obj != null) Destroy(obj);
                }
            }
            CreateOrbitObjects();
        }

        public bool CanDamage(int index)
        {
            if (lastDamageTimes == null) return true;
            if (Time.time - lastDamageTimes[index] >= damageInterval)
            {
                lastDamageTimes[index] = Time.time;
                return true;
            }
            return false;
        }

        private ShotPattern GetCurrentShotPattern()
        {
            if (weaponData?.shotPatterns != null && weaponData.shotPatterns.Length > 0)
            {
                int level = GetCurrentLevel();
                int idx = Mathf.Min(level - 1, weaponData.shotPatterns.Length - 1);
                return weaponData.shotPatterns[idx];
            }
            return new ShotPattern { projectileCount = 1 };
        }

        public void SetOrbitPrefab(GameObject prefab)
        {
            orbitPrefab = prefab;
        }

        protected override void Attack() { AudioManager.Instance?.PlayOrbitAttack(); }
    }


    public class OrbitDamageDealer : MonoBehaviour
    {
        private OrbitWeapon weapon;
        private int index; 

        public void Initialize(OrbitWeapon w, int idx, GameObject textPrefab, Transform canvasTrans)
        {
            weapon = w;
            index = idx;
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (weapon == null) return;
            if (collision.CompareTag("Player")) return;

            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null && weapon.CanDamage(index))
            {
                damageable.TakeDamage(weapon.GetDamage());
                weapon.AddDamage(weapon.GetDamage());

                weapon.ShowDamageNumber(collision.transform.position, Mathf.RoundToInt(weapon.GetDamage()));

            }
        }
    }
}
