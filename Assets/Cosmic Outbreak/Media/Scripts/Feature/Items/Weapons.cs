using System.Collections;
using UnityEngine;

namespace TopDownShooter
{
    public class Weapons : MonoBehaviour
    {
        [Header("Weapon info")] [Tooltip("The weapon index on DB.")]
        public int WeaponIndex;

        [Tooltip("Current weapon bullets.")] public int WeaponBulletAmount;

        [Tooltip("Effect when player take the weapon.")]
        public GameObject TakenEffect;

        [Tooltip("The item can be taken after this time")]
        public int ActivateTime = 1;

        private bool _isTaken;

        private void Start()
        {
            StartCoroutine(ActivateByTime(ActivateTime));
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !_isTaken)
            {
                var canBeAdd = other.GetComponent<PlayerController>().ShooterController
                    .AddWeaponWhitBullets(WeaponIndex, WeaponBulletAmount);

                if (canBeAdd)
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

        private IEnumerator ActivateByTime(int timeToActivate)
        {
            _isTaken = true;
            yield return new WaitForSeconds(timeToActivate);
            _isTaken = false;
        }
    }
}