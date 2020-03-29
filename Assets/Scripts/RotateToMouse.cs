// author: Marcus Xie
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToMouse : MonoBehaviour
{
    public Camera cam;
    public float maximumLength;

    private Ray rayMouse;
    private Vector3 pos;
    private Vector3 direction;
    private Quaternion rotation;

    void Update()
    {
        if (cam != null)
        {
            // get the position where the mouse clicks ont the screen
            var mousePos = Input.mousePosition;
            // a ray from camera going towards the direction you clicked
            rayMouse = cam.ScreenPointToRay(mousePos);
            RaycastHit hit;
            // if the ray hit somewhere on a object, rotate the gun towards the hit point
            if (Physics.Raycast(rayMouse.origin, rayMouse.direction, out hit, maximumLength))
                RotateToMouseDirection(gameObject, hit.point);
            else
            {
                // if the ray fails to hit anywhere, it cannot keep going forever, give it a maximal length to stop
                var pos = rayMouse.GetPoint(maximumLength);
                RotateToMouseDirection(gameObject, pos);
            }
        }
        else
            Debug.Log("Camera is not assigned");
    }

    void RotateToMouseDirection(GameObject obj, Vector3 destination)
    {
        direction = destination - obj.transform.position;
        rotation = Quaternion.LookRotation(direction);
        obj.transform.localRotation = Quaternion.Lerp(obj.transform.rotation, rotation, 1);
    }
}
