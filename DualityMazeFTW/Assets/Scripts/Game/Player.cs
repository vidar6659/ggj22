using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    Ray ray;
    RaycastHit hit;

    string tagEmptyTile = "EmptyTile";
    string tagBlockedTile = "BlockedTile";

    LevelManager levelManager;

    public Player(LevelManager lvlManager)
    {
        levelManager = lvlManager;
    }

    public void Act()
    {

        if (Input.GetMouseButtonUp(0))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if(hit.transform.tag == tagEmptyTile)
                {
                    hit.transform.parent.GetComponent<Tile>().MakeBlock();
                    levelManager.UpdateLogicLevel(hit.transform.parent.GetComponent<Tile>().GetCoord(), 1);
                }
                else if(hit.transform.tag == tagBlockedTile)
                {
                    hit.transform.parent.GetComponent<Tile>().MakeEmpty();
                    levelManager.UpdateLogicLevel(hit.transform.parent.GetComponent<Tile>().GetCoord(), 0);
                }
            }
        }
    }
}
