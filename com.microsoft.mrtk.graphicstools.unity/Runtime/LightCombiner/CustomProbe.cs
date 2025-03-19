// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.Rendering;

namespace Microsoft.MixedReality.GraphicsTools
{
	/// <summary>
	/// This component can be used to set the ambient probe to a custom value. 
	/// Used in conjunction with the LightCombinerWindow. Once lightmaps have been removed from the scene
	/// Unity will try to use light probes for the ambient light info. The ambient probe is too dark because it 
	/// does not consider the baked lights. 
	/// </summary>
	[ExecuteInEditMode]
	public class CustomProbe : MonoBehaviour
	{
		[SerializeField]
		private Color ambientLightColor = Color.white;

		private void Awake()
		{
			var _renderer = GetComponent<Renderer>();

			if (_renderer != null)
			{
				_renderer.lightProbeUsage = LightProbeUsage.CustomProvided;

				var probeList = new SphericalHarmonicsL2[1];
				probeList[0].AddAmbientLight(ambientLightColor);

				var propertyBlock = new MaterialPropertyBlock();
				_renderer.GetPropertyBlock(propertyBlock);
				propertyBlock.CopySHCoefficientArraysFrom(probeList);
				_renderer.SetPropertyBlock(propertyBlock);
			}
		}
	}
}
