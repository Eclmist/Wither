using UnityEngine;
using System.Collections;

public class TP_Camera : MonoBehaviour {

	public static TP_Camera Instance;

	public Transform target;
	public float desiredXOffset;
	public LayerMask collisionLayers;

	[Header("Camera Zooming")]
	public float distance = 5;
	public float distanceMin = 3;
	public float distanceMax = 10;
	public float distanceSmooth = 0.05F;
	public float distanceResumeSmooth = 0.1F;

	[Header("Camera Panning")]
	public float x_mouseSensitivity, y_mouseSensitivity;
	public float y_minLimit, y_maxLimit;
	public float mouseWheelSensitivity;
	public float x_smooth = 0.05F;
	public float y_smooth = 0.1F;

	[Header("Occlusion Checking")]
	public bool enableOcclusionChecking = false;
	public float occlusionDistanceStep = 0.5F;
	public int maxOcclusionSteps = 10;

	private float mouseX, mouseY;
	private float velX, velY, velZ;
	private float velDistance;
	private float startDistance, desiredDistance;
	private Vector3 desiredPosition, currentPosition;
	private float localDistanceSmooth;
	private float preOccludedDistance = 0;

	private Camera cam;

	private Vector3 shake;

	void Awake()
	{
		Instance = this;
	}

	void Start()
	{
		cam = GetComponent<Camera>();
		Reset();
	}

	void FixedUpdate()
	{
		if (target == null)
			return;

		HandlePlayerInput();

		int count = 0;


		if (enableOcclusionChecking)
		{

			do
			{
				CalculateDesiredPosition();
				count++;
			} while (CheckIfOccluded(count));
		}
		else
		{
			CalculateDesiredPosition();
		}

		UpdatePosition();

	}

	void HandlePlayerInput()
	{
		mouseX += Input.GetAxis("Mouse X") * x_mouseSensitivity;
		mouseY -= Input.GetAxis("Mouse Y") * y_mouseSensitivity;

		mouseY = Helper.ClampAngle(mouseY, y_minLimit, y_maxLimit);

		float deadZone = 0.01F;

		if (Input.GetAxis("Mouse ScrollWheel") < -deadZone ||
			Input.GetAxis("Mouse ScrollWheel") > deadZone)
		{

			desiredDistance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") *
				mouseWheelSensitivity,
				distanceMin,
				distanceMax);

				preOccludedDistance = desiredDistance;
				localDistanceSmooth = distanceSmooth;
		}
	}

	void CalculateDesiredPosition()
	{
		ResetDesiredDistance();

		distance = Mathf.SmoothDamp(distance, desiredDistance, ref velDistance, localDistanceSmooth);

		desiredPosition = CalculatePosition(mouseY, mouseX, distance);
	}

	Vector3 CalculatePosition(float rotationX, float rotationY, float distance)
	{
		Vector3 direction = new Vector3(0, 0, -distance);
		Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);

		Vector3 finalPos = target.position + rotation * direction;
		finalPos += transform.right * desiredXOffset * distance / 10;
		return finalPos;
	}

	bool CheckIfOccluded(int count)
	{
		bool isOccluded = false;

		float nearestDistance = CheckCameraPoints(target.position, desiredPosition);

		if (nearestDistance != -1)
		{
			if (count < maxOcclusionSteps)
			{
				isOccluded = true;
				desiredDistance -= occlusionDistanceStep;

				if (desiredDistance < 0.25F)
				{
					desiredDistance = 0.25F;
					//TODO: Fade character
				}
			}
			else
			{
				distance = nearestDistance - cam.nearClipPlane;
			}

			localDistanceSmooth = distanceResumeSmooth;         
		}

		return isOccluded;
	}

	float CheckCameraPoints(Vector3 from, Vector3 to)
	{
		float nearestDist = -1;

		RaycastHit hitInfo;

		Helper.ClipPlanePoints clipPlanePoints = Helper.ClipPlaneAtNear(to);

		//Draw line in editor for debug purposes

		Debug.DrawLine(from, to - transform.forward * cam.nearClipPlane, Color.red);
		Debug.DrawLine(from, clipPlanePoints.bottomLeft);
		Debug.DrawLine(from, clipPlanePoints.bottomRight);
		Debug.DrawLine(from, clipPlanePoints.topLeft);
		Debug.DrawLine(from, clipPlanePoints.topRight);

		Debug.DrawLine(clipPlanePoints.bottomLeft, clipPlanePoints.bottomRight);
		Debug.DrawLine(clipPlanePoints.topLeft, clipPlanePoints.topRight);
		Debug.DrawLine(clipPlanePoints.topLeft, clipPlanePoints.bottomLeft);
		Debug.DrawLine(clipPlanePoints.topRight, clipPlanePoints.bottomRight);

		if (Physics.Linecast(from, clipPlanePoints.bottomLeft, out hitInfo, collisionLayers))
		{
			if (hitInfo.distance < nearestDist)
				nearestDist = hitInfo.distance;
		}

		if (Physics.Linecast(from, clipPlanePoints.topLeft, out hitInfo, collisionLayers))
		{
			if (hitInfo.distance < nearestDist || nearestDist == -1)
				nearestDist = hitInfo.distance;
		}

		if (Physics.Linecast(from, clipPlanePoints.bottomRight, out hitInfo, collisionLayers))
		{
			if (hitInfo.distance < nearestDist || nearestDist == -1)
				nearestDist = hitInfo.distance;
		}

		if (Physics.Linecast(from, clipPlanePoints.topRight, out hitInfo, collisionLayers))
		{
			if (hitInfo.distance < nearestDist || nearestDist == -1)
				nearestDist = hitInfo.distance;
		}

		if (Physics.Linecast(from, to - transform.forward * cam.nearClipPlane, out hitInfo, collisionLayers))
		{
			if (hitInfo.distance < nearestDist || nearestDist == -1)
				nearestDist = hitInfo.distance;
		}

		return nearestDist;
	}

	void ResetDesiredDistance()
	{
		if (desiredDistance < preOccludedDistance)
		{
			float distanceToCheck = desiredDistance + occlusionDistanceStep;

			if (distanceToCheck > preOccludedDistance)
				distanceToCheck = preOccludedDistance;

			Vector3 pos = CalculatePosition(mouseY, mouseX, distanceToCheck);

			float nearestDist = CheckCameraPoints(target.position, pos);

			if (nearestDist == -1 || nearestDist > preOccludedDistance)
			{
				desiredDistance = distanceToCheck;
			}
		}
	}
	
	//returns true if camera is not occluded 
	bool CheckOcclusionAtDistance(float dist)
	{
		Vector3 pos = CalculatePosition(mouseY, mouseX, dist);
		float nearestDist = CheckCameraPoints(target.position, desiredPosition);

		return (nearestDist == -1);
	}

	void UpdatePosition()
	{
		float posX = Mathf.SmoothDamp(currentPosition.x, desiredPosition.x, ref velX, x_smooth);
		float posY = Mathf.SmoothDamp(currentPosition.y, desiredPosition.y, ref velY, y_smooth);
		float posZ = Mathf.SmoothDamp(currentPosition.z, desiredPosition.z, ref velZ, x_smooth);

		currentPosition = new Vector3(posX, posY, posZ);

		transform.position = currentPosition + shake;

		transform.LookAt(target.position + shake + transform.right * desiredXOffset * distance / 10);
		//transform.LookAt(target.position);
	}

	public void Reset()
	{
		mouseX = 0;
		mouseY = 10;
		distance = Mathf.Clamp(distance, distanceMin, distanceMax);
		startDistance = distance;
		desiredDistance = distance;
		preOccludedDistance = distance;
	}

	public void SetShakeVector(Vector3 amount)
	{
		shake = amount/10;
	}
}
