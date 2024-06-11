using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TopDownShooter
{
    public class SwimmingController : MonoBehaviour
    {
        public bool Swimming;
        public PlayerController PlayerController;
        
        public int CurrentIndexWeapon;
        
        
        private void SetSwimmingState(bool swimming)
        {
            Swimming = swimming;
            PlayerController.MovCharController.PlayerAnimator.SetTrigger("Swim");
            PlayerController.MovCharController.PlayerAnimator.SetBool("Swimming", swimming);
        }
        
        private void EnterWaterReset()
        {
            PlayerController.ShooterController.StopCoroutine(nameof(PlayerController.ShooterController.Reloading));
            CurrentIndexWeapon = PlayerController.ShooterController.CurrentDbWeaponIndex;
            PlayerController.ShooterController.ChangeWeapons(-1);
            SetSwimmingState(true);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Water")) return;
            EnterWaterReset();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Water")) return;
            SetSwimmingState(false);
            PlayerController.ShooterController.CurrentDbWeaponIndex = -1;
            PlayerController.ShooterController.ChangeWeapons(CurrentIndexWeapon);
            if (CurrentIndexWeapon <= 0)
            {
                PlayerController.MovCharController.PlayerAnimator.SetInteger("WeaponType", 0);
                PlayerController.MovCharController.PlayerAnimator.SetTrigger("Switch");
            }
        }
    }
    
    
}