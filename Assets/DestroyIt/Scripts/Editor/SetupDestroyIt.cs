using DestroyIt;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SetupDestroyIt
{
    [MenuItem("Window/DestroyIt/Setup - Minimal")]
    private static void NewMenuOption()
    {
        GameObject destroyIt = new GameObject("DestroyIt");
        destroyIt.AddComponent<MaterialPreloader>();
        destroyIt.AddComponent<DestructionManager>();
        destroyIt.AddComponent<ParticleManager>();
        ObjectPool pool = destroyIt.AddComponent<ObjectPool>();

        GameObject destroyItTest = new GameObject("DestroyIt-InputTest");
        destroyItTest.AddComponent<DestructionTest>();

        GameObject defaultLargeParticle = Resources.Load<GameObject>("Default_Particles/DefaultLargeParticle");
        GameObject defaultSmallParticle = Resources.Load<GameObject>("Default_Particles/DefaultSmallParticle");
        pool.prefabsToPool = new List<PoolEntry>();
        pool.prefabsToPool.Add(new PoolEntry(){Count=10, Prefab=defaultLargeParticle});
        pool.prefabsToPool.Add(new PoolEntry(){Count=10, Prefab=defaultSmallParticle});
    }
}
