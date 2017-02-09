using UnityEngine;
using System.Collections;

public class camerMovement : MonoBehaviour {

	public Transform lookAt;
	public Transform camTransform;

	private Camera cam;

	private float distance = 8.0f;
	private float currentX = 0.0f;
	private float currentY = 5.0f;
	private float sensivityX = 4.0f;
	private float sensivityy = 1.0f;

	private void Start(){
		camTransform = transform;
		cam = Camera.main;

	}

	private void LateUpdate(){
		Vector3 dir = new Vector3 (0, 0, - distance);
		Quaternion rotation = Quaternion.Euler(currentY + 15, currentX,0);
		camTransform.position = lookAt.position + rotation * dir;
		camTransform.LookAt (lookAt.position);
	}
		
}
