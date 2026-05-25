using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace Survivor.MainMenu
{
    /// <summary>
    /// option面板
    /// </summary>
    public class OptionsMenu : MonoBehaviour
    {
        [Header("面板")]
        [SerializeField] private GameObject optionsPanel;
        [SerializeField] private Button QuitButton;
        [SerializeField] private Button StartButton;
        [SerializeField] private Button OptionButton;
        [SerializeField] private Button EnhanceButton;
        [SerializeField] private Button RecordButton;
        [SerializeField] private Button CreditsButton;
        [SerializeField] private Button CollectionButton;
        [SerializeField] private Button backButton; 

        [Header("左边按钮")]
        [SerializeField] private Button collectionTab;
        [SerializeField] private Button displayTab;
        [SerializeField] private Button soundTab;

        [Header("右边内容")]
        [SerializeField] private GameObject collectionContent;
        [SerializeField] private GameObject displayContent;
        [SerializeField] private GameObject soundContent;

        [Header("收藏页")]
        [SerializeField] private Dropdown languageDropdown;
        [SerializeField] private Slider soundSlider;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Toggle damageNumberToggle;
        [SerializeField] private Toggle flashEffectToggle;

        [Header("显示页控件")]
        [SerializeField] private Dropdown resolutionDropdown;
        [SerializeField] private Dropdown windowModeDropdown;
        [SerializeField] private Toggle vSyncToggle;
        [SerializeField] private Dropdown borderDropdown;
        [SerializeField] private Toggle screenShakeToggle;

        [Header("声音页控件")]
        [SerializeField] private Slider soundSliderSound;
        [SerializeField] private Slider musicSliderSound;

        private void Start()
        {
            LoadSettings();

            int savedLang = PlayerPrefs.GetInt("Language", 0);
            var locales = LocalizationSettings.AvailableLocales.Locales;
            if (savedLang < locales.Count)
            {
                LocalizationSettings.SelectedLocale = locales[savedLang];
            }  //Language 保存的值：0 = 中文，1 = 英文


            // 绑定左侧图标切换
            collectionTab.onClick.AddListener(() => ShowContent(ContentType.Collection));
            displayTab.onClick.AddListener(() => ShowContent(ContentType.Display));
            soundTab.onClick.AddListener(() => ShowContent(ContentType.Sound));

            // 收藏页事件
            languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
            soundSlider.onValueChanged.AddListener(OnSoundVolumeChanged);
            musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            damageNumberToggle.onValueChanged.AddListener(OnDamageNumberChanged);
            flashEffectToggle.onValueChanged.AddListener(OnFlashEffectChanged);

            // 显示页事件
            resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
            windowModeDropdown.onValueChanged.AddListener(OnWindowModeChanged);
            vSyncToggle.onValueChanged.AddListener(OnVSyncChanged);
            borderDropdown.onValueChanged.AddListener(OnBorderChanged);
            screenShakeToggle.onValueChanged.AddListener(OnScreenShakeChanged);

            // 声音页事件
            soundSliderSound.onValueChanged.AddListener(val => AudioManager.Instance?.SetSFXVolume(val));
            musicSliderSound.onValueChanged.AddListener(val => AudioManager.Instance?.SetMusicVolume(val));

            backButton.onClick.AddListener(CloseOptions);

            ShowContent(ContentType.Collection);
        }

        private enum ContentType
        {
            Collection,
            Display,
            Sound
        }

        private void ShowContent(ContentType type) {
            collectionContent.SetActive(type == ContentType.Collection);
            displayContent.SetActive(type == ContentType.Display);
            soundContent.SetActive(type == ContentType.Sound);
        }

        public void OpenOptions() {
            QuitButton.gameObject.SetActive(false);
            StartButton.gameObject.SetActive(false);
            OptionButton.gameObject.SetActive(false);
            EnhanceButton.gameObject.SetActive(false);
            RecordButton.gameObject.SetActive(false);
            CreditsButton.gameObject.SetActive(false);
            CollectionButton.gameObject.SetActive(false);
            backButton.gameObject.SetActive(true);

            optionsPanel.SetActive(true);
        }

        public void CloseOptions() {
            optionsPanel.SetActive(false);
            QuitButton.gameObject.SetActive(true);
            StartButton.gameObject.SetActive(true);
            OptionButton.gameObject.SetActive(true);
            EnhanceButton.gameObject.SetActive(true);
            RecordButton.gameObject.SetActive(true);
            CreditsButton.gameObject.SetActive(true);
            CollectionButton.gameObject.SetActive(true);

            SaveSettings();
        }

        private void LoadSettings() {
            // 收藏页
            languageDropdown.value = PlayerPrefs.GetInt("Language", 0);
            soundSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.7f);
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
            damageNumberToggle.isOn = PlayerPrefs.GetInt("DamageNumber", 1) == 1;
            flashEffectToggle.isOn = PlayerPrefs.GetInt("FlashEffect", 1) == 1;

            // 显示页
            resolutionDropdown.value = PlayerPrefs.GetInt("Resolution", 0);
            windowModeDropdown.value = PlayerPrefs.GetInt("WindowMode", 0);
            vSyncToggle.isOn = PlayerPrefs.GetInt("VSync", 1) == 1;
            borderDropdown.value = PlayerPrefs.GetInt("Border", 0);
            screenShakeToggle.isOn = PlayerPrefs.GetInt("ScreenShake", 1) == 1;

            // 声音页
            soundSliderSound.value = PlayerPrefs.GetFloat("SFXVolume", 0.7f);
            musicSliderSound.value = PlayerPrefs.GetFloat("MusicVolume", 0.5f);

            // 刷新显示
            languageDropdown.RefreshShownValue();
            resolutionDropdown.RefreshShownValue();
            windowModeDropdown.RefreshShownValue();
            borderDropdown.RefreshShownValue();
        }

        private void SaveSettings() {
            // 收藏页
            PlayerPrefs.SetInt("Language", languageDropdown.value);
            PlayerPrefs.SetFloat("SFXVolume", soundSlider.value);
            PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
            PlayerPrefs.SetInt("DamageNumber", damageNumberToggle.isOn ? 1 : 0);
            PlayerPrefs.SetInt("FlashEffect", flashEffectToggle.isOn ? 1 : 0);

            // 显示页
            PlayerPrefs.SetInt("Resolution", resolutionDropdown.value);
            PlayerPrefs.SetInt("WindowMode", windowModeDropdown.value);
            PlayerPrefs.SetInt("VSync", vSyncToggle.isOn ? 1 : 0);
            PlayerPrefs.SetInt("Border", borderDropdown.value);
            PlayerPrefs.SetInt("ScreenShake", screenShakeToggle.isOn ? 1 : 0);

            PlayerPrefs.Save();

            ApplyDisplaySettings();
        }

        private void ApplyDisplaySettings() {
            // 分辨率
            int resIndex = PlayerPrefs.GetInt("Resolution", 0);
            if (resIndex == 0) Screen.SetResolution(1920, 1080, Screen.fullScreenMode);
            else if (resIndex == 1) Screen.SetResolution(1600, 900, Screen.fullScreenMode);
            else if (resIndex == 2) Screen.SetResolution(1280, 720, Screen.fullScreenMode);

            // 窗口模式
            int windowMode = PlayerPrefs.GetInt("WindowMode", 0);
            if (windowMode == 0) Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
            else if (windowMode == 1) Screen.fullScreenMode = FullScreenMode.Windowed;

            // 垂直同步
            QualitySettings.vSyncCount = PlayerPrefs.GetInt("VSync", 1);
        }

        private void OnLanguageChanged(int index) {
            PlayerPrefs.SetInt("Language", index);

            var locales = LocalizationSettings.AvailableLocales.Locales;
            if (index < locales.Count)
            {
                LocalizationSettings.SelectedLocale = locales[index];
            }
        }

        private void OnSoundVolumeChanged(float value) {
            if (soundSliderSound != null) soundSliderSound.value = value;
            if (soundSlider != null) soundSlider.value = value;
            AudioManager.Instance?.SetSFXVolume(value);
        }

        private void OnMusicVolumeChanged(float value)
        {
            if (musicSliderSound != null) musicSliderSound.value = value;
            if (musicSlider != null) musicSlider.value = value;
            AudioManager.Instance?.SetMusicVolume(value);
        }

        private void OnDamageNumberChanged(bool isOn) {
            PlayerPrefs.SetInt("DamageNumber", isOn ? 1 : 0);
        }

        private void OnFlashEffectChanged(bool isOn) {
            PlayerPrefs.SetInt("FlashEffect", isOn ? 1 : 0);
        }

        private void OnResolutionChanged(int index)
        {
            PlayerPrefs.SetInt("Resolution", index);
            ApplyDisplaySettings();
        }

        private void OnWindowModeChanged(int index)
        {
            PlayerPrefs.SetInt("WindowMode", index);
            ApplyDisplaySettings();
        }

        private void OnVSyncChanged(bool isOn)
        {
            PlayerPrefs.SetInt("VSync", isOn ? 1 : 0);
            QualitySettings.vSyncCount = isOn ? 1 : 0;
        }

        private void OnBorderChanged(int index)
        {
            PlayerPrefs.SetInt("Border", index);

            if (index == 1) 
            {
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            }
            else 
            {
                // 恢复之前保存的窗口模式
                int windowMode = PlayerPrefs.GetInt("WindowMode", 0);
                if (windowMode == 0) Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                else if (windowMode == 1) Screen.fullScreenMode = FullScreenMode.Windowed;
            }
        }

        private void OnScreenShakeChanged(bool isOn)
        {
            PlayerPrefs.SetInt("ScreenShake", isOn ? 1 : 0);
        }
    }
}
