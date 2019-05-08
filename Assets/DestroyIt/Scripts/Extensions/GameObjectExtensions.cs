using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace DestroyIt
{
    public static class GameObjectExtensions
    {
        /// <summary>Removes all components of type T from the game object.</summary>
        public static void RemoveAll<T>(this GameObject obj) where T : Component
        {
            foreach (Component comp in obj.GetComponents<T>())
                MonoBehaviour.Destroy(comp);
        }

        /// <summary>Removes all components of type T from the game object and its children.</summary>
        public static void RemoveAllFromChildren<T>(this GameObject obj) where T : Component
        {
            foreach (Component comp in obj.GetComponentsInChildren<T>())
                MonoBehaviour.Destroy(comp);
        }

        /// <summary>Removes all components of type T from the game object and its children.</summary>
        public static void RemoveAllFromChildrenImmediately<T>(this GameObject obj) where T : Component
        {
            foreach (Component comp in obj.GetComponentsInChildren<T>())
                MonoBehaviour.DestroyImmediate(comp, true);
        }

        public static void RemoveComponent<T>(this GameObject obj) where T : Component
        {
            T component = obj.GetComponent<T>();

            if (component != null)
                Object.Destroy(component);
        }

        public static void TagAllChildren(this GameObject obj, string tag)
        {
            Transform[] gameObjects = obj.GetComponentsInChildren<Transform>();
            foreach (Transform trans in gameObjects)
                trans.tag = tag;
        }

        public static List<T> GetComponentsInChildrenOnly<T>(this GameObject obj) where T : Component
        {
            return GetComponentsInChildrenOnly<T>(obj, false);
        }
        public static List<T> GetComponentsInChildrenOnly<T>(this GameObject obj, bool includeInactive) where T : Component
        {
            var components = obj.GetComponentsInChildren<T>(includeInactive).ToList();
            components.Remove(obj.GetComponent<T>());
            return components;
        }

        public static T[] GetComponentsInImmediateChildren<T>(this GameObject obj) where T : Component
        {
            List<T> componentList = new List<T>();
            foreach (Transform child in obj.transform)
            {
                T component = child.GetComponent<T>();
                if (component != null)
                    componentList.Add(component);
            }
            return componentList.ToArray();
        }

        /// <summary>
        /// Be sure to set SolverIterationCount to around 25-30 in your Project Settings in order to get solid joints.
        /// </summary>
        public static void AddStiffJoint(this GameObject go, Rigidbody connectedBody, Vector3 anchorPosition, Vector3 axis, float breakForce, float breakTorque)
        {
            FixedJoint joint = go.AddComponent<FixedJoint>();
            joint.anchor = anchorPosition;
            joint.connectedBody = connectedBody;
            joint.breakForce = breakForce;
            joint.breakTorque = breakTorque;
        }

        public static bool GetComponents<TOne, TTwo, TThree>(this IEnumerable<GameObject> inputObjs, out List<TOne> compListOne, out List<TTwo> compListTwo, out List<TThree> compListThree)
        {
            bool anyFound = false;
            compListOne = new List<TOne>();
            compListTwo = new List<TTwo>();
            compListThree = new List<TThree>();

            foreach (GameObject obj in inputObjs)
            {
                List<TOne> objectsOne = obj.GetComponentsInChildren<TOne>(true).ToList();
                List<TTwo> objectsTwo = obj.GetComponentsInChildren<TTwo>(true).ToList();
                List<TThree> objectsThree = obj.GetComponentsInChildren<TThree>(true).ToList();
                if (objectsOne.Count == 0 && objectsTwo.Count == 0 && objectsThree.Count == 0) continue;

                anyFound = true;
                if (objectsOne.Count > 0)
                    compListOne.AddRange(objectsOne);
                if (objectsTwo.Count > 0)
                    compListTwo.AddRange(objectsTwo);
                if (objectsThree.Count > 0)
                    compListThree.AddRange(objectsThree);
            }
            return anyFound;
        }
    }
}