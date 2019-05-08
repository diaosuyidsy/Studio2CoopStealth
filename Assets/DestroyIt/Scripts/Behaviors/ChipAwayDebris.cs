using UnityEngine;

namespace DestroyIt
{
    /// <summary>
    /// This script is attached to a debris piece at runtime. Your projectiles should check for collisions with 
    /// ChipAwayDebris and fire the BreakOff() method when impact occurs.
    /// </summary>
    public class ChipAwayDebris : MonoBehaviour
    {
        public float debrisMass = 1f;
        public float debrisDrag = 0f;
        public float debrisAngularDrag = 0.05f;
        private Renderer rend;

        public void BreakOff(Vector3 force, Vector3 point)
        {
            if (!CheckCanBreakOff()) return;
            
            // Make the debris fall away by attaching a rigidbody to it.
            Rigidbody rbody = rend.gameObject.AddComponent<Rigidbody>();
            rbody.mass = debrisMass;
            rbody.drag = debrisDrag;
            rbody.angularDrag = debrisAngularDrag;
            rbody.AddForceAtPosition(force, point, ForceMode.Impulse);

            Destroy(this);
        }

        public void BreakOff(float blastForce, Vector3 explosionPosition, float explosionRadius, float upwardsModifier)
        {
            if (!CheckCanBreakOff()) return;

            // Make the debris fall away by attaching a rigidbody to it.
            Rigidbody rbody = rend.gameObject.AddComponent<Rigidbody>();
            rbody.mass = debrisMass;
            rbody.drag = debrisDrag;
            rbody.angularDrag = debrisAngularDrag;
            rbody.AddExplosionForce(blastForce, transform.position, explosionRadius, upwardsModifier);

            Destroy(this);
        }

        private bool CheckCanBreakOff()
        {
            // If no collider is on this object, then something weird happened. Exit and destroy self.
            if (GetComponent<Collider>() == null)
            {
                Destroy(this);
                return false;
            }

            rend = gameObject.GetComponentInParent<Renderer>();
            if (rend == null)
            {
                Destroy(this);
                return false;
            }

            // If a rigidbody already exists on this debris piece, remove this script and exit.
            if (GetComponent<Collider>().attachedRigidbody != null)
            {
                Destroy(this);
                return false;
            }
            return true;
        }
    }
}