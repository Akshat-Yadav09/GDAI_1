using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("About Popup")]
    [Tooltip("The About popup panel (with the image and close button). Will be shown/hidden.")]
    public GameObject aboutPopup;

    void Awake()
    {
        // Make sure the about popup starts hidden
        if (aboutPopup != null)
            aboutPopup.SetActive(false);

        // Start the main menu music!
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayMainMenuMusic();
    }

    /// <summary>
    /// Loads the gameplay scene. Hook this to the Play button's OnClick event.
    /// </summary>
    public void PlayGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("SampleScene");
    }

    /// <summary>
    /// Loads the shop scene. Hook this to the Shop button's OnClick event.
    /// </summary>
    public void GoToShop()
    {
        SceneManager.LoadScene("Store");
    }

    /// <summary>
    /// Opens the About popup. Hook this to the About button's OnClick event.
    /// </summary>
    public void OpenAbout()
    {
        if (aboutPopup != null)
            aboutPopup.SetActive(true);
    }

    /// <summary>
    /// Closes the About popup. Hook this to the close button's OnClick event.
    /// </summary>
    public void CloseAbout()
    {
        if (aboutPopup != null)
            aboutPopup.SetActive(false);
    }

    /// <summary>
    /// Quits the application. Hook this to the Quit button's OnClick event.
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Quit Game!");
        Application.Quit();
    }
}
