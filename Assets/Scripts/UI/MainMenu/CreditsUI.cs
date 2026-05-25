using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace Survivor.MainMenu
{
    public class CreditsUI : MonoBehaviour
    {
        [Header("面板")]
        [SerializeField] private GameObject creditsPanel;
        [SerializeField] private Text creditsText;
        [SerializeField] private Button backButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private Button startButton;
        [SerializeField] private Button optionButton;
        [SerializeField] private Button enhanceButton;
        [SerializeField] private Button recordButton;
        [SerializeField] private Button creditsButton;
        [SerializeField] private Button collectionButton;

        private void Start()
        {
            backButton.onClick.AddListener(ClosePanel);
            creditsPanel.SetActive(false);
        }

        public void OpenPanel()
        {
            Debug.Log("OpenPanel 被调用");
            quitButton.gameObject.SetActive(false);
            startButton.gameObject.SetActive(false);
            optionButton.gameObject.SetActive(false);
            enhanceButton.gameObject.SetActive(false);
            recordButton.gameObject.SetActive(false);
            creditsButton.gameObject.SetActive(false);
            collectionButton.gameObject.SetActive(false);
            backButton.gameObject.SetActive(true);

            creditsPanel.SetActive(true);
            LoadCredits();
        }

        private void LoadCredits()
        {
            bool isChinese = LocalizationSettings.SelectedLocale.Identifier.Code == "zh-Hans";
            string fileName = isChinese ? "Credits_zh" : "Credits_en";
            TextAsset textAsset = Resources.Load<TextAsset>(fileName);

            Debug.Log($"加载文件: {fileName}, 内容长度: {(textAsset != null ? textAsset.text.Length : 0)}");

            if (textAsset != null && creditsText != null)
            {
                creditsText.text = textAsset.text;
                Debug.Log($"设置文本: {creditsText.text.Substring(0, Mathf.Min(50, creditsText.text.Length))}");
            }
        }

        public void ClosePanel()
        {
            creditsPanel.SetActive(false);

            quitButton.gameObject.SetActive(true);
            startButton.gameObject.SetActive(true);
            optionButton.gameObject.SetActive(true);
            enhanceButton.gameObject.SetActive(true);
            recordButton.gameObject.SetActive(true);
            creditsButton.gameObject.SetActive(true);
            collectionButton.gameObject.SetActive(true);
        }
    }
}
