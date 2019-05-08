using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace DestroyIt
{
    [CustomEditor(typeof(DestructionManager))]
    public class DestructionManagerEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            DestructionManager destManager = target as DestructionManager;

            destManager.useCameraDistanceLimit = EditorGUILayout.Toggle("Camera Distance Limit", destManager.useCameraDistanceLimit);
            if (destManager.useCameraDistanceLimit)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("", GUILayout.Width(15));
                destManager.cameraDistanceLimit = EditorGUILayout.IntField("If distance to camera >", destManager.cameraDistanceLimit);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("", GUILayout.Width(15));
                EditorGUILayout.LabelField("Limit destruction to", GUILayout.Width(100));
                EditorGUILayout.EnumPopup(DestructionType.ParticleEffect);
                GUILayout.EndHorizontal();
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(destManager);
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
        }
    }
}