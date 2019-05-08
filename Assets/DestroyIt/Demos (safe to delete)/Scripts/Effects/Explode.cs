using UnityEngine;

namespace DestroyIt
{
    public class Explode : MonoBehaviour
    {
        public float blastRadius = 10f;         // The maximum distance from the point of impact that objects will take damage or get pushed around.
        public float blastForce = 250f;         // The strength (or force) of the blast. Higher numbers make stuff blow up real good. 
        [Range(0f, 3f)]
        public float explosionUpwardPush = 1f;  // The amount of upward "push" explosions have. Higher numbers make debris fly up in the air, but can get unrealistic.

        public void Start()
        {
            ExplosiveDamage explosion = new ExplosiveDamage() { Position = transform.position, BlastForce = blastForce, Radius = blastRadius, UpwardModifier = explosionUpwardPush };
            Collider[] objectsInRange = Physics.OverlapSphere(transform.position, blastRadius);

            // Apply force and damage to colliders and rigidbodies in range of the explosion
            foreach (Collider col in objectsInRange)
            {
                // Ignore terrain colliders
                if (col is TerrainCollider)
                    continue;

                // Ignore self (the rocket)
                if (col == this.GetComponent<Collider>())
                    continue;

                // Apply explosive force if a rigidbody could be found on this object
                Rigidbody rbody = col.attachedRigidbody;
                if (rbody != null && !rbody.isKinematic)
                    rbody.AddExplosionForce(blastForce, transform.position, blastRadius, explosionUpwardPush);

                // Check for Chip-Away Debris
                ChipAwayDebris chipAwayDebris = col.gameObject.GetComponent<ChipAwayDebris>();
                if (chipAwayDebris != null)
                {
                    if (Random.Range(1, 100) > 50) // Do this about half the time...
                    {
                        chipAwayDebris.BreakOff(blastForce, transform.position, blastRadius, explosionUpwardPush);
                        continue; //Skip the destructible check if the debris hasn't chipped away yet.
                    }
                    else
                        continue;
                }

                // If it's a Destructible object, apply damage to it based on proximity to the blast
                Destructible destructibleObj = col.gameObject.GetComponentInParent<Destructible>();
                if (destructibleObj != null)
                {
                    float proximity = (transform.position - col.transform.position).magnitude;
                    float effectAmount = 1 - (proximity / blastRadius);

                    if (effectAmount > 0f)
                        destructibleObj.ApplyDamage(explosion.AdjustEffect(effectAmount));
                }
            }
        }
    }
}