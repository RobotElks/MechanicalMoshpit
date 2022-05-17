using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MultiplayerWorldParse : MonoBehaviour
{
    // Different tiles (Gameobject) and input file (String)
    public GameObject tile1Ground;
    public GameObject healthStation;
    public GameObject energyStation;
    public GameObject damageTile;
    public GameObject tile10wall_x;
    public GameObject tile11wall_z;
    public GameObject tileFlag;

    public GameObject leftTurningGear;
    public GameObject rightTurningGear;
    public GameObject conveyorBelt;

    public List<Vector3> robotSpawnPoints = new List<Vector3>();
    public List<Vector3> flagSpawnPoints = new List<Vector3>();

    public string worldString = "";

    GameObject worldParent;

    Vector3 worldMiddle = Vector3.zero;

    Vector3 defaultFlagSpawn = Vector3.zero;
    GameObject flag;

    // Start is called before the first frame update
    void Start()
    {
        //ClearWorld();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            RandomFlagPosition();
    }

    public void BuildWorldString(string part)
    {
        Debug.Log("part world : " + part);
        worldString += part;
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

    public void AddToWorldString(string worldStringPart)
    {
        worldString += worldStringPart;
    }

    public Vector3 GetLobbySpawnPoint()
    {
        return new Vector3(1015, 15, 15);
    }

    public Vector3[] GetSpawnPoints()
    {
        List<Vector3> sp = new List<Vector3>();
        Vector3[] save = robotSpawnPoints.ToArray();

        while (robotSpawnPoints.Count > 0)
        {
            int i = Random.Range(0, robotSpawnPoints.Count);
            sp.Add(robotSpawnPoints[i]);
            robotSpawnPoints.RemoveAt(i);
        }

        robotSpawnPoints.AddRange(save);
        return sp.ToArray();
    }

    public Vector3 GetSpawnPoint()
    {
        return robotSpawnPoints[Random.Range(0, robotSpawnPoints.Count)];
    }

    public void BuildLobby()
    {
        worldString = Resources.Load<TextAsset>("lobby").text;
        BuildWorld();
        worldString = "";
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

        flag = Instantiate(tileFlag, defaultFlagSpawn, Quaternion.identity, worldParent.transform);
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
            // Ground-block
            case 0:
                Instantiate(tile1Ground, new Vector3(x, y, z), Quaternion.identity, worldParent.transform);
                break;
            // Health station
            case 1:
                Instantiate(healthStation, new Vector3(x, y, z), Quaternion.identity, worldParent.transform);
                break;
            case 2:
                Instantiate(energyStation, new Vector3(x, y, z), Quaternion.identity, worldParent.transform);
                break;
            // Spikes
            case 3:
                Instantiate(damageTile, new Vector3(x, y - 0.1f, z), Quaternion.identity, worldParent.transform);
                break;
                //Gears
            case 4:
                Instantiate(leftTurningGear, new Vector3(x, y, z), Quaternion.identity, worldParent.transform);
                break;
            case 5:
                Instantiate(rightTurningGear, new Vector3(x, y, z), Quaternion.identity, worldParent.transform);
                break;
            case 6:
                Instantiate(conveyorBelt, new Vector3(x, y, z), Quaternion.identity, worldParent.transform);
                break;
            case 7:
                GameObject righttile = Instantiate(conveyorBelt, new Vector3(x, y, z), Quaternion.identity, worldParent.transform);
                righttile.transform.eulerAngles = 90f * Vector3.up;

                break;
            case 8:
                GameObject backwardtile = Instantiate(conveyorBelt, new Vector3(x, y, z), Quaternion.identity, worldParent.transform);
                backwardtile.transform.eulerAngles = 180f * Vector3.up;
                break;
            case 9:
                GameObject lefttile = Instantiate(conveyorBelt, new Vector3(x, y, z), Quaternion.identity, worldParent.transform);
                lefttile.transform.eulerAngles = 270f * Vector3.up;
                break;

                //Flag
            case 10:
                defaultFlagSpawn = new Vector3(x, y, z);
                break;
            // wall_x

            //Spawnpoint
            case 11:
                GameObject sp = new GameObject();
                sp.transform.position = new Vector3(x, y, z);
                sp.name = "SpawnPoint" + (robotSpawnPoints.Count + 1);
                sp.transform.parent = this.worldParent.transform;
                robotSpawnPoints.Add(sp.transform.position);
                break;

            case 12:
                Instantiate(tile10wall_x, new Vector3(x, y + 0.4f, z - 0.5f), Quaternion.identity, worldParent.transform);
                break;
            // wall_y
            case 13:
                Instantiate(tile11wall_z, new Vector3(x - 0.5f, y + 0.4f, z), Quaternion.identity, worldParent.transform);
                break;

            

        }


    }

    public void ClearWorld()
    {

        GameObject.Destroy(worldParent);
        CreateWorldParent();

        worldString = "";
        robotSpawnPoints.Clear();
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

    public void RandomFlagPosition()
    {
        Vector3 oldPos = flag.transform.position;

        while ((oldPos - flag.transform.position).magnitude < 10)
        {
            flag.transform.position = flagSpawnPoints[Random.Range(0, flagSpawnPoints.Count)];
        }
    }

    public Vector3 GetFlagPosition()
    {
        return flag.transform.position;
    }

    public void SetFlagPosition(Vector3 newPos)
    {
        flag.transform.position = newPos;
    }

}
