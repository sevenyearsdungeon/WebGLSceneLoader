using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Transform sphere;
    public Transform root;
    Vector3 lastPosition;
    private void Start()
    {
        lastPosition = Input.mousePosition;
    }
    private void Update()
    {
        Vector3 delta = Input.mousePosition - lastPosition;
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        { 
        }
        else if (Input.GetMouseButton(0))
        {
            root.transform.Rotate(0.1f * Vector3.up * delta.x, Space.World);
            Quaternion r = root.rotation;
            root.transform.Rotate(-0.1f * Vector3.right * delta.y, Space.Self);
            if (Mathf.Abs(Vector3.Dot(Vector3.up, root.up)) < 0.1f)
                root.rotation = r;
        }
        else if (Input.GetMouseButton(1))
        {
            sphere.transform.Rotate(root.up, -0.1f * delta.x, Space.World);
            sphere.transform.Rotate(root.right, 0.1f * delta.y, Space.World);
        }

        var dz = Mathf.Clamp(transform.localPosition.z * (1 - 0.1f * Input.mouseScrollDelta.y), -10, -1.11f);
        transform.localPosition = Vector3.forward * dz;

        lastPosition = Input.mousePosition;
    }
}
