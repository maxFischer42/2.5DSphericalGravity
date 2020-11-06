﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingSphere : MonoBehaviour {

    [SerializeField, Range(0f, 100f)]
    float maxSpeed = 10f;

    [SerializeField, Range(0f, 100f)]
    float maxAcceleration = 10f, maxAirAcceleration = 1f;

    [SerializeField, Range(0f, 10f)]
    float jumpHeight = 2f;

    // Removed bc unused
    //[SerializeField]
    //Rect allowedArea = new Rect(-5f, -5f, 10f, 10f);

    // removed bc unused
    //[SerializeField, Range(0f, 1f)]
    //float bounciness = 0.5f;

    [SerializeField, Range(0, 5)]
    int maxAirJumps = 0;

    int jumpPhase;

    Vector3 velocity, desiredVelocity;

    Rigidbody body;

    bool desiredJump;
    //public bool onGround;
    int groundContactCount;
    bool OnGround
    {
        get
        {
            return groundContactCount > 0;
        }
    }

    Vector3 contactNormal;

    int stepsSinceLastGrounded;

    [SerializeField, Range(0f, 90f)]
    float maxGroundAngle = 25f;

    float minGroundDotProduct;

    [SerializeField, Range(0f, 100f)]
    float maxSnapSpeed = 100f;

    [SerializeField, Min(0f)]
    float probeDistance = 1f;

    void OnValidate() {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
    }

    void Awake() {
        body = GetComponent<Rigidbody>();
        OnValidate();
    }

    void Update() {
        Vector2 playerInput;
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");
        desiredJump |= Input.GetButtonDown("Jump");

        playerInput = Vector2.ClampMagnitude(playerInput, 1f);

        desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;
        GetComponent<Renderer>().material.SetColor(
            "_Color", OnGround ? Color.black : Color.white
        );
    }

    void FixedUpdate() {
        UpdateState();
        AdjustVelocity();
        if (desiredJump) {
            desiredJump = false;
            Jump();
        }
        body.velocity = velocity;
        ClearState();
    }

    bool SnapToGround() {
        if (stepsSinceLastGrounded > 1) {
            return false;
        }
        float speed = velocity.magnitude;
        if (speed > maxSnapSpeed) {
            return false;
        }
        if (!Physics.Raycast(body.position, Vector3.down, out RaycastHit hit, probeDistance)) {
            return false;
        }
        if (hit.normal.y < minGroundDotProduct) {
            return false;
        }
        groundContactCount = 1;
        contactNormal = hit.normal;
        //float speed = velocity.magnitude;
        float dot = Vector3.Dot(velocity, hit.normal);
        if (dot > 0f) {
            velocity = (velocity - hit.normal * dot).normalized * speed;
        }
        return true;
    }

    void ClearState() {
        groundContactCount = 0;
        contactNormal = Vector3.zero;
    }

    void UpdateState() {
        stepsSinceLastGrounded += 1;
        velocity = body.velocity;
        if (OnGround || SnapToGround()) {
            stepsSinceLastGrounded = 0;
            jumpPhase = 0;
            if (groundContactCount > 1) {
                contactNormal.Normalize();
            }
        }
        if (desiredJump)
        {
            desiredJump = false;
            Jump();
        }

        float acceleration = (groundContactCount > 1) ? maxAcceleration : maxAirAcceleration;
        float maxSpeedChange = acceleration * Time.deltaTime;

        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);

        body.velocity = velocity;
    }

    void Jump() {
        if ((groundContactCount > 1) || jumpPhase < maxAirJumps) {
            float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
            float alignedSpeed = Vector3.Dot(velocity, contactNormal);
            if (alignedSpeed > 0f)
            {
                jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
            }
            jumpPhase++;
            velocity += contactNormal * jumpSpeed;
        }
    }

    Vector3 ProjectOnContactPlane(Vector3 vector) {
        return vector - contactNormal * Vector3.Dot(vector, contactNormal);
    }

    void AdjustVelocity() {
        Vector3 xAxis = ProjectOnContactPlane(Vector3.right).normalized;
        Vector3 zAxis = ProjectOnContactPlane(Vector3.forward).normalized;

        float currentX = Vector3.Dot(velocity, xAxis);
        float currentZ = Vector3.Dot(velocity, zAxis);

        float acceleration = (groundContactCount > 1) ? maxAcceleration : maxAirAcceleration;
        float maxSpeedChange = acceleration * Time.deltaTime;

        float newX = Mathf.MoveTowards(currentX, desiredVelocity.x, maxSpeedChange);
        float newZ = Mathf.MoveTowards(currentZ, desiredVelocity.z, maxSpeedChange);

        velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);
    }

    void OnCollisionEnter(Collision collision) {
        EvaluateCollision(collision);
    }

    void OnCollisionStay(Collision collision) {
        EvaluateCollision(collision);
    }

    void EvaluateCollision(Collision collision) {
        for (int i = 0; i < collision.contactCount; i++) {
            Vector3 normal = collision.GetContact(i).normal;
            if (normal.y >= minGroundDotProduct) {
                groundContactCount += 1;
                contactNormal += normal;
            }
        }
    }




}
