using UnityEngine;

/* Script to easy setup your own input configurations.
 * You can use the virtual joystick solution in this pack or use another solution.
 * Note: if you go to use joystick like a Xbox controller you need add this two
 * new axis to the input manager.
 * */

namespace TopDownShooter
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Scripts reference")] public MovementCharacterController MovCharController;
        public ShooterController ShooterController;
        public SwimmingController SwimmingController;

        [Header("Use mouse to shoot and rotate player")]
        public bool UseMouseToRotate = true;

        [Tooltip("This is the layer for the ground.")]
        public LayerMask GroundLayer;

        [Header("Use virtualJoystick to control the player")]
        public bool UseVirtualJoyStick;

        public Joystick JoystickControllerLeft;
        public Joystick JoystickControllerRight;

        private bool _activeJetPack;
        private bool _activeSlowFall;

        private void Awake()
        {
            //avoid use more than one control at the same time
            if (UseMouseToRotate)
            {
                UseVirtualJoyStick = false;
            }

            CheckForVirtualJoystick();
        }

        public float GetHorizontalValue()
        {
            if (UseVirtualJoyStick)
            {
                return JoystickControllerLeft.Horizontal;
            }

            return Input.GetAxis("Horizontal");
        }

        public float GetVerticalValue()
        {
            if (UseVirtualJoyStick)
            {
                return JoystickControllerLeft.Vertical;
            }

            return Input.GetAxis("Vertical");
        }

        public float GetHorizontal2Value()
        {
            if (UseMouseToRotate)
            {
                return GetMouseDirection().x;
            }

            if (UseVirtualJoyStick)
            {
                return JoystickControllerRight.Horizontal;
            }

            //if you go to use a joystick like a Xbox joystick replace "GetMouseDirection().x" put you new Horizontal axis in this place and uncheck mouse and virtual joystick like this:
            //return Input.GetAxis("NewControlAxis");
            return Input.GetAxis("Horizontal");
        }

        public float GetVertical2Value()
        {
            if (UseMouseToRotate)
            {
                return GetMouseDirection().z;
            }

            if (UseVirtualJoyStick)
            {
                return JoystickControllerRight.Vertical;
            }

            //if you go to use a joystick like a Xbox joystick replace "Input.GetAxis("Vertical")" put you new Vertical axis in this place and uncheck mouse and virtual joystick like this:
            //return Input.GetAxis("NewControlAxis");

            return Input.GetAxis("Vertical");
        }

        public bool GetJumpValue()
        {
            return Input.GetKeyDown(KeyCode.Space);
        }

        public bool GetDashValue()
        {
            return Input.GetKeyDown(KeyCode.F);
        }

        public bool GetJetPackValue()
        {
            if (UseVirtualJoyStick)
            {
                return _activeJetPack;
            }

            return Input.GetKey(KeyCode.X);
        }

        public bool GetSlowFallValue()
        {
            if (UseVirtualJoyStick)
            {
                if (!_activeSlowFall)
                {
                    return false;
                }

                _activeSlowFall = false;
                return true;
            }

            return Input.GetKeyDown(KeyCode.V);
        }

        public bool GetDropWeaponValue()
        {
            return Input.GetKeyDown(KeyCode.G);
        }

        public bool GetReloadWeaponValue()
        {
            return Input.GetKeyDown(KeyCode.R);
        }

        public void ActivateJetPack(bool active)
        {
            _activeJetPack = active;
        }

        public void ActivateSlowFall()
        {
            _activeSlowFall = true;
        }
        public void DeActivateSlowFall()
        {
            _activeSlowFall = false;
        }

        public Vector3 GetMouseDirection()
        {
            if (Camera.main == null) return Vector3.zero;
            var newRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit groundHit;

            //check if the player press mouse button and the ray hit the ground
            if (Input.GetMouseButton(0) && Physics.Raycast(newRay, out groundHit, 1000, GroundLayer))
            {
                var playerToMouse = groundHit.point - transform.position;

                playerToMouse.y = 0f;

                return playerToMouse;
            }

            return Vector3.zero;
        }

        //hide or show virtualJoystick if exist
        private void CheckForVirtualJoystick()
        {
            if (UseVirtualJoyStick)
            {
                if (JoystickControllerLeft)
                {
                    JoystickControllerLeft.gameObject.SetActive(true);
                }

                if (JoystickControllerRight)
                {
                    JoystickControllerRight.gameObject.SetActive(true);
                }
            }
            else
            {
                if (JoystickControllerLeft)
                {
                    JoystickControllerLeft.gameObject.SetActive(false);
                }

                if (JoystickControllerRight)
                {
                    JoystickControllerRight.gameObject.SetActive(false);
                }
            }
        }
    }
}