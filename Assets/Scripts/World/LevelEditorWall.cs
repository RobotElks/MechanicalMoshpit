using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditorWall : MonoBehaviour
{
    public void HideWall()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void ShowWall()
    {
        transform.GetChild(0).gameObject.SetActive(true);

    }

    public void ToggleWall()
    {
        transform.GetChild(0).gameObject.SetActive(!transform.GetChild(0).gameObject.activeInHierarchy);

    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
            ToggleWall();

        
    }
}
