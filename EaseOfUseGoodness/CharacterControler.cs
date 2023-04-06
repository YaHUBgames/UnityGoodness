using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterControler : MonoBehaviour
{
    [SerializeField] private Rigidbody RB;
    [SerializeField] private Transform TR;
    [Header("Input")]
    [SerializeField] private Vector2 input;
    private Vector3 corectedForward = Vector3.forward;
    private Vector3 corectedRight = Vector3.right;
    private Vector3 corectedInput = Vector3.zero;
    private bool gotMovementInput = false;
    [SerializeField] private Transform mainCam;
    [Header("Floating")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask waterMask;
    [SerializeField] private Vector3 downDirection = Vector3.down;
    [SerializeField] private float rayLength = 1f;
    [SerializeField] private float rideHeight = 0.8f;
    [SerializeField] private float rideSpringStrength = 1f;
    [SerializeField] private float rideSpringDamper = 1f;
    [SerializeField] private float walkableWaterDepth = 0.5f;
    private bool canFloat = false;
    private MovingPlatform curentPlatform = null;
    [Header("Upright")]
    [SerializeField] private Quaternion uprightTargetRotation = Quaternion.identity;
    [SerializeField] private float uprightSpringStrength = 1f;
    [SerializeField] private float uprightSpringDamper = 1f;
    [SerializeField] private float velocityLeanFactor = 1f;
    [Header("Moving")]
    [SerializeField] private float maxSpeed = 10;
    [SerializeField] private float acceleration = 100;
    [SerializeField] private AnimationCurve accelerationFactorFromDot;
    [SerializeField] private float maxAccelerationForce = 100;
    [SerializeField] private AnimationCurve maxAccelerationForceFactorFromDot;
    [SerializeField] private Vector3 forceScale;
    [SerializeField] private float gravityScaleDrop = 10;
    private Vector3 goalVelocity = Vector3.zero;
    [Header("Jumping")]
    //analog jump
    //cayote
    //input buffer

    [Header("PowerUps")]
    public float speedFactor = 1f;
    public float maxAccelForceFactor = 1f;

    private RaycastHit raycastHit = new RaycastHit();
    private RaycastHit waterRaycastHit = new RaycastHit();
    private bool rayDidHit = false;
    private bool waterRayDidHit = false;

    private void FixedUpdate()
    {
        Ray ray = new Ray(transform.position, downDirection);
        rayDidHit = Physics.Raycast(ray, out raycastHit, rayLength, groundMask, QueryTriggerInteraction.Collide);
        Rigidbody hitBody = null;

        Ray waterRay = new Ray(transform.position - downDirection, downDirection);
        waterRayDidHit = Physics.Raycast(waterRay, out waterRaycastHit, rayLength + 1, waterMask, QueryTriggerInteraction.Collide);

        canFloat = CanFloatCheck();

        if (canFloat)
        {
            Vector3 velocity = RB.velocity;
            Vector3 rayDirection = downDirection;

            Vector3 otherVelocity = Vector3.zero;
            hitBody = raycastHit.rigidbody;
            if (hitBody != null)
            {
                otherVelocity = hitBody.velocity;
            }

            float rayDirectionVelocity = Vector3.Dot(rayDirection, velocity);
            float otherDirectionVelocity = Vector3.Dot(rayDirection, otherVelocity);

            float relativeVelocity = rayDirectionVelocity - otherDirectionVelocity;

            float x = raycastHit.distance - rideHeight;

            float springForce = (x * rideSpringStrength) - (relativeVelocity * rideSpringDamper);

            Debug.DrawLine(transform.position, transform.position + downDirection * rayLength, Color.magenta);
            Debug.DrawLine(transform.position, transform.position + downDirection * rideHeight, Color.cyan);

            RB.AddForce(rayDirection * springForce);
            if (hitBody != null)
            {
                hitBody.AddForceAtPosition(rayDirection * -springForce, raycastHit.point);
            }
        }
        UpdateUprightForce();
        UpdateMovementForce();
    }

    private bool CanFloatCheck()
    {
        if (!rayDidHit)
        {
            curentPlatform?.GetOff(TR);
            curentPlatform = null;
            return false;
        }
        if(curentPlatform == null)
        {
            MovingPlatform newPlatform = raycastHit.transform.GetComponent<MovingPlatform>();
            if(newPlatform != null)
            {
                newPlatform.GetOn(TR);
                curentPlatform = newPlatform;
            }
        }
        else
            return true;

        if (!waterRayDidHit)
            return true;
        WaterController waterController = waterRaycastHit.transform.GetComponent<WaterController>();
        if (waterController == null)
            return true;
        if (waterController.GetWorldHeightAtPosition(transform.position) - raycastHit.point.y > walkableWaterDepth)
            return false;
        return true;
    }

    private void UpdateUprightForce()
    {
        if (gotMovementInput)
            uprightTargetRotation = Quaternion.Euler(RB.velocity.magnitude * velocityLeanFactor, Vector2.SignedAngle(Vector2.down, new Vector2(corectedInput.x, -corectedInput.z)), 0);
        else
            uprightTargetRotation = Quaternion.Euler(0, uprightTargetRotation.eulerAngles.y, 0);

        Quaternion currentRotation = transform.rotation;
        Quaternion toGoal = MathUtils.ShortestRotation(uprightTargetRotation, currentRotation);

        Vector3 rotationAxis;
        float rotationDegrees;

        toGoal.ToAngleAxis(out rotationDegrees, out rotationAxis);
        rotationAxis.Normalize();

        float rotationRadians = rotationDegrees * Mathf.Deg2Rad;

        RB.AddTorque((rotationAxis * (rotationRadians * uprightSpringStrength)) - (RB.angularVelocity * uprightSpringDamper));
    }

    private void UpdateMovementForce()
    {
        Vector3 unitVelocity = goalVelocity.normalized;

        float velocityDot = Vector3.Dot(corectedInput, unitVelocity);
        float curentAcceleration = acceleration * accelerationFactorFromDot.Evaluate(velocityDot);

        Vector3 curentGoalVelocity = corectedInput * maxSpeed * speedFactor;
        Vector3 platformVelocity = Vector3.zero;
        if(curentPlatform != null)
            platformVelocity = curentPlatform.GetVelocity();

        goalVelocity = Vector3.MoveTowards(goalVelocity, curentGoalVelocity + platformVelocity, acceleration * Time.fixedDeltaTime);

        Vector3 neededAcceleration = (goalVelocity - RB.velocity) / Time.fixedDeltaTime;
        float maxAcceleration = maxAccelerationForce * maxAccelerationForceFactorFromDot.Evaluate(velocityDot) * maxAccelForceFactor;
        neededAcceleration = Vector3.ClampMagnitude(neededAcceleration, maxAcceleration);

        RB.AddForce(Vector3.Scale(neededAcceleration * RB.mass, forceScale));
    }

    private void UpdateJumpForce()
    {

    }


    public void GetMovementInput(InputAction.CallbackContext context)
    {
        input = context.ReadValue<Vector2>();
        GetCorectedVectorsFromCam();
        if (context.performed)
        {
            gotMovementInput = true;
            return;
        }
        gotMovementInput = false;
    }

    public void GetJumpInput(InputAction.CallbackContext context)
    {
        if(context.performed)
            isJumpInputOn = true;
        if(context.canceled)
            isJumpInputOn = false;
    }
    float jumpInputTimer = 0;
    bool isJumpInputOn = false;
    private void Update() {
        if(isJumpInputOn)
            jumpInputTimer += Time.deltaTime;
        if(!isJumpInputOn)
            jumpInputTimer = 0;
    }

    private void GetCorectedVectorsFromCam()
    {
        Vector3 camForward = mainCam.forward;
        Vector3 camRight = mainCam.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();
        corectedForward = camForward;
        corectedRight = camRight;
        corectedInput = (input.x * camRight - input.y * camForward).normalized;
    }
}
