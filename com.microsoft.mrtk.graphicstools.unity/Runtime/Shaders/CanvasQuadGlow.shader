// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

Shader "Graphics Tools/Canvas/Quad Glow" {

Properties {

    [Header(Color)]
        _Color_("Color", Color) = (1,1,1,1)
     
    [Header(Shape)]
        _Radius_("Radius", Float) = 0.5
        [Toggle] _Fixed_Radius_("Fixed Radius", Float) = 0
        _Fixed_Unit_Multiplier_("Fixed Unit Multiplier", Float) = 1000
        _Filter_Width_("Filter Width", Range(0,4)) = 2
     
    [Header(Glow)]
        _Glow_Fraction_("Glow Fraction", Range(0.01,0.99)) = 0.5
        _Glow_Max_("Glow Max", Range(0,1)) = 0.5
        _Glow_Falloff_("Glow Falloff", Range(0,5)) = 2
     

    [Header(Blending)]
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Source Blend", Float) = 1       // "One"
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Destination Blend", Float) = 10  // "OneMinusSrcAlpha"
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlendAlpha("Source Blend Alpha", Float) = 1      // "One"
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlendAlpha("Destination Blend Alpha", Float) = 1 // "One"

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
    Tags{ "RenderType" = "AlphaTest" "Queue" = "AlphaTest"}
    Blend[_SrcBlend][_DstBlend],[_SrcBlendAlpha][_DstBlendAlpha]
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
    //#pragma multi_compile_local _ _CLIPPING_PLANE _CLIPPING_SPHERE _CLIPPING_BOX

    #include "UnityCG.cginc"
    #include "GraphicsToolsCommon.hlsl"

CBUFFER_START(UnityPerMaterial)
    half4 _Color_;
    float _Radius_;
    int _Fixed_Radius_;
    float _Fixed_Unit_Multiplier_;
    half _Filter_Width_;
    half _Glow_Fraction_;
    half _Glow_Max_;
    half _Glow_Falloff_;
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
        float3 posWorld : TEXCOORD7;
#ifdef UNITY_UI_CLIP_RECT
        float3 posLocal : TEXCOORD8;
#endif
        half4 normalWorld : TEXCOORD5;
        float2 uv : TEXCOORD0;
        float4 vertexColor : COLOR;
      UNITY_VERTEX_OUTPUT_STEREO
    };

    //BLOCK_BEGIN Scale_Sizes 29

    void Scale_Sizes_B29(
        float2 ScaleXY,
        float Radius,
        bool Fixed,
        float Fixed_Unit,
        out float3 RadiusAniso    )
    {
        RadiusAniso.x = Fixed ? Radius * Fixed_Unit / ScaleXY.y : Radius;
        RadiusAniso.y = ScaleXY.x / ScaleXY.y;  // anisotropy
        RadiusAniso.z = 0.0;
    }
    //BLOCK_END Scale_Sizes


    VertexOutput vert(VertexInput vertInput)
    {
        UNITY_SETUP_INSTANCE_ID(vertInput);
        VertexOutput o;
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

        // Object_To_World_Pos (#27)
        float3 Pos_World_Q27;
        Pos_World_Q27=(mul(UNITY_MATRIX_M, float4(vertInput.vertex.xyz, 1)));
        
        float3 RadiusAniso_Q29;
        Scale_Sizes_B29(vertInput.uv2,_Radius_,_Fixed_Radius_,_Fixed_Unit_Multiplier_,RadiusAniso_Q29);

        // ScaleUVs (#24)
        float2 XY_Q24 = (vertInput.uv0 - float2(0.5,0.5))*float2(vertInput.uv2.x/vertInput.uv2.y,1.0);

        float3 Position = Pos_World_Q27;
        float3 Normal = RadiusAniso_Q29;
        float2 UV = XY_Q24;
        float3 Tangent = float3(0,0,0);
        float3 Binormal = float3(0,0,0);
        float4 Color = vertInput.color;

        o.pos = mul(UNITY_MATRIX_VP, float4(Position,1));
        o.posWorld = Position;
#ifdef UNITY_UI_CLIP_RECT
        o.posLocal = vertInput.vertex.xyz;
#endif
        o.normalWorld.xyz = Normal; o.normalWorld.w=1.0;
        o.uv = UV;
        o.vertexColor = Color;

        return o;
    }

    //BLOCK_BEGIN Round_Rect 30

    half FilterStep_Bid30(half edge, half x, half filterWidth)
    {
       half dx = max(1.0E-5,fwidth(x)*filterWidth);
       return max((x+dx*0.5 - max(edge,x-dx*0.5))/dx,0.0);
    }
    void Round_Rect_B30(
        half Radius,
        half Size_X,
        half Size_Y,
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
        
        half insideRect = 1.0 - FilterStep_Bid30(1.0-Glow_Fraction,Distance,Filter_Width);
        
        half glow = saturate((1.0-Distance)/Glow_Fraction);
        glow = pow(glow, Glow_Falloff);
        Color = Rect_Color * max(insideRect, glow*Glow_Max);
    }
    //BLOCK_END Round_Rect


    half4 frag(VertexOutput fragInput) : SV_Target
    {
        ClipAgainstPrimitive(fragInput.posWorld);

    #ifdef UNITY_UI_CLIP_RECT
        clip(GTUnityUIClipRect(fragInput.posLocal.xy, _ClipRect, _ClipRectRadii) - 0.5);
    #endif
        half4 result;

        // To_XYZ (#20)
        half X_Q20;
        half Y_Q20;
        half Z_Q20;
        X_Q20=fragInput.normalWorld.xyz.x;
        Y_Q20=fragInput.normalWorld.xyz.y;
        Z_Q20=fragInput.normalWorld.xyz.z;
        
        half4 Color_Q30;
        Round_Rect_B30(X_Q20,Y_Q20,1.0,_Color_,_Filter_Width_,fragInput.uv,_Glow_Fraction_,_Glow_Max_,_Glow_Falloff_,Color_Q30);

        // Multiply_Colors (#31)
        half4 Product_Q31 = Color_Q30 * fragInput.vertexColor;

        half4 Out_Color = Product_Q31;
        half Clip_Threshold = 0;

        result = Out_Color;
        return result;
    }

    ENDCG
  }
 }
}
