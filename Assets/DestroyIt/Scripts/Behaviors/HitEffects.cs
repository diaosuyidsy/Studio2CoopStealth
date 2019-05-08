using System.Collections.Generic;
using UnityEngine;

namespace DestroyIt
{
    /// <summary>
    /// This script allows you to assign particle system hit effects for each weapon type.
    /// PlayEffect() will attempt to play the hit effect for the specified weapon type.
    /// </summary>
    [DisallowMultipleComponent]
    public class HitEffects : MonoBehaviour
    {
        public List<HitEffect> effects;

        public void PlayEffect(HitBy weaponType, Vector3 hitPoint, Vector3 hitNormal)
        {
            GameObject effect = null;
            for (int i=0; i<effects.Count; i++)
            {
                if ((effects[i].hitBy & weaponType) > 0)
                {
                    effect = effects[i].effect;
                    break;
                }
            }
            
            if (effect != null)
                ObjectPool.Instance.Spawn(effect, hitPoint, Quaternion.LookRotation(hitNormal));
        }
    }
}
