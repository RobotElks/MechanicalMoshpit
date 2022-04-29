using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mapSelection : MonoBehaviour
{
    public GameObject[] Maps;
    public int selectedMap = 0;

    public void NextMap()
    {
        Maps[selectedMap].SetActive(false);
        selectedMap = (selectedMap + 1) % Maps.Length;
        Maps[selectedMap].SetActive(true);
    }

    public void PreviousMap()
    {
        Maps[selectedMap].SetActive(false);
        selectedMap--;
        if (selectedMap < 0) selectedMap += Maps.Length;
        Maps[selectedMap].SetActive(true);
    }

    public string GetMap()
    {
        switch (selectedMap)
        {
            case 0:
                return "Assets/Worlds/level20x20.txt";
            case 1:
                return "Assets/Worlds/level10x10.txt";
            case 2:
                return "Assets/Worlds/leveltest.txt";
        }
        
        return Maps[selectedMap].name;
    }
}
