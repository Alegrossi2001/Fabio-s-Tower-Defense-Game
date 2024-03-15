namespace Dypsloom.RhythmTimeline.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEditor.UIElements;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UIElements;

    public static class EditorHelper
    {

        //Used to get the subclasses of ItemDefinition
        public static List<Type> Subclasses(Type baseClass, bool includeBase, bool includeGeneric = false)
        {
            List<Type> subclasses = new List<Type>();
            if (includeBase) {
                subclasses.Add(baseClass);
            }
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach (var type in asm.GetTypes()) {
                    if (type.IsSubclassOf(baseClass) && (!type.IsGenericType || includeGeneric)) {
                        subclasses.Add(type);
                    }
                }
            }
            return subclasses;
        }

        /// <summary>
        /// Flags the object as dirty.
        /// </summary>
        /// <param name="obj">The object that was changed.</param>
        public static void SetDirty(UnityEngine.Object obj)
        {
            if (obj == null || Application.isPlaying) {
                return;
            }

            PrefabUtility.RecordPrefabInstancePropertyModifications(obj);
            if (obj is Component) {
                EditorSceneManager.MarkSceneDirty((obj as Component).gameObject.scene);
            } else if (obj is GameObject) {
                EditorSceneManager.MarkSceneDirty((obj as GameObject).scene);
            } else if (!EditorUtility.IsPersistent(obj)) {
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            } else {
                EditorUtility.SetDirty(obj);
            }
        }

        /// <summary>
        /// To be used in OnGUI PropertyDrawers to display a message when UIElements should be used instead of IMGUI
        /// Note the Property drawer should also set the 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public static void OnGUIUseUIelementsMessage(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var message = new GUIContent("This property must be drawn with UIElements.\nPlease check the documentation to learn how");

            EditorGUI.LabelField(position, message);

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        /// <summary>
        /// Creates a VisualElement that draws all the properties of a serializedObject
        /// </summary>
        /// <param name="target"></param>
        /// <param name="propertiesToExclude"></param>
        /// <returns></returns>
        public static VisualElement CreateUIElementInspector(UnityEngine.Object target, List<string> propertiesToExclude)
        {
            var container = new VisualElement();

            var serializedObject = new SerializedObject(target);

            var fields = GetVisibleSerializedFields(target.GetType());

            for (int i = 0; i < fields.Length; ++i) {
                var field = fields[i];
                // Do not draw HideInInspector fields or excluded properties.
                if ( propertiesToExclude != null && propertiesToExclude.Contains(field.Name)) {
                    continue;
                }
                
                var serializedProperty = serializedObject.FindProperty(field.Name);

                var propertyField = new PropertyField(serializedProperty);

                container.Add(propertyField);
            }
            
            container.Bind(serializedObject);


            return container;
        }

        public static FieldInfo[] GetVisibleSerializedFields(Type T)
        {
            List<FieldInfo> infoFields = new List<FieldInfo>();

            var publicFields = T.GetFields(BindingFlags.Instance | BindingFlags.Public);
            for (int i = 0; i < publicFields.Length; i++) {
                if (publicFields[i].GetCustomAttribute<HideInInspector>() == null) {
                    infoFields.Add(publicFields[i]);
                }
            }

            var privateFields = T.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
            for (int i = 0; i < privateFields.Length; i++) {
                if (privateFields[i].GetCustomAttribute<SerializeField>() != null) { 
                    infoFields.Add(privateFields[i]);
                }
            }

            return infoFields.ToArray();
        }
        
        /// <summary>
        /// Finds the Unity Object based on the GUID.
        /// </summary>
        /// <param name="guid">The GUID to find the Object with.</param>
        /// <returns>The Object with the specified GUID (can be null).</returns>
        public static T LoadAsset<T>(string guid) where T : UnityEngine.Object
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            if (!string.IsNullOrEmpty(assetPath)) {
                return AssetDatabase.LoadAssetAtPath(assetPath, typeof(T)) as T;
            }
            return null;
        }
    }
    
}
