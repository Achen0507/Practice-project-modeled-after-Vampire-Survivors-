using UnityEditor;
using UnityEngine;

public class PrefsCleanerWindow : EditorWindow
{
    [MenuItem("Window/清理工具")]
    public static void ShowWindow()
    {
        GetWindow<PrefsCleanerWindow>("清理工具");
    }

    void OnGUI()
    {
        if (GUILayout.Button("清除所有 PlayerPrefs", GUILayout.Height(50)))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }

        if (GUILayout.Button("打开持久化数据文件夹", GUILayout.Height(50)))
        {
            EditorUtility.RevealInFinder(Application.persistentDataPath);
        }
    }
}