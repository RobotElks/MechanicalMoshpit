using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGameAsHost()
    {
        SceneManager.LoadScene("Multiplayer");
    }

    public void PlayGameAsClient()
    {
        SceneManager.LoadScene("Multiplayer");
    }

    public void QuitGame()
    {
        Debug.Log("QUIT THE GAME");
        Application.Quit();
    }
}
