using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Transform))]
[RequireComponent(typeof(CapsuleCollider))]
public class FallToGround : MonoBehaviour {

    public float fallSpeed = 10.0f; // m/s

    private new Transform transform;
    private new CapsuleCollider collider;


    void Start () {
        transform = GetComponent<Transform>();
        collider = GetComponent<CapsuleCollider>();
	}
	
	void Update () {
        if (collider.enabled && !IsGrounded()) {
            // Object is airborn
            Ray r = new Ray(transform.position, -Vector3.up);
            RaycastHit hit;
            float yDelta = fallSpeed * Time.deltaTime;

            // Check if our move step would put us through the ground
            if (Physics.Raycast(r, out hit, collider.bounds.extents.y + yDelta)) {
                // Ground is less than the delta, only move enough to hit the ground.
                yDelta = hit.distance;
            }

            // Move the object down
            Vector3 pos = transform.position;
            pos.y -= yDelta;
            transform.position = pos;
        }
	}

    private bool IsGrounded() {
        const float errorMargin = 0.1f;
        return Physics.Raycast(transform.position, -Vector3.up, collider.bounds.extents.y + errorMargin);
    }
}
