﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.SDK.Inspectors.Input.Handlers;
using Microsoft.MixedReality.Toolkit.SDK.UX.Pointers;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.SDK.Inspectors.UX.Pointers
{
    [CustomEditor(typeof(BaseControllerPointer))]
    public class BaseControllerPointerInspector : ControllerPoseSynchronizerInspector
    {
        private SerializedProperty cursorPrefab;
        private SerializedProperty raycastOrigin;
        private SerializedProperty pointerExtent;
        private SerializedProperty activeHoldAction;
        private SerializedProperty useSourcePoseData;
        private SerializedProperty inputSourceAction;
        private SerializedProperty pointerAction;
        private SerializedProperty pointerOrientation;
        private SerializedProperty requiresHoldAction;

        protected override void OnEnable()
        {
            base.OnEnable();

            cursorPrefab = serializedObject.FindProperty("cursorPrefab");
            raycastOrigin = serializedObject.FindProperty("raycastOrigin");
            pointerExtent = serializedObject.FindProperty("pointerExtent");
            activeHoldAction = serializedObject.FindProperty("activeHoldAction");
            useSourcePoseData = serializedObject.FindProperty("useSourcePoseData");
            inputSourceAction = serializedObject.FindProperty("inputSourceAction");
            pointerAction = serializedObject.FindProperty("pointerAction");
            pointerOrientation = serializedObject.FindProperty("pointerOrientation");
            requiresHoldAction = serializedObject.FindProperty("requiresHoldAction");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(cursorPrefab);
            EditorGUILayout.PropertyField(raycastOrigin);
            EditorGUILayout.PropertyField(pointerExtent);
            EditorGUILayout.PropertyField(pointerOrientation);
            EditorGUILayout.PropertyField(useSourcePoseData);

            if (!useSourcePoseData.boolValue)
            {
                EditorGUILayout.PropertyField(inputSourceAction);
            }

            EditorGUILayout.PropertyField(pointerAction);
            EditorGUILayout.PropertyField(requiresHoldAction);

            if (requiresHoldAction.boolValue)
            {
                EditorGUILayout.PropertyField(activeHoldAction);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}