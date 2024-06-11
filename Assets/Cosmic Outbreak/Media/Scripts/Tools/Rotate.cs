using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour
{
    public float RotationSpeed = 10.0f;
    public WhichWayToRotate Way = WhichWayToRotate.AroundX;

    public enum WhichWayToRotate
    {
        AroundX,
        AroundY,
        AroundZ
    }

    private void Update()
    {
        switch (Way)
        {
            case WhichWayToRotate.AroundX:
                transform.Rotate(Vector3.right * Time.deltaTime * RotationSpeed);
                break;
            case WhichWayToRotate.AroundY:
                transform.Rotate(Vector3.up * Time.deltaTime * RotationSpeed);
                break;
            case WhichWayToRotate.AroundZ:
                transform.Rotate(Vector3.forward * Time.deltaTime * RotationSpeed);
                break;
        }
    }
}