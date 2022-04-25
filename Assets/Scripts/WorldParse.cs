using System.Collections;
using System.Collections.Generic;
using System;
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
        //Debug.Log("In parse with n : " + n);
        //Debug.Log("Array length : " + input.Length);

        if(n < input.Length)
        {
            //Debug.Log("Char at newline : " + (char)input[n] + " : byte = " + input[n]);
            int new_index = 0;

            switch (input[n])
            {
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
                    throw new Exception("Wrong format in level file");          
            }

            Parse(input, new_index);
        }
        else
        {
            Debug.Log("Load done!");
        }

    }

    int Comment(byte[] input, int n)
    {
        while(input[n++] != '\n');
        return n;
    }

    int Load(byte[] input, int n)
    {
        while (n < input.Length && input[n] != '\n')
        {
            Debug.Log((char)input[n++]);
        }
        return ++n;
    }

}
