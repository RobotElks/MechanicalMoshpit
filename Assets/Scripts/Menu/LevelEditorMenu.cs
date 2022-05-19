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
    public GameObject createPanel;


    public LevelEditor levelEditor;

    public CameraMovement cameraMovement;

    //Save panel 
    public TMP_InputField saveNameInput;

    //Load panel
    public TMP_Dropdown levelDropdown;

    //Create panel
    public TMP_InputField lengthInput, widthInput;

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

    public void ShowCreatePanel()
    {
        createPanel.SetActive(true);
        editorPanel.SetActive(false);
        cameraMovement.enabled = false;

        lengthInput.contentType = TMP_InputField.ContentType.IntegerNumber;
        widthInput.contentType = TMP_InputField.ContentType.IntegerNumber;

        lengthInput.text = levelEditor.LevelLength + "";
        widthInput.text = levelEditor.LevelWidth + "";
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
        createPanel.SetActive(false);
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
        saveNameInput.text = levelDropdown.options[levelDropdown.value].text;
        ClosePanels();
    }


    public void CreateMap()
    {
        levelEditor.LevelLength = System.Convert.ToInt32(lengthInput.text);
        levelEditor.LevelWidth = System.Convert.ToInt32(widthInput.text);
        saveNameInput.text = "";

        levelEditor.NewWorld();
        ClosePanels();
    }

    public void QuitToMenu()
    {
        SceneManager.LoadScene(0);
    }

}
