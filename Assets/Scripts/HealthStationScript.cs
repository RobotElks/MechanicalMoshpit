using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthStationScript : MonoBehaviour
{
    // Start is called before the first frame update
    bool isAvailable = true;

    public bool GetIfAvailable(){
        return isAvailable;
    }
    
    public void Inactivate(){
        isAvailable = false;
        gameObject.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
    }

    public void Activate(){
        isAvailable = true;
    }

}
