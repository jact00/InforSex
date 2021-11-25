using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float rotationAngle = -80;
    void Update()
    {
        transform.Rotate(new Vector3(0, rotationAngle, 0) * Time.deltaTime, Space.World);
    }
}
