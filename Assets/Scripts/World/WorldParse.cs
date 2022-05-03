using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

public class WorldParse : MonoBehaviour
{
    // Different tiles (Gameobject) and input file (String)
    public GameObject tile1Grass;
    public GameObject tile2Water;
    public GameObject tile3Bridge;
    public GameObject tile4Spikes;
    public String fileName;

    // Start is called before the first frame update
    void Start()
    {
        // Read content of textfile, save as a String[]
        String[] input = File.ReadAllLines(fileName);
        //Call Parse() with String[] and start with line 0
        Parse(input);
    }

    public void Parse(String[] input)
    {
        int n = 0;
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
            // Grass-block
            case 1:
                Instantiate(tile1Grass, new Vector3(x, y, z), Quaternion.identity, transform);
                break;
            // Water
            case 2:
                Instantiate(tile2Water, new Vector3(x, y, z), Quaternion.identity, transform);
                break;
            // Bridge
            case 3:
                Instantiate(tile3Bridge, new Vector3(x, y, z), Quaternion.identity, transform);
                break;
            // Spikes
            case 4:
                Instantiate(tile4Spikes, new Vector3(x, y, z), Quaternion.identity, transform);
                break;
        }
    }
}
