using UnityEngine;

namespace DestroyIt
{
    /// <summary>
    /// This script applies damage to all destructible objects in the scene every time you press the "0" key.
    /// This script is for testing purposes.
    /// </summary>
    public class DestructionTest : MonoBehaviour
    {
        public int damagePerPress = 13; // The amount of damage to apply to all destructible objects per keypress.

        void Update()
        {
            if (Input.GetKeyUp("0"))
            {
                Destructible[] destObjs = FindObjectsOfType<Destructible>();
                foreach (Destructible destObj in destObjs)
                    destObj.ApplyDamage(damagePerPress);
            }
        }
    }
}