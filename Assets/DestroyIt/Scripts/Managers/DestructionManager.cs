using System;
using System.Collections.Generic;
using UnityEngine;
// ReSharper disable LoopCanBeConvertedToQuery
// ReSharper disable SuggestVarOrType_Elsewhere
// ReSharper disable SuggestVarOrType_BuiltInTypes
// ReSharper disable SuggestVarOrType_SimpleTypes
// ReSharper disable InconsistentNaming
// ReSharper disable ForCanBeConvertedToForeach

namespace DestroyIt
{
    /// <summary>
    /// Destruction Manager (Singleton) - manages all destructible objects.
    /// Put this script on an empty game object in your scene.
    /// </summary>
    public class DestructionManager : MonoBehaviour
    {
        [Tooltip("Maximum allowed persistent debris pieces in the scene.")]
        public int maxPersistentDebris = 400;
        [Tooltip("Maximum allowed destroyed prefabs within [withinSeconds] seconds. When this limit is reached, a particle effect will be used instead.")]
        public int destroyedPrefabLimit = 15;
        [Tooltip("Number of seconds within which no more than [destroyedPrefabLimit] destructions will be instantiated.")]
        public int withinSeconds = 4;
        [Tooltip("The default particle effect to use when a large object is destroyed.")]
        public ParticleSystem defaultLargeParticle;
        [Tooltip("The default particle effect to use when a small object is destroyed.")]
        public ParticleSystem defaultSmallParticle;
        [Tooltip("Anything larger than this (in game units) is considered a large object, and will get the default large particle.")]
        public float smallObjectMaxSize = 1f;
        [Tooltip("If damage done to an object is more than this many times its hit points, it will be obliterated into a particle effect if CanBeObliterated=true.")]
        public int obliterateMultiplier = 3;
        [Tooltip("If true, persistent debris is allowed to be culled even if the camera is currently rendering it.")]
        public bool removeVisibleDebris = true;
        [Tooltip("The time (in seconds) this script processes updates.")]
        public float updateFrequency = .5f;

        [HideInInspector]
        public bool useCameraDistanceLimit = true;  // If true, things beyond the specified distance from the main camera will be destroyed in a more limiting (ie, higher performance) way.
        [HideInInspector]
        public int cameraDistanceLimit = 100;       // Specified game units (usually meters) from camera, where destruction limiting will occur.
        [HideInInspector]
        public int debrisLayer = -1;

        // Private Variables
        private float nextUpdate;
        private List<Destructible> destroyedObjects;
        private List<Debris> debrisPieces;

        // Events
        public event Action DestroyedPrefabCounterChangedEvent;
        public event Action ActiveDebrisCounterChangedEvent;

        // Properties
        public List<float> DestroyedPrefabCounter { get; private set; }

        public bool IsDestroyedPrefabLimitReached { get { return DestroyedPrefabCounter.Count >= destroyedPrefabLimit; } }

        public int ActiveDebrisCount
        {
            get
            {
                int count = 0;
                foreach (Debris debris in debrisPieces)
                {
                    if (debris.IsActive)
                        count ++;
                }
                return count;
            }
        }

        // Hide the default constructor (use DestructionManager.Instance instead).
        private DestructionManager() { }

        // Here is a private reference only this class can access
        private static DestructionManager _instance;

        // This is the public reference that other classes will use
        public static DestructionManager Instance
        {
            get
            {
                // If _instance hasn't been set yet, we grab it from the scene.
                // This will only happen the first time this reference is used.
                if (_instance == null)
                    _instance = FindObjectOfType<DestructionManager>();
                return _instance;
            }
        }

        private void Awake()
        {
            // Initialize variables
            DestroyedPrefabCounter = new List<float>();

            debrisLayer = LayerMask.NameToLayer("DestroyItDebris");
            debrisPieces = new List<Debris>();
            destroyedObjects = new List<Destructible>();
            nextUpdate = Time.time + updateFrequency;

            // If the default particles haven't been assigned, try to get them from the Resources folder.
            if (defaultLargeParticle == null)
                defaultLargeParticle = Resources.Load<ParticleSystem>("Default_Particles/DefaultLargeParticle");
            if (defaultSmallParticle == null)
                defaultSmallParticle = Resources.Load<ParticleSystem>("Default_Particles/DefaultSmallParticle");
            
            // Checks
            Check.IsDefaultLargeParticleAssigned();
            Check.IsDefaultSmallParticleAssigned();
            if (Check.LayerExists("DestroyItDebris", false) == false)
                Debug.LogWarning("DestroyItDebris layer not found. Add a layer named 'DestroyItDebris' to your project if you want debris to ignore other debris when using Cling Points.");
        }

        private void Update()
        {
            if (Time.time < nextUpdate) return;
            
            // Manage Destroyed Prefab counter
            DestroyedPrefabCounter.Update(withinSeconds);

            // Manage Debris Queue
            if (debrisPieces.Count > 0)
            {
                // Cleanup references to debris no longer in the game
                int itemsRemoved = debrisPieces.RemoveAll(x => x == null || !x.IsActive);
                if (itemsRemoved > 0)
                    FireActiveDebrisCounterChangedEvent();
                //TODO: Debris is getting removed from the list, but not destroyed from the game. Debris parent objects should probably check their children periodically for enabled meshes.

                // Disable debris until the Max Debris limit is satisfied.
                if (ActiveDebrisCount > maxPersistentDebris)
                {
                    int overBy = ActiveDebrisCount - maxPersistentDebris;
                        
                    foreach (Debris debris in debrisPieces)
                    {
                        if (overBy <= 0) break;
                        if (!debris.IsActive) continue;
                        if (!removeVisibleDebris)
                        {
                            if (debris.Rigidbody.GetComponent<Renderer>() == null) continue;
                            if (debris.Rigidbody.GetComponent<Renderer>().isVisible) continue;
                        }
                        // Disable the debris.
                        debris.Disable();
                        overBy -= 1;
                    }
                }
            }

            // Manage Destroyed Objects list (ie, we're spacing out the Destroy() calls for performance)
            if (destroyedObjects.Count > 0)
            {
                // Destroy a maximum of 5 gameobjects per update, to space it out a little.
                int nbrObjects = destroyedObjects.Count > 5 ? 5 : destroyedObjects.Count;
                for (int i=0; i<nbrObjects; i++)
                {
                    // Destroy the gameobject and remove it from the list.
                    if (destroyedObjects[i] != null && destroyedObjects[i].gameObject != null)
                        Destroy(destroyedObjects[i].gameObject);
                }
                destroyedObjects.RemoveRange(0, nbrObjects);
            }

            nextUpdate = Time.time + updateFrequency; // reset the next update time.
        }

        /// <summary>Swaps the current destructible object with a new one and applies the correct materials to the new object.</summary>
        public void ProcessDestruction<T>(Destructible oldObj, GameObject destroyedPrefab, T damageInfo, bool isObliterated)
        {
            // If there is no destructible object to create debris from, exit. (This should never happen, it's just thrown in here as a safety.)
            if (oldObj == null) return;

            // If for some reason, ProcessDestruction has been called on an object that is not able to be destroyed, exit.
            if (!oldObj.canBeDestroyed) return;

            oldObj.FireDestroyedEvent();

            // Look for any debris objects clinging to the old object and un-parent them before destroying the old object.
            oldObj.ReleaseClingingDebris();

            // Remove any Joints from the destroyed object
            //TODO: Is this necessary?
            Joint[] joints = oldObj.GetComponentsInChildren<Joint>();
            foreach (Joint jnt in joints)
                Destroy(jnt);

            // Unparent DamageEffects and turn off all particle emissions
            if (oldObj.damageEffects != null)
            {
                for (int i = 0; i < oldObj.damageEffects.Count; i++)
                {
                    if (oldObj.damageEffects[i].UnparentOnDestroy && oldObj.damageEffects[i].GameObject != null)
                    {
                        oldObj.damageEffects[i].GameObject.transform.SetParent(null, true);
                        if (oldObj.damageEffects[i].StopEmittingOnDestroy &&
                            oldObj.damageEffects[i].ParticleSystems != null &&
                            oldObj.damageEffects[i].ParticleSystems.Length > 0)
                        {
                            for (int j = 0; j < oldObj.damageEffects[i].ParticleSystems.Length; j++)
                            {
                                var emission = oldObj.damageEffects[i].ParticleSystems[j].emission;
                                emission.enabled = false;
                            }
                        }
                    }
                }
            }

            // OPTION: DESTRUCTIBLE OBJECT SINKS THROUGH TERRAIN WHEN DESTROYED.
            if (oldObj.sinkWhenDestroyed)
            {
                // First, turn Kinematic off for all rigidbodies under this object.
                Rigidbody[] rbodies = oldObj.GetComponentsInChildren<Rigidbody>();
                foreach (Rigidbody rbody in rbodies)
                {
                    rbody.isKinematic = false;
                    rbody.WakeUp();
                }
                // Next, strip off all colliders so it falls through the terrain.
                Collider[] colliders = oldObj.GetComponentsInChildren<Collider>();
                foreach (Collider coll in colliders)
                    coll.enabled = false;
                // Attach the DestroyAfter script to the object so it will get removed from the game.
                DestroyAfter destAfter = oldObj.gameObject.AddComponent<DestroyAfter>();
                destAfter.seconds = 5f;
                // Exit immediately - don't do any more destruction processing.
                return;
            }

            // If there is a destructible object to destroy, but no debris to create, pop a particle effect and exit.
            // Likewise, if the object was obliterated, use only a particle effect.
            if (destroyedPrefab == null || (IsDestroyedPrefabLimitReached && oldObj.canBeObliterated) || isObliterated)
            {
                DestroyWithParticleEffect(oldObj, oldObj.fallbackParticle, damageInfo);
                return;
            }
            
            if (useCameraDistanceLimit && oldObj.canBeObliterated)
            {
                // Find the distance between the camera and the destroyed object
                float distance = Vector3.Distance(oldObj.transform.position, Camera.main.transform.position);
                if (distance > cameraDistanceLimit)
                {
                    DestroyWithParticleEffect(oldObj, oldObj.fallbackParticle, damageInfo);
                    return;
                }
            }

            // If we've passed the checks above, then we are creating debris.
            DestroyedPrefabCounter.Add(Time.time);
            FireDestroyedPrefabCounterChangedEvent();

            // Unparent any specified child objects before destroying
            UnparentSpecifiedChildren(oldObj);

            // Put the destroyed object in the Debris layer to keep new debris from clinging to it
            if (debrisLayer != -1)
                oldObj.gameObject.layer = debrisLayer;

            // Try to get the object from the pool
            GameObject newObj = ObjectPool.Instance.Spawn(destroyedPrefab, oldObj.PositionFixedUpdate, oldObj.RotationFixedUpdate, oldObj.GetInstanceID());
            InstantiateDebris(newObj, oldObj, damageInfo);

            oldObj.gameObject.SetActive(false);
            destroyedObjects.Add(oldObj);
        }

        private void DestroyWithParticleEffect<T>(Destructible oldObj, ParticleSystem customParticle, T damageInfo)
        {
            if (oldObj.useFallbackParticle)
            {
                // Use the DestructibleGroup instance ID if it exists, otherwise use the Destructible object's parent's instance ID.
                GameObject parentObj = oldObj.gameObject.GetHighestParentWithTag(Tag.DestructibleGroup) ?? oldObj.gameObject;
                int instanceId = parentObj.GetInstanceID();

                // Use the mesh center point as the starting position for the particle effect.
                var position = oldObj.MeshCenterPoint;
                
                // If a particle spawn point has been specified, use that instead.
                if (oldObj.centerPointOverride != Vector3.zero)
                    position = oldObj.centerPointOverride;
                
                // Convert the particle spawn point position to world coordinates.
                position = oldObj.transform.TransformPoint(position);

                // If the ParticleManager is available, use it to play the particle. Otherwise, just spawn it directly.
                if (customParticle != null)
                {
                    // If the ParticleManager is available, use it.
                    if (ParticleManager.Instance == null)
                        ObjectPool.Instance.Spawn(customParticle.gameObject, position, oldObj.transform.rotation);
                    else
                        ParticleManager.Instance.PlayEffect(customParticle, position, oldObj.transform.rotation, instanceId);
                }
                else //Otherwise, use a default effect attached to the DestructionManager.
                {
                    // Find out how large the object is by adding the bounds extents of all its meshes.
                    ParticleSystem defaultParticle = defaultSmallParticle;
                    //TODO: Should probably switch this to use total COLLIDER bounds instead of MESH bounds to determine the object's overall size.
                    float totalMeshSize = 0f;
                    foreach (MeshFilter mesh in oldObj.GetComponentsInChildren<MeshFilter>())
                    {
                        if (mesh.sharedMesh == null) continue;
                        Bounds bounds = mesh.sharedMesh.bounds;
                        float maxMeshSize = Mathf.Max(bounds.size.x*mesh.transform.localScale.x,
                            bounds.size.y*mesh.transform.localScale.y, bounds.size.z*mesh.transform.localScale.z);
                        totalMeshSize += maxMeshSize;
                    }

                    if (totalMeshSize > smallObjectMaxSize)
                        defaultParticle = defaultLargeParticle;

                    // If the ParticleManager is available, use it to play the particle. Otherwise, just spawn it directly.
                    if (ParticleManager.Instance == null)
                        ObjectPool.Instance.Spawn(defaultParticle.gameObject, position, oldObj.transform.rotation);
                    else
                    {
                        ParticleManager.Instance.PlayEffect(defaultParticle, position, oldObj.transform.rotation,
                            instanceId, MaterialPreloader.Instance.GetDestroyedMaterial(oldObj.fallbackParticleMaterial, oldObj));
                    }
                }
            }

            UnparentSpecifiedChildren(oldObj);
            oldObj.gameObject.SetActive(false);
            destroyedObjects.Add(oldObj);

            // Reapply impact force to impact object so it punches through the destroyed object along its original path. 
            // If you turn this off, impact objects will be deflected even though the impacted object was destroyed.
            if (damageInfo.GetType() == typeof(ImpactDamage))
                DestructibleHelper.ReapplyImpactForce(damageInfo as ImpactDamage, oldObj.VelocityReduction);
        }

        private static void UnparentSpecifiedChildren(Destructible obj)
        {
            if (obj.unparentOnDestroy == null) return;

            foreach (GameObject child in obj.unparentOnDestroy)
            {
                if (child == null)
                    continue;

                // Unparent the child object from the destructible object.
                child.transform.parent = null;

                // Initialize any DelayedRigidbody scripts on the object.
                DelayedRigidbody[] delayedRigidbodies = child.GetComponentsInChildren<DelayedRigidbody>();
                foreach (DelayedRigidbody dr in delayedRigidbodies)
                    dr.Initialize();

                // Check whether we should turn off Kinematic on child objects, so they will fall freely.
                if (obj.disableKinematicOnUparentedChildren)
                {
                    Rigidbody[] rigidbodies = child.GetComponentsInChildren<Rigidbody>();
                    foreach (Rigidbody rbody in rigidbodies)
                        rbody.isKinematic = false;
                }
                
                // Turn off any animations
                Animation[] animations = child.GetComponentsInChildren<Animation>();
                foreach (Animation anim in animations)
                    anim.enabled = false;
            }
        }

        private void InstantiateDebris<T>(GameObject newObj, Destructible oldObj, T damageInfo)
        {
            // Apply new materials derived from previous object's materials
            if (!oldObj.autoPoolDestroyedPrefab) // if the old object was autopooled, the destroyed object will come from the pool already having the right materials on it.
                DestructibleHelper.TransferMaterials(oldObj, newObj);

            // Re-scale destroyed version if original destructible object has been scaled. (Scaling rigidbodies in general is bad, but this is put here for convenience.)
            if (oldObj.transform.lossyScale != new Vector3(1f, 1f, 1f)) // if destructible object has been scaled in the scene
                newObj.transform.localScale = oldObj.transform.lossyScale;

            if (oldObj.isDebrisChipAway)
            {
                // If we are doing chip-away debris, attach the ChipAwayDebris script to each piece of debris and exit.
                Collider[] debrisColliders = newObj.GetComponentsInChildren<Collider>();
                foreach (Collider coll in debrisColliders)
                {
                    if (coll.gameObject.GetComponent<ChipAwayDebris>() != null) continue;

                    if (coll.attachedRigidbody != null) continue;

                    ChipAwayDebris chipAwayDebris = coll.gameObject.AddComponent<ChipAwayDebris>();
                    chipAwayDebris.debrisMass = oldObj.chipAwayDebrisMass;
                    chipAwayDebris.debrisDrag = oldObj.chipAwayDebrisDrag;
                    chipAwayDebris.debrisAngularDrag = oldObj.chipAwayDebrisAngularDrag;
                }
                return;
            }
	        
	        if (oldObj.destroyedPrefabParent != null)
	        	newObj.transform.parent = oldObj.destroyedPrefabParent.transform;

            if (oldObj.childrenToReParentByName != null)
            {
                if (oldObj.childrenToReParentByName.Count > 0)
                {
                    foreach (string childName in oldObj.childrenToReParentByName)
                    {
                        Transform child = oldObj.transform.Find(childName);
                        if (child != null)
                            child.parent = newObj.transform;
                    }
                }
            }

            // Attempt to get the debris rigidbodies from auto-pooled rigidbodies first.
            Rigidbody[] debrisRigidbodies = oldObj.PooledRigidbodies;
	        GameObject[] debrisRigidbodyGos = oldObj.PooledRigidbodyGos;
            if (debrisRigidbodies != null)
            {
                if (debrisRigidbodies.Length == 0) // If none exist, do it the more expensive way...
                {
                    debrisRigidbodies = newObj.GetComponentsInChildren<Rigidbody>();
                    debrisRigidbodyGos = new GameObject[debrisRigidbodies.Length];
                    for (int i = 0; i < debrisRigidbodies.Length; i++)
                        debrisRigidbodyGos[i] = debrisRigidbodies[i].gameObject;
                }

                for (int i = 0; i < debrisRigidbodies.Length; i++)
                {
                    // Assign each piece of debris to the Debris layer if it exists.
                    if (debrisLayer != -1)
                        debrisRigidbodies[i].gameObject.layer = debrisLayer;

                    // Reparent any debris tagged for reparenting.
	                if (oldObj.debrisToReParentByName != null && oldObj.debrisToReParentByName.Count > 0 && oldObj.transform.parent != null && (oldObj.debrisToReParentByName.Contains("ALL DEBRIS") || oldObj.debrisToReParentByName.Contains(debrisRigidbodies[i].name)))
	                {
                        debrisRigidbodies[i].gameObject.transform.parent = oldObj.transform.parent;
                        debrisRigidbodies[i].isKinematic = oldObj.debrisToReParentIsKinematic;
                    }

                    // Add leftover velocity from destroyed object
                    if (!debrisRigidbodies[i].isKinematic)
                        debrisRigidbodies[i].velocity = oldObj.VelocityFixedUpdate;

                    // Add debris to the debris queue.
                    Debris debris = new Debris {Rigidbody = debrisRigidbodies[i], GameObject = debrisRigidbodyGos[i]};
                    debrisPieces.Add(debris);
                    FireActiveDebrisCounterChangedEvent();
                }
            }

            // Attempt to make some of the debris cling to adjacent rigidbodies
            if (oldObj.CheckForClingingDebris)
                newObj.MakeDebrisCling();

            // Reapply impact force to impact object so it punches through the destroyed object along its original path. 
            // If you turn this off, impact objects will be deflected even though the impacted object was destroyed.
            if (damageInfo.GetType() == typeof(ImpactDamage))
                DestructibleHelper.ReapplyImpactForce(damageInfo as ImpactDamage, oldObj.VelocityReduction);

            if (damageInfo.GetType() == typeof(ExplosiveDamage) || damageInfo.GetType() == typeof(ImpactDamage))
                ExplosionHelper.ApplyForcesToDebris(newObj, 1f, damageInfo);
        }

        /// <summary>Fires when the Destroyed Prefab counter changes.</summary>
        public void FireDestroyedPrefabCounterChangedEvent()
        {
            if (DestroyedPrefabCounterChangedEvent != null) // first, make sure there is at least one listener.
                DestroyedPrefabCounterChangedEvent(); // if so, trigger the event.
        }

        /// <summary>Fires when the Active Debris count changes.</summary>
        public void FireActiveDebrisCounterChangedEvent()
        {
            if (ActiveDebrisCounterChangedEvent != null) // first, make sure there is at least one listener.
                ActiveDebrisCounterChangedEvent(); // if so, trigger the event.
        }
    }
}