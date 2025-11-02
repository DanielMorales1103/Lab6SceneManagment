using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class LevelGoal : MonoBehaviour
{
    [SerializeField] private string nextSceneName = "";
    [SerializeField] private bool quitIfLast = true;

    private void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;


        if (!string.IsNullOrEmpty(nextSceneName))
        {
            Load(nextSceneName);
            return;
        }

        int current = SceneManager.GetActiveScene().buildIndex;
        int next = current + 1;
        if (next >= SceneManager.sceneCountInBuildSettings)
        {
            if (quitIfLast) QuitGame();
            else Load("MainMenu");
            return;
        }

        string path = SceneUtility.GetScenePathByBuildIndex(next);
        Load(GetName(path));
    }

    private void Load(string sceneName)
    {
        if (SceneLoader.I != null) SceneLoader.I.LoadLevelAsync(sceneName);
        else SceneManager.LoadScene(sceneName); // fallback
    }

    private static string GetName(string scenePath)
    {
        int s = scenePath.LastIndexOf('/');
        int d = scenePath.LastIndexOf('.');
        return (s >= 0 && d > s) ? scenePath.Substring(s + 1, d - s - 1) : scenePath;
    }

    private static void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
