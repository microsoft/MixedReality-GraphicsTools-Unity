// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

Shader "Graphics Tools/Non-Canvas/Glow" {

Properties {

    [Header(Rounded Rectangle)]
        _Bevel_Radius_("Bevel Radius", Range(0,1)) = 0.014
        _Line_Width_("Line Width", Range(0,1)) = 0.008
        [Toggle] _Absolute_Sizes_("Absolute Sizes", Float) = 1
     
    [Header(Animation)]
        _Tuning_Motion_("Tuning Motion", Range(0,1)) = 0
        [PerRendererData] _Motion_("Motion",Range(0,1)) = 0
        _Max_Intensity_("Max Intensity", Range(0,1)) = 0.7
        _Intensity_Fade_In_Exponent_("Intensity Fade In Exponent", Range(0,5)) = 2
        _Outer_Fuzz_Start_("Outer Fuzz Start", Range(0,1)) = 0.004
        _Outer_Fuzz_End_("Outer Fuzz End", Range(0,1)) = 0.001
     
    [Header(Color)]
        _Color_("Color", Color) = (0.682353,0.698039,1,1)
        _Inner_Color_("Inner Color", Color) = (0.356863,0.392157,0.796078,1)
        _Blend_Exponent_("Blend Exponent", Range(0,9)) = 1.5
     
    [Header(Inner Transition)]
        _Falloff_("Falloff", Range(0,5)) = 2
        _Bias_("Bias", Range(0,1)) = 0.5
     

    [Header(Stencil)]
        _StencilReference("Stencil Reference", Range(0, 255)) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)]_StencilComparison("Stencil Comparison", Int) = 0
        [Enum(UnityEngine.Rendering.StencilOp)]_StencilOperation("Stencil Operation", Int) = 0
}

SubShader {
    Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
    Blend One One
    ZWrite Off
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
    #pragma multi_compile_instancing
    #pragma target 4.0
    #pragma multi_compile_local _ _CLIPPING_PLANE _CLIPPING_SPHERE _CLIPPING_BOX

    #include "UnityCG.cginc"
    #include "GraphicsToolsCommon.hlsl"

CBUFFER_START(UnityPerMaterial)
    float _Bevel_Radius_;
    half _Line_Width_;
    int _Absolute_Sizes_;
    half _Tuning_Motion_;
    half _Motion_;
    half _Max_Intensity_;
    half _Intensity_Fade_In_Exponent_;
    half _Outer_Fuzz_Start_;
    half _Outer_Fuzz_End_;
    half4 _Color_;
    half4 _Inner_Color_;
    half _Blend_Exponent_;
    half _Falloff_;
    half _Bias_;

CBUFFER_END

    struct VertexInput {
        float4 vertex : POSITION;
        half3 normal : NORMAL;
        float2 uv0 : TEXCOORD0;
        float4 tangent : TANGENT;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct VertexOutput {
        float4 pos : SV_POSITION;
        half4 normalWorld : TEXCOORD5;
        float2 uv : TEXCOORD0;
        float3 posWorld : TEXCOORD7;
      UNITY_VERTEX_OUTPUT_STEREO
    };


    VertexOutput vert(VertexInput vertInput)
    {
        UNITY_SETUP_INSTANCE_ID(vertInput);
        VertexOutput o;
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

        // Object_To_World_Dir (#196)
        float3 Dir_World_Q196=(mul((float3x3)UNITY_MATRIX_M, vertInput.tangent));

        // Object_To_World_Dir (#195)
        float3 Dir_World_Q195=(mul((float3x3)UNITY_MATRIX_M, (normalize(cross(vertInput.normal,vertInput.tangent)))));

        // Max (#179)
        half MaxAB_Q179=max(_Tuning_Motion_,_Motion_);

        // Length3 (#171)
        float Length_Q171 = length(Dir_World_Q196);

        // Length3 (#172)
        float Length_Q172 = length(Dir_World_Q195);

        // Greater_Than (#192)
        bool Greater_Than_Q192 = MaxAB_Q179 > 0;

        // ScaleUVs (#190)
        float3 Sizes_Q190;
        float2 XY_Q190;
        Sizes_Q190 = (_Absolute_Sizes_ ? float3(Length_Q171,Length_Q172,0) : float3(Length_Q171/Length_Q172,1,0));
        XY_Q190 = (vertInput.uv0 - float2(0.5,0.5))*Sizes_Q190.xy;

        // Conditional_Vec3 (#193)
        float3 Result_Q193 = Greater_Than_Q192 ? vertInput.vertex.xyz : float3(0,0,0);

        // Object_To_World_Pos (#194)
        float3 Pos_World_Q194=(mul(UNITY_MATRIX_M, float4(Result_Q193, 1)));

        float3 Position = Pos_World_Q194;
        float3 Normal = Sizes_Q190;
        float2 UV = XY_Q190;
        float3 Tangent = float3(0,0,0);
        float3 Binormal = float3(0,0,0);
        float4 Color = float4(1,1,1,1);

        o.pos = mul(UNITY_MATRIX_VP, float4(Position,1));
        o.posWorld = Position;
        o.normalWorld.xyz = Normal; o.normalWorld.w=1.0;
        o.uv = UV;

        return o;
    }

    //BLOCK_BEGIN Ease_Transition 200

    float BiasFunc(float b, float v) {
      return pow(v,log(clamp(b,0.001,0.999))/log(0.5));
    }
    //BLOCK_END Ease_Transition

    //BLOCK_BEGIN Fuzzy_Round_Rect 188

    void Fuzzy_Round_Rect_B188(
        float Size_X,
        float Size_Y,
        float Radius_X,
        float Radius_Y,
        half Line_Width,
        float2 UV,
        half Outer_Fuzz,
        half Max_Outer_Fuzz,
        out half Rect_Distance,
        out half Inner_Distance    )
    {
        float2 halfSize = float2(Size_X,Size_Y)*0.5;
        float2 r = max(min(float2(Radius_X,Radius_Y),halfSize),float2(0.001,0.001));
        float radius = min(r.x,r.y)-Max_Outer_Fuzz;
        float2 v = abs(UV);
        float2 nearestp = min(v,halfSize-r);
        half d = distance(nearestp,v);
        Inner_Distance = saturate(1.0-(radius-d)/Line_Width);
        Rect_Distance = saturate(1.0-(d-radius)/Outer_Fuzz)*Inner_Distance;
    }
    //BLOCK_END Fuzzy_Round_Rect


    half4 frag(VertexOutput fragInput) : SV_Target
    {
        ClipAgainstPrimitive(fragInput.posWorld);

        half4 result;

        // To_XY (#197)
        float X_Q197;
        float Y_Q197;
        X_Q197=fragInput.normalWorld.xyz.x;
        Y_Q197=fragInput.normalWorld.xyz.y;

        // Max (#179)
        half MaxAB_Q179=max(_Tuning_Motion_,_Motion_);

        // Sqrt (#182)
        half Sqrt_F_Q182 = sqrt(MaxAB_Q179);

        // Power (#198)
        half Power_Q198 = pow(MaxAB_Q179, _Intensity_Fade_In_Exponent_);

        // Lerp (#181)
        half Value_At_T_Q181=lerp(_Outer_Fuzz_Start_,_Outer_Fuzz_End_,Sqrt_F_Q182);

        // Multiply (#178)
        half Product_Q178 = _Max_Intensity_ * Power_Q198;

        half Rect_Distance_Q188;
        half Inner_Distance_Q188;
        Fuzzy_Round_Rect_B188(X_Q197,Y_Q197,_Bevel_Radius_,_Bevel_Radius_,_Line_Width_,fragInput.uv,Value_At_T_Q181,_Outer_Fuzz_Start_,Rect_Distance_Q188,Inner_Distance_Q188);

        // Power (#199)
        half Power_Q199 = pow(Inner_Distance_Q188, _Blend_Exponent_);

        // Ease_Transition (#200)
        half Result_Q200 = pow(BiasFunc(_Bias_,Rect_Distance_Q188),_Falloff_);

        // Mix_Colors (#180)
        half4 Color_At_T_Q180 = lerp(_Inner_Color_, _Color_,float4( Power_Q199, Power_Q199, Power_Q199, Power_Q199));

        // Multiply (#177)
        half Product_Q177 = Result_Q200 * Product_Q178;

        // Scale_Color (#183)
        half4 Result_Q183 = Product_Q177 * Color_At_T_Q180;

        half4 Out_Color = Result_Q183;
        half Clip_Threshold = 0;

        result = Out_Color;
        return result;
    }

    ENDCG
  }
 }
}
