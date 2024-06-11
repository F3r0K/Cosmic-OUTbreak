using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TopDownShooter
{
    public class ShowPlayerWeapons : MonoBehaviour
    {
        public ShooterController PlayerShooterControllerComponent;

        public Text PlayerBullet0;
        public Text PlayerBullet1;
        public Text PlayerBullet2;
        public GameObject WeaponButton;
        public Transform ButtonContent;
        public List<WeaponButton> CurrentWeaponsButtons;

        private int _lasWeaponCount;

        private void FixedUpdate()
        {
            if (_lasWeaponCount != PlayerShooterControllerComponent.WeaponsBullets.Count)
            {
                AddWeaponButton();
                _lasWeaponCount = PlayerShooterControllerComponent.WeaponsBullets.Count;
            }

            PlayerBullet0.text = PlayerShooterControllerComponent.BulletType[0].BulletInPocket.ToString();
            PlayerBullet1.text = PlayerShooterControllerComponent.BulletType[1].BulletInPocket.ToString();
            PlayerBullet2.text = PlayerShooterControllerComponent.BulletType[2].BulletInPocket.ToString();
        }

        public void EquipWeapon(int weaponToEquip)
        {
            PlayerShooterControllerComponent.ChangeWeapons(weaponToEquip);
            foreach (var button in CurrentWeaponsButtons)
            {
                ChangeColors(button);
            }
        }

        private void AddWeaponButton()
        {
            var activeButtons = 0;
            for (int i = 0; i < PlayerShooterControllerComponent.WeaponsBullets.Count; i++)
            {
                CurrentWeaponsButtons[i].SetButton(PlayerShooterControllerComponent.WeaponsBullets[i].WeaponName,
                    PlayerShooterControllerComponent.WeaponsBullets[i].WeaponLoadIndex,
                    i + 1, this,
                    PlayerShooterControllerComponent);
                CurrentWeaponsButtons[i].gameObject.SetActive(true);

                activeButtons++;
            }

            for (int i = activeButtons; i < CurrentWeaponsButtons.Count; i++)
            {
                CurrentWeaponsButtons[i].gameObject.SetActive(false);
            }

            foreach (var button in CurrentWeaponsButtons)
            {
                ChangeColors(button);
            }
        }

        private void ChangeColors(WeaponButton button)
        {
            if (button.CurrentButtonWeaponIndex == PlayerShooterControllerComponent.CurrentDbWeaponIndex)
            {
                button.WeaponName.color = Color.green;
                button.transform.GetComponent<Image>().color = Color.green;
            }
            else
            {
                button.WeaponName.color = Color.white;
                button.transform.GetComponent<Image>().color = Color.white;
            }
        }
    }
}