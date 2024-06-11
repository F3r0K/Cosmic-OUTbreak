using System;
using UnityEngine;

namespace TopDownShooter
{
    public class Damage : MonoBehaviour
    {
        public float DamagePower;
        public GameObject BulletImpact;

        [Header("DamageOnExplode")] public bool ExplodeDamageBullet;
        public float DamageRadius;
        public float ExplosionForcePower;
        public LayerMask RayCasterLayer;

        public Rigidbody RigidBodyComponent;
        private bool _impact;

        private void Awake()
        {
            if (!RigidBodyComponent)
            {
                RigidBodyComponent = GetComponent<Rigidbody>();
            }
        }

        public void SetupBullet(Vector3 launchForce, float bulletDamage)
        {
            DamagePower = bulletDamage;
            RigidBodyComponent
                .AddForce(launchForce);
        }

        public void SetupMeleeAttack(float damage)
        {
            DamagePower = damage;
        }

        private void ExplodeDamage()
        {
            var hitCollider = Physics.OverlapSphere(transform.position, DamageRadius, RayCasterLayer);
            foreach (var hit in hitCollider)
            {
                var rigidBody = hit.GetComponent<Rigidbody>();

                if (hit.GetComponent<HitPoint>())
                {
                    hit.GetComponent<HitPoint>().ApplyDamage(DamagePower);
                }

                if (rigidBody)
                {
                    rigidBody.AddExplosionForce(ExplosionForcePower, transform.position, DamageRadius);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_impact) return;
            _impact = true;
            Instantiate(BulletImpact, other.ClosestPointOnBounds(transform.position), BulletImpact.transform.rotation);
            if (ExplodeDamageBullet)
            {
                ExplodeDamage();
            }

            gameObject.SetActive(false);
            Destroy(gameObject, 0.1f);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (_impact) return;
            _impact = true;
            Instantiate(BulletImpact, other.contacts[0].point, BulletImpact.transform.rotation);
            if (ExplodeDamageBullet)
            {
                ExplodeDamage();
            }

            gameObject.SetActive(false);
            Destroy(gameObject, 0.1f);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, DamageRadius);
        }
    }
}