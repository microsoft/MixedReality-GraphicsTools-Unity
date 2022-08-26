// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

Shader "Graphics Tools/Non-Canvas/Quad Glow" {

Properties {

    [Header(Color)]
        _Color_("Color", Color) = (1,1,1,1)
     
    [Header(Shape)]
        _Radius_("Radius", Range(0,0.5)) = 0.5
        [Toggle] _Fixed_Radius_("Fixed Radius", Float) = 0
        _Filter_Width_("Filter Width", Range(0,4)) = 2
     
    [Header(Glow)]
        _Glow_Fraction_("Glow Fraction", Range(0.01,0.99)) = 0.5
        _Glow_Max_("Glow Max", Range(0,1)) = 0.5
        _Glow_Falloff_("Glow Falloff", Range(0,5)) = 2
     

    [Header(Stencil)]
        _StencilReference("Stencil Reference", Range(0, 255)) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)]_StencilComparison("Stencil Comparison", Int) = 0
        [Enum(UnityEngine.Rendering.StencilOp)]_StencilOperation("Stencil Operation", Int) = 0

    [Header(Blend)]
    [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("Depth Test", Float) = 4
    [Enum(Microsoft.MixedReality.GraphicsTools.Editor.DepthWrite)] _ZWrite("Depth Write", Float) = 0
}

SubShader {
    Tags{ "RenderType" = "AlphaTest" "Queue" = "AlphaTest"}
    Blend One OneMinusSrcAlpha
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

    ZWrite[_ZWrite]
    ZTest[_ZTest]

    CGPROGRAM

    #pragma vertex vert
    #pragma fragment frag
    #pragma multi_compile_instancing
    #pragma target 4.0
    #pragma multi_compile_local _ _CLIPPING_PLANE _CLIPPING_SPHERE _CLIPPING_BOX

    #if defined(_CLIPPING_PLANE) || defined(_CLIPPING_SPHERE) || defined(_CLIPPING_BOX)
        #define _CLIPPING_PRIMITIVE
    #else
        #undef _CLIPPING_PRIMITIVE
    #endif

    #include "UnityCG.cginc"
    #include "GraphicsToolsCommon.hlsl"

CBUFFER_START(UnityPerMaterial)
    half4 _Color_;
    float _Radius_;
    int _Fixed_Radius_;
    half _Filter_Width_;
    half _Glow_Fraction_;
    half _Glow_Max_;
    half _Glow_Falloff_;

CBUFFER_END

#if defined (_CLIPPING_PLANE)
    half _ClipPlaneSide;
    float4 _ClipPlane;
#endif
#if defined(_CLIPPING_SPHERE)
    half _ClipSphereSide;
    float4x4 _ClipSphereInverseTransform;
#endif
#if defined (_CLIPPING_BOX)
    half _ClipBoxSide;
    float4x4 _ClipBoxInverseTransform;
#endif

    struct VertexInput {
        float4 vertex : POSITION;
        half3 normal : NORMAL;
        float2 uv0 : TEXCOORD0;
        float4 tangent : TANGENT;
        float4 color : COLOR;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct VertexOutput {
        float4 pos : SV_POSITION;
        float2 uv : TEXCOORD0;
        float3 posWorld : TEXCOORD7;
        float4 tangent : TANGENT;
      UNITY_VERTEX_OUTPUT_STEREO
    };


    VertexOutput vert(VertexInput vertInput)
    {
        UNITY_SETUP_INSTANCE_ID(vertInput);
        VertexOutput o;
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

        // Object_To_World_Pos (#1371)
        float3 Pos_World_Q1371;
        Pos_World_Q1371=(mul(UNITY_MATRIX_M, float4(vertInput.vertex.xyz, 1)));
        
        // Object_To_World_Dir (#1372)
        float3 Dir_World_Q1372;
        Dir_World_Q1372=(mul((float3x3)UNITY_MATRIX_M, vertInput.tangent));
        
        // Object_To_World_Dir (#1373)
        float3 Dir_World_Q1373;
        Dir_World_Q1373=(mul((float3x3)UNITY_MATRIX_M, (normalize(cross(vertInput.normal,vertInput.tangent)))));
        
        // Length3 (#1362)
        float Length_Q1362 = length(Dir_World_Q1372);

        // Length3 (#1363)
        float Length_Q1363 = length(Dir_World_Q1373);

        // Divide (#1366)
        float Quotient_Q1366 = Length_Q1362 / Length_Q1363;

        // Divide (#1377)
        float Quotient_Q1377 = _Radius_ / Length_Q1363;

        // TransformUVs (#1375)
        float2 Result_Q1375;
        Result_Q1375 = float2((vertInput.uv0.x-0.5)*Length_Q1362/Length_Q1363,(vertInput.uv0.y-0.5));
        
        // Conditional_Float (#1380)
        float Result_Q1380 = _Fixed_Radius_ ? Quotient_Q1377 : _Radius_;

        // From_XYZ (#1365)
        float3 Vec3_Q1365 = float3(Quotient_Q1366,Result_Q1380,0);

        float3 Position = Pos_World_Q1371;
        float3 Normal = float3(0,0,0);
        float2 UV = Result_Q1375;
        float3 Tangent = Vec3_Q1365;
        float3 Binormal = float3(0,0,0);
        float4 Color = vertInput.color;

        o.pos = mul(UNITY_MATRIX_VP, float4(Position,1));
        o.posWorld = Position;
        o.uv = UV;
        o.tangent.xyz = Tangent; o.tangent.w=1.0;

        return o;
    }

    //BLOCK_BEGIN Round_Rect 1376

    half FilterStep_Bid1376(half edge, half x, half filterWidth)
    {
       half dx = max(1.0E-5,fwidth(x)*filterWidth);
       return max((x+dx*0.5 - max(edge,x-dx*0.5))/dx,0.0);
    }
    void Round_Rect_B1376(
        half Size_X,
        half Size_Y,
        half Radius,
        half4 Rect_Color,
        half Filter_Width,
        half2 UV,
        half Glow_Fraction,
        half Glow_Max,
        half Glow_Falloff,
        out half4 Color    )
    {
        half2 halfSize = half2(Size_X,Size_Y)*0.5;
        half2 r = max(min(half2(Radius,Radius),halfSize),half2(0.01,0.01));
        
        half2 v = abs(UV);
        
        half2 nearestp = min(v,halfSize-r);
        half2 delta = (v-nearestp)/max(half2(0.01,0.01),r);
        half Distance = length(delta);
        
        half insideRect = 1.0 - FilterStep_Bid1376(1.0-Glow_Fraction,Distance,Filter_Width);
        
        half glow = saturate((1.0-Distance)/Glow_Fraction);
        glow = pow(glow, Glow_Falloff);
        Color = Rect_Color * max(insideRect, glow*Glow_Max);
    }
    //BLOCK_END Round_Rect


    half4 frag(VertexOutput fragInput) : SV_Target
    {
#if defined(_CLIPPING_PRIMITIVE)
        float primitiveDistance = 1.0;
#if defined(_CLIPPING_PLANE)
        primitiveDistance = min(primitiveDistance, GTPointVsPlane(fragInput.posWorld.xyz, _ClipPlane) * _ClipPlaneSide);
#endif
#if defined(_CLIPPING_SPHERE)
        primitiveDistance = min(primitiveDistance, GTPointVsSphere(fragInput.posWorld.xyz, _ClipSphereInverseTransform) * _ClipSphereSide);
#endif
#if defined(_CLIPPING_BOX)
        primitiveDistance = min(primitiveDistance, GTPointVsBox(fragInput.posWorld.xyz, _ClipBoxInverseTransform) * _ClipBoxSide);
#endif
        clip(primitiveDistance);
#endif
        half4 result;

        // To_XYZ (#1374)
        half X_Q1374;
        half Y_Q1374;
        half Z_Q1374;
        X_Q1374=fragInput.tangent.xyz.x;
        Y_Q1374=fragInput.tangent.xyz.y;
        Z_Q1374=fragInput.tangent.xyz.z;
        
        half4 Color_Q1376;
        Round_Rect_B1376(X_Q1374,1.0,Y_Q1374,_Color_,_Filter_Width_,fragInput.uv,_Glow_Fraction_,_Glow_Max_,_Glow_Falloff_,Color_Q1376);

        float4 Out_Color = Color_Q1376;
        float Clip_Threshold = 0;

        result = Out_Color;
        return result;
    }

    ENDCG
  }
 }
}
