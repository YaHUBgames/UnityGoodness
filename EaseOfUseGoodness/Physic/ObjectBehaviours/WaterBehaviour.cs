using UnityEngine;

public class WaterBehaviour : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;

    [SerializeField] private float depthBeforeSubmerged = 1f;
    [SerializeField] private float cubeVolume = 3f;
    [SerializeField] private int floaterCount = 1;
    [SerializeField] private float waterDrag = 0.99f;
    [SerializeField] private float waterAngularDrag = 0.5f;

    [SerializeField] private float splashSize = 1f;
    [SerializeField] private float movementSlashingTreshold = 1f;

    private float distanceTraveled = 0;
    private Vector3 lastPosition = Vector3.zero;

    public void SetupDistanceTraveled()
    {
        Vector3 newPosition = transform.position;
        newPosition.y = 0;
        lastPosition = newPosition;
        distanceTraveled = 0;
    }

    private void DistanceTraveled()
    {
        Vector3 newPosition = transform.position;
        newPosition.y = 0;
        distanceTraveled += (lastPosition - newPosition).magnitude;
        lastPosition = newPosition;
    }

    public bool CheckDistanceTraveled()
    {
        if(movementSlashingTreshold > distanceTraveled)
            return false;
        distanceTraveled -= movementSlashingTreshold;
        return true;
    }

    public Vector3 GetObjectVelocity()
    {
        return _rigidbody.velocity;
    }

    public Vector3 GetObjectPosition()
    {
        return transform.position;
    }

    public float GetObjectsSplashSize()
    {
        return splashSize;
    }

    public void MoveObject(float waveHeight)
    {
        _rigidbody.AddForceAtPosition(Physics.gravity / floaterCount, transform.position, ForceMode.Acceleration);
        DistanceTraveled();
        if (transform.position.y >= waveHeight)
            return;
        
        float displacementMultiplier = Mathf.Clamp01((waveHeight - transform.position.y) / depthBeforeSubmerged) * cubeVolume;
        _rigidbody.AddForceAtPosition(new Vector3(0f, Mathf.Abs(Physics.gravity.y) * displacementMultiplier, 0f), transform.position, ForceMode.Acceleration);
        _rigidbody.AddForce(displacementMultiplier * -_rigidbody.velocity * waterDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
        _rigidbody.AddTorque(displacementMultiplier * -_rigidbody.angularVelocity * waterAngularDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
    }
}
