using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCheck : MonoBehaviour
{
    private CharacterMovement characterMovement;

    private void Start()
    {
        characterMovement = transform.parent.GetComponent<CharacterMovement>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Ball")
        {
            characterMovement.BallCheckTriggered(true, other.GetComponent<Rigidbody2D>());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Ball")
        {
            characterMovement.BallCheckTriggered(false, null);
        }
    }
}