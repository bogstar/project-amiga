using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CameraController
{
	public Transform rig;
	public Camera camera;

	public Vector2 zoomMinMax;
	public Vector4 positionMinMax;

	public Vector3 targetRigPosition;
	public Vector3 targetRigRotation;
	public float targetZoom;

	public bool shouldPan;
	public bool shouldOrbit;


	public void UpdateCameraTransform()
	{
		if (shouldPan)
		{
			rig.transform.position -= targetRigPosition;
			rig.transform.position = new Vector3(Mathf.Clamp(rig.transform.position.x, positionMinMax.x, positionMinMax.y), 0, Mathf.Clamp(rig.transform.position.z, positionMinMax.z, positionMinMax.w));
		}
		if (shouldOrbit)
		{
			rig.transform.eulerAngles += new Vector3(0, targetRigRotation.x, 0);
		}

		float multiplier = 1 - (targetZoom / 10);
		Vector3 newPos = camera.transform.localPosition * multiplier;
		float magnitude = newPos.magnitude;

		newPos = newPos.normalized * Mathf.Clamp(magnitude, zoomMinMax.x, zoomMinMax.y);

		camera.transform.localPosition = newPos;
	}
}