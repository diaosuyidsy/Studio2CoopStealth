using DestroyItReady;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DestroyIt
{
    public static class StubExtensions
    {
        public static void ToStubs(this IList<Destructible> destObjects)
        {
            if (destObjects == null || destObjects.Count == 0) return;

            foreach (Destructible destObj in destObjects)
            {
                if (destObj == null) continue;

                // Remove any existing DestructibleStub scripts.
                DestructibleStub[] existingStubs = destObj.gameObject.GetComponents<DestructibleStub>();
                if (existingStubs != null)
                {
                    for (int i=0; i<existingStubs.Length; i++)
                        Object.DestroyImmediate(existingStubs[i], true);
                }

                DestructibleStub stub = destObj.gameObject.AddComponent<DestructibleStub>();

                // Assign values from the Destructible script to the stub.
                stub.totalHitPoints = destObj.totalHitPoints;
                stub.currentHitPoints = destObj.currentHitPoints;
                stub.damageLevels = destObj.damageLevels.ToDestroyItReady();
                stub.velocityReduction = destObj.velocityReduction;
                stub.ignoreCollisionsUnder = destObj.ignoreCollisionsUnder;
                stub.destroyedPrefab = destObj.destroyedPrefab;
                stub.destroyedPrefabParent = destObj.destroyedPrefabParent;
                stub.fallbackParticle = destObj.fallbackParticle;
                stub.useFallbackParticle = destObj.useFallbackParticle;
                stub.fallbackParticleMaterial = destObj.fallbackParticleMaterial;
                stub.damageEffects = destObj.damageEffects.ToDestroyItReady();
                stub.unparentOnDestroy = destObj.unparentOnDestroy;
                stub.disableKinematicOnUparentedChildren = destObj.disableKinematicOnUparentedChildren;
                stub.replaceMaterials = destObj.replaceMaterials.ToDestroyItReady();
                stub.canBeDestroyed = destObj.canBeDestroyed;
                stub.canBeRepaired = destObj.canBeRepaired;
                stub.canBeObliterated = destObj.canBeObliterated;
                stub.debrisToReParentByName = destObj.debrisToReParentByName;
                stub.debrisToReParentIsKinematic = destObj.debrisToReParentIsKinematic;
                stub.childrenToReParentByName = destObj.childrenToReParentByName;
                stub.destructibleGroupId = destObj.destructibleGroupId;
                stub.isDebrisChipAway = destObj.isDebrisChipAway;
                stub.chipAwayDebrisAngularDrag = destObj.chipAwayDebrisAngularDrag;
                stub.chipAwayDebrisDrag = destObj.chipAwayDebrisDrag;
                stub.chipAwayDebrisMass = destObj.chipAwayDebrisMass;
                stub.autoPoolDestroyedPrefab = destObj.autoPoolDestroyedPrefab;
                stub.centerPointOverride = destObj.centerPointOverride;
                stub.sinkWhenDestroyed = destObj.sinkWhenDestroyed;

                Object.DestroyImmediate(destObj, true);
            }
        }

        public static void ToStubs(this IList<TagIt> tagItObjects)
        {
            if (tagItObjects == null || tagItObjects.Count == 0) return;

            foreach (TagIt tagIt in tagItObjects)
            {
                if (tagIt == null) continue;

                // Remove any existing TagItStub scripts.
                TagItStub[] existingStubs = tagIt.gameObject.GetComponents<TagItStub>();
                if (existingStubs != null)
                {
                    for (int i = 0; i < existingStubs.Length; i++)
                        Object.DestroyImmediate(existingStubs[i], true);
                }

                TagItStub stub = tagIt.gameObject.AddComponent<TagItStub>();

                // Assign values from the TagIt script to the stub.
                stub.tags = tagIt.tags.Select(x => (DestroyItReady.Tag)x).ToList();

                Object.DestroyImmediate(tagIt, true);
            }
        }

        public static void ToStubs(this IList<HitEffects> objs)
        {
            if (objs == null || objs.Count == 0) return;

            foreach (HitEffects obj in objs)
            {
                if (obj == null) continue;

                // Remove any existing HitEffects scripts.
                HitEffectsStub[] existingStubs = obj.gameObject.GetComponents<HitEffectsStub>();
                if (existingStubs != null)
                {
                    for (int i = 0; i < existingStubs.Length; i++)
                        Object.DestroyImmediate(existingStubs[i], true);
                }

                HitEffectsStub stub = obj.gameObject.AddComponent<HitEffectsStub>();

                // Assign values from the HitEffects script to the stub.
                stub.effects = obj.effects.ToDestroyItReady();

                Object.DestroyImmediate(obj, true);
            }
        }

        public static void ToHitEffects(this IList<HitEffectsStub> stubs)
        {
            if (stubs == null || stubs.Count == 0) return;

            foreach (HitEffectsStub stub in stubs)
            {
                if (stub == null) continue;

                // Remove any existing HitEffects scripts.
                HitEffects[] existingHitEffects = stub.gameObject.GetComponents<HitEffects>();
                if (existingHitEffects != null)
                {
                    for (int i = 0; i < existingHitEffects.Length; i++)
                        Object.DestroyImmediate(existingHitEffects[i], true);
                }

                HitEffects hitEffects = stub.gameObject.AddComponent<HitEffects>();

                // Assign values from the HitEffects script to the stub.
                hitEffects.effects = stub.effects.ToHitEffects();

                Object.DestroyImmediate(stub, true);
            }
        }

        public static void ToTagIt(this IList<TagItStub> stubs)
        {
            if (stubs == null || stubs.Count == 0) return;

            foreach (TagItStub stub in stubs)
            {
                if (stub == null) continue;

                // Remove any existing TagIt scripts.
                TagIt[] existingTagIts = stub.gameObject.GetComponents<TagIt>();
                if (existingTagIts != null)
                {
                    for (int i = 0; i < existingTagIts.Length; i++)
                        Object.DestroyImmediate(existingTagIts[i], true);
                }

                TagIt tagIt = stub.gameObject.AddComponent<TagIt>();

                // Assign values from the TagIt script to the stub.
                tagIt.tags = stub.tags.Select(x => (Tag)x).ToList();

                Object.DestroyImmediate(stub, true);
            }
        }

        /// <summary>For a collection of objects, converts all DestructibleStub scripts to Destructible scripts.</summary>
        public static void ToDestructible(this IList<DestructibleStub> stubs)
        {
            if (stubs == null || stubs.Count == 0) return;

            foreach (DestructibleStub stub in stubs)
            {
                if (stub == null) continue;

                // Remove any existing Destructible scripts.
                Destructible[] existingDestructibles = stub.gameObject.GetComponents<Destructible>();
                if (existingDestructibles != null)
                {
                    for (int i = 0; i < existingDestructibles.Length; i++)
                        Object.DestroyImmediate(existingDestructibles[i], true);
                }

                Destructible dest = stub.gameObject.AddComponent<Destructible>();

                // Assign values from the Destructible script to the stub.
                dest.totalHitPoints = stub.totalHitPoints;
                dest.currentHitPoints = stub.currentHitPoints;
                dest.damageLevels = stub.damageLevels.ToDestructible();
                dest.velocityReduction = stub.velocityReduction;
                dest.ignoreCollisionsUnder = stub.ignoreCollisionsUnder;
                dest.destroyedPrefab = stub.destroyedPrefab;
                dest.destroyedPrefabParent = stub.destroyedPrefabParent;
                dest.fallbackParticle = stub.fallbackParticle;
                dest.useFallbackParticle = stub.useFallbackParticle;
                dest.fallbackParticleMaterial = stub.fallbackParticleMaterial;
                dest.damageEffects = stub.damageEffects.ToDestructible();
                dest.unparentOnDestroy = stub.unparentOnDestroy;
                dest.disableKinematicOnUparentedChildren = stub.disableKinematicOnUparentedChildren;
                dest.replaceMaterials = stub.replaceMaterials.ToDestructible();
                dest.canBeDestroyed = stub.canBeDestroyed;
                dest.canBeRepaired = stub.canBeRepaired;
                dest.canBeObliterated = stub.canBeObliterated;
                dest.debrisToReParentByName = stub.debrisToReParentByName;
                dest.debrisToReParentIsKinematic = stub.debrisToReParentIsKinematic;
                dest.childrenToReParentByName = stub.childrenToReParentByName;
                dest.destructibleGroupId = stub.destructibleGroupId;
                dest.isDebrisChipAway = stub.isDebrisChipAway;
                dest.chipAwayDebrisAngularDrag = stub.chipAwayDebrisAngularDrag;
                dest.chipAwayDebrisDrag = stub.chipAwayDebrisDrag;
                dest.chipAwayDebrisMass = stub.chipAwayDebrisMass;
                dest.autoPoolDestroyedPrefab = stub.autoPoolDestroyedPrefab;
                dest.centerPointOverride = stub.centerPointOverride;
                dest.sinkWhenDestroyed = stub.sinkWhenDestroyed;

                Object.DestroyImmediate(stub, true);
            }
        }
    }
}
