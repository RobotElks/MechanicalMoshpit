using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.EventSystems;
using TMPro;
using System.Linq;

public class LevelEditor : MonoBehaviour
{

    public GameObject levelTileBlock;
    public GameObject worldOrigin;
    public LevelEditorMenu editorMenu;


    public List<GameObject> tilePrefabs = new List<GameObject>();
    public GameObject wallXPrefab;
    public GameObject wallZPrefab;

    public int beltInedx = 6;
    public int flagIndex = 10;
    public int wallIndex = 12;



    List<GameObject> worldBlocks = new List<GameObject>();
    List<GameObject> worldWalls = new List<GameObject>();

    public Vector2 worldSize = new Vector2(31, 31);
    GameObject worldParent, wallParent;

    public Camera camera;
    public float cameraSpeed = 5f;
    bool editWalls = false;


    public TMP_Dropdown tileSelector;

    // Start is called before the first frame update
    void Start()
    {
        worldParent = new GameObject("Editor World");
        wallParent = new GameObject("Editor Walls");

        SpawnWalls();
        //NewWorld();
        worldOrigin.transform.position = new Vector3(worldSize.x / 2, 0, worldSize.y / 2);

    }

    // Update is called once per frame
    void Update()
    {
        //Move camera
        if (!editorMenu.HasMenuopen())
        {
            if (Input.GetKey(KeyCode.W))
                worldOrigin.transform.position += camera.transform.forward * cameraSpeed * Time.deltaTime;
            else if (Input.GetKey(KeyCode.S))
                worldOrigin.transform.position -= camera.transform.forward * cameraSpeed * Time.deltaTime;
            else if (Input.GetKey(KeyCode.A))
                worldOrigin.transform.position -= camera.transform.right * cameraSpeed * Time.deltaTime;
            else if (Input.GetKey(KeyCode.D))
                worldOrigin.transform.position += camera.transform.right * cameraSpeed * Time.deltaTime;
        }


        //Click to edit map and not on UI
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && !editorMenu.HasMenuopen())
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.CompareTag("LevelEditorBlock"))
                {
                    hit.collider.GetComponent<LevelEditorBlock>().CurrentTileID = tileSelector.value;
                }

                else if (hit.collider.gameObject.CompareTag("LevelEditorWall"))
                {
                    hit.collider.GetComponent<LevelEditorWall>().ToggleWall();
                }
            }
        }
    }

    private void SpawnWalls()
    {
        GameObject.Destroy(wallParent);
        wallParent = new GameObject("Editor Walls");
        worldWalls.Clear();

        for (int z = 0; z <= worldSize.y; z++)
        {
            for (int x = 0; x <= worldSize.x; x++)
            {
                if (x != worldSize.x)
                {
                    GameObject wall = Instantiate(wallXPrefab, new Vector3(x, 0, z), Quaternion.identity, wallParent.transform);
                    wall.GetComponent<LevelEditorWall>().HideWall();
                    wall.GetComponent<LevelEditorWall>().walltype = LevelEditorWall.Walltype.WallX;
                    wall.GetComponent<BoxCollider>().enabled = false;

                    worldWalls.Add(wall);
                }

                if (z != worldSize.y)
                {
                    GameObject wall = Instantiate(wallZPrefab, new Vector3(x, 0, z), Quaternion.identity, wallParent.transform);
                    wall.GetComponent<LevelEditorWall>().HideWall();
                    wall.GetComponent<LevelEditorWall>().walltype = LevelEditorWall.Walltype.WallZ;
                    wall.GetComponent<BoxCollider>().enabled = false;

                    worldWalls.Add(wall);
                }

            }
        }
    }

    public void HideAllWalls()
    {
        foreach (GameObject wall in worldWalls)
        {
            wall.GetComponent<LevelEditorWall>().HideWall();
            wall.GetComponent<BoxCollider>().enabled = false;
        }
    }

    public string[] GetMapNames()
    {
        string[] names = Directory.GetFiles(@"Worlds");

        for (int i = 0; i < names.Length; i++)
        {
            names[i] = names[i].Split("\\")[1].Split(".")[0];
        }

        return names;
    }

    public void NewWorld()
    {
        GameObject.Destroy(worldParent);
        worldParent = new GameObject("Editor World");
        worldParent.transform.parent = this.transform;
        worldBlocks.Clear();

        HideAllWalls();
        tileSelector.value = 0;

        for (int z = 0; z < worldSize.y; z++)
        {
            for (int x = 0; x < worldSize.x; x++)
            {
                GameObject tile = GameObject.Instantiate(levelTileBlock, new Vector3(x, 0, z), Quaternion.identity, worldParent.transform);
                tile.GetComponent<LevelEditorBlock>().SetTileList(tilePrefabs, beltInedx);
                tile.GetComponent<LevelEditorBlock>().CurrentTileID = 0;
                worldBlocks.Add(tile);
            }
        }

        worldOrigin.transform.position = new Vector3(worldSize.x / 2, 0, worldSize.y / 2);
    }

    public void SaveWorldToFile(string name)
    {
        StreamWriter writer = new System.IO.StreamWriter(@"Worlds\" + name + ".txt", false);

        //Blocks
        foreach (GameObject tile in worldBlocks)
        {
            if (tile.GetComponent<LevelEditorBlock>().CurrentTileID >= flagIndex)
            {
                writer.WriteLine("{" + tile.transform.position.x + "," + (tile.transform.position.y + 1) + "," + tile.transform.position.z + "," + tile.GetComponent<LevelEditorBlock>().CurrentTileID + "}");
                writer.WriteLine("{" + tile.transform.position.x + "," + (tile.transform.position.y) + "," + tile.transform.position.z + "," + 0 + "}");
            }

            else
                writer.WriteLine("{" + tile.transform.position.x + "," + tile.transform.position.y + "," + tile.transform.position.z + "," + tile.GetComponent<LevelEditorBlock>().CurrentTileID + "}");
        }

        //Walls
        foreach (GameObject wall in worldWalls)
        {
            LevelEditorWall wallScript = wall.GetComponent<LevelEditorWall>();

            if (wallScript.IsVisible())
                writer.WriteLine("{" + wall.transform.position.x + "," + wall.transform.position.y + "," + wall.transform.position.z + "," + (wallIndex + (int)wallScript.walltype) + "}");


        }

        writer.Close();

    }

    public void LoadWorldFromFile(string name)
    {
        GameObject.Destroy(worldParent);
        worldParent = new GameObject("Editor World");
        worldParent.transform.parent = this.transform;
        worldBlocks.Clear();

        HideAllWalls();

        tileSelector.value = 0;


        string path = @"Worlds\" + name + ".txt";

        string[] fileLines = File.ReadAllLines(path).Where(l => l[0] == '{').ToArray();

        List<string> tileInfo = new List<string>();

        int x = 0;
        int z = 0;
        int y = 0;
        int tileID = 0;
        //Tiles at ground level (not spawns or flags)
        tileInfo.AddRange(fileLines.Where(l => int.Parse(l.Replace("{", "").Replace("}", "").Split(',').Last()) < flagIndex).ToArray());
        foreach (string line in tileInfo)
        {

            string[] extracted = line.Split(',', '{', '}');
            try
            {
                x = int.Parse(extracted[1]);
                z = int.Parse(extracted[3]);
                tileID = int.Parse(extracted[4]);

                //Always spawn at 0 so that spawnpoints dont place to high
                GameObject tile = GameObject.Instantiate(levelTileBlock, new Vector3(x, 0, z), Quaternion.identity, worldParent.transform);
                tile.GetComponent<LevelEditorBlock>().SetTileList(tilePrefabs, beltInedx);
                tile.GetComponent<LevelEditorBlock>().CurrentTileID = tileID;
                worldBlocks.Add(tile);
            }
            catch
            {
                Debug.Log($"Unable to parse!");
            }



        }

        //Flags, spawns, and walls
        tileInfo.Clear();
        tileInfo.AddRange(fileLines.Where(l => int.Parse(l.Replace("{", "").Replace("}", "").Split(',').Last()) >= flagIndex).ToArray());

        foreach (string line in tileInfo)
        {

            string[] extracted = line.Split(',', '{', '}');
            try
            {
                x = int.Parse(extracted[1]);
                z = int.Parse(extracted[3]);
                y = int.Parse(extracted[2]);
                tileID = int.Parse(extracted[4]);

                //Flag, spawn
                if (tileID < wallIndex)
                {
                    Vector3 pos = new Vector3(x, 0, z);

                    worldBlocks.Where(t => t.transform.position == pos).First().GetComponent<LevelEditorBlock>().CurrentTileID = tileID;
                }

                //Walls
                else
                {
                    Vector3 pos = new Vector3(x, y, z);
                    worldWalls.Where(t => t.transform.position == pos && (int)t.GetComponent<LevelEditorWall>().walltype == (tileID - wallIndex)).First().GetComponent<LevelEditorWall>().ShowWall();
                }
            }
            catch
            {
                Debug.Log($"Unable to parse!");
            }

        }



        worldOrigin.transform.position = new Vector3(worldSize.x / 2, 0, worldSize.y / 2);
    }

    public void CheckMouseLayerMask()
    {

        if (tileSelector.value >= wallIndex)
        {
            if (!editWalls)
            {
                editWalls = true;
                foreach (GameObject block in worldBlocks)
                    block.GetComponent<BoxCollider>().enabled = false;
                foreach (GameObject wall in worldWalls)
                    wall.GetComponent<BoxCollider>().enabled = true;
            }

        }
        else if (editWalls)
        {
            editWalls = false;
            foreach (GameObject block in worldBlocks)
                block.GetComponent<BoxCollider>().enabled = true;
            foreach (GameObject wall in worldWalls)
                wall.GetComponent<BoxCollider>().enabled = false;
        }

    }
}
