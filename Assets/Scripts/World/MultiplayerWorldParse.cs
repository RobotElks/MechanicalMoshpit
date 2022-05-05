using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Unity.Netcode;

public class MultiplayerWorldParse : MonoBehaviour
{
    // Different tiles (Gameobject) and input file (String)
    public GameObject tile1Grass;
    public GameObject tile2Water;
    public GameObject tile3Bridge;
    public GameObject tile4Spikes;
    public List<GameObject> SpawnArea = new List<GameObject>();
    public List<GameObject> avaliableSpawnPoints = new List<GameObject>();
    public List<GameObject> usedSpawnPoints = new List<GameObject>();

    public string fileName;

    public string worldString = "";

    // Start is called before the first frame update
    void Start()
    {

    }


    public void LoadWorldFromFile()
    {
        SetWorldString(File.ReadAllText(fileName).Replace("\r", ""));
    }

    public string GetWorldString()
    {
        return worldString;
    }

    public void SetWorldString(string newWorldString)
    {
        if (newWorldString != worldString)
        {
            worldString = newWorldString;
            BuildWorld();
        }
    }

    public Vector3 GetSpawnAreaPoint()
    {
        int i = Random.Range(0, SpawnArea.Count - 1);

        Vector3 point = SpawnArea[i].transform.position;

        usedSpawnPoints.Add(SpawnArea[i]);
        SpawnArea.RemoveAt(i);

        return point;
    }

    public Vector3 GetSpawnPoint()
    {
        int i = Random.Range(0, 1);
        GameObject spawnPoint;
        if(i == 1)
        {
            spawnPoint = avaliableSpawnPoints[0];
        }
        else
        {
            spawnPoint = avaliableSpawnPoints[avaliableSpawnPoints.Count-1];
        }
        
        avaliableSpawnPoints.Remove(spawnPoint);
        usedSpawnPoints.Add(spawnPoint);
        Vector3 point = spawnPoint.transform.position;
        return point;
        
    }

    public void BuildWorld()
    {

        if (transform.childCount > 0)
            return;

        //Debug.Log("Parsing world");
        string[] rows = worldString.Split("\n");

        int n = 0;
        //Iterate through every line
        while (n < rows.Length)
        {
            // Extract line-string and check whether it contains comment or level-data
            string line = rows[n];

            if (line.Length > 0)
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
                        break;
                }
            // Go to next line
            n++;
        }
    }

    // Extract information form specific line
    void Load(string line)
    {
        // Declare variables where we save data
        int x = 0;
        int y = 0;
        int z = 0;
        int tileID = 0;
        // Split line into String[], while using , { and } as delimiters
        string[] extracted = line.Split(',', '{', '}');
        try
        {
            x = int.Parse(extracted[1]);
            y = int.Parse(extracted[2]);
            z = int.Parse(extracted[3]);
            tileID = int.Parse(extracted[4]);
        }
        catch
        {
            Debug.Log($"Unable to parse!");
        }

        // Decide which tile we will use and instantiate it
        switch (tileID)
        {
            //Spawnpoint
            case 0:
                GameObject sp = new GameObject();
                sp.transform.position = new Vector3(x, y, z);
                sp.name = "SpawnPoint" + (avaliableSpawnPoints.Count + SpawnArea.Count + 1);
                sp.transform.parent = this.transform;
                if (x >= 1000)
                    SpawnArea.Add(sp);
                else
                    avaliableSpawnPoints.Add(sp);
                break;

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
