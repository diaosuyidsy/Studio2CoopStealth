using UnityEngine;

namespace DestroyIt
{
    /// <summary>
    /// This script is used in conjunction with Object Pooling to enhance performance, in particular with mobile devices.
    /// This script enables gravity on a game object's rigidbody over a small amount of time, so the physics load on the CPU is spaced out.
    /// </summary>
    public class EnableGravityAfter : MonoBehaviour
    {
        public float seconds;   // seconds to wait before enabling rigidbody gravity on this game object.
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

            if (GetComponent<Rigidbody>() == null)
            {
                Destroy(this);
                return;
            }

            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0)
            {
                GetComponent<Rigidbody>().useGravity = true;
                Destroy(this);
            }
        }
    }
}