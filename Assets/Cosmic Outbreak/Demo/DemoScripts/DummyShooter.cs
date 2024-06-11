using System;
using System.Collections;
using System.Collections.Generic;
using TopDownShooter;
using UnityEngine;

public class DummyShooter : MonoBehaviour
{
    public bool Active;
    public int CurrentDbWeaponIndex;
    public WeaponData WeaponData;
    public Transform LaunchPoint;

    private float _tempFireRate;

    public void FixedUpdate()
    {
        if (!Active) return;
        _tempFireRate -= Time.fixedDeltaTime;
        if (_tempFireRate<=0)
        {
            Shot();
        }
    }

    public void Shot()
    {
        var bullet = Instantiate(WeaponData.Weapons[CurrentDbWeaponIndex].Bullet, LaunchPoint.position,
            transform.rotation);
        //muzzle effect
        Instantiate(WeaponData.Weapons[CurrentDbWeaponIndex].MuzzleEffect, LaunchPoint.position,
            LaunchPoint.rotation);
        //add speed and direction to the bullet
        bullet.GetComponent<Rigidbody>()
            .AddForce(LaunchPoint.forward * WeaponData.Weapons[CurrentDbWeaponIndex].BulletSpeed);
        //add damage to bullet
        if (bullet.GetComponent<Damage>())
        {
            bullet.GetComponent<Damage>().DamagePower = WeaponData.Weapons[CurrentDbWeaponIndex].Damage;
        }


        _tempFireRate = WeaponData.Weapons[CurrentDbWeaponIndex].FireRate;
    }
}
