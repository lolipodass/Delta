using UnityEditor;
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
        ObfuscatedInt tempObfuscatedInt = new();
        tempObfuscatedInt.SetMaskAndMasked(maskProperty.intValue, maskedProperty.intValue);
        intField.value = tempObfuscatedInt.Value;
        intField.RegisterValueChangedCallback(evt =>
        {
            property.serializedObject.Update();
            var currentObfuscatedInt = new ObfuscatedInt();
            currentObfuscatedInt.SetMaskAndMasked(maskProperty.intValue, maskedProperty.intValue);
            currentObfuscatedInt.Value = evt.newValue;

            maskProperty.intValue = currentObfuscatedInt.GetInternalMask();
            maskedProperty.intValue = currentObfuscatedInt.GetInternalMasked();
            property.serializedObject.ApplyModifiedProperties();
        });

        root.Add(intField);
        return root;
    }

}
