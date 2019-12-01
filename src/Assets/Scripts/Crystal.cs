using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : MonoBehaviour
{

    public GameObject player;
    void OnTriggerEnter2D(Collider2D other)
    { 

        if (other.gameObject.name == "s")
        {
            Physics2D.gravity = new Vector2(0, 2.4f);
        }
      
    }

    private void OnTriggerExit2D(Collider2D other)
    {

        if (other.gameObject.name == "s")
        {
            Physics2D.gravity = new Vector2(0, -9.8f);
        }
       
    }
}
