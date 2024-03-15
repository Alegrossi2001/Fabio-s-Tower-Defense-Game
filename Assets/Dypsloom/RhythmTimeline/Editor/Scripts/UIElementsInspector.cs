namespace Dypsloom.RhythmTimeline.Editor
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.UIElements;
    using UnityEngine;
    using UnityEngine.UIElements;

    public abstract class UIElementsInspector : UnityEditor.Editor
    {
        protected abstract List<string> propertiesToExclude { get; }

        public override VisualElement CreateInspectorGUI()
        {
            return CreateUIElementInspectorGUI(serializedObject, propertiesToExclude);
        }
    
    
        /// <summary>
        /// Creates a VisualElement that draws all the properties of a serializedObject
        /// </summary>
        /// <param name="serializedObject"></param>
        /// <returns></returns>
        public static VisualElement CreateUIElementInspectorGUI(SerializedObject serializedObject, List<string> propertiesToExclude)
        {
            var container = new VisualElement();

            var fields = EditorHelper.GetVisibleSerializedFields(serializedObject.targetObject.GetType());
            for (int i = 0; i < fields.Length; ++i) {
                var field = fields[i];

                // Do not draw HideInInspector fields or excluded properties.
                if (field.GetCustomAttributes(typeof(HideInInspector), false).Length > 0 ||
                    (propertiesToExclude != null &&
                     propertiesToExclude.Contains(field.Name))) {
                    continue;
                }

                //Debug.Log(field.Name);
                var serializedProperty = serializedObject.FindProperty(field.Name);

                var propertyField = new PropertyField(serializedProperty);

                container.Add(propertyField);
            }


            return container;
        }
    
    }
}

