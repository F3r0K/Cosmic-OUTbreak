using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TopDownShooter
{
    public class ShooterController : MonoBehaviour
    {
        public AudioClip AudioShoot;
        public AudioClip AudioReload;
        public AudioClip AudioDrop;
        [Header("Start weapon")]
        [Tooltip("This is the index to load weapons from WeaponData when start, -1 don't load any weapon")]
        public int StartWeaponIndex;

        [Header("Current weapon info:")] public List<WeaponsBullets> WeaponsBullets;

        [Header("Type of bullets:")]
        [Tooltip("This is the type of bullet the player can carry and the current number of them.")]
        public List<TypeOfBullet> BulletType;

        [Header("Player current weapons:")] [Tooltip("Maxim number of weapons for the player")]
        public int MaxPlayerWeapons = 2;

        [Tooltip("This is the index on weaponDB for the current weapon.")]
        public int CurrentDbWeaponIndex = -1;

        [Tooltip("Index of bullet for the current weapon.")]
        public int CurrentBulletsId = -1;

        [Tooltip("Weapon class, this is use to know the animations to play for this weapon.")]
        public Weapon.WeaponType CurrentWeaponClass = Weapon.WeaponType.Hands;

        [Header("Position to instantiate bullets")]
        public Transform BulletPoint;

        [Tooltip("The new weapon is instantiate in this position.")]
        public Transform WeaponPosition;

        [Header("Weapons Database:")] public WeaponData WeaponData;

        [Header("Script reference")] public PlayerController PlayerController;

        //input
        private bool _reload;

        private bool _dropWeapon;

        //temporal vars
        private float _tempFireRate;
        private GameObject _currentWeapon;
        private bool _reloading;

        [HideInInspector] public float DelayToTurnOn;

        private void Start()
        {
            StarterWeapon(StartWeaponIndex);
        }

        private void Update()
        {
            //input for dop weapon
            _dropWeapon = PlayerController.GetDropWeaponValue();
            //input for reload 
            _reload = PlayerController.GetReloadWeaponValue();

            if (_reload && !_reloading)
            {
                ReloadWeapon();
            }

            if (_dropWeapon)
            {
                DropWeapon();
            }
        }

        public void ManageShoot()
        {
            //handle fire rate time
            if (_tempFireRate > 0)
            {
                _tempFireRate -= Time.deltaTime;
            }

            //check if the player is moving the joystick to aim and have weapons
            if (PlayerController.MovCharController.TensionFoRightStickLowerThan(0.1f) &&
                CurrentWeaponClass != Weapon.WeaponType.Hands)
            {
                SetAimAnimation(true);
                //If the player joystick is push more than 0.4f and have bullet in the magazine then shoot else try to reload
                if (!PlayerController.MovCharController.TensionFoRightStickLowerThan(0.4f) || !(_tempFireRate <= 0) ||
                    _reloading) return;
                //if current weapon is melee make swing
                if (CurrentWeaponClass == Weapon.WeaponType.Melee)
                {
                    MeleeAttack();
                    return;
                }


                //if have bullets in magazine shot, if not try to reload 
                if (WeaponsBullets.Find(weapon => weapon.WeaponLoadIndex == CurrentDbWeaponIndex)
                        .WeaponCurrentBullets > 0)
                {
                    //if current weapon is launch item like Grenade launch it
                    if (CurrentWeaponClass == Weapon.WeaponType.TrowItem)
                    {
                        Grenade();
                    }
                    else
                    {
                        Shoot();
                    }
                }
                else
                {
                    ReloadWeapon();
                }
            }
            else
            {
                DelayToTurnOn -= Time.deltaTime;
                if (DelayToTurnOn <= 0)
                {
                    SetAimAnimation(false);
                }
            }
        }

        //shoot bullet
        private void Shoot()
        {
            GetComponent<AudioSource>().PlayOneShot(AudioShoot);
            //play shoot animation
            ShotAnimation();
            //instantiate bullet and set speed, direction and damage.
            var bullet = Instantiate(WeaponData.Weapons[CurrentDbWeaponIndex].Bullet, BulletPoint.position,
                transform.rotation).GetComponent<Damage>();
            //muzzle effect
            if (WeaponData.Weapons[CurrentDbWeaponIndex].MuzzleEffect)
            {
                Instantiate(WeaponData.Weapons[CurrentDbWeaponIndex].MuzzleEffect, BulletPoint.position,
                    BulletPoint.rotation);
            }

            //add speed and direction to the bullet
            bullet.SetupBullet(BulletPoint.forward * WeaponData.Weapons[CurrentDbWeaponIndex].BulletSpeed,
                WeaponData.Weapons[CurrentDbWeaponIndex].Damage);

            WeaponsBullets.Find(weapon => weapon.WeaponLoadIndex == CurrentDbWeaponIndex).WeaponCurrentBullets--;


            _tempFireRate = WeaponData.Weapons[CurrentDbWeaponIndex].FireRate;
        }

        //make melee attack (note: if you can make this on animation the result is better and more accurate)
        private void MeleeAttack()
        {
            StopCoroutine(LaunchMeleeAttack());
            StartCoroutine(LaunchMeleeAttack());
        }

        private void Grenade()
        {
            StopCoroutine(LaunchGrenade());
            StartCoroutine(LaunchGrenade());
        }

        //This allow the player to add weapon at the start
        private void StarterWeapon(int weaponId)
        {
            AddNewWeapon(weaponId);
        }

        //add a new weapon to the player, if the player don't have any weapon, then equip this weapon
        private void AddNewWeapon(int weaponIndex)
        {
            if (weaponIndex < 0)
            {
                return;
            }

            if (WeaponsBullets.Count >= MaxPlayerWeapons)
            {
                Debug.Log("Maximo number of weapons you can carry");
                return;
            }

            //check if the player have already this weapon
            if (WeaponsBullets.Count > 0 &&
                WeaponsBullets.Exists(e => e.WeaponName == WeaponData.Weapons[weaponIndex].WeaponName))
            {
                Debug.Log("You already have this weapon " + WeaponData.Weapons[weaponIndex].WeaponName);
            }
            //Add the new weapon full of bullets
            else
            {
                var newWeaponMaximBullets = WeaponData.Weapons[weaponIndex].WeaponMagazine;

                WeaponsBullets.Add(new WeaponsBullets(WeaponData.Weapons[weaponIndex].WeaponName, weaponIndex,
                    newWeaponMaximBullets
                ));
                //if dont have weapons equip this
                if (CurrentDbWeaponIndex < 0)
                {
                    ChangeWeapons(weaponIndex);
                }
            }
        }

        //add a  weapon to the player whit the bullet amount, if the player don't have any weapon, then equip this weapon
        //this is for get  new weapons in the ground or drooped be the player after use it
        public bool AddWeaponWhitBullets(int weaponIndex, int bulletAmount)
        {
            var weaponType = WeaponData.Weapons[weaponIndex].WeaponClass;

            if (weaponIndex < 0)
            {
                return false;
            }

            if (WeaponsBullets.Count >= MaxPlayerWeapons)
            {
                Debug.Log("Maximo number of weapons you can carry");
                return false;
            }

            //check if the player  already have this weapon
            if (WeaponsBullets.Count > 0 &&
                WeaponsBullets.Exists(e => e.WeaponName == WeaponData.Weapons[weaponIndex].WeaponName))
            {
                if (weaponType == Weapon.WeaponType.TrowItem)
                {
                    AddBullet(WeaponData.Weapons[weaponIndex].BulletId, bulletAmount);
                    return true;
                }

                Debug.Log("You already have this weapon " + WeaponData.Weapons[weaponIndex].WeaponName);
                return false;
            }

            //Add the new weapon and bullets
            var newWeaponMaximBullets = WeaponData.Weapons[weaponIndex].WeaponMagazine;
            //if bulletAmount is more than weapon magazine, full charge weapon bullets
            if (bulletAmount > newWeaponMaximBullets)
            {
                WeaponsBullets.Add(new WeaponsBullets(WeaponData.Weapons[weaponIndex].WeaponName, weaponIndex,
                    newWeaponMaximBullets));

                //Add other bullet to pocket
                AddBullet(WeaponData.Weapons[weaponIndex].BulletId, bulletAmount - newWeaponMaximBullets);
            }
            //if bulletAmount is less than 0 or negative add weapon empty of bullets
            else if (bulletAmount <= 0)
            {
                WeaponsBullets.Add(new WeaponsBullets(WeaponData.Weapons[weaponIndex].WeaponName, weaponIndex, 0));
            }
            //if bulletAmount is equal or less than weapon magazine add this amount
            else
            {
                WeaponsBullets.Add(new WeaponsBullets(WeaponData.Weapons[weaponIndex].WeaponName, weaponIndex,
                    bulletAmount));
            }

            //if dont have weapons equip this
            if (CurrentDbWeaponIndex < 0)
            {
                ChangeWeapons(weaponIndex);
            }

            return true;
        }

        //change player weapons
        public void ChangeWeapons(int weaponIndex)
        {
            if (weaponIndex == CurrentDbWeaponIndex)
            {
                return;
            }
            Debug.Log("Change weapons in");
            //if no exist this weapon in the player pockets:
            if (weaponIndex >= 0 &&
                !WeaponsBullets.Exists(weapon => weapon.WeaponName == WeaponData.Weapons[weaponIndex].WeaponName))
            {
                Debug.Log("The player don't have this weapon.");
                return;
            }

            if (_currentWeapon)
            {
                Destroy(_currentWeapon);
            }

            CurrentDbWeaponIndex = weaponIndex;
            //change to empty hand if less than 0
            if (weaponIndex < 0)
            {
                SwitchAnimation(Weapon.WeaponType.Hands);
                CurrentWeaponClass = Weapon.WeaponType.Hands;
                CurrentBulletsId = -1;
            }
            else
            {
                //avoid change animations on swimming and keep weapon to 
                if (PlayerController.SwimmingController.Swimming)
                {
                    PlayerController.SwimmingController.CurrentIndexWeapon = weaponIndex;
                }
                else
                {
                    //if exist switch to weapon type
                    _currentWeapon = Instantiate(WeaponData.Weapons[weaponIndex].WeaponPrefab, WeaponPosition.position,
                        WeaponPosition.rotation, WeaponPosition);
                    CurrentWeaponClass = WeaponData.Weapons[weaponIndex].WeaponClass;
                    CurrentBulletsId = WeaponData.Weapons[weaponIndex].BulletId;
                    SwitchAnimation(WeaponData.Weapons[weaponIndex].WeaponClass);
                }
            }
        }

        //reload current player weapon
        private void ReloadWeapon()
        {
            if (CurrentWeaponClass != Weapon.WeaponType.Hands && CurrentWeaponClass != Weapon.WeaponType.Melee)
            {
               
                //check if have bullets in pocket for this weapon
                var bulletsInPocket = CurrentBulletsForThisWeapon(CurrentDbWeaponIndex);
                if (bulletsInPocket > 0)
                {
                    GetComponent<AudioSource>().PlayOneShot(AudioReload);
                    //get the reload time for current weapon
                    var reloadTime = WeaponData.Weapons[CurrentDbWeaponIndex].ReloadTime;
                    StartCoroutine(Reloading(reloadTime, bulletsInPocket));
                }
                else
                {
                    if (CurrentWeaponClass == Weapon.WeaponType.TrowItem)
                    {
                        RemoveAndDestroyTrowItem();
                    }
                    else
                    {
                        Debug.Log("No bullets");
                    }
                }
            }
        }


        //drop current player weapon
        public void DropWeapon()
        {
            GetComponent<AudioSource>().PlayOneShot(AudioDrop);
            StopAllCoroutines();
            _reloading = false;
            if (CurrentDbWeaponIndex >= 0 &&
                WeaponsBullets.Exists(weapon => weapon.WeaponLoadIndex == CurrentDbWeaponIndex))
            {
                //drop the weapon in front of the player
                var weaponDrop = Instantiate(WeaponData.Weapons[CurrentDbWeaponIndex].WeaponDrop,
                    transform.position + transform.forward,
                    transform.rotation).GetComponent<Weapons>();

                if (WeaponData.Weapons[CurrentDbWeaponIndex].WeaponClass == Weapon.WeaponType.TrowItem)
                {
                    //add item quantity to the drop
                    weaponDrop.WeaponBulletAmount =
                        WeaponsBullets.Find(weapon => weapon.WeaponLoadIndex == CurrentDbWeaponIndex)
                            .WeaponCurrentBullets + CurrentBulletsForThisWeapon(CurrentDbWeaponIndex);

                    RemoveAllBulletFromPocketById(WeaponData.Weapons[CurrentDbWeaponIndex].BulletId);
                    //remove weapon from player inventory
                    WeaponsBullets.Remove(WeaponsBullets.Find(weapon =>
                        weapon.WeaponLoadIndex == CurrentDbWeaponIndex));
                }
                else
                {
                    //add the current weapon bullet to the drop
                    weaponDrop.WeaponBulletAmount =
                        WeaponsBullets.Find(weapon => weapon.WeaponLoadIndex == CurrentDbWeaponIndex)
                            .WeaponCurrentBullets;
                    //remove weapon from player inventory
                    WeaponsBullets.Remove(WeaponsBullets.Find(weapon =>
                        weapon.WeaponLoadIndex == CurrentDbWeaponIndex));
                }


                Debug.Log("Change weapons");
                ChangeWeapons(-1);
            }
        }

        private void RemoveAndDestroyTrowItem()
        {
            StopAllCoroutines();
            _reloading = false;
            if (CurrentDbWeaponIndex >= 0 &&
                WeaponsBullets.Exists(weapon => weapon.WeaponLoadIndex == CurrentDbWeaponIndex))
            {
                //remove weapon from player inventory
                WeaponsBullets.Remove(WeaponsBullets.Find(weapon => weapon.WeaponLoadIndex == CurrentDbWeaponIndex));
                ChangeWeapons(-1);
            }
        }

        //add bullet amount to bulletId type if can be done return true, false if not.
        public bool AddBullet(int bulletId, int bulletAmount)
        {
            foreach (var currentBullet in BulletType)
            {
                if (currentBullet.BulletId == bulletId)
                {
                    currentBullet.BulletInPocket += bulletAmount;
                    return true;
                }
            }

            return false;
        }

        //Consume the bullets
        public void RemoveBulletsFromPocket(int bulletId, int bulletAmount)
        {
            foreach (var currentBullet in BulletType)
            {
                if (currentBullet.BulletId == bulletId)
                {
                    //consume the amount of bullet
                    currentBullet.BulletInPocket -= bulletAmount;

                    if (currentBullet.BulletInPocket < 0)
                    {
                        currentBullet.BulletInPocket = 0;
                    }
                }
            }
        }

        public void RemoveAllBulletFromPocketById(int bulletId)
        {
            foreach (var currentBullet in BulletType)
            {
                if (currentBullet.BulletId == bulletId)
                {
                    currentBullet.BulletInPocket = 0;
                }
            }
        }

        //this allow to check whe the player is reload the weapon
        public bool PlayerIsReloading()
        {
            return _reloading;
        }

        //Return bullet in pocket for the weapon
        private int CurrentBulletsForThisWeapon(int currentWeaponIndex)
        {
            var currentWeaponBulletId = WeaponData.Weapons[currentWeaponIndex].BulletId;

            return BulletType.Find(e => e.BulletId == currentWeaponBulletId).BulletInPocket;
        }

        private int GetWeaponMaxBulletCapacity(int currentWeaponIndex)
        {
            var currentWeaponName = WeaponData.Weapons[currentWeaponIndex].WeaponName;
            return WeaponData.GetWeaponByName(currentWeaponName).WeaponMagazine;
        }

        //return how much bullet is needed to reload the current weapon
        private int BulletNeededToReload()
        {
            //get weapon max bullet capacity
            var maxWeaponMagazine = GetWeaponMaxBulletCapacity(CurrentDbWeaponIndex);
            //get weapon currents bullet in magazine
            var currentWeaponBullets = WeaponsBullets.Find(weapon => weapon.WeaponLoadIndex == CurrentDbWeaponIndex)
                .WeaponCurrentBullets;
            return maxWeaponMagazine - currentWeaponBullets;
        }

        #region Animator

        private void SwitchAnimation(Weapon.WeaponType weaponType)
        {
            PlayerController.MovCharController.PlayerAnimator.SetInteger("WeaponType", (int) weaponType);
            PlayerController.MovCharController.PlayerAnimator.SetTrigger("Switch");
        }

        private void SetAimAnimation(bool active)
        {
            PlayerController.MovCharController.PlayerAnimator.SetBool("Shooting", active);
        }

        private void ShotAnimation()
        {
            PlayerController.MovCharController.PlayerAnimator.SetTrigger("Shot");
        }

        #endregion

        #region Coroutines

        public IEnumerator Reloading(float reloadTime, int bulletsInPocket)
        {
            _reloading = true;
            yield return new WaitForSeconds(reloadTime);
            if (PlayerController.SwimmingController.Swimming)
            {
                _reloading = false;
                yield break;
            }

            var bulletNeeded = BulletNeededToReload();
            //if have enough bullet in pocket fulfill the weapon 
            if (bulletsInPocket > bulletNeeded)
            {
                WeaponsBullets.Find(weapon => weapon.WeaponLoadIndex == CurrentDbWeaponIndex)
                        .WeaponCurrentBullets +=
                    bulletNeeded;
                RemoveBulletsFromPocket(CurrentBulletsId, bulletNeeded);
            }
            //if not have enough bullet add all bullet in pocket to the weapon
            else
            {
                WeaponsBullets.Find(weapon => weapon.WeaponLoadIndex == CurrentDbWeaponIndex)
                        .WeaponCurrentBullets +=
                    bulletsInPocket;
                RemoveBulletsFromPocket(CurrentBulletsId, bulletsInPocket);
            }

            _reloading = false;
        }

        private IEnumerator LaunchMeleeAttack()
        {
            _tempFireRate = WeaponData.Weapons[CurrentDbWeaponIndex].FireRate;
            DelayToTurnOn = WeaponData.Weapons[CurrentDbWeaponIndex].DelayToTurn;
            //play shoot animation
            ShotAnimation();
            yield return new WaitForSeconds(WeaponData.Weapons[CurrentDbWeaponIndex].DelayToLaunch);
            //instantiate melee attack and set damage.
            var meleeAttack = Instantiate(WeaponData.Weapons[CurrentDbWeaponIndex].Bullet, BulletPoint.position,
                transform.rotation).GetComponent<Damage>();

            meleeAttack.SetupBullet(BulletPoint.forward * WeaponData.Weapons[CurrentDbWeaponIndex].BulletSpeed,
                WeaponData.Weapons[CurrentDbWeaponIndex].Damage);

            _tempFireRate = WeaponData.Weapons[CurrentDbWeaponIndex].FireRate;
            DelayToTurnOn = WeaponData.Weapons[CurrentDbWeaponIndex].DelayToTurn;
        }

        private IEnumerator LaunchGrenade()
        {
            _tempFireRate = WeaponData.Weapons[CurrentDbWeaponIndex].FireRate;
            DelayToTurnOn = WeaponData.Weapons[CurrentDbWeaponIndex].DelayToTurn;
            //play shoot animation
            ShotAnimation();
            yield return new WaitForSeconds(WeaponData.Weapons[CurrentDbWeaponIndex].DelayToLaunch);
            //instantiate melee attack and set damage.
            var meleeAttack = Instantiate(WeaponData.Weapons[CurrentDbWeaponIndex].Bullet, BulletPoint.position,
                transform.rotation).GetComponent<Damage>();

            meleeAttack.SetupBullet(BulletPoint.forward * WeaponData.Weapons[CurrentDbWeaponIndex].BulletSpeed,
                WeaponData.Weapons[CurrentDbWeaponIndex].Damage);
            WeaponsBullets.Find(weapon => weapon.WeaponLoadIndex == CurrentDbWeaponIndex).WeaponCurrentBullets--;
            _tempFireRate = WeaponData.Weapons[CurrentDbWeaponIndex].FireRate;
            DelayToTurnOn = WeaponData.Weapons[CurrentDbWeaponIndex].DelayToTurn;
            if (WeaponsBullets.Find(weapon => weapon.WeaponLoadIndex == CurrentDbWeaponIndex).WeaponCurrentBullets <= 0)
            {
                ReloadWeapon();
            }
        }

        #endregion
    }

    [Serializable]
    public class TypeOfBullet
    {
        public string BulletName;
        public int BulletId;
        public int BulletInPocket;
    }

    [Serializable]
    public class WeaponsBullets
    {
        public string WeaponName;
        public int WeaponLoadIndex;

        [Tooltip("This is the weapon current bullets, if equal to 0 then the player need reload the weapon.")]
        public int WeaponCurrentBullets;

        public WeaponsBullets(string weaponName, int weaponLoadIndex, int weaponCurrentBullets)
        {
            WeaponName = weaponName;
            WeaponLoadIndex = weaponLoadIndex;
            WeaponCurrentBullets = weaponCurrentBullets;
        }
    }
}