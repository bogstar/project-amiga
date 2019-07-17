using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
	void LateUpdate()
	{
		transform.rotation = FindObjectOfType<InputManager>().cameraController.camera.transform.rotation;
	}
}