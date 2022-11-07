using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
//https://dev-youngil.tistory.com/1 참고

//Attribute
public class ArrayElementTitleAttribute : PropertyAttribute
{
    public string varName;
    public ArrayElementTitleAttribute(string _titleVar)
    {
        varName = _titleVar;
    }
}

//이름 그리기
[CustomPropertyDrawer(typeof(ArrayElementTitleAttribute))]
public class ArrayElementTitleDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)  //필드의 GUI 높이 정의
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    protected virtual ArrayElementTitleAttribute Attribute => attribute as ArrayElementTitleAttribute;  //PropertyDrawer의 attribute(PropertyAttribute)를 가져옴
    SerializedProperty TitleNameProp;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        string FullPathName = property.propertyPath + "." + Attribute.varName;  //기존 이름으로 전체 경로를 가져옴
        TitleNameProp = property.serializedObject.FindProperty(FullPathName);   //풀이름으로 스크립트가 있는 오브젝트를 찾음
        string newLabel = GetTitle();
        if (string.IsNullOrEmpty(newLabel)) //리턴 값이 없을 경우
            newLabel = label.text;  //이름 원상복구
        EditorGUI.PropertyField(position, property, new GUIContent(newLabel, label.tooltip), true); //이름 변경, 툴팁은 원본 그대로
    }

    string GetTitle()   //ToString()
    {
        switch (TitleNameProp.propertyType) //종류가 워낙 많아서 출처의 원본 코드를 그대로 가져옴
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
            case SerializedPropertyType.Enum:   //코드를 가져온 목적인 Enum ToString
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