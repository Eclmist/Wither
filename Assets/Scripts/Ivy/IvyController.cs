using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class IvyController : MonoBehaviour
{

	[SerializeField][Range(0, 10)] private float floatingHeight;

	[SerializeField] private readonly LayerMask ignoreMask = 1 << 9;

	[SerializeField] private Transform player;


	[Header("Movements")]
	[SerializeField] [Range(0, 10)] private float speed;
	[SerializeField] private AnimationCurve speedOverDistance;
	[SerializeField] private AnimationCurve turnRateOverAngle;
	//[SerializeField] private AnimationCurve turnRateOverDistance;

	[Header("Avoidance")]
	[SerializeField] [Range(0, 10)] private float minimumDistance;
	[SerializeField] [Range(0, 30)] private float avoidanceRotationForce;

	[Header("Pulse")] [SerializeField] private LayerMask revealLayers;

	[Range(0, 100)]
	[SerializeField]
	private float p_maxDistance;
	[Range(0, 1)]
	[SerializeField]
	private float p_growRate;
	[Range(0, 10)]
	[SerializeField]
	private float p_ringWidth;


	private Vector3 targetPosition;
	private Vector3 tempTarget;
	private float moveSpeed = 0;
	private bool emergencyTurn = false;
	private bool pulseCoroutineStarted = false;
	// Use this for initialization
	void Start()
	{
	}

	void Update()
	{
		if (Input.GetAxis("Jump") > 0)
		{
			Pulse();
		}
	}

	void Pulse()
	{
		if (!pulseCoroutineStarted)
		{
			StartCoroutine("DoPulse");
		}
	}

	IEnumerator DoPulse()
	{
		float expGrowRate = 0;
		pulseCoroutineStarted = true;

		WaitForFixedUpdate wait = new WaitForFixedUpdate();

		float currentDistance = 0;

		Mask_Pulse.Instance.SetPulsePosition(transform.position);
		Mask_Pulse.Instance.SetMaxDistance(p_maxDistance);
		SSDistortion.Instance.distortionMultiplier = 0;

		Vector3 pulsePosition = transform.position;

		List<Collider> interracted = new List<Collider>();

		while (currentDistance < p_maxDistance)
		{
			currentDistance += expGrowRate;
			expGrowRate += p_growRate;
			Mask_Pulse.Instance.SetPulse(currentDistance, p_ringWidth);
			SSDistortion.Instance.radius = currentDistance;
			SSDistortion.Instance.distortionMultiplier = Mathf.Sin(currentDistance/p_maxDistance * 3.14f);

			Collider[] interractables = Physics.OverlapSphere(pulsePosition, currentDistance, revealLayers);

			foreach (Collider c in interractables)
			{
				IInteractable interactable = c.GetComponent<IInteractable>();
				if (interactable != null && !interracted.Contains(c))
				{
					interactable.Pulse();
					interracted.Add(c);
				}

			}

			yield return wait;
		}

		Mask_Pulse.Instance.SetPulse(0, 0);
		SSDistortion.Instance.radius = 0;
		SSDistortion.Instance.distortionMultiplier = 0;

		yield return wait;

		pulseCoroutineStarted = false;
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		emergencyTurn = false;

		RaycastHit hit;

		if (Input.GetAxis("Fire1") > 0)
		{
			Ray targetPosRay = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(targetPosRay, out hit, float.MaxValue, ~ignoreMask))
			{
				targetPosition = hit.point;
				targetPosition.y += floatingHeight;
			}

		}
		else
		{
			targetPosition = player.position;
			targetPosition.y = player.position.y - 1.125F + floatingHeight;
		}

		tempTarget = targetPosition;

		float distance = (tempTarget - transform.position).magnitude / 60;
		float angle = Vector3.Angle(tempTarget - transform.position, transform.forward) / 180;
		float rotationSpeed = turnRateOverAngle.Evaluate(angle)/10;
		//rotationSpeed += turnRateOverDistance.Evaluate((hit.point - transform.position).magnitude / 60);

		float targetSpeed = speedOverDistance.Evaluate(distance) * speed / 10;
		moveSpeed = Mathf.Lerp(moveSpeed, targetSpeed, Time.deltaTime);

		Vector3 lookAtTarget = AvoidObstacles();

		transform.rotation = Quaternion.Slerp(transform.rotation,
			Quaternion.LookRotation(lookAtTarget, Vector3.up),
			emergencyTurn ? 0.1F : rotationSpeed);

		Vector3 targetPosCurrFrame = transform.position + transform.forward;
		transform.position = Vector3.Lerp(transform.position, targetPosCurrFrame, moveSpeed);

	}

	void OnDrawGizmos()
	{
		Gizmos.DrawSphere(targetPosition, 0.1F);
		Gizmos.DrawSphere(tempTarget, 0.05F);

		Gizmos.DrawLine(transform.position, transform.position + transform.forward * 3);
		Gizmos.DrawLine(transform.position, transform.position + (transform.forward + transform.right) * 2);
		Gizmos.DrawLine(transform.position, transform.position + (transform.forward - transform.right) * 2);

	}

	public Vector3 AvoidObstacles()
	{
		RaycastHit Hit;
		//Check that the vehicle hit with the obstacles within it's minimum distance to avoid
		Vector3 right45 = (transform.forward + transform.right).normalized;
		Vector3 left45 = (transform.forward - transform.right).normalized;

		if (Physics.Raycast(transform.position, right45, out Hit,
			minimumDistance))
		{
			emergencyTurn = true;
			//Get the new directional vector by adding force to vehicle's current forward vector
			return transform.forward - transform.right * avoidanceRotationForce;
		}
		else if (Physics.Raycast(transform.position, left45, out Hit,
			minimumDistance))
		{
			emergencyTurn = true;
			//Get the new directional vector by adding force to vehicle's current forward vector
			return transform.forward + transform.right * avoidanceRotationForce;
		}
		else if (Physics.Raycast(transform.position, transform.forward, out Hit,
			minimumDistance))
		{
			emergencyTurn = true;
			//Get the normal of the hit point to calculate the new direction
			Vector3 hitNormal = Hit.normal;
			hitNormal.y = 0.0f; //Don't want to move in Y-Space
			return transform.forward + hitNormal * avoidanceRotationForce;
		}
		else
			return targetPosition - transform.position;
	}

}
