// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    /// <summary>
    /// An abstract editor component to improve the editor experience with ClippingPrimitives.
    /// </summary>
    [CustomEditor(typeof(ClippingPrimitive))]
    [CanEditMultipleObjects]
    public abstract class ClippingPrimitiveEditor : UnityEditor.Editor
    {
        /// <summary>
        /// Notifies the Unity editor if this object has custom frame bounds.
        /// </summary>
        /// <returns>True if custom frame bounds can be used from OnGetFrameBounds.</returns>
        protected abstract bool HasFrameBounds();

        /// <summary>
        /// Returns the bounds the editor should focus on.
        /// </summary>
        /// <returns>The bounds of the clipping primitive.</returns>
        protected abstract Bounds OnGetFrameBounds();

        private ClippingPrimitive clippingPrimitive;
        private SerializedProperty m_Script;
        private SerializedProperty applyToSharedMaterial;

        private void OnEnable()
        {
            clippingPrimitive = (ClippingPrimitive)target;
            m_Script = serializedObject.FindProperty(nameof(m_Script));
            applyToSharedMaterial = serializedObject.FindProperty(nameof(applyToSharedMaterial));
        }

        /// <summary>
        /// Looks for changes to the list of renderers and materials and gracefully adds and removes them.
        /// </summary>
        public override void OnInspectorGUI()
        {
            var previousRenderers = clippingPrimitive.GetRenderersCopy();
            var previousMaterials = clippingPrimitive.GetMaterialsCopy();

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                InspectorUtilities.DrawReadonlyPropertyField(m_Script);

                if (HasNoRenderers(previousRenderers))
                {
                    EditorGUILayout.PropertyField(applyToSharedMaterial);
                }
                else
                {
                    InspectorUtilities.DrawReadonlyPropertyField(applyToSharedMaterial);
                }

                DrawPropertiesExcluding(serializedObject, nameof(m_Script), nameof(applyToSharedMaterial));

                if (check.changed)
                {
                    // Flagging changes other than renderers
                    clippingPrimitive.IsDirty = true;

                    serializedObject.ApplyModifiedProperties();
                }
            }

            // Add or remove and renderers that were added or removed via the inspector.
            var currentRenderers = clippingPrimitive.GetRenderersCopy();

            foreach (var renderer in previousRenderers.Except(currentRenderers))
            {
                // ResetRenderer rather than RemoveRenderer since the renderer has already been removed from the list.
                clippingPrimitive.ResetRenderer(renderer);
            }

            foreach (var renderer in currentRenderers.Except(previousRenderers))
            {
                clippingPrimitive.AddRenderer(renderer);
            }

            // Add or remove and materials that were added or removed via the inspector.
            var currentMaterials = clippingPrimitive.GetMaterialsCopy();

            foreach (var material in previousMaterials.Except(currentMaterials))
            {
                // ResetMaterial rather than RemoveMaterial since the material has already been removed from the list.
                clippingPrimitive.ResetMaterial(material);
            }

            foreach (var material in currentMaterials.Except(previousMaterials))
            {
                clippingPrimitive.AddMaterial(material);
            }
        }

        private static bool HasNoRenderers(IEnumerable<Renderer> renderers)
        {
            foreach (var renderer in renderers)
            {
                if (renderer != null)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
