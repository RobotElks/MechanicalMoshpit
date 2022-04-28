using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SaveIP : MonoBehaviour
{
    public GameObject SaveIp;
    public TMP_InputField addressInput;
    [SerializeField]
    private string saved;
       // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(SaveIp);
    }

    public void SaveIPAddress(string IP){
        saved = addressInput.text;
    }
}
