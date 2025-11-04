using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMover : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 6.5f;
    [SerializeField] float turnSmoothTime = 0.08f;   // smaller = snappier
    [SerializeField] float inputDeadzone = 0.15f;    // filters tiny stick/keys noise

    [Header("Camera (optional)")]
    [SerializeField] Transform cam; // leave empty to auto-grab Camera.main

    Rigidbody rb;
    float yawVel;        // for SmoothDampAngle
    Vector3 input;       // cached each Update
    Vector3 desiredDir;  // camera-relative direction

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        if (!cam && Camera.main) cam = Camera.main.transform;
    }

    void Update()
    {
        // 1) Read input (no physics work here)
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 raw = new Vector3(h, 0f, v);

        // Deadzone to stop micro-rotations
        input = (raw.magnitude < inputDeadzone) ? Vector3.zero : raw.normalized;

        // 2) Convert to camera-relative direction on the XZ plane
        if (cam)
        {
            Vector3 fwd = cam.forward;  fwd.y = 0f;  fwd.Normalize();
            Vector3 rgt = cam.right;    rgt.y = 0f;  rgt.Normalize();
            desiredDir = (fwd * input.z + rgt * input.x);
        }
        else
        {
            desiredDir = input; // fallback: world-relative
        }
        desiredDir = desiredDir.normalized;
    }

    void FixedUpdate()
    {
        // Kill any physics-induced spin on Y
        rb.angularVelocity = Vector3.zero;

        // If no input, just keep current velocity on Y (gravity) and stop horizontal drift
        if (desiredDir.sqrMagnitude < 0.0001f)
        {
            Vector3 vel = rb.linearVelocity;
            vel.x = 0f; vel.z = 0f;
            rb.linearVelocity = vel;
            return;
        }

        // 1) Smoothly rotate toward desired direction
        float currentYaw = rb.rotation.eulerAngles.y;
        float targetYaw  = Mathf.Atan2(desiredDir.x, desiredDir.z) * Mathf.Rad2Deg;
        float smoothYaw  = Mathf.SmoothDampAngle(currentYaw, targetYaw, ref yawVel, turnSmoothTime);
        Quaternion targetRot = Quaternion.Euler(0f, smoothYaw, 0f);
        rb.MoveRotation(targetRot);

        // 2) Move forward in facing direction
        Vector3 move = targetRot * Vector3.forward * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);
    }
}
