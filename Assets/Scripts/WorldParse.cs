using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

public class WorldParse : MonoBehaviour
{
    // Different tiles (Gameobject) and input file (String)
    public GameObject tile1Prefab;
    public GameObject tile2Prefab;
    public String fileName;

    // Start is called before the first frame update
    void Start()
    {
        // Read content of textfile, save as a String[]
        String[] input = File.ReadAllLines(fileName);
        //Call Parse() with String[] and start with line 0
        Parse(input, 0);
    }

    void Parse(String[] input, int n)
    {
        //Iterate through every line
        while (n < input.Length)
        {
            // Extract line-string and check whether it contains comment or level-data
            String line = input[n];
            switch (line[0])
            {
                // Comment
                case '#':
                    break;
                // Level-data
                case '{':
                    Load(line);
                    break;
                // Something is wrong with file
                default:
                    throw new Exception("Wrong format in level file");
            }
            // Go to next line
            n++;
        }
    }

    // Extract information form specific line
    void Load(String line)
    {
        // Declare variables where we save data
        int x = 0;
        int y = 0;
        int z = 0;
        int sprite = 0;
        // Split line into String[], while using , { and } as delimiters
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

        // Decide which tile we will use and instantiate it
        switch (sprite)
        {
            case 1:
                Instantiate(tile1Prefab, new Vector3(x, y, z), Quaternion.identity, transform);
                break;
            case 2:
                Instantiate(tile2Prefab, new Vector3(x, y, z), Quaternion.identity, transform);
                break;
        }
    }
}
