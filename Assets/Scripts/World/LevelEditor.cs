using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditor : MonoBehaviour
{

    public GameObject levelTileBlock;
    public GameObject worldOrigin;

    public List<GameObject> tilePrefabs = new List<GameObject>();
    int firstConveryorBelt = 6;


    GameObject selector;
    LevelEditorBlock selectorScript;
    public float selectorCameraDistance = 5f;
    public Vector3 selectorCameraOffset = new Vector3(0, 0, 0);


    public Vector2 worldSize = new Vector2(30, 30);

    GameObject worldParent;

    public Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        selector = GameObject.Instantiate(levelTileBlock, transform);
        selectorScript = selector.GetComponent<LevelEditorBlock>();
        selectorScript.SetTileList(tilePrefabs, firstConveryorBelt);
        selector.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

        worldParent = new GameObject("Generated World");
        worldParent.transform.parent = this.transform;

        for (int z = 0; z < worldSize.y; z++)
        {
            for (int x = 0; x < worldSize.x; x++)
            {
                GameObject.Instantiate(levelTileBlock, new Vector3(x, 0, z), Quaternion.identity, worldParent.transform).GetComponent<LevelEditorBlock>().SetTileList(tilePrefabs, firstConveryorBelt);
            }
        }

        worldOrigin.transform.position = new Vector3(worldSize.x / 2, 0, worldSize.y / 2);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 offset = new Vector3(selectorCameraOffset.x * camera.transform.right.x, selectorCameraOffset.y * camera.transform.right.y, selectorCameraOffset.z * camera.transform.right.z);
        selector.transform.position = camera.transform.position + camera.transform.forward * selectorCameraDistance ;


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


}
