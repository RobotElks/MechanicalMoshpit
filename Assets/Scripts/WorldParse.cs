using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

public class WorldParse : MonoBehaviour
{
    public GameObject tile1Prefab;
    public GameObject tile2Prefab;

    // Start is called before the first frame update
    void Start()
    {
        String[] input = File.ReadAllLines("Assets/Worlds/level1.txt");
        Parse(input, 0);
    }

    void Parse(String[] input, int n)
    {
        //Debug.Log("In parse with n : " + n);
        //Debug.Log("Array length : " + input.Length);
        while (n < input.Length)
        {
            String line = input[n];
            switch (line[0])
            {
                case '#':
                    break;
                case '{':
                    Load(line);
                    break;
                default:
                    throw new Exception("Wrong format in level file");
            }
            n++;
        }
    }

    void Load(String line)
    {
        //Extract information
        int x = 0;
        int y = 0;
        int z = 0;
        int sprite = 0;
        String[] extracted = line.Split(',', '{', '}');
        try
        {
            x = Int32.Parse(extracted[1]);
            y = Int32.Parse(extracted[2]);
            z = Int32.Parse(extracted[3]);
            sprite = Int32.Parse(extracted[4]);
        }
        catch (FormatException)
        {
            Console.WriteLine($"Unable to parse!");
        }        

        if (sprite == 0) Instantiate(tile1Prefab, new Vector3(x,y,z), Quaternion.identity);
        else Instantiate(tile2Prefab, new Vector3(x,y,z), Quaternion.identity);
        
        Debug.Log(line);
    }
}
