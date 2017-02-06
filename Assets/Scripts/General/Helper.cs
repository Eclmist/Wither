using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper {

	public static Vector3 SuperSmoothLerp(Vector3 x0, Vector3 y0, Vector3 yt, float t, float k)
	{
		Vector3 f = x0 - y0 + (yt - y0) / (k * t);
		return yt - (yt - y0) / (k * t) + f * Mathf.Exp(-k * t);
	}

	public struct ClipPlanePoints
	{
		public Vector3 topLeft;
		public Vector3 topRight;
		public Vector3 bottomLeft;
		public Vector3 bottomRight;
	}

	public static float ClampAngle(float angle, float min, float max)
	{
		do
		{
			if (angle < -360)
			{
				angle += 360;
				continue;
			}

			if (angle > 360)
			{
				angle -= 360;
			}
		}
		while (angle < -360 || angle > 360);

		return Mathf.Clamp(angle, min, max);
	}

	public static ClipPlanePoints ClipPlaneAtNear(Vector3 pos)
	{
		ClipPlanePoints clipPlanePoints = new ClipPlanePoints();

		if (Camera.main == null)
			return clipPlanePoints;

		Transform transform = Camera.main.transform;
		float halfFOV = (Camera.main.fieldOfView/2)*Mathf.Deg2Rad + 10;
		float aspect = Camera.main.aspect;
		float distance = Camera.main.nearClipPlane;
		float height = distance*Mathf.Tan(halfFOV);
		float width = height*aspect;

		clipPlanePoints.bottomRight = pos + transform.right*width;
		clipPlanePoints.bottomRight -= transform.up*height;
		clipPlanePoints.bottomRight += transform.forward*distance;

		clipPlanePoints.bottomLeft = pos - transform.right*width;
		clipPlanePoints.bottomLeft -= transform.up*height;
		clipPlanePoints.bottomLeft += transform.forward*distance;

		clipPlanePoints.topRight = pos + transform.right*width;
		clipPlanePoints.topRight += transform.up*height;
		clipPlanePoints.topRight += transform.forward*distance;

		clipPlanePoints.topLeft = pos - transform.right*width;
		clipPlanePoints.topLeft += transform.up*height;
		clipPlanePoints.topLeft += transform.forward*distance;

		return clipPlanePoints;
	}
}
