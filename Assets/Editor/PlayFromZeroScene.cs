using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
[InitializeOnLoad]
public static class PlayFromZeroScene
{
    private const string PreviousScenePathKey = "PlayFromZeroScene_PreviousScenePath";
    private const string ShouldLoadPreviousSceneKey = "PlayFromZeroScene_ShouldLoadPreviousScene";

    static PlayFromZeroScene()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            string currentScenePath = SceneManager.GetActiveScene().path;
            EditorPrefs.SetString(PreviousScenePathKey, currentScenePath);
            EditorPrefs.SetBool(ShouldLoadPreviousSceneKey, true);
            string zeroScenePath = GetScenePathByBuildIndex(0);

            if (!string.IsNullOrEmpty(zeroScenePath))
            {
                EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(zeroScenePath);
            }
            else
            {
                EditorPrefs.SetBool(ShouldLoadPreviousSceneKey, false);
                EditorSceneManager.playModeStartScene = null;
            }

        }
        else if (state == PlayModeStateChange.EnteredEditMode)
        {

            EditorSceneManager.playModeStartScene = null;

            if (EditorPrefs.GetBool(ShouldLoadPreviousSceneKey, false))
            {
                string previousScenePath = EditorPrefs.GetString(PreviousScenePathKey, "");

                EditorApplication.delayCall += () =>
                {
                    EditorSceneManager.OpenScene(previousScenePath, OpenSceneMode.Single);
                };

                EditorPrefs.DeleteKey(PreviousScenePathKey);
                EditorPrefs.DeleteKey(ShouldLoadPreviousSceneKey);
            }
        }
    }

    private static string GetScenePathByBuildIndex(int buildIndex)
    {
        if (buildIndex < 0 || buildIndex >= EditorBuildSettings.scenes.Length)
        {
            return string.Empty;
        }
        return EditorBuildSettings.scenes[buildIndex].path;
    }
}