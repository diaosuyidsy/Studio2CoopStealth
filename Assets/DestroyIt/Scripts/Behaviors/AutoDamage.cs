using UnityEngine;

namespace DestroyIt
{
    public class AutoDamage : MonoBehaviour
    {
        public int startAtHitPoints = 30;
        public float damageIntervalSeconds = 0.5f;
        public int damagePerInterval = 5;

        private bool isInitialized;
        private Destructible destructible;
        private bool autoDamageStarted;

        void Start()
        {
            destructible = gameObject.GetComponent<Destructible>();
            if (destructible == null)
            {
                Debug.LogWarning("No Destructible object found! AutoDamage removed.");
                Destroy(this);
            }
            isInitialized = true;
        }

        void Update()
        {
            if (!isInitialized) return;
            if (destructible == null) return;
            if (autoDamageStarted) return;

            if (destructible.currentHitPoints <= startAtHitPoints)
            {
                InvokeRepeating("ApplyDamage", 0f, damageIntervalSeconds);
                autoDamageStarted = true;
            }
        }

        void ApplyDamage()
        {
            if (destructible == null) return;

            destructible.ApplyDamage(damagePerInterval);
        }
    }
}
