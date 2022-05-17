using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreBoardScript : MonoBehaviour
{
    public GameObject chickenDinner;
    public GameObject content;
    public GameObject row;
    public int rank = 1;
    // Start is called before the first frame update
    public void AddToScoreBoard(string name, int stars, int deaths, int shotsFired)
    {
        GameObject newRow = Instantiate(row, content.transform);

        GameObject rankObject = newRow.transform.GetChild(0).gameObject;
        rankObject.GetComponent<TextMeshProUGUI>().text = rank.ToString();
        GameObject nameObject = newRow.transform.GetChild(1).gameObject;
        nameObject.GetComponent<TextMeshProUGUI>().text = name;
        GameObject starsObject = newRow.transform.GetChild(2).gameObject;
        starsObject.GetComponent<TextMeshProUGUI>().text = stars.ToString();
        GameObject deathsObject = newRow.transform.GetChild(3).gameObject;
        deathsObject.GetComponent<TextMeshProUGUI>().text = deaths.ToString();
        GameObject shotsFiredObject = newRow.transform.GetChild(4).gameObject;
        shotsFiredObject.GetComponent<TextMeshProUGUI>().text = shotsFired.ToString();
        rank +=1;


    }
    public void ChickenDinner(string name)
    {
        if(content.transform.GetChild(1).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text == name)
            chickenDinner.GetComponent<TextMeshProUGUI>().text = "You won!";
        else 
            chickenDinner.GetComponent<TextMeshProUGUI>().text = "Sucks to suck!";
    }
}
