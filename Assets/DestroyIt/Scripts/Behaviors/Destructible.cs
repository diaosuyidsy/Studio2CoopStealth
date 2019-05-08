﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
// ReSharper disable InconsistentNaming
// ReSharper disable SuggestVarOrType_SimpleTypes
// ReSharper disable SuggestVarOrType_Elsewhere
// ReSharper disable SuggestVarOrType_BuiltInTypes
// ReSharper disable ForCanBeConvertedToForeach
// ReSharper disable CheckNamespace

namespace DestroyIt
{
    /// <summary>
    /// Put this script on any rigidbody object you want to be destructible. The script should go on the same GameObject as the RigidBody.
    /// The rigidbody should also have attached colliders, either on the same game object or one or more children under it.
    /// </summary>
    [DisallowMultipleComponent]
    public class Destructible : MonoBehaviour
    {
        [HideInInspector] public int totalHitPoints = 50;
        [HideInInspector] public int currentHitPoints = 50;
        [HideInInspector] public List<DamageLevel> damageLevels;
	    [HideInInspector] public GameObject destroyedPrefab;
	    [HideInInspector] public GameObject destroyedPrefabParent;
        [HideInInspector] public ParticleSystem fallbackParticle;        
        [HideInInspector] public Material fallbackParticleMaterial;
        [HideInInspector]
        [FormerlySerializedAs("damageLevelParticles")]
        public List<DamageEffect> damageEffects;
        [HideInInspector] public float velocityReduction = .5f;
        [HideInInspector] public float ignoreCollisionsUnder = 2f;
        [HideInInspector] public List<GameObject> unparentOnDestroy;
        [HideInInspector] public bool disableKinematicOnUparentedChildren = true;
        [HideInInspector] public List<MaterialMapping> replaceMaterials;
        [HideInInspector] public bool canBeDestroyed = true;
        [HideInInspector] public bool canBeRepaired = true;
        [HideInInspector] public bool canBeObliterated = true;
        [HideInInspector] public List<string> debrisToReParentByName;
        [HideInInspector] public bool debrisToReParentIsKinematic;
        [HideInInspector] public List<string> childrenToReParentByName;
        [HideInInspector] public int destructibleGroupId;
        [HideInInspector] public bool isDebrisChipAway;
        [HideInInspector] public float chipAwayDebrisMass = 1f;
        [HideInInspector] public float chipAwayDebrisDrag;
        [HideInInspector] public float chipAwayDebrisAngularDrag = 0.05f;
        [HideInInspector] public bool autoPoolDestroyedPrefab = true;
        [HideInInspector] public bool useFallbackParticle = true;
        [HideInInspector] public Vector3 centerPointOverride;
        [HideInInspector] public bool sinkWhenDestroyed; 

        // Private variables
        private const float invulnerableTimer = 0.5f; // How long (in seconds) the destructible object is invulnerable after instantiation.
        private DamageLevel currentDamageLevel;
        private bool isObliterated;
        private bool isInitialized;
        private bool checkForClingingDebris = true;
        private bool useProgressiveDamage = true;
        /// <summary>
        /// Determines whether the destructible object starts with a short period of invulnerability. 
        /// Prevents destructible debris from being immediately destroyed by the same forces that destroyed the original object.
        /// </summary>
        private bool IsInvulnerable { get; set; }

        /// <summary>Stores a reference to this destructible object's rigidbody, so we don't have to use GetComponent() at runtime.</summary>
        private Rigidbody Rigidbody { get; set; }

        // Properties
        public bool UseProgressiveDamage { get { return useProgressiveDamage; } set { useProgressiveDamage = value; } }
        public bool CheckForClingingDebris { get { return checkForClingingDebris; } set { checkForClingingDebris = value;} } // This is an added optimization used when we are auto-pooling destroyed prefabs. It allows us to avoid a GetComponentsInChildren() check for ClingPoints destruction time.
        public Rigidbody[] PooledRigidbodies { get; set; } // This is an added optimization used when we are auto-pooling destroyed prefabs. It allows us to avoid multiple GetComponentsInChildren() checks for Rigidbodies at destruction time.
        public GameObject[] PooledRigidbodyGos { get; set; } // This is an added optimization used when we are auto-pooling destroyed prefabs. It allows us to avoid multiple GetComponentsInChildren() checks for the GameObjects on Rigidibodies at destruction time.
        public float VelocityReduction { get { return Mathf.Abs(velocityReduction - 1f); /* invert the velocity reduction value (so it makes sense in the UI) */ } }
        public Quaternion RotationFixedUpdate { get; private set; }
        public Vector3 PositionFixedUpdate { get; private set; }
        public Vector3 VelocityFixedUpdate { get; private set; }
        public int LastRepairedAmount { get; private set; }
        public int LastDamagedAmount { get; private set; }
        public bool IsDestroyed {get { return !IsInvulnerable && canBeDestroyed && currentHitPoints <= 0; }}
        public Vector3 MeshCenterPoint { get; private set; }

        // Events
        public event Action DamagedEvent;
        public event Action DestroyedEvent;
        public event Action RepairedEvent;

        public void Start()
        {
            checkForClingingDebris = true;
            Renderer[] meshes = GetComponentsInChildren<Renderer>();

            // Checks
            Check.HasNonTriggerCollider(gameObject);
            Check.HasRenderer(gameObject, meshes);
            Check.HasMaterial(gameObject, meshes);

            if (damageLevels == null || damageLevels.Count == 0)
                damageLevels = DestructibleHelper.DefaultDamageLevels();
            damageLevels.CalculateDamageLevels(totalHitPoints);

            // Set fallbackParticle material (destroyed material version)
            if (fallbackParticleMaterial == null)
            {
                Renderer rend = gameObject.GetComponentInChildren<Renderer>();
                Material particleMat = null;
                if (rend != null && rend.sharedMaterials.Length > 0)
                    particleMat = rend.sharedMaterials[0];
                fallbackParticleMaterial = particleMat;
            }

            // Store a reference to this object's rigidbody, for better performance.
            Rigidbody = GetComponent<Rigidbody>();

            MeshCenterPoint = GetMeshCenterPoint(this);
            PlayDamageEffects();
            IsInvulnerable = true;
            Invoke("RemoveInvulnerability", invulnerableTimer);
            isInitialized = true;
        }

        // ReSharper disable once UnusedMember.Global
        public void RemoveInvulnerability()
        {
            IsInvulnerable = false;
        }

        public void FixedUpdate()
        {
            if (!isInitialized) return;
            if (DestructionManager.Instance == null) return;

            // Use the fixed update position/rotation for placement of the destroyed prefab.
            PositionFixedUpdate = transform.position;
            RotationFixedUpdate = transform.rotation;
            if (Rigidbody != null)
                VelocityFixedUpdate = Rigidbody.velocity;

            SetDamageLevel();
            PlayDamageEffects();

            if (IsDestroyed) 
                DestructionManager.Instance.ProcessDestruction(this, destroyedPrefab, new ExplosiveDamage(), isObliterated);
        }

        private static Vector3 GetMeshCenterPoint(Destructible destructibleObj)
        {
            if (destructibleObj.gameObject.isStatic)
                return Vector3.zero; // use the current world position for static objects.

            Bounds combinedBounds = new Bounds();

            MeshFilter[] meshFilters = destructibleObj.GetComponentsInChildren<MeshFilter>();
            foreach (MeshFilter meshFilter in meshFilters)
                combinedBounds.Encapsulate(meshFilter.sharedMesh.bounds);

            return combinedBounds.center;
        }

        /// <summary>Applies a generic amount of damage, with no specific impact or explosive force.</summary>
        public void ApplyDamage(int amount)
        {
            if (IsDestroyed || IsInvulnerable) return; // don't try to apply damage to an already-destroyed or invulnerable object.

            LastDamagedAmount = amount > currentHitPoints ? currentHitPoints : amount;
            FireDamagedEvent();

            currentHitPoints -= amount;
            CheckForObliterate(amount);
            if (currentHitPoints > 0) return;
            if (currentHitPoints < 0) currentHitPoints = 0;

            PlayDamageEffects();

            // Advance to the next destructible object, passing in the leftover damage to apply to the next model and 
            // the projectile, which may have force added to it so it can punch through the final (destroyed) object.
            if (IsDestroyed)
                DestructionManager.Instance.ProcessDestruction(this, destroyedPrefab, new DirectDamage{DamageAmount = amount}, isObliterated);
        }

        public void ApplyDamage(Damage damage)
        {
            if (IsDestroyed || IsInvulnerable) return; // don't try to apply damage to an already-destroyed or invulnerable object.

            LastDamagedAmount = damage.DamageAmount > currentHitPoints ? currentHitPoints : damage.DamageAmount;
            FireDamagedEvent();

            currentHitPoints -= damage.DamageAmount;
            CheckForObliterate(damage.DamageAmount);
            if (currentHitPoints > 0) return;
            if (currentHitPoints < 0) currentHitPoints = 0;

            PlayDamageEffects();

            if (IsDestroyed)
                DestructionManager.Instance.ProcessDestruction(this, destroyedPrefab, damage, isObliterated);
        }

        public void RepairDamage(int amount) 
        {
            if (IsDestroyed) return; // cannot repair an already-destroyed object.
            if (!canBeRepaired) return; // object cannot be repaired.

            if (amount > (totalHitPoints - currentHitPoints))
                LastRepairedAmount = totalHitPoints - currentHitPoints;
            else
                LastRepairedAmount = amount;

            currentHitPoints += amount;
            if (currentHitPoints > totalHitPoints) // don't allow object to be over-repaired beyond its total hit points.
                currentHitPoints = totalHitPoints;

            PlayDamageEffects();
            FireRepairedEvent();
        }

        /// <summary>Check to see if the destructible object has been obliterated from taking excessive damage. If so, set the ObliteratedLevel on the object.</summary>
        /// <param name="damage">The amount of damage applied to the object from a single source.</param>
        private void CheckForObliterate(int damage)
        {
            if (IsInvulnerable || !canBeDestroyed || !canBeObliterated) return;

            if (damage >= (DestructionManager.Instance.obliterateMultiplier * totalHitPoints))
                isObliterated = true;
        }

        /// <summary>Advances the damage state, applies damage-level materials as needed, and plays particle effects.</summary>
        private void SetDamageLevel()
        {
            if (damageLevels == null) return;
            DamageLevel damageLevel = damageLevels.GetDamageLevel(currentHitPoints);
            if (damageLevel == null) return;
            if (currentDamageLevel != null && damageLevel == currentDamageLevel) return;

            currentDamageLevel = damageLevel;
            Renderer[] renderers = GetComponentsInChildren<Renderer>();

            foreach (Renderer rend in renderers)
            {
                Destructible parentDestructible = rend.GetComponentInParent<Destructible>(); // child Destructible objects should not be affected by damage on their parents.
                if (parentDestructible != this) continue;
                bool isAcceptableRenderer = rend is MeshRenderer || rend is SkinnedMeshRenderer;
                if (isAcceptableRenderer && !rend.gameObject.HasTag(Tag.ClingingDebris) && rend.gameObject.layer != DestructionManager.Instance.debrisLayer)
                {
                    Material[] replacementMats = new Material[rend.sharedMaterials.Length];
                    for (int j = 0; j < rend.sharedMaterials.Length; j++)
                        replacementMats[j] = MaterialPreloader.Instance.GetDamagedMaterial(rend.sharedMaterials[j], currentDamageLevel);

                    rend.sharedMaterials = replacementMats;
                }
            }

            PlayDamageEffects();
        }

        private void PlayDamageEffects()
        {
            // Check if we should play a particle effect for this damage level
            if (damageEffects == null || damageEffects.Count == 0) return;

            int currentDamageLevelIndex = 0;
            if (currentDamageLevel != null)
                currentDamageLevelIndex = damageLevels.IndexOf(currentDamageLevel); // FindIndex(a => a == currentDamageLevel);
            
            foreach (DamageEffect effect in damageEffects)
            {
                if (effect == null || effect.Prefab == null) continue;

                // get rotation
                Quaternion rotation = transform.rotation;
                if (effect.Rotation != Vector3.zero)
                    rotation = transform.rotation * Quaternion.Euler(effect.Rotation);

                // Is this effect only played if the destructible object has a certain tag?
                if (effect.HasTagDependency && !gameObject.HasTag(effect.TagDependency))
                    continue;

                if (currentDamageLevel != null && effect.TriggeredAt < damageLevels.Count)
                {
                    // TURN ON pre-destruction damage effects
                    if (currentDamageLevelIndex >= effect.TriggeredAt && !effect.HasStarted)
                    {
                        if (effect.GameObject != null)
                        {
                            for (int i = 0; i < effect.ParticleSystems.Length; i++)
                            {
                                ParticleSystem.EmissionModule emission = effect.ParticleSystems[i].emission;
                                emission.enabled = true;
                            }
                        }
                        else
                        {
                            // set parent to this destructible object and play
                            effect.GameObject = ObjectPool.Instance.Spawn(effect.Prefab, effect.Offset, rotation, transform);

                            if (effect.GameObject != null)
                                effect.ParticleSystems = effect.GameObject.GetComponentsInChildren<ParticleSystem>();
                        }

                        effect.HasStarted = true;
                    }

                    // TURN OFF pre-destruction damage effects
                    if (currentDamageLevelIndex < effect.TriggeredAt && effect.HasStarted)
                    {
                        if (effect.GameObject != null)
                        {
                            for (int i = 0; i < effect.ParticleSystems.Length; i++)
                            {
                                ParticleSystem.EmissionModule emission = effect.ParticleSystems[i].emission;
                                emission.enabled = false;
                            }
                        }

                        effect.HasStarted = false;
                    }
                }

                // Destroyed effects
                if (effect.TriggeredAt == damageLevels.Count && IsDestroyed && !effect.HasStarted)
                {
                    if (canBeDestroyed)
                        effect.GameObject = ObjectPool.Instance.Spawn(effect.Prefab, transform.TransformPoint(effect.Offset), rotation);
                    else
                        effect.GameObject = ObjectPool.Instance.Spawn(effect.Prefab, effect.Offset, rotation, transform);

                    if (effect.GameObject != null)
                        effect.ParticleSystems = effect.GameObject.GetComponentsInChildren<ParticleSystem>();

                    effect.HasStarted = true;
                }
            }
        }

        // NOTE: OnCollisionEnter will only fire if a rigidbody is attached to this object!
        public void OnCollisionEnter(Collision collision)
        {
            if (DestructionManager.Instance == null) return;

            this.ProcessDestructibleCollision(collision, GetComponent<Rigidbody>());
            
            if (collision.contacts.Length <= 0) return;
            
            Destructible destructibleObj = collision.contacts[0].otherCollider.gameObject.GetComponentInParent<Destructible>();
            if (destructibleObj != null && collision.contacts[0].otherCollider.attachedRigidbody == null)
                destructibleObj.ProcessDestructibleCollision(collision, GetComponent<Rigidbody>());
        }

        public void OnDrawGizmos()
        {
            if (damageEffects != null)
            {
                foreach (DamageEffect effect in damageEffects)
                {
                    if (effect == null) continue;
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawWireCube(transform.TransformPoint(effect.Offset), new Vector3(0.1f, 0.1f, 0.1f));
                    Quaternion rotatedVector = transform.rotation * Quaternion.Euler(effect.Rotation);
                    Gizmos.DrawRay(transform.TransformPoint(effect.Offset), rotatedVector * Vector3.forward * .5f);
                }
            }

            if (centerPointOverride != Vector3.zero)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(transform.TransformPoint(centerPointOverride), 0.1f);
            }
        }

        public void FireDestroyedEvent()
        {
            if (DestroyedEvent != null) // first, make sure there is at least one listener.
                DestroyedEvent(); // if so, trigger the event.
        }

        public void FireRepairedEvent()
        {
            if (RepairedEvent != null) // first, make sure there is at least one listener.
                RepairedEvent(); // if so, trigger the event.
        }

        public void FireDamagedEvent()
        {
            if (DamagedEvent != null) // first, make sure there is at least one listener.
                DamagedEvent(); // if so, trigger the event.
        }
    }
}