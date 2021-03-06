using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditorBlock : MonoBehaviour
{
    List<GameObject> visableTiles = new List<GameObject>();

    int currentTile = 0;



    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log(tilePrefabs.Count);


    }

    public void SetTileList(List<GameObject> tilePrefabs, int firstConveryorBelt)
    {
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


    public void NextTile()
    {
        visableTiles[currentTile].SetActive(false);
        currentTile = (currentTile + 1 + visableTiles.Count) % visableTiles.Count;
        visableTiles[currentTile].SetActive(true);
    }

    public void PreviousTile()
    {
        visableTiles[currentTile].SetActive(false);
        currentTile = (currentTile - 1 + visableTiles.Count) % visableTiles.Count;
        visableTiles[currentTile].SetActive(true);
    }



    public int CurrentTileID
    {
        get { return currentTile; }
        set
        {
            visableTiles[currentTile].SetActive(false);
            currentTile = value;
            visableTiles[currentTile].SetActive(true);

        }
    }



}
