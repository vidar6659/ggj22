using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
            if (Game.isGamePaused)
                return;
            if (Game.isPathFound)
                return;

            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if(hit.transform.tag == tagEmptyTile)
                {
                    if (!levelManager.CheckIfTileIsInteractive(hit.transform.parent.GetComponent<Tile>().GetCoord()))
                        return;
                    hit.transform.parent.GetComponent<Tile>().MakeBlock();
                    levelManager.UpdateLogicLevel(hit.transform.parent.GetComponent<Tile>().GetCoord(), 1);
                }
                else if(hit.transform.tag == tagBlockedTile)
                {
                    if (!levelManager.CheckIfTileIsInteractive(hit.transform.parent.GetComponent<Tile>().GetCoord()))
                        return;
                    hit.transform.parent.GetComponent<Tile>().MakeEmpty();
                    levelManager.UpdateLogicLevel(hit.transform.parent.GetComponent<Tile>().GetCoord(), 0);
                }
            }
        }
    }

    private bool CheckIfTileIsInteractive()
    {
        if (hit.transform.parent.GetComponent<Tile>().isExit ||
            hit.transform.parent.GetComponent<Tile>().isStart)
            return false;
        return true;
    }
}
