using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bob : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
        if (other.transform.name == "Exit")
        {
            Debug.Log("You win");
        }
    }
}
