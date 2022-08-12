// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

Shader "Graphics Tools/Canvas/Glow" {

Properties {

    [Header(Sizes)]
        [Toggle] _Relative_To_Height_("Relative To Height", Float) = 1
        _Bevel_Radius_("Bevel Radius", Float) = .05
        _Line_Width_("Line Width", Float) = 0.03
        _Outer_Fuzz_Start_("Outer Fuzz Start", Float) = 0.002
        _Outer_Fuzz_End_("Outer Fuzz End", Float) = 0.001
        _Fixed_Unit_Multiplier_("Fixed Unit Multiplier", Float) = 1000
     
    [Header(Animation)]
        _Motion_("Motion", Range(0,1)) = 1
        _Max_Intensity_("Max Intensity", Range(0,1)) = 0.5
        _Intensity_Fade_In_Exponent_("Intensity Fade In Exponent", Range(0,5)) = 2
     
    [Header(Color)]
        _Color_("Color", Color) = (1,1,1,1)
        _Inner_Color_("Inner Color", Color) = (1,1,1,1)
        _Blend_Exponent_("Blend Exponent", Range(0,9)) = 1
     
    [Header(Inner Transition)]
        _Falloff_("Falloff", Range(0,5)) = 1.0
        _Bias_("Bias", Range(0,1)) = 0.5
     

    [Header(Blending)]
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Source Blend", Float) = 1       // "One"
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Destination Blend", Float) = 1  // "One"

    [Header(Stencil)]
        _StencilReference("Stencil Reference", Range(0, 255)) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)]_StencilComparison("Stencil Comparison", Int) = 0
        [Enum(UnityEngine.Rendering.StencilOp)]_StencilOperation("Stencil Operation", Int) = 0

    [Header(Depth)]
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("Depth Test", Float) = 4 // "LessEqual"
        [Enum(Microsoft.MixedReality.GraphicsTools.Editor.DepthWrite)] _ZWrite("Depth Write", Float) = 0 // "Off"

    [HideInInspector] _MainTex("Texture", 2D) = "white" {} // Added to avoid UnityUI warnings.
    [HideInInspector] _ClipRect("Clip Rect", Vector) = (-32767.0, -32767.0, 32767.0, 32767.0) // Added to avoid SRP warnings.
    [HideInInspector] _ClipRectRadii("Clip Rect Radii", Vector) = (10.0, 10.0, 10.0, 10.0) // Added to avoid SRP warnings.
}

SubShader {
    Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
    Blend One One
    ZWrite[_ZWrite]
    ZTest[_ZTest]
    Tags {"DisableBatching" = "True"}
    Stencil
    {
        Ref[_StencilReference]
        Comp[_StencilComparison]
        Pass[_StencilOperation]
    }

    LOD 100


    Pass

    {

    CGPROGRAM

    #pragma vertex vert
    #pragma fragment frag

    #pragma target 4.0
    #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
    #pragma multi_compile_local _ _UI_CLIP_RECT_ROUNDED _UI_CLIP_RECT_ROUNDED_INDEPENDENT

    #include "UnityCG.cginc"
    #include "GraphicsToolsCommon.hlsl"

CBUFFER_START(UnityPerMaterial)
    int _Relative_To_Height_;
    float _Bevel_Radius_;
    float _Line_Width_;
    float _Outer_Fuzz_Start_;
    float _Outer_Fuzz_End_;
    float _Fixed_Unit_Multiplier_;
    half _Motion_;
    half _Max_Intensity_;
    half _Intensity_Fade_In_Exponent_;
    half4 _Color_;
    half4 _Inner_Color_;
    half _Blend_Exponent_;
    half _Falloff_;
    half _Bias_;
    // #if defined(UNITY_UI_CLIP_RECT)
    float4 _ClipRect;
    // #if defined(_UI_CLIP_RECT_ROUNDED) || defined(_UI_CLIP_RECT_ROUNDED_INDEPENDENT)
    float4 _ClipRectRadii;

CBUFFER_END


    struct VertexInput {
        float4 vertex : POSITION;
        float2 uv0 : TEXCOORD0;
        float2 uv2 : TEXCOORD2;
        float4 color : COLOR;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct VertexOutput {
        float4 pos : SV_POSITION;
#ifdef UNITY_UI_CLIP_RECT
        float3 posLocal : TEXCOORD8;
#endif
        half4 normalWorld : TEXCOORD5;
        float2 uv : TEXCOORD0;
        float4 tangent : TANGENT;
        float4 vertexColor : COLOR;
      UNITY_VERTEX_OUTPUT_STEREO
    };

    //BLOCK_BEGIN Scale_Sizes 32

    void Scale_Sizes_B32(
        float2 ScaleXY,
        bool Relative_To_Height,
        float Radius,
        float Line_Width,
        float Max_Outer_Fuzz,
        float Outer_Fuzz,
        float Fixed_Unit,
        out float3 RadiusLineAniso,
        out float3 OuterFuzz    )
    {
        float fixedScale = Fixed_Unit / ScaleXY.y;
        RadiusLineAniso.x = Relative_To_Height ? Radius : Radius * fixedScale;
        RadiusLineAniso.y = Relative_To_Height ? Line_Width : Line_Width * fixedScale;
        RadiusLineAniso.z = ScaleXY.x / ScaleXY.y;  // anisotropy
        OuterFuzz.x = Relative_To_Height ? Outer_Fuzz : Outer_Fuzz * fixedScale;
        OuterFuzz.y = Relative_To_Height ? Max_Outer_Fuzz : Max_Outer_Fuzz * fixedScale;
        OuterFuzz.z = 0.0;
    }
    //BLOCK_END Scale_Sizes


    VertexOutput vert(VertexInput vertInput)
    {
        UNITY_SETUP_INSTANCE_ID(vertInput);
        VertexOutput o;
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

        // ScaleUVs (#37)
        float2 XY_Q37 = (vertInput.uv0 - float2(0.5,0.5))*float2(vertInput.uv2.x/vertInput.uv2.y,1.0);

        // Max (#19)
        half MaxAB_Q19=max(0,_Motion_);

        // Greater_Than (#39)
        bool Greater_Than_Q39 = MaxAB_Q19 > 0;

        // Sqrt (#24)
        half Sqrt_F_Q24 = sqrt(MaxAB_Q19);

        // Conditional_Vec3 (#36)
        float3 Result_Q36 = Greater_Than_Q39 ? vertInput.vertex.xyz : float3(0,0,0);

        // Lerp (#22)
        float Value_At_T_Q22=lerp(_Outer_Fuzz_Start_,_Outer_Fuzz_End_,Sqrt_F_Q24);

        // Object_To_World_Pos (#38)
        float3 Pos_World_Q38=(mul(UNITY_MATRIX_M, float4(Result_Q36, 1)));

        float3 RadiusLineAniso_Q32;
        float3 OuterFuzz_Q32;
        Scale_Sizes_B32(vertInput.uv2,_Relative_To_Height_,_Bevel_Radius_,_Line_Width_,_Outer_Fuzz_Start_,Value_At_T_Q22,_Fixed_Unit_Multiplier_,RadiusLineAniso_Q32,OuterFuzz_Q32);

        float3 Position = Pos_World_Q38;
        float3 Normal = RadiusLineAniso_Q32;
        float2 UV = XY_Q37;
        float3 Tangent = OuterFuzz_Q32;
        float3 Binormal = float3(0,0,0);
        float4 Color = vertInput.color;

        o.pos = mul(UNITY_MATRIX_VP, float4(Position,1));
#ifdef UNITY_UI_CLIP_RECT
        o.posLocal = vertInput.vertex.xyz;
#endif
        o.normalWorld.xyz = Normal; o.normalWorld.w=1.0;
        o.uv = UV;
        o.tangent.xyz = Tangent; o.tangent.w=1.0;
        o.vertexColor = Color;

        return o;
    }

    //BLOCK_BEGIN Ease_Transition 42

    float Bias_Bid42(float b, float v) {
      return pow(v,log(clamp(b,0.001,0.999))/log(0.5));
    }
    void Ease_Transition_B42(
        half Falloff,
        half Bias,
        half F,
        out half Result    )
    {
        Result = pow(Bias_Bid42(Bias,F),Falloff);
    }
    //BLOCK_END Ease_Transition

    //BLOCK_BEGIN Round_Rect_Fragment 26

    void Round_Rect_Fragment_B26(
        half Radius,
        half Line_Width,
        half Anisotropy,
        float2 UV,
        half Outer_Fuzz,
        half Max_Outer_Fuzz,
        out half Rect_Distance,
        out half Inner_Distance    )
    {
        half2 center = half2(Anisotropy*0.5-Radius,0.5-Radius);
        half d = length(max(abs(UV)-center,0.0));
        half radius = Radius - Max_Outer_Fuzz;
        Inner_Distance = saturate(1.0-(radius-d)/Line_Width);
        Rect_Distance = saturate(1.0-(d-radius)/Outer_Fuzz)*Inner_Distance;
    }
    //BLOCK_END Round_Rect_Fragment


    half4 frag(VertexOutput fragInput) : SV_Target
    {
    #ifdef UNITY_UI_CLIP_RECT
        clip(GTUnityUIClipRect(fragInput.posLocal.xy, _ClipRect, _ClipRectRadii) - 0.5);
    #endif
        half4 result;

        // To_XYZ (#35)
        half X_Q35;
        half Y_Q35;
        half Z_Q35;
        X_Q35=fragInput.normalWorld.xyz.x;
        Y_Q35=fragInput.normalWorld.xyz.y;
        Z_Q35=fragInput.normalWorld.xyz.z;
        
        // To_XYZ (#34)
        half X_Q34;
        half Y_Q34;
        half Z_Q34;
        X_Q34=fragInput.tangent.xyz.x;
        Y_Q34=fragInput.tangent.xyz.y;
        Z_Q34=fragInput.tangent.xyz.z;
        
        // Max (#19)
        half MaxAB_Q19=max(0,_Motion_);

        half Rect_Distance_Q26;
        half Inner_Distance_Q26;
        Round_Rect_Fragment_B26(X_Q35,Y_Q35,Z_Q35,fragInput.uv,X_Q34,Y_Q34,Rect_Distance_Q26,Inner_Distance_Q26);

        // Power (#40)
        half Power_Q40 = pow(MaxAB_Q19, _Intensity_Fade_In_Exponent_);

        // Power (#41)
        half Power_Q41 = pow(Inner_Distance_Q26, _Blend_Exponent_);

        half Result_Q42;
        Ease_Transition_B42(_Falloff_,_Bias_,Rect_Distance_Q26,Result_Q42);

        // Multiply (#18)
        half Product_Q18 = _Max_Intensity_ * Power_Q40;

        // Mix_Colors (#21)
        half4 Color_At_T_Q21 = lerp(_Inner_Color_, _Color_,float4( Power_Q41, Power_Q41, Power_Q41, Power_Q41));

        // Multiply (#17)
        half Product_Q17 = Result_Q42 * Product_Q18;

        // Scale_Color (#25)
        half4 Result_Q25 = Product_Q17 * Color_At_T_Q21;

        // Multiply_Colors (#45)
        half4 Product_Q45 = fragInput.vertexColor * Result_Q25;

        half4 Out_Color = Product_Q45;
        half Clip_Threshold = 0;

        result = Out_Color;
        return result;
    }

    ENDCG
  }
 }
}
