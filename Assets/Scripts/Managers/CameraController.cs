using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Controller of the camera rotation (and zoom in and out on mobile devices)
/// </summary>
public class CameraController : MonoBehaviour
{
	public Transform target;
	public Vector3 targetOffset;

	public float distance = 5.0f;
	public float maxDistance = 20;
	public float minDistance = .6f;
	public float xSpeed = 5.0f;
	public float ySpeed = 5.0f;
	public int yMinLimit = -80;
	public int yMaxLimit = 80;
	public float zoomRate = 10.0f;
	public float panSpeed = 0.3f;
	public float zoomDampening = 5.0f;

	private Quaternion currentRotation;
	private Quaternion desiredRotation;
	private Quaternion rotation;
	private Vector3 position;
	private Vector3 FirstPosition;
	private Vector3 SecondPosition;
	private Vector3 delta;
	private Vector3 lastOffset;

	private float xDeg = 0.0f;
	private float yDeg = 0.0f;
	private float currentDistance;
	private float desiredDistance;

	private void Awake()
	{
		Initialize();
	}

	/// <summary>
	/// Initialize parameters to prepare the rotation and the position of the camera
	/// </summary>
	private void Initialize()
	{
		if (!target)
		{
			GameObject go = new GameObject("Cam Target");
			go.transform.position = transform.position + (transform.forward * distance);
			target = go.transform;
		}

		distance = Vector3.Distance(transform.position, target.position);
		currentDistance = distance;
		desiredDistance = distance;

		position = transform.position;
		rotation = transform.rotation;
		currentRotation = transform.rotation;
		desiredRotation = transform.rotation;

		xDeg = Vector3.Angle(Vector3.right, transform.right);
		yDeg = Vector3.Angle(Vector3.up, transform.up);
	}

	private void LateUpdate()
	{
		if (IsPointerOverUIObject())
			return;

		HandleInput();
		Rotate();
	}

	/// <summary>
	/// Detect if the mouse or touch is on an UI object and stop the rotation
	/// </summary>
	/// <returns></returns>
	private bool IsPointerOverUIObject()
	{
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

		return results.Count > 0;
	}

	/// <summary>
	/// Rotate the camera around the target
	/// </summary>
	private void Rotate()
	{
		desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
		currentRotation = transform.rotation;
		rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime * zoomDampening);
		transform.rotation = rotation;
	}

	/// <summary>
	/// Detect mouse and touch input
	/// </summary>
	private void HandleInput()
	{
		if (SystemInfo.deviceType == DeviceType.Desktop)
		{
			if (Input.GetMouseButtonDown(1))
			{
				FirstPosition = Input.mousePosition;
				lastOffset = targetOffset;
			}

			if (Input.GetMouseButton(1))
			{
				SecondPosition = Input.mousePosition;
				delta = SecondPosition - FirstPosition;

				if (delta.magnitude > .1)
				{
					xDeg += delta.x * xSpeed / 2 * 0.002f;
					yDeg -= delta.y * ySpeed / 2 * 0.002f;
					yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
				}
			}
		}
		else if (SystemInfo.deviceType == DeviceType.Handheld)
		{
			// Zoom
			if (Input.touchCount == 2)
			{
				Touch touchZero = Input.GetTouch(0);
				Touch touchOne = Input.GetTouch(1);

				Vector2 touchZeroPreviousPosition = touchZero.position - touchZero.deltaPosition;
				Vector2 touchOnePreviousPosition = touchOne.position - touchOne.deltaPosition;

				float prevTouchDeltaMag = (touchZeroPreviousPosition - touchOnePreviousPosition).magnitude;
				float TouchDeltaMag = (touchZero.position - touchOne.position).magnitude;
				float deltaMagDiff = prevTouchDeltaMag - TouchDeltaMag;

				desiredDistance += deltaMagDiff * Time.deltaTime * zoomRate * 0.0025f * Mathf.Abs(desiredDistance);
			}
			// Rotation
			else if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
			{
				Vector2 touchposition = Input.GetTouch(0).deltaPosition;
				xDeg += touchposition.x * xSpeed * 0.2f;
				yDeg -= touchposition.y * ySpeed * 0.2f;
				yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
			}
		}
	}

	private float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360)
			angle += 360;
		if (angle > 360)
			angle -= 360;

		return Mathf.Clamp(angle, min, max);
	}
}