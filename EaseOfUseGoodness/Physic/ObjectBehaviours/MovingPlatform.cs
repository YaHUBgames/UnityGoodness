using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform passengerParent;
    [SerializeField] private Rigidbody platform;

    public void GetOn(Transform passenger)
    {
        passenger.SetParent(passengerParent, true);
    }

    public void GetOff(Transform passenger)
    {
        passenger.SetParent(null, true);
    }

    public Vector3 GetVelocity()
    {
        return platform.velocity;
    }
}
