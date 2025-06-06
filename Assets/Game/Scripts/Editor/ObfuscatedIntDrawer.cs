using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(ObfuscatedInt))]
public class ObfuscatedIntDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var root = new VisualElement();


        SerializedProperty maskProperty = property.FindPropertyRelative("_mask");
        SerializedProperty maskedProperty = property.FindPropertyRelative("_masked");

        var intField = new IntegerField(property.displayName);


        int currentMask = maskProperty.intValue;
        int currentMasked = maskedProperty.intValue;

        intField.value = currentMasked ^ currentMask;

        intField.RegisterValueChangedCallback(evt =>
        {
            property.serializedObject.Update();

            int newValue = evt.newValue;
            int oldMask = maskProperty.intValue;

            if (oldMask == 0)
            {
                oldMask = ObfuscatedInt.GetRandomInteger();
                while (oldMask == 0)
                {
                    oldMask = ObfuscatedInt.GetRandomInteger();
                }
            }

            int newMasked = newValue ^ oldMask;

            maskProperty.intValue = oldMask;
            maskedProperty.intValue = newMasked;

            property.serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(property.serializedObject.targetObject);
        });
        root.Add(intField);

        return root;
    }

}
