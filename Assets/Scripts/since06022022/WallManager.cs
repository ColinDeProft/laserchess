using System.Collections.Generic;
using UnityEngine;
using System;

namespace since06022022
{
    public class WallManager : MonoBehaviour
    {
        private List<Transform> wallableTiles;


        private void Start()
        {
            GameObject wall = GameObject.FindGameObjectsWithTag(Constants.WallTag)[0];
            foreach (Transform tile in GameObject.Find("Board2/Tiles/WallableTiles").transform)
            {
                for (int i = 0; i < 8; i++)
                {
                    if((int)UnityEngine.Random.Range(0f, 1000f) % Constants.WallPlacementRarityFactor == 0) { 
                        try {
                            Transform newWall = Instantiate(wall).transform;
                            //newWall.position = tile.position;
                            newWall.position = new Vector3(tile.position.x, newWall.localPosition.y, tile.position.z);
                            newWall.SetParent(tile);
                            newWall.eulerAngles = new Vector3(newWall.eulerAngles.x, newWall.eulerAngles.y + i*45, newWall.eulerAngles.z);
                        } catch (InvalidOperationException e) { print(e); }
                    }
                }
            }
        }
        
    }
}