using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class LeaveGame : MonoBehaviour
{
    public GameObject panel;
    SaveIP informationScript;
    public TextMeshProUGUI textSavedIp;

    private void Start()
    {
        informationScript = GameObject.Find("Information").GetComponent<SaveIP>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (panel.activeInHierarchy)
            {
                panel.SetActive(false);
            }
            else
            {

                panel.SetActive(true);

                if (informationScript.IsHost())
                {
                    textSavedIp.text = "Hosting on:\n" + informationScript.GetSaved();
                }

                else if (NetworkManager.Singleton.IsConnectedClient)
                {
                    textSavedIp.text = "Connected to:\n" + informationScript.GetSaved();
                }

                else
                {
                    textSavedIp.text = "No connection to:\n" + informationScript.GetSaved();

                }
            }
        }

    }
    public void LeaveGameToMenu()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene(0);
    }
}
