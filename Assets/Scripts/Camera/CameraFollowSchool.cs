using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowSchool : MonoBehaviour
{
    public Vector3 offset;
    public SchoolController schoolController;

    private void Update()
    {
        transform.position = schoolController.centroid + offset;
    }
}
