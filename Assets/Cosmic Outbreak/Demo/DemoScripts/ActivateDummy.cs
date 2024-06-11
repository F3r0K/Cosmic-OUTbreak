using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateDummy : MonoBehaviour
{
    public DummyShooter DummyShooter;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DummyShooter.Active = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DummyShooter.Active = false;
        }
    }
}