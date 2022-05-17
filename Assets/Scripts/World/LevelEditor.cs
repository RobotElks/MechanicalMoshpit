using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.EventSystems;
using TMPro;

public class LevelEditor : MonoBehaviour
{

    public GameObject levelTileBlock;
    public GameObject worldOrigin;
    public LevelEditorMenu editorMenu;

    public List<GameObject> tilePrefabs = new List<GameObject>();
    public int firstConveryorBelt = 7;

    
    public List<GameObject> worldBlocks = new List<GameObject>();


    public Vector2 worldSize = new Vector2(30, 30);
    GameObject worldParent;

    public Camera camera;

    public TMP_Dropdown tileSelector;

    // Start is called before the first frame update
    void Start()
    {
        worldParent = new GameObject("Editor World");
        //NewWorld();
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.S))
        //    SaveWorldToFile("TestMap");

        //if (Input.GetKeyDown(KeyCode.L))
        //    LoadWorldFromFile("TestMap");

        //if (Input.GetKeyDown(KeyCode.M))
        //    Debug.Log(GetMapNames().Length);

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
            }
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


        for (int z = 0; z < worldSize.y; z++)
        {
            for (int x = 0; x < worldSize.x; x++)
            {
                GameObject tile = GameObject.Instantiate(levelTileBlock, new Vector3(x, 0, z), Quaternion.identity, worldParent.transform);
                tile.GetComponent<LevelEditorBlock>().SetTileList(tilePrefabs, firstConveryorBelt);
                tile.GetComponent<LevelEditorBlock>().CurrentTileID = 1;
                worldBlocks.Add(tile);
            }
        }

        worldOrigin.transform.position = new Vector3(worldSize.x / 2, 0, worldSize.y / 2);
    }

    public void SaveWorldToFile(string name)
    {
        StreamWriter writer = new System.IO.StreamWriter(@"Worlds\" + name + ".txt", false);
        foreach (GameObject tile in worldBlocks)
        {
            string line = "{" + tile.transform.position.x + "," + tile.transform.position.y + "," + tile.transform.position.z + "," + tile.GetComponent<LevelEditorBlock>().CurrentTileID + "}";

            writer.WriteLine(line);

        }

        writer.Close();

    }

    public void LoadWorldFromFile(string name)
    {
        GameObject.Destroy(worldParent);
        worldParent = new GameObject("Editor World");
        worldParent.transform.parent = this.transform;
        worldBlocks.Clear();

        string path = @"Worlds\" + name + ".txt";


        string[] lines = File.ReadAllLines(path);
        foreach (string line in lines)
        {

            switch (line[0])
            {
                // Comment
                case '#':
                    break;
                // Level-data
                case '{':

                    int x = 0;
                    int y = 0;
                    int z = 0;
                    int tileID = 0;
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

                    GameObject tile = GameObject.Instantiate(levelTileBlock, new Vector3(x, 0, z), Quaternion.identity, worldParent.transform);
                    tile.GetComponent<LevelEditorBlock>().SetTileList(tilePrefabs, firstConveryorBelt);
                    tile.GetComponent<LevelEditorBlock>().CurrentTileID = tileID;
                    worldBlocks.Add(tile);

                    break;
                // Something is wrong with file
                default:
                    break;
            }
        }

        worldOrigin.transform.position = new Vector3(worldSize.x / 2, 0, worldSize.y / 2);

    }

}
