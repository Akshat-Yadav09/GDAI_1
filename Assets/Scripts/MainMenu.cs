using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
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
    /// Quits the application. Hook this to the Quit button's OnClick event.
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Quit Game!");
        Application.Quit();
    }
}
