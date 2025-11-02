using UnityEngine;
using UnityEngine.SceneManagement;

public class GameRules : MonoBehaviour
{
    public static GameRules I;

    [Header("Falling / Death")]
    public float killY = -5f;
    public float checkInterval = 0.20f;
    public enum FallAction { ReloadLevel, GoToMenu }
    public FallAction onFall = FallAction.ReloadLevel;
    public string menuSceneName = "Menu";

    Transform _player;
    bool _monitoring;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        if (I == this) SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnEnable()
    {
        ResolvePlayer();
        StartMonitoring();
    }

    void OnDisable()
    {
        StopMonitoring();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopMonitoring();
        ResolvePlayer();
        StartMonitoring();
    }

    void StartMonitoring()
    {
        if (_monitoring) return;
        InvokeRepeating(nameof(CheckFall), checkInterval, checkInterval);
        _monitoring = true;
    }

    void StopMonitoring()
    {
        if (!_monitoring) return;
        CancelInvoke(nameof(CheckFall));
        _monitoring = false;
    }

    void ResolvePlayer()
    {
        _player = null;

        var tagged = GameObject.FindGameObjectWithTag("Player");
        if (tagged) { _player = tagged.transform; return; }

        var tpc = Object.FindFirstObjectByType<StarterAssets.ThirdPersonController>();
        if (tpc) _player = tpc.transform;
    }

    void CheckFall()
    {
        if (_player == null)
        {
            ResolvePlayer();
            return;
        }

        if (_player.position.y <= killY)
        {
            StopMonitoring();
            HandleDeath();
        }
    }

    void HandleDeath()
    {
        string current = SceneManager.GetActiveScene().name;

        if (onFall == FallAction.ReloadLevel)
        {
            if (SceneLoader.I) SceneLoader.I.LoadLevelAsync(current);
            else SceneManager.LoadScene(current);
        }
        else
        {
            if (SceneLoader.I) SceneLoader.I.LoadLevelAsync(menuSceneName);
            else SceneManager.LoadScene(menuSceneName);
        }
    }
}
