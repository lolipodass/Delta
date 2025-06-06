using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;

[CustomPropertyDrawer(typeof(UlidAttribute))]
public class UlidDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        if (property.propertyType != SerializedPropertyType.String)
        {
            throw new ArgumentException("The [Ulid] attribute can only be applied to string fields.", nameof(property.propertyType));
        }

        var container = new VisualElement();
        container.style.flexDirection = FlexDirection.Row;
        container.style.alignItems = Align.Center;

        var textField = new TextField(property.displayName)
        {
            isDelayed = true,
        };
        textField.AddToClassList(TextField.alignedFieldUssClassName);
        textField.BindProperty(property);
        textField.style.flexGrow = 1;
        textField.style.flexShrink = 1;

        var generateButton = new Button(() =>
        {
            Ulid newUlid = Ulid.NewUlid();
            property.stringValue = newUlid.ToString();
            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
            textField.SetValueWithoutNotify(newUlid.ToString());
            Debug.Log($"[UlidDrawer] Manually generated new ULID for '{property.displayName}': {newUlid}", property.serializedObject.targetObject);
        })
        {
            text = "Generate"
        };
        generateButton.style.marginLeft = 5;

        container.Add(textField);
        container.Add(generateButton);

        textField.RegisterValueChangedCallback(changed =>
        {
            bool newValid = Ulid.TryParse(changed.newValue, out Ulid newUlid);
            if (string.IsNullOrEmpty(changed.newValue) || !newValid)
            {
                bool oldValid = Ulid.TryParse(changed.previousValue, out Ulid oldUlid);

                if (oldValid)
                {
                    newUlid = oldUlid;
                }
                else
                {
                    newUlid = Ulid.NewUlid();
                    Debug.LogWarning($"[UlidDrawer] Invalid ULID entered or found for '{property.displayName}'. Generated new ULID: {newUlid}", property.serializedObject.targetObject);
                }
            }
            textField.SetValueWithoutNotify(newUlid.ToString());

            property.stringValue = newUlid.ToString();
            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
        });


        if (string.IsNullOrEmpty(property.stringValue) || !Ulid.TryParse(property.stringValue, out _))
        {
            Ulid initialUlid = Ulid.NewUlid();
            property.stringValue = initialUlid.ToString();
            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
            textField.SetValueWithoutNotify(initialUlid.ToString());
            Debug.Log($"[UlidDrawer] Initializing empty/invalid ULID for '{property.displayName}'. Generated: {initialUlid}", property.serializedObject.targetObject);
        }

        return container;
    }
}