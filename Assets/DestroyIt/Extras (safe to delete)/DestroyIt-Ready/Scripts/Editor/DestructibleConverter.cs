using DestroyIt;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DestroyItReady
{
    public class DestructibleConverter
    {
        [MenuItem("Window/DestroyIt/Convert Destructibles to Stubs")]
        public static bool ConvertDestructiblesToStubs()
        {
            List<GameObject> gameObjs = GetSelectedGameObjects();
            if (gameObjs.Count == 0)
                return EditorUtility.DisplayDialog("Nothing Selected", "You must select one or more objects or prefabs to continue.", "Ok");

            List<Destructible> allDestObjects;
            List<TagIt> allTagItObjects;
            List<HitEffects> allHitEffectsObjects;
            if (!gameObjs.GetComponents(out allDestObjects, out allTagItObjects, out allHitEffectsObjects))
                return EditorUtility.DisplayDialog("No Scripts Found", "No Destructible, TagIt, or HitEffects scripts found on selected objects.", "Ok");

            // Confirm the action.
            if (!EditorUtility.DisplayDialog("Convert Functional Scripts to Stubs?",
                String.Format("Are you sure you want to replace the following functional scripts with stubs?\n\nDestructible: {0} scripts\nTagIt: {1} scripts\nHitEffects: {2} scripts",
                allDestObjects.Count, allTagItObjects.Count, allHitEffectsObjects.Count), "Replace", "Do Not Replace"))
                return false;

            // Do the work.
            allDestObjects.ToStubs();
            allTagItObjects.ToStubs();
            allHitEffectsObjects.ToStubs();

            // Save the work.
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            AssetDatabase.SaveAssets();
            return true;
        }

        [MenuItem("Window/DestroyIt/Convert Stubs to Destructibles")]
        public static bool ConvertStubsToDestructibles()
        {
            List<GameObject> gameObjs = GetSelectedGameObjects();
            if (gameObjs.Count == 0)
                return EditorUtility.DisplayDialog("Nothing Selected", "You must select one or more objects or prefabs to continue.", "Ok");

            List<DestructibleStub> allDestStubs;
            List<TagItStub> allTagItStubs;
            List<HitEffectsStub> allHitEffectsStubs;
            if (!gameObjs.GetComponents(out allDestStubs, out allTagItStubs, out allHitEffectsStubs))
                return EditorUtility.DisplayDialog("No Scripts Found", "No DestructibleStub, TagItStub, or HitEffectsStub scripts found on selected objects.", "Ok");

            // Confirm the action.
            if (!EditorUtility.DisplayDialog("Convert Stubs to Functional Scripts?",
                String.Format("Are you sure you want to replace the following stubs with functional scripts?\n\nDestructibleStub: {0} scripts\nTagItStub: {1} scripts\nHitEffectsStub: {2} scripts",
                allDestStubs.Count, allTagItStubs.Count, allHitEffectsStubs.Count), "Replace", "Do Not Replace"))
                return false;

            // Do the work.
            allDestStubs.ToDestructible();
            allTagItStubs.ToTagIt();
            allHitEffectsStubs.ToHitEffects();

            // Save the work.
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            AssetDatabase.SaveAssets();
            return true;
        }

        /// <summary>Gets all selected game objects, whether they are in the Hierarchy panel or prefabs in the Project panel.</summary>
        private static List<GameObject> GetSelectedGameObjects()
        {
            var gameObjs = new List<GameObject>();

            // First, get all the assets selected from the Project tab.
            foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
            {
                // Check if the selected object is a GameObject prefab. If so, put it in the collection.
                GameObject selectedPrefab = obj as GameObject;
                if (selectedPrefab != null && PrefabUtility.GetPrefabType(selectedPrefab) == PrefabType.Prefab && !gameObjs.Contains(selectedPrefab))
                {
                    gameObjs.Add(obj as GameObject);
                    continue;
                }

                // Not a GameObject Prefab, assume it's a folder. 
                string path = AssetDatabase.GetAssetPath(obj);
                gameObjs.AddFromFolder(path);
            }

            // Next, get all the game objects selected from the Hierarchy tab.
            foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.ExcludePrefab))
            {
                GameObject selectedGameObj = obj as GameObject;
                // Try to get the object's prefab
                GameObject prefabObj = PrefabUtility.GetPrefabParent(selectedGameObj) as GameObject;
                if (prefabObj != null && gameObjs.Contains(prefabObj))
                    continue;

                if (selectedGameObj != null && !gameObjs.Contains(selectedGameObj))
                    gameObjs.Add(obj as GameObject);
            }

            return gameObjs;
        }
    }

    public static class GameObjectExtensions
    {
        public static void AddFromFolder(this List<GameObject> gameObjects, string folderPath)
        {
            if (folderPath.Length <= 0) return;
            if (!Directory.Exists(folderPath)) return;

            string[] fileNames = Directory.GetFiles(folderPath);
            string[] folderNames = Directory.GetDirectories(folderPath);
            for (int i = 0; i < fileNames.Length; i++)
            {
                fileNames[i] = fileNames[i].Replace("\\", "/"); // GetFiles() returns the wrong slash before FileName...
                GameObject nestedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(fileNames[i]);
                if (nestedPrefab != null && PrefabUtility.GetPrefabType(nestedPrefab) == PrefabType.Prefab && !gameObjects.Contains(nestedPrefab))
                    gameObjects.Add(nestedPrefab);
            }
            for (int i = 0; i < folderNames.Length; i++)
            {
                folderNames[i] = folderNames[i].Replace("\\", "/"); // GetDirectories() returns the wrong slash before FolderName...
                gameObjects.AddFromFolder(folderNames[i]);
            }
        }
    }
}
