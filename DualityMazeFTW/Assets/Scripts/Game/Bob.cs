using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bob : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("colliding with: " + collision.transform.name);
        //if (collision.transform.name == "Exit")
        //{
        //    Debug.Log("You win");
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.name == "Exit" && !Game.isLevelCompleted)
        {
            Debug.Log("You win");
            Game.EndLevel();
        }
    }
}
