using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;

    [Header("Scene")]
    [Tooltip("Scene name to load when pressing Play (must be in Build Settings).")]
    [SerializeField] private string playSceneName = "Game";

    private void Awake()
    {
        // Safety checks
        if (playButton == null) Debug.LogError($"{nameof(MainMenu)}: Play Button is not assigned.");
        if (quitButton == null) Debug.LogError($"{nameof(MainMenu)}: Quit Button is not assigned.");

        // Remove any existing listeners to avoid duplicates if object persists/reloads
        if (playButton != null) playButton.onClick.RemoveListener(OnPlayClicked);
        if (quitButton != null) quitButton.onClick.RemoveListener(OnQuitClicked);

        // Add listeners
        if (playButton != null) playButton.onClick.AddListener(OnPlayClicked);
        if (quitButton != null) quitButton.onClick.AddListener(OnQuitClicked);
    }

    private void OnPlayClicked()
    {
        if (string.IsNullOrWhiteSpace(playSceneName))
        {
            Debug.LogError($"{nameof(MainMenu)}: Play scene name is empty.");
            return;
        }

        SceneManager.LoadScene(playSceneName);
    }

    private void OnQuitClicked()
    {
        // Quits in a built game. In the editor it won't close the editor.
        Application.Quit();

#if UNITY_EDITOR
        Debug.Log("Quit clicked (Unity Editor won't close).");
#endif
    }
}
