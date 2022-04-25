using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WorldParse : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        byte[] input = File.ReadAllBytes("Assets/Worlds/level1.txt");
        Parse(input, 0);
    }

    void Parse(byte[] input, int n)
    {
        int length = input.Length;
        int new_index;
        switch(input[n]){
            // 35 = '#'
            case 35:
                new_index = Comment(input, n);
                break;
            // 123 = '{'
            case 123:
                new_index = Load(input, n);
                break;
            default:
                Debug.Log("Error, wrong format in world file!");
                break;
        }
    }

    int Comment(byte[] input, int n)
    {
        while(input[n++] != '\n');
        return n;
    }

    int Load(byte[] input, int n)
    {
        return n;
    }
    // Update is called once per frame
    //void Update()
    //{
    //    
    //}
}
