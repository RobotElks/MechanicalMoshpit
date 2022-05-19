using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelEditorMenu : MonoBehaviour
{

    public GameObject savePanel;
    public GameObject editorPanel;
    public GameObject loadPanel;


    public LevelEditor levelEditor;

    public CameraMovement cameraMovement;

    public TextMeshProUGUI saveNameInput;
    public TMP_Dropdown levelDropdown;

    // Start is called before the first frame update
    void Start()
    {

    }

    public bool HasMenuopen()
    {
        return savePanel.activeInHierarchy || loadPanel.activeInHierarchy;
    }

    public void ShowSavePanel()
    {
        savePanel.SetActive(true);
        editorPanel.SetActive(false);
        cameraMovement.enabled = false;

    }

    public void ShowLoadPanel()
    {
        levelDropdown.options.Clear();

        string[] levels = levelEditor.GetMapNames();
        foreach (string mapName in levels)
            levelDropdown.options.Add(new TMP_Dropdown.OptionData(mapName));


        loadPanel.SetActive(true);
        editorPanel.SetActive(false);
        cameraMovement.enabled = false;
    }

    public void ClosePanels()
    {
        savePanel.SetActive(false);
        loadPanel.SetActive(false);
        editorPanel.SetActive(true);
        cameraMovement.enabled = true;

    }

    public void SaveMap()
    {
        if (saveNameInput.text != "")
        {
            levelEditor.SaveWorldToFile(saveNameInput.text);
            ClosePanels();
        }
    }

    public void LoadMap()
    {
        levelEditor.LoadWorldFromFile(levelDropdown.options[levelDropdown.value].text);
        ClosePanels();
    }

    public void QuitToMenu()
    {
        SceneManager.LoadScene(0);
    }

}
