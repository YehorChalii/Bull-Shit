using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class GarbageObject : MonoBehaviour
{
    private Collider _collider;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void SetPhysical(bool physical)
    {
        _collider.enabled = physical;

        _rigidbody.isKinematic = !physical;
        _rigidbody.useGravity = physical;
    }
}