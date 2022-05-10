using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MultiplayerWorldParse : MonoBehaviour
{
    // Different tiles (Gameobject) and input file (String)
    public GameObject tile1Grass;
    public GameObject tile2Water;
    public GameObject tile3Bridge;
    public GameObject tile4Spikes;


    public List<Vector3> avaliableSpawnPoints = new List<Vector3>();

    public string worldString = "";

    GameObject worldParent;

    Vector3 worldMiddle = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        //ClearWorld();
    }


    public void LoadWorldFromInformation()
    {
        SetWorldString(GameObject.Find("Information").GetComponent<SaveIP>().GetWorldString());
    }

    public string GetWorldString()
    {
        return worldString;
    }

    public void SetWorldString(string newWorldString)
    {
        worldString = newWorldString;
    }

    public Vector3 GetLobbySpawnPoint()
    {
        return new Vector3(1005, 5, 5);
    }

    public Vector3 GetSpawnPoint()
    {

        return Vector3.zero;
    }

    public void BuildLobby()
    {
        worldString = Resources.Load<TextAsset>("lobby").text;
        BuildWorld();
    }

    public void BuildWorld()
    {
        
        worldMiddle = Vector3.zero;

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

        worldMiddle /= 2;

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

        if (x > worldMiddle.x)
            worldMiddle.x = x;
        if (z > worldMiddle.z)
            worldMiddle.z = z;

        // Decide which tile we will use and instantiate it
        switch (tileID)
        {
            //Spawnpoint
            case 0:
                GameObject sp = new GameObject();
                sp.transform.position = new Vector3(x, y, z);
                sp.name = "SpawnPoint" + (avaliableSpawnPoints.Count + 1);
                sp.transform.parent = this.worldParent.transform;
                avaliableSpawnPoints.Add(sp.transform.position);
                break;

            // Grass-block
            case 1:
                Instantiate(tile1Grass, new Vector3(x, y, z), Quaternion.identity, worldParent.transform);
                break;
            // Water
            case 2:
                Instantiate(tile2Water, new Vector3(x, y, z), Quaternion.identity, worldParent.transform);
                break;
            // Bridge
            case 3:
                Instantiate(tile3Bridge, new Vector3(x, y, z), Quaternion.identity, worldParent.transform);
                break;
            // Spikes
            case 4:
                Instantiate(tile4Spikes, new Vector3(x, y, z), Quaternion.identity, worldParent.transform);
                break;
        }


    }

    public void ClearWorld()
    {

        GameObject.Destroy(worldParent);
        CreateWorldParent();

        worldString = "";
        avaliableSpawnPoints.Clear();
        worldMiddle = Vector3.zero;
    }

    public void CreateWorldParent()
    {
        worldParent = new GameObject("Generated World");
        worldParent.transform.position = Vector3.zero;
        worldParent.transform.parent = this.transform;
    }

    public void MoveWorldToOrigin()
    {
        worldParent.transform.position = -worldMiddle;
    }

}
