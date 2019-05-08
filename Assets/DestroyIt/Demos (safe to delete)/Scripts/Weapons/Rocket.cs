using System.Collections;
using UnityEngine;

namespace DestroyIt
{
    /// <summary>
    /// Attach this script to a rocket.
    /// It handles fading out the flare effect (if any), applying blast damage and force to all objects within range, playing an explosive effect (with lighting), 
    /// and separating the smoke trail from the rocket on impact, so the smoke hangs in the air after the rocket is gone.
    /// </summary>
    public class Rocket : MonoBehaviour
    {
        [Range(1, 100)]
        public int speed = 30;                  // The amount of constant force applied to the missile. This translates to the missile's speed.
        public float blastRadius = 10f;         // The maximum distance from the point of impact that objects will take damage or get pushed around.
        public float blastForce = 250f;         // The strength (or force) of the blast. Higher numbers make stuff blow up real good. 
        [Range(0f, 3f)]
        public float explosionUpwardPush = 1f;  // The amount of upward "push" explosions have. Higher numbers make debris fly up in the air, but can get unrealistic. 
        public GameObject explosionPrefab;      // The particle effect to play when this object collides with something.
        public ParticleSystem smokeTrailPrefab;
        [Range(0f, 10f)]
        public float flightTime = 3.5f;         // How long the rocket will fly (in seconds) before running out of fuel.
        [Range(0f, 20f)]
        public float maxLifetime = 10f;         // Remove the rocket from the scene after this many seconds, regardless of whether it's out of fuel.
        [Range(0f, .1f)]
        public float flareFadeSpeed = .025f;    // How quickly the rocket flare fades as the rocket travels.

        private float checkFrequency = 0.1f;    // The time (in seconds) this script checks for updates.
        private float nextUpdateCheck;
        private bool outOfFuel;
        private float flightTimer;
        private GameObject smokeTrailObj;
        private bool isExploding = false;
        private bool isInitialized = false;
        private bool isStarted = false;
        private float smokeTrailDistance = 0.27f;

        private void Start()
        {
            isInitialized = true;
        }

        private void OnEnable()
        {
            isStarted = false;
            nextUpdateCheck = Time.time + checkFrequency;
        }

        private void Update()
        {
            if (!isInitialized) return;

            if (!isStarted)
            {
                EngineStartUp();
                isStarted = true;
            }

            if (Time.time > nextUpdateCheck)
            {
                float remainingFlightTime = Time.time - flightTimer;

                // Check if rocket should be culled
                if (remainingFlightTime > maxLifetime)
                    StartCoroutine(Recover());

                // Check if rocket is out of fuel    
                if (!outOfFuel && remainingFlightTime > flightTime)
                    EngineShutDown();

                // reset the counter
                nextUpdateCheck = Time.time + checkFrequency;
            }
        }

        private void EngineStartUp()
        {
            flightTimer = Time.time;
            isExploding = false;
            outOfFuel = false;

            // set the missile's speed (constant force)
            GetComponent<ConstantForce>().relativeForce = new Vector3(0, 0, speed);

            // create the smoke trail
            smokeTrailObj = ObjectPool.Instance.Spawn(smokeTrailPrefab.gameObject, new Vector3(0, 0, smokeTrailDistance * -1), Quaternion.identity, transform);
        }

        private void EngineShutDown()
        {
            if (GetComponent<ConstantForce>() != null)
                GetComponent<ConstantForce>().relativeForce = Vector3.zero;
            GetComponent<Rigidbody>().useGravity = true;
            Transform exhaust = transform.Find("exhaust");
            if (exhaust != null)
                exhaust.gameObject.SetActive(false);
            outOfFuel = true;

            // turn off point light (no thrust means no light source)
            Transform pointLight = transform.Find("point light");
            if (pointLight != null)
                pointLight.gameObject.SetActive(false);
        }

        private void TurnOffSmokeTrail()
        {
            if (smokeTrailObj == null) return;
            // Unparent smoke trail from rocket, 
            // turn off particle emitters, and queue it up for culling.
            smokeTrailObj.transform.parent = null;
            var emission = smokeTrailObj.GetComponent<ParticleSystem>().emission;
            emission.enabled = false;
            PoolAfter poolAfter = smokeTrailObj.AddComponent<PoolAfter>();
            poolAfter.seconds = 7f;
            poolAfter.removeWhenPooled = true;
        }

        public void OnCollisionEnter(Collision collision)
        {
            // If rocket is already exploding, exit.
            if (!isExploding)
                this.Explode();
        }

        public void Explode()
        {
            isExploding = true;
            
            ExplosiveDamage explosion = new ExplosiveDamage() { Position = transform.position, BlastForce = blastForce, Radius = blastRadius, UpwardModifier = explosionUpwardPush };
            Collider[] objectsInRange = Physics.OverlapSphere(transform.position, blastRadius);

            TurnOffSmokeTrail();

            // Play explosion particle effect.
            ObjectPool.Instance.Spawn(explosionPrefab, transform.position, this.GetComponent<Rigidbody>().rotation);

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
                    if (UnityEngine.Random.Range(1, 100) > 50) // Do this about half the time...
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

            StartCoroutine(Recover());
        }

        private IEnumerator Recover()
        {
            yield return new WaitForFixedUpdate();
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            GetComponent<Rigidbody>().Sleep();
            GetComponent<Rigidbody>().useGravity = false;
            ObjectPool.Instance.PoolObject(this.gameObject, true);
            StopAllCoroutines();
        }
    }
}