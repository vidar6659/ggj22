using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2 coord;
    public bool isBlock;

    void Start()
    {

    }

    void Update()
    {

    }

    public void DefineTileStatus(int status)
    {
        if (status != 1)
            MakeEmpty();
    }

    public void MakeBlock()
    {
        isBlock = true;
        transform.GetChild(1).gameObject.SetActive(true);
    }

    public void MakeEmpty()
    {
        isBlock = false;
        transform.GetChild(1).gameObject.SetActive(false);
    }

    public Vector2 GetCoord()
    {
        return coord;
    }

    public void SetCoord(int x, int y)
    {
        coord.x = x;
        coord.y = y;
    }
}
