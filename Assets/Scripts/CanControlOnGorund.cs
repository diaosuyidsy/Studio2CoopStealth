using System.Collections;
using System.Collections.Generic;
using Invector.vCharacterController;
using Invector.vShooter;
using UnityEngine;

[RequireComponent(typeof(vThirdPersonController))]
[RequireComponent(typeof(vShooterMeleeInput))]
public class CanControlOnGorund : MonoBehaviour
{
    private float _distToGround;
    private LayerMask JumpMask;

    private void Awake()
    {
        _distToGround = GetComponent<CapsuleCollider>().bounds.extents.y;
        JumpMask = 1<<LayerMask.NameToLayer("Ground");
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == 0)
        {
            GetComponent<vThirdPersonController>().enabled = true;
            GetComponent<vShooterMeleeInput>().enabled = true;
        }
    }

    public bool IsGrounded()
    {
        RaycastHit hit;
        bool result = Physics.SphereCast(transform.position, 0.3f, Vector3.down, out hit, 0.1f, JumpMask);

        return result;
    }
}
