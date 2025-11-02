using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader I;

    [Header("Overlay")]
    [SerializeField] private GameObject loadingOverlayPrefab;

    [SerializeField] private float fadeInSeconds = 0.35f;
    [SerializeField] private float minVisibleSeconds = 1.20f;   
    [SerializeField] private float progressSmoothSeconds = 0.35f; 
    [SerializeField] private float holdAtFullSeconds = 0.40f;

    private Canvas _canvas;                 
    private GameObject _overlayInstance;
    private CanvasGroup _cg;
    private Slider _progressBar;
    private TextMeshProUGUI _label;
    private Transform _spinner;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        _canvas = EnsureOwnCanvas();
    }

    Canvas EnsureOwnCanvas()
    {
        var go = new GameObject("OverlayCanvas_Persistent");
        DontDestroyOnLoad(go);

        var c = go.AddComponent<Canvas>();
        c.renderMode = RenderMode.ScreenSpaceOverlay;
        c.sortingOrder = 32767; 

        go.AddComponent<CanvasScaler>();
        go.AddComponent<GraphicRaycaster>();
        return c;
    }

    public void LoadLevelAsync(string sceneName)
    {
        var tpc = Object.FindFirstObjectByType<StarterAssets.ThirdPersonController>();
        if (tpc) tpc.enabled = false;

        var pi = Object.FindFirstObjectByType<UnityEngine.InputSystem.PlayerInput>();
        if (pi) pi.enabled = false;

        if (_canvas == null) _canvas = EnsureOwnCanvas();

        if (_overlayInstance == null)
        {
            _overlayInstance = Instantiate(loadingOverlayPrefab, _canvas.transform);
            _cg = _overlayInstance.GetComponent<CanvasGroup>();
            _progressBar = _overlayInstance.GetComponentInChildren<Slider>(true);
            _label = _overlayInstance.GetComponentInChildren<TextMeshProUGUI>(true);
            var sp = _overlayInstance.transform.Find("Panel/Spiner");
            if (sp) _spinner = sp;
        }

        ResetOverlayVisuals();
        StartCoroutine(LoadRoutine(sceneName));
    }

    void ResetOverlayVisuals()
    {
        _overlayInstance.SetActive(true);
        if (_cg) { _cg.blocksRaycasts = true; _cg.alpha = 0f; }
        if (_progressBar) _progressBar.value = 0f;
        if (_label) _label.text = "Cargando 0%";
        StartCoroutine(FadeCanvasGroup(_cg, 0f, 1f, fadeInSeconds));
    }

    IEnumerator LoadRoutine(string sceneName)
    {
        float minTime = 1.2f; 
        float timer = 0f;

        var op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        while (!op.isDone)
        {
            timer += Time.unscaledDeltaTime;

            // target real
            float target = Mathf.Clamp01(op.progress / 0.9f);

            // Tiempo mínimo: mientras no pase 1.5s no dejemos que llegue a 100%
            float adjusted = target * Mathf.Clamp01(timer / minTime);

            if (_progressBar) _progressBar.value = adjusted;
            if (_label) _label.text = $"Cargando {(int)(adjusted * 100f)}%";

            if (_spinner) _spinner.Rotate(0f, 0f, -360f * Time.unscaledDeltaTime);

            if (adjusted >= 1f && timer >= minTime)
            {
                op.allowSceneActivation = true;
            }

            yield return null;
        }

        yield return FadeCanvasGroup(_cg, 1f, 0f, 0.25f);
        _overlayInstance.SetActive(false);


        var tpc = Object.FindFirstObjectByType<StarterAssets.ThirdPersonController>();
        if (tpc) tpc.enabled = true;

        var pi = Object.FindFirstObjectByType<UnityEngine.InputSystem.PlayerInput>();
        if (pi) pi.enabled = true;
    }


    IEnumerator FadeCanvasGroup(CanvasGroup cg, float a, float b, float t)
    {
        if (!cg) yield break;
        float t0 = 0f;
        while (t0 < t)
        {
            t0 += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(a, b, t0 / t);
            yield return null;
        }
        cg.alpha = b;
    }
}
