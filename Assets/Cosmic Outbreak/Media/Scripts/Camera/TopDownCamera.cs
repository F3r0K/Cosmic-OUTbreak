using UnityEngine;

namespace TopDownShooter
{
    public class TopDownCamera : MonoBehaviour
    {
        public Transform Target;

        public float SeeForward;

        public float RotationSpeed = 3;

        // The speed with which the camera will be following.
        public float Smoothing = 5f;

        // The initial offset from the target.
        private Vector3 _offset;

        private bool _rotateToLeft;
        private bool _rotateToRight;


        private void Start()
        {
            if (!Target) 
            {
                Debug.LogWarning("Nothing to follow");
                return;
            }
            _offset = transform.position - Target.position;
        }

        private void Update()
        {
            if (!Target) 
            {
                Debug.LogWarning("Nothing to follow");
                return;
            }
            //rotate camera input
            _rotateToLeft = Input.GetKey(KeyCode.E);
            _rotateToRight = Input.GetKey(KeyCode.Q);
        }

        private void FixedUpdate()
        {
            if (!Target) 
            {
                Debug.LogWarning("Nothing to follow");
                return;
            }
            var targetCamPos = Target.position + _offset + Target.forward * SeeForward;
            transform.position = Vector3.Lerp(transform.position, targetCamPos, Smoothing * Time.deltaTime);
            if (_rotateToLeft) RotateToLeft();
            if (_rotateToRight) RotateToRight();
        }

        private void RotateToLeft()
        {
            transform.Rotate(Vector3.up * RotationSpeed, Space.World);
        }

        private void RotateToRight()
        {
            transform.Rotate(Vector3.up * -RotationSpeed, Space.World);
        }
    }
}