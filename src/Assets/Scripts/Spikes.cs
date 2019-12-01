using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        CharacterController2D controller = other.GetComponent<CharacterController2D>();

        if (controller != null)
        {
            StartCoroutine(controller.Die());
        }
    }
}
