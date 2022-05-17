using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public TMP_InputField addressInput;
    public TMP_InputField playerNameInput;
    public SaveIP informationScript;

    public void PlayGameAsHost()
    {
        SceneManager.LoadScene(1);
    }

    public void PlayGameAsClient()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void CheckIPEndWithEnter()
    {
        informationScript.SaveIPAddress(addressInput.text);
        if (addressInput.text.Contains('\n'))
        {
            addressInput.text = addressInput.text.Replace("\r", "").Replace("\n", "");
            PlayGameAsClient();
        }
    }

    private void getPlayerName()
    {
        informationScript.savePlayerName(playerNameInput.text);
    }
}
