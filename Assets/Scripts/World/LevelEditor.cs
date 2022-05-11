using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditor : MonoBehaviour
{

    public GameObject levelTileBlock;
    public GameObject worldOrigin;

    public Vector2 worldSize = new Vector2(30, 30);

    GameObject worldParent;

    public Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        worldParent = new GameObject("Generated World");
        worldParent.transform.parent = this.transform;

        for (int z = 0; z < worldSize.y; z++)
        {
            for (int x = 0; x < worldSize.x; x++)
            {
                GameObject.Instantiate(levelTileBlock, new Vector3(x, 0, z), Quaternion.identity, worldParent.transform);
            }
        }

        worldOrigin.transform.position = new Vector3(worldSize.x / 2, 0, worldSize.y / 2);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        { // if left button pressed...
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if(hit.collider.gameObject.CompareTag("LevelEditorBlock"))
                {
                    hit.collider.GetComponent<LevelEditorBlock>().NextTile();
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        { // if left button pressed...
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.CompareTag("LevelEditorBlock"))
                {
                    hit.collider.GetComponent<LevelEditorBlock>().PreviousTile();
                }
            }
        }
    }


}
