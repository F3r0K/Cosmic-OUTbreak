using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Missiles
{
    public class FaceCamera : MonoBehaviour
    {

        void LateUpdate()
        {
            // Face the Camera
            transform.rotation = Camera.main.transform.rotation;
        }
    }
}
