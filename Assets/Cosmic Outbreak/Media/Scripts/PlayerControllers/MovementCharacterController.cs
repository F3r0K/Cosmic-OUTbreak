using System;
using System.Collections;
using UnityEngine;

namespace TopDownShooter
{
    public class MovementCharacterController : MonoBehaviour
    {
        [Header("Player Controller Settings")] [Tooltip("Speed for the player.")]
        public float RunningSpeed = 5f;

        [Header("Speed when player is shooting")] [Range(0.2f, 1)] [Tooltip("This is the % from player normal speed.")]
        public float RunningShootSpeed;

        [Tooltip("Slope angle limit to slide.")]
        public float SlopeLimit = 45;

        [Tooltip("Slide friction.")] [Range(0.1f, 0.9f)]
        public float SlideFriction = 0.3f;

        [Tooltip("Gravity force.")] [Range(0, -100)]
        public float Gravity = -30f;

        [Tooltip("Maxima speed for the player when fall.")] [Range(0, 100)]
        public float MaxDownYVelocity = 15;

        [Tooltip("Can the user control the player?")]
        public bool CanControl = true;

        [Header("Jump Settings")] [Tooltip("This allow the character to jump.")]
        public bool CanJump = true;

        [Tooltip("Jump maxima elevation for the character.")]
        public float JumpHeight = 2f;

        [Tooltip("This allow the character to jump in air after another jump.")]
        public bool CanDobleJump = true;

        [Header("Dash Settings")] [Tooltip("The player have dash?.")]
        public bool CanDash = true;

        [Tooltip("Cooldown for the dash.")] public float DashColdown = 3;

        [Tooltip("Force for the dash, a greater value more distance for the dash.")]
        public float DashForce = 5f;

        [Header("JetPack")] [Tooltip("Player have jetpack?")]
        public bool Jetpack = true;

        [Tooltip("The fuel maxima capacity for the jetpack.")]
        public float JetPackMaxFuelCapacity = 90;

        [Tooltip("The current fuel for the jetpack, if 0 the jet pack off.")]
        public float JetPackFuel;

        [Tooltip("The force for the jetpack, this impulse the player up.")]
        public float JetPackForce;

        [Tooltip("Jet pack consume this quantity by second active.")]
        public float FuelConsumeSpeed;

        [Header("SlowFall")] [Tooltip("This allow the player a slow fall, you can use an item like a parachute.")]
        public bool HaveSlowFall;

        [Tooltip("Speed vertical for the slow fall.")] [Range(0, 5)]
        public float SlowFallSpeed = 1.5f;

        [Tooltip("Slow fall forward speed.")] [Range(0, 1)]
        public float SlowFallForwardSpeed = 0.1f;

        [Tooltip("This is the drag force for the character, a standard value are (8, 0, 8). ")]
        public Vector3 DragForce;

        [Tooltip("This is the animator for you character.")]
        public Animator PlayerAnimator;

        [Header("Effects")] [Tooltip("This position is in the character feet and is use to instantiate effects.")]
        public Transform LowZonePosition;

        public GameObject JumpEffect;
        public GameObject DashEffect;
        public GameObject JetPackObject;
        public GameObject SlowFallObject;

        public PlayerController PlayerController;

        //Input.
        public float Horizontal;
        public float Vertical;
        public float Horizontal2;
        public float Vertical2;

        //private vars
        private CharacterController _controller;
        private Vector3 _velocity;


        private bool _jump;
        private bool _dash;
        private bool _flyJetPack;
        private bool _slowFall;
        private bool _shooting;

        //get direction for the camera
        private Transform _cameraTransform;
        private Vector3 _forward;
        private Vector3 _right;

        //temporal vars
        private float _originalRunningSpeed;
        private float _dashColdown;
        private float _gravity;
        private bool _doubleJump;
        private bool _invertedControl;
        private bool _isCorrectGrounded;
        private Vector3 _hitNormal;
        private Vector3 _move;
        private Vector3 _direction;
        private bool _activeFall;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _originalRunningSpeed = RunningSpeed;
        }

        private void Start()
        {
            if (Camera.main != null) _cameraTransform = Camera.main.transform;
            _dashColdown = DashColdown;
            _gravity = Gravity;
        }

        private void Update()
        {
            //capture input from direct input
            //this is for normal movement
            Horizontal = PlayerController.GetHorizontalValue();
            Vertical = PlayerController.GetVerticalValue();

            //this is for aim and shoot
            Horizontal2 = PlayerController.GetHorizontal2Value();
            Vertical2 = PlayerController.GetVertical2Value();

            //other buttons skills
            _jump = PlayerController.GetJumpValue();
            _dash = PlayerController.GetDashValue();
            _flyJetPack = PlayerController.GetJetPackValue();
            _slowFall = PlayerController.GetSlowFallValue();

            //this invert controls 
            if (_invertedControl)
            {
                Horizontal *= -1;
                Vertical *= -1;
                Horizontal2 *= -1;
                Vertical2 *= -1;
                _jump = PlayerController.GetDashValue();
                _dash = PlayerController.GetJumpValue();
            }

            //if player can control the character
            if (CanControl)
            {
                //jump
                if (_jump)
                {
                    Jump(JumpHeight);
                }

                //dash
                if (_dash)
                {
                    Dash();
                }

                //jetPack
                if (_flyJetPack)
                {
                    FlyByJetPack();
                }

                //this activate or deactivate jetPack Object and effect.
                JetPackObject.SetActive(Jetpack && _flyJetPack && JetPackFuel > 0);

                //slowFall
                if (HaveSlowFall && _slowFall)
                {
                    _activeFall = !_activeFall;
                    SlowFallObject.SetActive(_activeFall);
                }
                
            }
            else
            {
                Horizontal = 0;
                Vertical = 0;
                Vertical2 = 0;
                Horizontal2 = 0;
            }

            //dash colDown
            if (DashColdown > 0)
            {
                DashColdown -= Time.fixedDeltaTime;
            }
            else
            {
                DashColdown = 0;
            }

            //set running animation
            SetRunningAnimation((Math.Abs(Horizontal) > 0 || Math.Abs(Vertical) > 0));

            SetGorundedState();
        }

        private void FixedUpdate()
        {
            if (_activeFall)
            {
                if (!_controller.isGrounded && !PlayerController.SwimmingController.Swimming)
                {
                    SlowFall();
                }
                else
                {
                    SlowFallObject.SetActive(false);
                    _activeFall = false;
                }
            }
            //get the input direction for the camera position.
            _forward = _cameraTransform.TransformDirection(Vector3.forward);
            _forward.y = 0f;
            _forward = _forward.normalized;
            _right = new Vector3(_forward.z, 0.0f, -_forward.x);

            _move = (Horizontal * _right + Vertical * _forward);

            //if no is correct grounded then slide.
            if (!_isCorrectGrounded && _controller.isGrounded)
            {
                _move.x += (1f - _hitNormal.y) * _hitNormal.x * (1f - SlideFriction);
                _move.z += (1f - _hitNormal.y) * _hitNormal.z * (1f - SlideFriction);
            }

            _move.Normalize();
            SetAimAnimation(_move);
            //move the player if no is active the slow fall(this avoid change the speed for the fall)
            if (!_activeFall && _controller.enabled)
            {
                if (_shooting)
                {
                    _controller.Move(Time.deltaTime * RunningSpeed * RunningShootSpeed * _move);
                }
                else
                {
                    _controller.Move(Time.deltaTime * RunningSpeed * _move);
                }
            }

            //Check if is correct grounded.
            _isCorrectGrounded = (Vector3.Angle(Vector3.up, _hitNormal) <= SlopeLimit);

            //set the forward direction if have some weapons loaded
            if (PlayerController.ShooterController.CurrentWeaponClass != Weapon.WeaponType.Hands)
            {
                //check if the player is using the mouse to rotate and shoot
                if (PlayerController.UseMouseToRotate)
                {
                    _direction = PlayerController.GetMouseDirection();
                    //invert controls
                    if (_invertedControl)
                    {
                        _direction *= -1;
                    }
                }
                else
                {
                    _direction = (Horizontal2 * _right + Vertical2 * _forward);
                }
            }
            else
            {
                _direction = Vector3.zero;
            }

            if (_direction != Vector3.zero)
            {
                _shooting = true;
                transform.forward = Vector3.Lerp(transform.forward, _direction, 1f);
            }
            else
            {
                if (_move != Vector3.zero && PlayerController.ShooterController.DelayToTurnOn <= 0)
                {
                    _shooting = false;
                    transform.forward = Vector3.Lerp(transform.forward, _move, 0.5f);
                }
            }

            PlayerController.ShooterController.ManageShoot();

            //gravity force
            if (_velocity.y >= -MaxDownYVelocity)
            {
                _velocity.y += Gravity * Time.deltaTime;
            }

            //stop gravity if player are on water
            if (PlayerController.SwimmingController.Swimming)
            {
                _velocity.y = 0;
            }

            _velocity.x /= 1 + DragForce.x * Time.deltaTime;
            _velocity.y /= 1 + DragForce.y * Time.deltaTime;
            _velocity.z /= 1 + DragForce.z * Time.deltaTime;
            if (_controller.enabled)
            {
                _controller.Move(_velocity * Time.deltaTime);
            }
        }

        //This check how much the player are pushing the fire stick
        public bool TensionFoRightStickLowerThan(float value)
        {
            return (Mathf.Abs(PlayerController.MovCharController.Horizontal2) > value ||
                    Mathf.Abs(PlayerController.MovCharController.Vertical2) > value);
        }

        public void Jump(float jumpHeight)
        {
            if (!CanJump)
            {
                return;
            }

            //removing parachute if active;
            _activeFall = false;
            SlowFallObject.SetActive(_activeFall);

            //
            if (_controller.isGrounded)
            {
                _hitNormal = Vector3.zero;
                SetJumpAnimation();
                _doubleJump = true;
                _velocity.y = 0;
                _velocity.y += Mathf.Sqrt(jumpHeight * -2f * Gravity);

                //Instatiate jump effect
                if (JumpEffect)
                {
                    Instantiate(JumpEffect, LowZonePosition.position, LowZonePosition.rotation);
                }
            }
            else if (CanDobleJump && _doubleJump)
            {
                _doubleJump = false;
                _velocity.y = 0;
                _velocity.y += Mathf.Sqrt(jumpHeight * -2f * Gravity);

                //Instatiate jump effect
                if (JumpEffect)
                {
                    Instantiate(JumpEffect, LowZonePosition.position, LowZonePosition.rotation);
                }
            }
        }

        public void JumpWhitCurrentForce()
        {
            Jump(JumpHeight);
        }

        public void Dash()
        {
            if (!CanDash || DashColdown > 0 || _flyJetPack)
            {
                return;
            }

            DashColdown = _dashColdown;

            if (DashEffect)
            {
                Instantiate(DashEffect, transform.position, transform.rotation);
            }

            SetDashAnimation();
            StartCoroutine(Dashing(DashForce / 10));

            if (_direction != Vector3.zero && _move != Vector3.zero)
            {
                _velocity += Vector3.Scale(_move,
                    DashForce * new Vector3((Mathf.Log(1f / (Time.deltaTime * DragForce.x + 1)) / -Time.deltaTime),
                        0, (Mathf.Log(1f / (Time.deltaTime * DragForce.z + 1)) / -Time.deltaTime)));
            }
            else
            {
                _velocity += Vector3.Scale(transform.forward,
                    DashForce * new Vector3((Mathf.Log(1f / (Time.deltaTime * DragForce.x + 1)) / -Time.deltaTime),
                        0, (Mathf.Log(1f / (Time.deltaTime * DragForce.z + 1)) / -Time.deltaTime)));
            }
        }

        public void FlyByJetPack()
        {
            if (!Jetpack || JetPackFuel <= 0)
            {
                return;
            }

            //if slowFall is active deactivate.
            if (_activeFall)
            {
                _activeFall = false;
                SlowFallObject.SetActive(false);
            }

            JetPackFuel -= Time.deltaTime * FuelConsumeSpeed;
            _velocity.y = 0;
            _velocity.y += Mathf.Sqrt(JetPackForce * -2f * Gravity);
        }

        //this is for a slow fall like a parachute.
        private void SlowFall()
        {
            _controller.Move(transform.forward * SlowFallForwardSpeed);
            _velocity.y = 0;
            _velocity.y += -SlowFallSpeed;
        }

        //add fuel to the jetPack
        public void AddFuel(float fuel)
        {
            JetPackFuel += fuel;
            if (JetPackFuel > JetPackMaxFuelCapacity)
            {
                JetPackFuel = JetPackMaxFuelCapacity;
            }

            Debug.Log("Fuel +" + fuel);
        }

        public void ResetOriginalSpeed()
        {
            RunningSpeed = _originalRunningSpeed;
        }

        //change the speed for the player
        public void ChangeSpeed(float speed)
        {
            RunningSpeed = speed;
        }

        //change the speed for the player for a time period
        public void ChangeSpeedInTime(float speedPlus, float time)
        {
            StartCoroutine(ModifySpeedByTime(speedPlus, time));
        }

        //invert player control(like a confuse skill)
        public void InvertPlayerControls(float invertTime)
        {
            //check if not are already inverted
            if (!_invertedControl)
            {
                StartCoroutine(InvertControls(invertTime));
            }
        }

        public void ActivateDeactivateJump(bool canJump)
        {
            CanJump = canJump;
        }

        public void ActivateDeactivateDoubleJump(bool canDoubleJump)
        {
            //if double jump is active activate normal jump
            if (canDoubleJump)
            {
                CanJump = true;
            }

            CanDobleJump = canDoubleJump;
        }

        public void ActivateDeactivateDash(bool canDash)
        {
            CanDash = canDash;
        }

        public void ActivateDeactivateSlowFall(bool canSlowFall)
        {
            HaveSlowFall = canSlowFall;
        }

        public void ActivateDeactivateJetPack(bool haveJetPack)
        {
            Jetpack = haveJetPack;
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            _hitNormal = hit.normal;
        }
        //Animation

        #region Animator

        private void SetRunningAnimation(bool run)
        {
            PlayerAnimator.SetBool("Running", run);
        }

        private void SetAimAnimation(Vector3 movementDirection)
        {
            Vector3 aimDirection = transform.InverseTransformDirection(movementDirection);
            PlayerAnimator.SetFloat("Y", aimDirection.z, 0.1f, Time.deltaTime);
            PlayerAnimator.SetFloat("X", aimDirection.x, 0.1f, Time.deltaTime);
        }

        private void SetJumpAnimation()
        {
            PlayerAnimator.SetTrigger("Jump");
        }

        private void SetDashAnimation()
        {
            PlayerAnimator.SetTrigger("Dash");
        }

        private void SetGorundedState()
        {
            //avoid set the grounded var in animator multiple time
            if (PlayerAnimator.GetBool("Grounded") != _controller.isGrounded)
            {
                PlayerAnimator.SetBool("Grounded", _controller.isGrounded);
            }
        }

        #endregion

        #region Coroutine

        //Use this to deactivate te player control for a period of time.
        public IEnumerator DeactivatePlayerControlByTime(float time)
        {
            _controller.enabled = false;
            CanControl = false;
            yield return new WaitForSeconds(time);
            CanControl = true;
            _controller.enabled = true;
        }

        //dash coroutine.
        private IEnumerator Dashing(float time)
        {
            CanControl = false;
            if (!_controller.isGrounded)
            {
                Gravity = 0;
                _velocity.y = 0;
            }

            //animate hear to true
            yield return new WaitForSeconds(time);
            CanControl = true;
            //animate hear to false
            Gravity = _gravity;
        }

        //modify speed by time coroutine.
        private IEnumerator ModifySpeedByTime(float speedPlus, float time)
        {
            if (RunningSpeed + speedPlus > 0)
            {
                RunningSpeed += speedPlus;
            }
            else
            {
                RunningSpeed = 0;
            }

            yield return new WaitForSeconds(time);
            RunningSpeed = _originalRunningSpeed;
        }

        private IEnumerator InvertControls(float invertTime)
        {
            yield return new WaitForSeconds(0.1f);
            _invertedControl = true;
            yield return new WaitForSeconds(invertTime);
            _invertedControl = false;
        }

        #endregion
    }
}