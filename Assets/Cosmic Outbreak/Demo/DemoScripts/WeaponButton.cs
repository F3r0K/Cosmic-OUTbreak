using System;
using System.Collections;
using System.Collections.Generic;
using TopDownShooter;
using UnityEngine;
using UnityEngine.UI;

namespace TopDownShooter
{
    public class WeaponButton : MonoBehaviour
    {
        public ShowPlayerWeapons ShowPlayerWeaponsComponent;
        public ShooterController ShooterControllerComponent;
        public Text InputNumberText;
        public Text WeaponName;
        public Text WeaponCurrentBullets;
        public Image WeaponIcon;
        public int CurrentButtonWeaponIndex;
        public int InputNumber;

        private void Update()
        {
            if (Input.GetKeyDown(InputNumber.ToString()))
            {
                ChangeWeaponToThis();
            }
        }

        private void FixedUpdate()
        {
            if (ShooterControllerComponent.CurrentDbWeaponIndex == CurrentButtonWeaponIndex &&
                ShooterControllerComponent.PlayerIsReloading())
            {
                WeaponCurrentBullets.text = "Reload";
            }
            else
            {
                WeaponCurrentBullets.text = ShooterControllerComponent.WeaponsBullets
                    .Find(weapon => weapon.WeaponLoadIndex == CurrentButtonWeaponIndex).WeaponCurrentBullets.ToString();
            }
        }

        public void SetButton(string weaponName, int weaponIndex, int inputNumber,
            ShowPlayerWeapons showPlayerWeaponComponent, ShooterController shooterControllerComponent)
        {
            InputNumberText.text = inputNumber.ToString();
            WeaponName.text = weaponName;
            CurrentButtonWeaponIndex = weaponIndex;
            InputNumber = inputNumber;
            ShowPlayerWeaponsComponent = showPlayerWeaponComponent;
            ShooterControllerComponent = shooterControllerComponent;
            WeaponIcon.sprite = ShooterControllerComponent.WeaponData.Weapons[weaponIndex].WeaponImage;

            WeaponCurrentBullets.gameObject.SetActive(
                ShooterControllerComponent.WeaponData.Weapons[weaponIndex].WeaponClass != Weapon.WeaponType.Melee);
        }

        public void ChangeWeaponToThis()
        {
            ShowPlayerWeaponsComponent.EquipWeapon(CurrentButtonWeaponIndex);
        }
    }
}