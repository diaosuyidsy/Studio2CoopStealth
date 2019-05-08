using System.Collections.Generic;

namespace DestroyIt
{
    public static class DestroyItReadyExtensions
    {
        public static List<DestroyItReady.DamageEffect> ToDestroyItReady(this List<DamageEffect> damageEffects)
        {
            if (damageEffects == null || damageEffects.Count <= 0) return null;

            var retVal = new List<DestroyItReady.DamageEffect>();
            foreach (var source in damageEffects)
            {
                var dest = new DestroyItReady.DamageEffect
                {
                    TriggeredAt = source.TriggeredAt,
                    Offset = source.Offset,
                    Rotation = source.Rotation,
                    Effect = source.Prefab,
                    HasStarted = source.HasStarted,
                    UseDependency = source.HasTagDependency,
                    TagDependency = (DestroyItReady.Tag) source.TagDependency
                };
                retVal.Add(dest);
            }
            return retVal;
        }

        public static List<DestroyItReady.MaterialMapping> ToDestroyItReady(this List<MaterialMapping> replaceMaterials)
        {
            if (replaceMaterials == null || replaceMaterials.Count <= 0) return null;

            var retVal = new List<DestroyItReady.MaterialMapping>();
            foreach (var source in replaceMaterials)
            {
                var dest = new DestroyItReady.MaterialMapping()
                {
                    SourceMaterial = source.SourceMaterial,
                    ReplacementMaterial = source.ReplacementMaterial
                };
                retVal.Add(dest);
            }
            return retVal;
        }

        public static List<DamageEffect> ToDestructible(this List<DestroyItReady.DamageEffect> damageEffects)
        {
            if (damageEffects == null || damageEffects.Count <= 0) return null;

            var retVal = new List<DamageEffect>();
            foreach (var source in damageEffects)
            {
                var dest = new DamageEffect
                {
                    TriggeredAt = source.TriggeredAt,
                    Offset = source.Offset,
                    Rotation = source.Rotation,
                    Prefab = source.Effect,
                    HasStarted = source.HasStarted,
                    HasTagDependency = source.UseDependency,
                    TagDependency = (DestroyIt.Tag)source.TagDependency
                };
                retVal.Add(dest);
            }
            return retVal;
        }

        public static List<MaterialMapping> ToDestructible(this List<DestroyItReady.MaterialMapping> replaceMaterials)
        {
            if (replaceMaterials == null || replaceMaterials.Count <= 0) return null;

            var retVal = new List<MaterialMapping>();
            foreach (var source in replaceMaterials)
            {
                var dest = new MaterialMapping()
                {
                    SourceMaterial = source.SourceMaterial,
                    ReplacementMaterial = source.ReplacementMaterial
                };
                retVal.Add(dest);
            }
            return retVal;
        }

        public static List<DestroyItReady.DamageLevel> ToDestroyItReady(this List<DamageLevel> damageLevels)
        {
            if (damageLevels == null || damageLevels.Count <= 0) return null;

            var retVal = new List<DestroyItReady.DamageLevel>();
            foreach (var source in damageLevels)
            {
                var dest = new DestroyItReady.DamageLevel
                {
                    maxHitPoints = source.maxHitPoints,
                    minHitPoints = source.minHitPoints,
                    healthPercent = source.healthPercent,
                    hasError = source.hasError,
                    visibleDamageLevel = source.visibleDamageLevel
                };
                retVal.Add(dest);
            }
            return retVal;
        }

        public static List<DamageLevel> ToDestructible(this List<DestroyItReady.DamageLevel> damageLevels)
        {
            if (damageLevels == null || damageLevels.Count <= 0) return null;

            var retVal = new List<DamageLevel>();
            foreach (var source in damageLevels)
            {
                var dest = new DamageLevel
                {
                    maxHitPoints = source.maxHitPoints,
                    minHitPoints = source.minHitPoints,
                    healthPercent = source.healthPercent,
                    hasError = source.hasError,
                    visibleDamageLevel = source.visibleDamageLevel
                };
                retVal.Add(dest);
            }
            return retVal;
        }

        public static List<DestroyItReady.HitEffect> ToDestroyItReady(this List<HitEffect> hitEffects)
        {
            if (hitEffects == null || hitEffects.Count <= 0) return null;

            var retVal = new List<DestroyItReady.HitEffect>();
            foreach (var source in hitEffects)
            {
                var dest = new DestroyItReady.HitEffect()
                {
                    hitBy = (DestroyItReady.HitBy)source.hitBy,
                    effect = source.effect
                };
                retVal.Add(dest);
            }
            return retVal;
        }

        public static List<HitEffect> ToHitEffects(this List<DestroyItReady.HitEffect> hitEffects)
        {
            if (hitEffects == null || hitEffects.Count <= 0) return null;

            var retVal = new List<HitEffect>();
            foreach (var source in hitEffects)
            {
                var dest = new HitEffect()
                {
                    hitBy = (HitBy)source.hitBy,
                    effect = source.effect
                };
                retVal.Add(dest);
            }
            return retVal;
        }
    }
}
