using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringArm : MonoBehaviour
{
    public Camera Camera;
	public float TargetLength = 3.0f;
    public Transform CollisionSocket;
    public LayerMask CollisionMask = 0;
    public float CollisionRadius = 0.25f;
    public float SpeedDamp = 0.0f;
    public float CameraViewportExtentsMultipllier = 1.0f;

    private Vector3 _socketVelocity;
    private void LateUpdate()
    {
        if (Camera != null)
        {
            CollisionRadius = GetCollisionRadiusForCamera(Camera);
            Camera.transform.localPosition = -Vector3.forward * Camera.nearClipPlane;
        }

        UpdateLength();
    }

    private float GetCollisionRadiusForCamera(Camera cam)
    {
        float halfFOV = (cam.fieldOfView / 2.0f) * Mathf.Deg2Rad; // vertical FOV in radians
        float nearClipPlaneHalfHeight = Mathf.Tan(halfFOV) * cam.nearClipPlane * CameraViewportExtentsMultipllier;
        float nearClipPlaneHalfWidth = nearClipPlaneHalfHeight * cam.aspect;
        float collisionRadius = new Vector2(nearClipPlaneHalfWidth, nearClipPlaneHalfHeight).magnitude; // Pythagoras

        return collisionRadius;
    }

    private void UpdateLength()
    {
        float targetLength = GetDesiredTargetLength();
        Vector3 newSocketLocalPosition = -Vector3.forward * targetLength;

        CollisionSocket.localPosition = Vector3.SmoothDamp(
            CollisionSocket.localPosition, newSocketLocalPosition, ref _socketVelocity, SpeedDamp);
    }


    private float GetDesiredTargetLength()
		{
			Ray ray = new Ray(transform.position, -transform.forward);
			RaycastHit hit;

			if (Physics.SphereCast(ray, Mathf.Max(0.001f, CollisionRadius), out hit, TargetLength, CollisionMask))
			{
				return hit.distance;
			}
			else
			{
				return TargetLength;
			}
		}

}
