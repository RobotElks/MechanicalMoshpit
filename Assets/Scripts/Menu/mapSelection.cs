using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class mapSelection : MonoBehaviour
{
    public string[] maps;
    public int selectedMap;
    public MultiplayerWorldParse worldBuilderScript;
    public SaveIP informationScript;

    public TextMeshProUGUI mapNameText;

    public void Start()
    {
        //TODO: Filter out all non text files
        maps = Directory.GetFiles("Worlds");
        selectedMap = 0;
        worldBuilderScript.CreateFlag();

        GenerateMap();

    }

    public void NextMap()
    {
        selectedMap = (selectedMap + 1 + maps.Length) % maps.Length;
        GenerateMap();
    }

    public void PreviousMap()
    {
        selectedMap = (selectedMap - 1 + maps.Length) % maps.Length;
        GenerateMap();

    }

    private void GenerateMap()
    {
        worldBuilderScript.ClearWorld();
        informationScript.SetWorldString(File.ReadAllText(maps[selectedMap]).Replace("\r", ""));
        worldBuilderScript.LoadWorldFromInformation();
        worldBuilderScript.BuildWorld();
        worldBuilderScript.MoveWorldToOrigin();

        mapNameText.text = Path.GetFileName(maps[selectedMap]).Replace(".txt", "") + " - " + worldBuilderScript.NumberOfSpawnpoints + " Players";
    }


}
