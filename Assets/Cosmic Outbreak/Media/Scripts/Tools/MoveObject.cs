using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TopDownShooter
{
    public class MoveObject : MonoBehaviour
    {
        [Tooltip("Direction and speed to move the gameObject")]
        public Vector3[] MovementDirection;

        private int _randomPosition;
        private int _lastRandomPosition;

        private void Awake()
        {
            _randomPosition = UnityEngine.Random.Range(0, MovementDirection.Length);
        }

        private void Update()
        {
            transform.Translate(MovementDirection[_randomPosition] * Time.deltaTime, Space.Self);
        }
    }
}