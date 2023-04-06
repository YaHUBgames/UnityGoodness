using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddForceOnObject : MonoBehaviour
{
    [SerializeField] private Rigidbody RB;
    [SerializeField] private Vector3 force;
    private void FixedUpdate()
    {
        RB.AddForce(force);
    }
}
