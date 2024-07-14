using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    private void OnTriggerEnter(Collider other)
    {
        //makes sure it doesnt count itsef
        if (other.gameObject == playerController.gameObject) { return; }

        playerController.SetGroundedState(true);
    }

    private void OnTriggerExit(Collider other)
    {
        //makes sure it doesnt count itsef
        if (other.gameObject == playerController.gameObject) { return; }

        playerController.SetGroundedState(false);
    }

    private void OnTriggerStay(Collider other)
    {
        //makes sure it doesnt count itsef
        if (other.gameObject == playerController.gameObject) { return; }

        playerController.SetGroundedState(true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //makes sure it doesnt count itsef
        if (collision.gameObject == playerController.gameObject) { return; }

        playerController.SetGroundedState(true);
    }

    private void OnCollisionExit(Collision collision)
    {
        //makes sure it doesnt count itsef
        if (collision.gameObject == playerController.gameObject) { return; }

        playerController.SetGroundedState(false);
    }

    private void OnCollisionStay(Collision collision)
    {
        //makes sure it doesnt count itsef
        if (collision.gameObject == playerController.gameObject) { return; }

        playerController.SetGroundedState(true);
    }
}
