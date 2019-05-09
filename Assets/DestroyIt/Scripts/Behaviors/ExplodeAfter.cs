using UnityEngine;

namespace DestroyIt
{
    /// <summary>This script will intantiate the specified explosion prefab after [seconds].</summary>
    public class ExplodeAfter : MonoBehaviour
    {
        [Tooltip("Prefab to instantiate when time runs out.")]
        public GameObject explosionPrefab;
        [Tooltip("Seconds to wait before explosion.")]
        public float seconds = 5f;

        private float timeLeft;
        private bool isInitialized;

        void Start()
        {
            timeLeft = seconds;
            isInitialized = true;
        }

        void OnEnable()
        {
            timeLeft = seconds;
        }

        void Update()
        {
            if (!isInitialized) return;

            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0)
            {
                Instantiate(explosionPrefab, this.transform.position, Quaternion.identity);
                Destroy(this.gameObject);
            }
        }
    }
}