using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button playButton;

    [Header("Scene")]
    [Tooltip("Scene name to load when pressing Play (must be in Build Settings).")]
    [SerializeField] private string playSceneName = "Game";
    private void Awake()
    {
        // Remove any existing listeners to avoid duplicates if object persists/reloads
        if (playButton != null) playButton.onClick.RemoveListener(OnPlayClicked);

        // Add listeners
        if (playButton != null) playButton.onClick.AddListener(OnPlayClicked);
    }

    private void OnPlayClicked()
    {
        SceneManager.LoadScene(playSceneName);
    }
}
