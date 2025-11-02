using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button btnLevel1, btnLevel2, btnLevel3, btnQuit;

    private void Awake()
    {
        btnLevel1.onClick.AddListener(() => SceneLoader.I.LoadLevelAsync("Level_01"));
        btnLevel2.onClick.AddListener(() => SceneLoader.I.LoadLevelAsync("Level_02"));
        btnLevel3.onClick.AddListener(() => SceneLoader.I.LoadLevelAsync("Level_03"));
        btnQuit.onClick.AddListener(QuitGame);
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
