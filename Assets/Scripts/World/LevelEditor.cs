using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditor : MonoBehaviour
{

    public GameObject levelTileBlock;
    public GameObject worldOrigin;

    public List<GameObject> tilePrefabs = new List<GameObject>();
    int firstConveryorBelt = 6;

    public List<GameObject> editorBlocks = new List<GameObject>();


    public Vector2 worldSize = new Vector2(30, 30);

    GameObject worldParent;

    public Camera camera;

    // Start is called before the first frame update
    void Start()
    {

        worldParent = new GameObject("Editor World");
        worldParent.transform.parent = this.transform;

        for (int z = 0; z < worldSize.y; z++)
        {
            for (int x = 0; x < worldSize.x; x++)
            {
                GameObject tile = GameObject.Instantiate(levelTileBlock, new Vector3(x, 0, z), Quaternion.identity, worldParent.transform);
                tile.GetComponent<LevelEditorBlock>().SetTileList(tilePrefabs, firstConveryorBelt);
                editorBlocks.Add(tile);
            }
        }

        worldOrigin.transform.position = new Vector3(worldSize.x / 2, 0, worldSize.y / 2);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
            SaveWorldToFile("TestMap");

        if (Input.GetKeyDown(KeyCode.L))
            LoadWorldFromFile("TestMap");

        if (Input.GetMouseButtonDown(0))
        { // if left button pressed...
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.CompareTag("LevelEditorBlock"))
                {
                    hit.collider.GetComponent<LevelEditorBlock>().NextTile();
                }
            }
        }
    }

    public void SaveWorldToFile(string name)
    {
        using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"Worlds\" + name + ".txt", false))
        {
            foreach (GameObject tile in editorBlocks)
            {
                string line = "{" + tile.transform.position.x + "," + tile.transform.position.y + "," + tile.transform.position.z + "," + tile.GetComponent<LevelEditorBlock>().CurrentTileID + "}";

                writer.WriteLine(line);

            }

            writer.Close();
            Debug.Log("Saved to file");
        }
    }

    public void LoadWorldFromFile(string name)
    {
        GameObject.Destroy(worldParent);
        worldParent = new GameObject("Editor World");
        worldParent.transform.parent = this.transform;
        editorBlocks.Clear();

        string[] lines = System.IO.File.ReadAllLines(@"Worlds\" + name + ".txt");
        Debug.Log(lines.Length);
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
                    editorBlocks.Add(tile);

                    break;
                // Something is wrong with file
                default:
                    break;
            }


        }

    }


}
