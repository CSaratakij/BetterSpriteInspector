using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ImageSample))]
public class ImageSampleEditor : Editor
{
    int currentSize;
    bool isShowImageSection = true;

    SerializedProperty spriteRendererProperty;
    SerializedProperty imageProperty;


    void OnEnable()
    {
        Initialize();
    }

    public override void OnInspectorGUI()
    {
        DrawCustomUI();
        DragAndDropHandler();
    }

    void Initialize()
    {
        spriteRendererProperty = serializedObject.FindProperty("spriteRenderer");
        imageProperty = serializedObject.FindProperty("images");
        currentSize = imageProperty.arraySize;
    }

    void DrawCustomUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(spriteRendererProperty);

        EditorGUILayout.Space();
        isShowImageSection = EditorGUILayout.BeginFoldoutHeaderGroup(isShowImageSection, "Image");

        serializedObject.ApplyModifiedProperties();

        if (!isShowImageSection)
            return;

        EditorGUILayout.BeginHorizontal();
        currentSize = EditorGUILayout.IntField("Size", currentSize);

        if (GUILayout.Button("Clear"))
        {
            currentSize = 0;
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        if (currentSize != imageProperty.arraySize)
        {
            if (currentSize < 0)
                currentSize = 0;

            serializedObject.Update();
            imageProperty.arraySize = currentSize;
            serializedObject.ApplyModifiedProperties();
        }

        ImageSample imageSample = (ImageSample)target;
        serializedObject.Update();

        for (int i = 0; i < imageProperty.arraySize; ++i)
        {
            var element = imageProperty.GetArrayElementAtIndex(i);
            var sprite = element.objectReferenceValue;

            if (sprite == null)
            {
                GUILayout.Label("(NULL)");
            }
            else
            {
                GUILayout.Label(imageSample.Images[i].texture);
            }

            EditorGUILayout.PropertyField(element);
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Down"))
            {
                int target = i + 1;

                if (target < imageProperty.arraySize)
                {
                    var temp = imageSample.Images[target];
                    imageSample.Images[target] = imageSample.Images[i];
                    imageSample.Images[i] = temp;
                }
            }

            if (GUILayout.Button("Up"))
            {
                int target = i - 1;

                if (target >= 0)
                {
                    var temp = imageSample.Images[target];
                    imageSample.Images[target] = imageSample.Images[i];
                    imageSample.Images[i] = temp;
                }
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }

        serializedObject.ApplyModifiedProperties();
    }

    void DragAndDropHandler()
    {
        if (!isShowImageSection)
            return;

        Event currentEvent = Event.current;
        Rect dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));

        GUI.Box(dropArea, "Add Sprite\n(Drag and Drop sprite here)");
        EditorGUILayout.Space();

        if (currentEvent.type == EventType.DragUpdated ||
            currentEvent.type == EventType.DragPerform)
        {
            if (!dropArea.Contains(currentEvent.mousePosition))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.None;
                return;
            }

            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

            if (currentEvent.type == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();
                ImageSample imageSample = (ImageSample)target;

                int oldSize = imageProperty.arraySize;
                int newSize = imageProperty.arraySize + DragAndDrop.objectReferences.Length;

                serializedObject.Update();

                currentSize = newSize;
                imageProperty.arraySize = currentSize;

                serializedObject.ApplyModifiedProperties();

                for (int i = oldSize; i < newSize; ++i)
                {
                    try
                    {
                        Texture2D texture = (Texture2D)DragAndDrop.objectReferences[i - oldSize];
                        string assetPath = AssetDatabase.GetAssetPath(texture);

                        Sprite targetSprite = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Sprite)) as Sprite;
                        imageSample.Images[i] = targetSprite;
                    }
                    catch (InvalidCastException)
                    {
                        Debug.Log("Can't assign : " + DragAndDrop.objectReferences[i - oldSize].name);
                    }
                }
            }

            currentEvent.Use();
        }
    }
}
