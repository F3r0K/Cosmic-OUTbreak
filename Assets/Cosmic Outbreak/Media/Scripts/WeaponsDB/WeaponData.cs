using System;
using System.Collections.Generic;
using UnityEngine;

namespace TopDownShooter
{
    public class WeaponData : MonoBehaviour
    {
        public List<Weapon> Weapons;

        public Weapon GetWeaponByName(string weaponName)
        {
            return Weapons.Find(e => e.WeaponName == weaponName);
        }
    }

    [Serializable]
    public class Weapon
    {
        public enum WeaponType
        {
            Hands = 0,
            Rifle = 1,
            Pistol = 2,
            Melee = 3,
            TrowItem = 4
        }

        [Header("Weapon settings")] public string WeaponName = "Empty weapon";
        public WeaponType WeaponClass = WeaponType.Hands;
        public GameObject WeaponPrefab;
        public Sprite WeaponImage;
        public GameObject MuzzleEffect;

        [Tooltip("Number of bullet that can load this weapon.")]
        public int WeaponMagazine;

        [Tooltip("Time to reload the weapon")] public float ReloadTime;

        [Tooltip("Time between bullets spawn.")]
        public float FireRate;

        [Tooltip("Id for the type of bullet use for this weapon.")]
        public int BulletId;

        [Tooltip("Bullets prefab to spawn.")] public GameObject Bullet;
        [Tooltip("Bullet damage.")] public float Damage;
        [Tooltip("Launched bullet speed.")] public float BulletSpeed;
        [Tooltip("Delay to launch.")] public float DelayToLaunch;

        [Tooltip("Delay to turn on the player after actions.")]
        public float DelayToTurn;

        [Header("Weapon drop prefab")]
        [Tooltip(
            "When the player drop the weapon this object appear in the floor, and is use to get the weapon again.")]
        public GameObject WeaponDrop;
    }
}