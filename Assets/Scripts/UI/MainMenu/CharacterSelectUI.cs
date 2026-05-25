using Survivor.Core;
using Survivor.Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Survivor.MainMenu
{
    /// <summary>
    /// 选人页面
    /// </summary>
    public class CharacterSelectUI : MonoBehaviour
    {
        [Header("主菜单按钮")]
        [SerializeField] private Button quitButton;
        [SerializeField] private Button startButton;
        [SerializeField] private Button optionButton;
        [SerializeField] private Button enhanceButton;
        [SerializeField] private Button recordButton;
        [SerializeField] private Button creditsButton;
        [SerializeField] private Button collectionButton;
        [SerializeField] private Button backButton;

        [Header("面板")]
        [SerializeField] private GameObject characterPanel;
        [SerializeField] private Button goButton;

        [Header("角色列表")]
        [SerializeField] private Transform characterContainer;
        [SerializeField] private GameObject characterItemPrefab;

        [Header("地图列表")]
        [SerializeField] private Transform mapContainer;
        [SerializeField] private GameObject mapItemPrefab;

        [Header("底部详情")]
        [SerializeField] private Text characterNameText;
        [SerializeField] private Text characterDescText;
        [SerializeField] private Image weaponIcon;
        [SerializeField] private Image portraitImage;

        [Header("左侧属性")]
        [SerializeField] private Text healthText;
        [SerializeField] private Text regenText;
        [SerializeField] private Text armorText;
        [SerializeField] private Text speedText;
        [SerializeField] private Text powerText;
        [SerializeField] private Text projSpeedText;
        [SerializeField] private Text durationText;
        [SerializeField] private Text rangeText;
        [SerializeField] private Text cooldownText;
        [SerializeField] private Text projCountText;
        [SerializeField] private Text reviveText;
        [SerializeField] private Text luckText;
        [SerializeField] private Text growthText;
        [SerializeField] private Text greedText;

        private List<CharacterData> characters;
        private List<MapData> maps;
        private CharacterData currentCharacter;
        private MapData currentMap;
        private List<Toggle> mapToggles = new List<Toggle>();

        private void Start()
        {
            LoadAllCharacters();
            LoadAllMaps();

            goButton.onClick.AddListener(OnConfirm);
            if (backButton != null) backButton.onClick.AddListener(ClosePanel);

            InitCharacterList();
            InitMapList();

            if (characters.Count > 0)
                SelectCharacter(0);
            if (maps.Count > 0)
                SelectMap(0);

            characterPanel.SetActive(false);
        }

        private void LoadAllCharacters()
        {
            CharacterData[] loaded = Resources.LoadAll<CharacterData>("Data/CharacterData");
            System.Array.Sort(loaded, (a, b) => a.name.CompareTo(b.name));
            characters = new List<CharacterData>(loaded);

            if (characters.Count == 0)
            {
                Debug.LogError("没有找到角色数据！请把 CharacterData 文件放到 Assets/Resources/Characters/ 文件夹");
            }
        }

        private void LoadAllMaps()
        {
            MapData[] loaded = Resources.LoadAll<MapData>("Data/MapData");
            System.Array.Sort(loaded, (a, b) => a.name.CompareTo(b.name));
            maps = new List<MapData>(loaded);

            if (maps.Count == 0)
            {
                Debug.LogError("没有找到地图数据！请把 MapData 文件放到 Assets/Resources/Maps/ 文件夹");
            }
        }

        private void OnEnable()
        {
            GameEvents.OnPlayerStatsChanged += RefreshStats;
        }

        private void OnDisable()
        {
            GameEvents.OnPlayerStatsChanged -= RefreshStats;
        }

        private void RefreshStats()
        {
            if (currentCharacter != null)
                UpdateStatPanel(currentCharacter);
        }
        public void OpenPanel()
        {
            quitButton.gameObject.SetActive(false);
            startButton.gameObject.SetActive(false);
            optionButton.gameObject.SetActive(false);
            enhanceButton.gameObject.SetActive(false);
            recordButton.gameObject.SetActive(false);
            creditsButton.gameObject.SetActive(false);
            collectionButton.gameObject.SetActive(false);
            backButton.gameObject.SetActive(true);

            PlayerAttributes.Instance.ResetToBase();  //attr不累加 dondestroy

            if (currentCharacter != null)
            {
                UpdateStatPanel(currentCharacter);
            }
            else if (characters.Count > 0)
            {
                SelectCharacter(0);
            }

            characterPanel.SetActive(true);
        }

        public void ClosePanel()
        {
            characterPanel.SetActive(false);

            quitButton.gameObject.SetActive(true);
            startButton.gameObject.SetActive(true);
            optionButton.gameObject.SetActive(true);
            enhanceButton.gameObject.SetActive(true);
            recordButton.gameObject.SetActive(true);
            creditsButton.gameObject.SetActive(true);
            collectionButton.gameObject.SetActive(true);
        }

        private void InitCharacterList() {
            foreach (Transform child in characterContainer) { Destroy(child.gameObject); }

            for (int i = 0; i < characters.Count; i++)
            {
                int index = i;
                GameObject go = Instantiate(characterItemPrefab, characterContainer);
                Button btn = go.GetComponent<Button>();

                Image charIcon = go.transform.Find("Icon")?.GetComponent<Image>();
                if (charIcon != null) charIcon.sprite = characters[i].Charactericon;

                Text nameText = go.transform.Find("Name")?.GetComponent<Text>();
                LocalizeStringEvent nameEvent = nameText?.GetComponent<LocalizeStringEvent>();
                if (nameEvent != null)
                {
                    nameEvent.StringReference.TableReference = "UIText";
                    nameEvent.StringReference.TableEntryReference = characters[i].nameKey;
                }
                btn.onClick.AddListener(() => SelectCharacter(index));
            }
        }

        private void InitMapList() {
            foreach (Transform child in mapContainer)
                Destroy(child.gameObject);

            mapToggles.Clear();

            for (int i = 0; i < maps.Count; i++)
            {
                int index = i;
                GameObject go = Instantiate(mapItemPrefab, mapContainer);
                Toggle toggle = go.GetComponentInChildren<Toggle>();
                if (toggle == null)
                {
                    Debug.LogError("mapItemPrefab 没有找到 Toggle 组件");
                    continue;
                }
                Text mapNameText = go.GetComponentInChildren<Text>();
                LocalizeStringEvent nameEvent = mapNameText?.GetComponent<LocalizeStringEvent>();
                if (nameEvent != null)
                {
                    nameEvent.StringReference.TableReference = "UIText";
                    nameEvent.StringReference.TableEntryReference = maps[i].nameKey;
                }

                toggle.onValueChanged.AddListener((isOn) => {
                    if (isOn) SelectMap(index);
                });

                mapToggles.Add(toggle);

                if (i == 0) toggle.isOn = true;
            }
        }

        private void SelectMap(int index) {
            currentMap = maps[index];

            // 手动确保只有当前选中的是 true
            for (int i = 0; i < mapToggles.Count; i++)
            {
                mapToggles[i].isOn = (i == index);
            }
        }

        private void SelectCharacter(int index) {
            currentCharacter = characters[index];

            characterNameText.text = LocalizationSettings.StringDatabase.GetLocalizedString("UIText", currentCharacter.nameKey);
            characterDescText.text = LocalizationSettings.StringDatabase.GetLocalizedString("UIText", currentCharacter.descKey);

            if (!string.IsNullOrEmpty(currentCharacter.starterWeaponName)) 
            {
                WeaponData weaponData = Resources.Load<WeaponData>($"Data/WeaponData/{currentCharacter.starterWeaponName}");
                if (weaponData != null)
                {
                    weaponIcon.sprite = weaponData.icon;
                }
                else
                {
                    Debug.LogError($"找不到武器数据: {currentCharacter.starterWeaponName}");
                }
            }
            UpdateStatPanel(currentCharacter);
        }

        private void UpdateStatPanel(CharacterData character) {
            // 使用角色的基础属性，不是 PlayerAttributes
            if (character == null || character.baseStats == null) return;

            CharacterBaseStats baseStats = character.baseStats;
            characterNameText.text = LocalizationSettings.StringDatabase.GetLocalizedString("UIText", character.nameKey);
            characterDescText.text = LocalizationSettings.StringDatabase.GetLocalizedString("UIText", character.descKey);

            // 角色加成
            StatModifier GetCharBonus(StatType type)
            {
                if (character.TryGetStatModifier(type, out StatModifier mod))
                    return mod;
                return new StatModifier { statType = type, value = 0, isPercentage = false };
            }

            // 局外加成
            StatModifier GetEnhanceBonus(StatType type)
            {
                float value = EnhanceManager.Instance?.GetTotalBonus(type) ?? 0;
                bool isPercentage =EnhanceManager.Instance?.IsPercentage(type) ?? false;
                return new StatModifier
                {
                    statType = type,
                    value = value,
                    isPercentage = isPercentage
                };
            } 

            float CalculateFinalValue(float baseValue, StatModifier charBonus, StatModifier enhanceBonus)
            {
                if (baseValue == 0 && (charBonus.isPercentage || enhanceBonus.isPercentage))
                {
                    float total = 0;

                    // 角色加成
                    if (charBonus.isPercentage)
                        total += charBonus.value * 100f;
                    else
                        total += charBonus.value; 

                    // 局外加成
                    if (enhanceBonus.isPercentage)
                        total += enhanceBonus.value * 100f;
                    else
                        total += enhanceBonus.value;

                    return total;
                }

                // 正常情况
                float value = baseValue;

                if (charBonus.isPercentage)
                    value = baseValue * (1 + charBonus.value);
                else
                    value = baseValue + charBonus.value;

                if (enhanceBonus.isPercentage)
                    value = value * (1 + enhanceBonus.value);
                else
                    value = value + enhanceBonus.value;

                return value;
            }


            // 基础值从 baseStats 读取
            float baseHealth = baseStats.maxHealth;
            float baseRegen = baseStats.healthRegen;
            float baseArmor = baseStats.armor;
            float baseSpeed = baseStats.moveSpeed;
            float basePower = baseStats.power;
            float baseProjSpeed = baseStats.projectileSpeed;
            float baseDuration = baseStats.duration;
            float baseRange = baseStats.attackRange;
            float baseCooldown = baseStats.cooldownReduction;
            float baseProjCount = baseStats.projectileCount;
            float baseLuck = baseStats.luck;
            float baseGrowth = baseStats.growth;
            float baseGreed = baseStats.greed;

            // 获取加成
            StatModifier charHealth = GetCharBonus(StatType.MaxHealth);
            StatModifier enhanceHealth = GetEnhanceBonus(StatType.MaxHealth);

            StatModifier charRegen = GetCharBonus(StatType.HealthRegen);
            StatModifier enhanceRegen = GetEnhanceBonus(StatType.HealthRegen);

            StatModifier charArmor = GetCharBonus(StatType.Armor);
            StatModifier enhanceArmor = GetEnhanceBonus(StatType.Armor);

            StatModifier charSpeed = GetCharBonus(StatType.MoveSpeed);
            StatModifier enhanceSpeed = GetEnhanceBonus(StatType.MoveSpeed);

            StatModifier charPower = GetCharBonus(StatType.DamageBonus);
            StatModifier enhancePower = GetEnhanceBonus(StatType.DamageBonus);

            StatModifier charProjSpeed = GetCharBonus(StatType.ProjectileSpeed);
            StatModifier enhanceProjSpeed = GetEnhanceBonus(StatType.ProjectileSpeed);

            StatModifier charDuration = GetCharBonus(StatType.Duration);
            StatModifier enhanceDuration = GetEnhanceBonus(StatType.Duration);

            StatModifier charRange = GetCharBonus(StatType.AttackRange);
            StatModifier enhanceRange = GetEnhanceBonus(StatType.AttackRange);

            StatModifier charCooldown = GetCharBonus(StatType.CooldownReduction);
            StatModifier enhanceCooldown = GetEnhanceBonus(StatType.CooldownReduction);

            StatModifier charProjCount = GetCharBonus(StatType.ProjectileCount);
            StatModifier enhanceProjCount = GetEnhanceBonus(StatType.ProjectileCount); 

            StatModifier charLuck = GetCharBonus(StatType.Luck);
            StatModifier enhanceLuck = GetEnhanceBonus(StatType.Luck);

            StatModifier charGrowth = GetCharBonus(StatType.Growth);
            StatModifier enhanceGrowth = GetEnhanceBonus(StatType.Growth);

            StatModifier charGreed = GetCharBonus(StatType.Greed);
            StatModifier enhanceGreed = GetEnhanceBonus(StatType.Greed);

            // 计算最终值并显示
            float finalHealth = CalculateFinalValue(baseHealth, charHealth, enhanceHealth);
            healthText.text = $"{finalHealth:F0}";

            float finalRegen = CalculateFinalValue(baseRegen, charRegen, enhanceRegen);
            regenText.text = $"{finalRegen:F1}/S";

            float finalArmor = CalculateFinalValue(baseArmor, charArmor, enhanceArmor);
            armorText.text = $"{finalArmor:F0}%";

            float finalSpeed = CalculateFinalValue(baseSpeed, charSpeed, enhanceSpeed);
            speedText.text = $"{finalSpeed:F1}";

            float finalPower = CalculateFinalValue(basePower, charPower, enhancePower);
            powerText.text = $"{finalPower:F0}%";

            float finalProjSpeed = CalculateFinalValue(baseProjSpeed, charProjSpeed, enhanceProjSpeed);
            projSpeedText.text = $"{finalProjSpeed:F0}";

            float finalDuration = CalculateFinalValue(baseDuration, charDuration, enhanceDuration);
            durationText.text = $"{finalDuration:F1}/S";

            float finalRange = CalculateFinalValue(baseRange, charRange, enhanceRange);
            rangeText.text = $"{finalRange:F1}";

            float finalCooldown = CalculateFinalValue(baseCooldown, charCooldown, enhanceCooldown);
            cooldownText.text = $"{finalCooldown:F0}%";

            float finalProjCount = CalculateFinalValue(baseProjCount, charProjCount, enhanceProjCount);
            projCountText.text = $"{finalProjCount:F0}";

            float finalLuck = CalculateFinalValue(baseLuck, charLuck, enhanceLuck);
            luckText.text = $"{finalLuck:F0}%";

            float finalGrowth = CalculateFinalValue(baseGrowth, charGrowth, enhanceGrowth);
            growthText.text = $"{finalGrowth:F0}%";

            float finalGreed = CalculateFinalValue(baseGreed, charGreed, enhanceGreed); 
            greedText.text = $"{finalGreed:F0}%";
        }

        private void OnConfirm()
        { 
            if (currentCharacter == null) return;
            if (currentMap == null) return;

            GameManager.Instance.SelectedCharacter = currentCharacter;
            GameManager.Instance.SelectedMap = currentMap;

            var finalStats = GetCurrentFinalStats();
            GameManager.Instance.FinalCharacterStats = finalStats;

            SceneManager.LoadScene(currentMap.sceneName);

        }

        private Dictionary<StatType, float> GetCurrentFinalStats()
        {
            var stats = new Dictionary<StatType, float>();

            float SafeParse(Text text, params string[] removeStrings)
            {
                if (text == null || string.IsNullOrEmpty(text.text)) return 0;

                string raw = text.text;

                foreach (string remove in removeStrings)
                {
                    raw = raw.Replace(remove, "");
                }

                // 移除所有非数字、小数点和负号的字符
                string numberOnly = System.Text.RegularExpressions.Regex.Replace(raw, @"[^0-9.-]", "");

                if (float.TryParse(numberOnly, System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture, out float result))
                {
                    return result;
                }

                Debug.LogWarning($"无法解析: {text.text}, 处理后的结果: {numberOnly}");
                return 0;
            }

            stats[StatType.MaxHealth] = SafeParse(healthText);
            stats[StatType.HealthRegen] = SafeParse(regenText, "/秒");
            stats[StatType.Armor] = SafeParse(armorText, "%");
            stats[StatType.MoveSpeed] = SafeParse(speedText);
            stats[StatType.DamageBonus] = SafeParse(powerText, "%");
            stats[StatType.ProjectileSpeed] = SafeParse(projSpeedText);
            stats[StatType.Duration] = SafeParse(durationText, "秒");
            stats[StatType.AttackRange] = SafeParse(rangeText);
            stats[StatType.CooldownReduction] = SafeParse(cooldownText, "%");
            stats[StatType.ProjectileCount] = SafeParse(projCountText);
            stats[StatType.Luck] = SafeParse(luckText, "%");
            stats[StatType.Growth] = SafeParse(growthText, "%");
            stats[StatType.Greed] = SafeParse(greedText, "%");

            return stats;
        }
    }
}
