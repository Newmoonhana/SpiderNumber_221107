using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
//https://dev-youngil.tistory.com/1 ����

//Attribute
public class ArrayElementTitleAttribute : PropertyAttribute
{
    public string varName;
    public ArrayElementTitleAttribute(string _titleVar)
    {
        varName = _titleVar;
    }
}

//�̸� �׸���
[CustomPropertyDrawer(typeof(ArrayElementTitleAttribute))]
public class ArrayElementTitleDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)  //�ʵ��� GUI ���� ����
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    protected virtual ArrayElementTitleAttribute Attribute => attribute as ArrayElementTitleAttribute;  //PropertyDrawer�� attribute(PropertyAttribute)�� ������
    SerializedProperty TitleNameProp;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        string FullPathName = property.propertyPath + "." + Attribute.varName;  //���� �̸����� ��ü ��θ� ������
        TitleNameProp = property.serializedObject.FindProperty(FullPathName);   //Ǯ�̸����� ��ũ��Ʈ�� �ִ� ������Ʈ�� ã��
        string newLabel = GetTitle();
        if (string.IsNullOrEmpty(newLabel)) //���� ���� ���� ���
            newLabel = label.text;  //�̸� ���󺹱�
        EditorGUI.PropertyField(position, property, new GUIContent(newLabel, label.tooltip), true); //�̸� ����, ������ ���� �״��
    }

    string GetTitle()   //ToString()
    {
        switch (TitleNameProp.propertyType) //������ ���� ���Ƽ� ��ó�� ���� �ڵ带 �״�� ������
        {
            case SerializedPropertyType.Generic:
                break;
            case SerializedPropertyType.Integer:
                return TitleNameProp.intValue.ToString();
            case SerializedPropertyType.Boolean:
                return TitleNameProp.boolValue.ToString();
            case SerializedPropertyType.Float:
                return TitleNameProp.floatValue.ToString();
            case SerializedPropertyType.String:
                return TitleNameProp.stringValue;
            case SerializedPropertyType.Color:
                return TitleNameProp.colorValue.ToString();
            case SerializedPropertyType.ObjectReference:
                return TitleNameProp.objectReferenceValue.ToString();
            case SerializedPropertyType.LayerMask:
                break;
            case SerializedPropertyType.Enum:   //�ڵ带 ������ ������ Enum ToString
                return TitleNameProp.enumNames[TitleNameProp.enumValueIndex];
            case SerializedPropertyType.Vector2:
                return TitleNameProp.vector2Value.ToString();
            case SerializedPropertyType.Vector3:
                return TitleNameProp.vector3Value.ToString();
            case SerializedPropertyType.Vector4:
                return TitleNameProp.vector4Value.ToString();
            case SerializedPropertyType.Rect:
                break;
            case SerializedPropertyType.ArraySize:
                break;
            case SerializedPropertyType.Character:
                break;
            case SerializedPropertyType.AnimationCurve:
                break;
            case SerializedPropertyType.Bounds:
                break;
            case SerializedPropertyType.Gradient:
                break;
            case SerializedPropertyType.Quaternion:
                break;
            case SerializedPropertyType.ExposedReference:
                break;
            case SerializedPropertyType.FixedBufferSize:
                break;
            case SerializedPropertyType.Vector2Int:
                break;
            case SerializedPropertyType.Vector3Int:
                break;
            case SerializedPropertyType.RectInt:
                break;
            case SerializedPropertyType.BoundsInt:
                break;
            default:
                break;
        }
        return "";
    }
}

#endif