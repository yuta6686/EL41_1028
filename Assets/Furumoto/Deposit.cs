using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deposit : MonoBehaviour
{
    [SerializeField]
    private float mass = 1.0f;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Player>().Adhesion(mass, transform);
            Destroy(transform.GetComponent<Rigidbody>());
        }
    }
}
