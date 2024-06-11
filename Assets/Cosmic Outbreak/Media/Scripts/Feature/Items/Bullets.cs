using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TopDownShooter
{
    public class Bullets : MonoBehaviour
    {
        [Header("Bullets info")] [Tooltip("The id for the bullet, this is the type of bullet.")]
        public int BulletId;

        [Tooltip("Amount of bullets to add to the player")]
        public int BulletAmount;

        public GameObject TakenEffect;

        private bool _isTaken;

        private void OnTriggerEnter(Collider other)
        {
            //add bulletAmount to the player if the player can carry this type of bullet
            if (!_isTaken && other.CompareTag("Player") && other.GetComponent<PlayerController>().ShooterController)
            {
                var bulletCanBeAdd = other.GetComponent<PlayerController>().ShooterController
                    .AddBullet(BulletId, BulletAmount);
                if (bulletCanBeAdd)
                {
                    _isTaken = true;
                    //Instantiate the effect if have effect
                    if (TakenEffect)
                    {
                        Instantiate(TakenEffect, transform.position, transform.rotation);
                    }

                    Destroy(gameObject);
                }
            }
        }
    }
}