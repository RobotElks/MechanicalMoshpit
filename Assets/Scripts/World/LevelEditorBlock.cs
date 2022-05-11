using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditorBlock : MonoBehaviour
{
    public List<GameObject> tilePrefabs = new List<GameObject>();
    List<GameObject> visableTiles = new List<GameObject>();

    public int currentTile = 1;

    int firstConveryorBelt = 6;

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log(tilePrefabs.Count);

        for (int i = 0; i < tilePrefabs.Count; i++)
        {
            GameObject tile = Instantiate(tilePrefabs[i], this.transform.position, Quaternion.identity, this.transform);
            tile.SetActive(false);
            tile.GetComponent<BoxCollider>().enabled = false;
            //Rotate conveoyrbelts
            if (i > firstConveryorBelt && i < firstConveryorBelt + 4)
            {
                tile.transform.eulerAngles = (i - firstConveryorBelt) * 90f * Vector3.up;
            }

            visableTiles.Add(tile);
        }

        visableTiles[0].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            NextTile();
    }

    public void NextTile()
    {
        visableTiles[currentTile].SetActive(false);
        currentTile = (currentTile + 1 + tilePrefabs.Count) % tilePrefabs.Count;
        visableTiles[currentTile].SetActive(true);
    }

    public void PreviousTile()
    {
        visableTiles[currentTile].SetActive(false);
        currentTile = (currentTile - 1 + tilePrefabs.Count) % tilePrefabs.Count;
        visableTiles[currentTile].SetActive(true);
    }
}
