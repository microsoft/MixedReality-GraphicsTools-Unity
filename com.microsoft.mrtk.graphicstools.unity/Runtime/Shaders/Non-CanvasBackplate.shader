// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

Shader "Graphics Tools/Non-Canvas/Backplate" {

Properties {

    [Header(Round Rect)]
        _Radius_("Radius", Range(0,0.5)) = 0.012
        [Toggle(_LINE_ENABLE_)] _Line_Enable_("Line Enable", Float) = 1
        _Line_Width_("Line Width", Range(0,1)) = 0.001
        [Toggle] _Absolute_Sizes_("Absolute Sizes", Float) = 1
        _Filter_Width_("Filter Width", Range(0,4)) = 1
        _Base_Color_("Base Color", Color) = (0,0,0,1)
        _Line_Color_("Line Color", Color) = (0.2,0.262745,0.4,1)
     
    [Header(Radii Multipliers)]
        _Radius_Top_Left_("Radius Top Left", Range(0,1)) = 1
        _Radius_Top_Right_("Radius Top Right", Range(0,1)) = 1.0
        _Radius_Bottom_Left_("Radius Bottom Left", Range(0,1)) = 1.0
        _Radius_Bottom_Right_("Radius Bottom Right", Range(0,1)) = 1.0
     
    [Header(Line Highlight)]
        _Rate_("Rate", Range(0,1)) = 0
        _Highlight_Color_("Highlight Color", Color) = (0.98,0.98,0.98,1)
        _Highlight_Width_("Highlight Width", Range(0,2)) = 0
        _Highlight_Transform_("Highlight Transform", Vector) = (1, 1, 0, 0)
        _Highlight_("Highlight", Range(0,1)) = 1
     
    [Header(Iridescence)]
        [Toggle(_IRIDESCENCE_ENABLE_)] _Iridescence_Enable_("Iridescence Enable", Float) = 1
        _Iridescence_Intensity_("Iridescence Intensity", Range(0,1)) = 0.45
        _Iridescence_Edge_Intensity_("Iridescence Edge Intensity", Range(0,1)) = 1
        _Iridescence_Tint_("Iridescence Tint", Color) = (1,1,1,1)
        [NoScaleOffset] _Iridescent_Map_("Iridescent Map", 2D) = "" {}
        _Angle_("Angle", Range(-90,90)) = -45
        [Toggle] _Reflected_("Reflected", Float) = 1
        _Frequency_("Frequency", Range(0,10)) = 1
        _Vertical_Offset_("Vertical Offset", Range(0,2)) = 0
     
    [Header(Gradient)]
        [Toggle(_GRADIENT_ENABLE_)] _Gradient_Enable_("Gradient Enable", Float) = 1
        _Gradient_Color_("Gradient Color", Color) = (0.74902,0.74902,0.74902,1)
        _Top_Left_("Top Left", Color) = (0.00784314,0.294118,0.580392,1)
        _Top_Right_("Top Right", Color) = (0.305882,0,1,1)
        _Bottom_Left_("Bottom Left", Color) = (0.133333,0.258824,0.992157,1)
        _Bottom_Right_("Bottom Right", Color) = (0.176471,0.176471,0.619608,1)
        [Toggle(_EDGE_ONLY_)] _Edge_Only_("Edge Only", Float) = 0
        _Edge_Width_("Edge Width", Range(0,1)) = 0.5
        _Edge_Power_("Edge Power", Range(0,10)) = 1
        _Line_Gradient_Blend_("Line Gradient Blend", Range(0,1)) = 0.5
     
    [Header(Fade)]
        _Fade_Out_("Fade Out", Range(0,1)) = 1
     
    [Header(Antialiasing)]
        [Toggle(_SMOOTH_EDGES_)] _Smooth_Edges_("Smooth Edges", Float) = 0
     
    [Header(Blending)]
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Source Blend", Float) = 1       // "One"
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Destination Blend", Float) = 0  // "Zero"
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlendAlpha("Source Blend Alpha", Float) = 1      // "One"
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlendAlpha("Destination Blend Alpha", Float) = 1 // "One"

    [Header(Stencil)]
        _StencilReference("Stencil Reference", Range(0, 255)) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)]_StencilComparison("Stencil Comparison", Int) = 0
        [Enum(UnityEngine.Rendering.StencilOp)]_StencilOperation("Stencil Operation", Int) = 0

    [Header(Depth)]
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("Depth Test", Float) = 4 // "LessEqual"
        [Enum(Microsoft.MixedReality.GraphicsTools.Editor.DepthWrite)] _ZWrite("Depth Write", Float) = 1 // "On"
}

SubShader {
    Tags{ "RenderType" = "Opaque" }
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

    #pragma multi_compile_instancing

    #pragma target 4.0

    #pragma shader_feature_local _ _SMOOTH_EDGES_
    #pragma shader_feature_local _ _IRIDESCENCE_ENABLE_
    #pragma shader_feature_local _ _LINE_ENABLE_
    #pragma shader_feature_local _ _GRADIENT_ENABLE_
    #pragma shader_feature_local _ _EDGE_ONLY_

    #pragma multi_compile_local _ _CLIPPING_PLANE _CLIPPING_SPHERE _CLIPPING_BOX

    #if defined(_CLIPPING_PLANE) || defined(_CLIPPING_SPHERE) || defined(_CLIPPING_BOX)
        #define _CLIPPING_PRIMITIVE
    #else
        #undef _CLIPPING_PRIMITIVE
    #endif

    #include "UnityCG.cginc"
    #include "GraphicsToolsCommon.hlsl"

CBUFFER_START(UnityPerMaterial)
    float _Radius_;
    //bool _Line_Enable_;
    float _Line_Width_;
    int _Absolute_Sizes_;
    float _Filter_Width_;
    float4 _Base_Color_;
    float4 _Line_Color_;
    float _Radius_Top_Left_;
    float _Radius_Top_Right_;
    float _Radius_Bottom_Left_;
    float _Radius_Bottom_Right_;
    float _Rate_;
    half4 _Highlight_Color_;
    half _Highlight_Width_;
    float4 _Highlight_Transform_;
    half _Highlight_;
    //bool _Iridescence_Enable_;
    float _Iridescence_Intensity_;
    float _Iridescence_Edge_Intensity_;
    half4 _Iridescence_Tint_;
    sampler2D _Iridescent_Map_;
    float _Angle_;
    int _Reflected_;
    float _Frequency_;
    float _Vertical_Offset_;
    //bool _Gradient_Enable_;
    half4 _Gradient_Color_;
    half4 _Top_Left_;
    half4 _Top_Right_;
    half4 _Bottom_Left_;
    half4 _Bottom_Right_;
    //bool _Edge_Only_;
    half _Edge_Width_;
    half _Edge_Power_;
    half _Line_Gradient_Blend_;
    half _Fade_Out_;
    //bool _Smooth_Edges_;

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
        float4 tangent : TANGENT;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct VertexOutput {
        float4 pos : SV_POSITION;
        half4 normalWorld : TEXCOORD5;
        float2 uv : TEXCOORD0;
        float3 posWorld : TEXCOORD7;
        float4 tangent : TANGENT;
        float4 binormal : TEXCOORD6;
        float4 extra1 : TEXCOORD4;
        float4 extra2 : TEXCOORD3;
      UNITY_VERTEX_OUTPUT_STEREO
    };

    //BLOCK_BEGIN Object_To_World_Pos 1123

    void Object_To_World_Pos_B1123(
        float3 Pos_Object,
        out float3 Pos_World    )
    {
        Pos_World=(mul(UNITY_MATRIX_M, float4(Pos_Object, 1)));
        
    }
    //BLOCK_END Object_To_World_Pos

    //BLOCK_BEGIN Round_Rect_Vertex 1165

    void Round_Rect_Vertex_B1165(
        float2 UV,
        float Radius,
        float Margin,
        float Anisotropy,
        float Gradient1,
        float Gradient2,
        float3 Normal,
        float4 Color_Scale_Translate,
        out float2 Rect_UV,
        out float4 Rect_Parms,
        out float2 Scale_XY,
        out float2 Line_UV,
        out float2 Color_UV_Info    )
    {
        Scale_XY = float2(Anisotropy,1.0);
        Line_UV = (UV - float2(0.5,0.5));
        Rect_UV = Line_UV * Scale_XY;
        Rect_Parms.xy = Scale_XY*0.5-float2(Radius,Radius)-float2(Margin,Margin);
        Rect_Parms.z = Gradient1; //Radius - Line_Width;
        Rect_Parms.w = Gradient2;
        Color_UV_Info = (Line_UV + float2(0.5,0.5)) * Color_Scale_Translate.xy + Color_Scale_Translate.zw;
    }
    //BLOCK_END Round_Rect_Vertex

    //BLOCK_BEGIN Line_Vertex 1166

    void Line_Vertex_B1166(
        float2 Scale_XY,
        float2 UV,
        float Time,
        float Rate,
        float4 Highlight_Transform,
        out float3 Line_Vertex    )
    {
        float angle2 = (Rate*Time) * 2.0 * 3.1416;
        float sinAngle2 = sin(angle2);
        float cosAngle2 = cos(angle2);
        float2 xformUV = UV * Highlight_Transform.xy + Highlight_Transform.zw;
        Line_Vertex.x = 0.0;
        Line_Vertex.y = cosAngle2*xformUV.x-sinAngle2*xformUV.y;
        Line_Vertex.z = 0.0; //sinAngle2*xformUV.x+cosAngle2*xformUV.y;
    }
    //BLOCK_END Line_Vertex

    //BLOCK_BEGIN PickDir 1168

    void PickDir_B1168(
        float Degrees,
        float3 DirX,
        float3 DirY,
        out float3 Dir    )
    {
        float a = Degrees*3.14159/180.0;
        Dir = cos(a)*DirX+sin(a)*DirY;
    }
    //BLOCK_END PickDir

    //BLOCK_BEGIN Move_Verts 1167

    void Move_Verts_B1167(
        float Anisotropy,
        float3 P,
        float Radius,
        out float3 New_P,
        out float2 New_UV,
        out float Radial_Gradient,
        out float3 Radial_Dir    )
    {
        float2 UV = P.xy * 2 + 0.5;
        float2 center = saturate(UV);
        float2 delta = UV - center;        
        float2 r2 = 2.0 * float2(Radius / Anisotropy, Radius);        
        New_UV = center + r2 * (UV - 2 * center + 0.5);
        New_P = float3(New_UV - 0.5, P.z);      
        Radial_Gradient = 1.0 - length(delta) * 2.0;
        Radial_Dir = float3(delta * r2, 0.0);
    }
    //BLOCK_END Move_Verts

    //BLOCK_BEGIN Pick_Radius 1138

    void Pick_Radius_B1138(
        float Radius,
        float Radius_Top_Left,
        float Radius_Top_Right,
        float Radius_Bottom_Left,
        float Radius_Bottom_Right,
        float3 Position,
        out float Result    )
    {
        bool whichY = Position.y>0;
        Result = Position.x<0 ? (whichY ? Radius_Top_Left : Radius_Bottom_Left) : (whichY ? Radius_Top_Right : Radius_Bottom_Right);
        Result *= Radius;
    }
    //BLOCK_END Pick_Radius

    //BLOCK_BEGIN Edge_AA_Vertex 1164

    void Edge_AA_Vertex_B1164(
        float3 Position_World,
        float3 Position_Object,
        float3 Normal_Object,
        float3 Eye,
        float Radial_Gradient,
        float3 Radial_Dir,
        float3 Tangent,
        out float Gradient1,
        out float Gradient2    )
    {
        float3 I = (Eye-Position_World);
        float3 T = UnityObjectToWorldNormal(Tangent);
        float g = (dot(T,I)<0.0) ? 0.0 : 1.0;
        if (Normal_Object.z==0) { // edge
            Gradient1 = Position_Object.z>0.0 ? g : 1.0;
            Gradient2 = Position_Object.z>0.0 ? 1.0 : g;
        } else {
            Gradient1 = g + (1.0-g)*(Radial_Gradient);
            Gradient2 = 1.0;
        }
    }
    //BLOCK_END Edge_AA_Vertex

    //BLOCK_BEGIN Object_To_World_Dir 1135

    void Object_To_World_Dir_B1135(
        float3 Dir_Object,
        out float3 Binormal_World,
        out float3 Binormal_World_N,
        out float Binormal_Length    )
    {
        Binormal_World = (mul((float3x3)UNITY_MATRIX_M, Dir_Object));
        Binormal_Length = length(Binormal_World);
        Binormal_World_N = Binormal_World / Binormal_Length;
    }
    //BLOCK_END Object_To_World_Dir

    //BLOCK_BEGIN RelativeOrAbsoluteDetail 1163

    void RelativeOrAbsoluteDetail_B1163(
        float Nominal_Radius,
        float Nominal_LineWidth,
        bool Absolute_Measurements,
        float Height,
        out float Radius,
        out float Line_Width    )
    {
        float scale = Absolute_Measurements ? 1.0/Height : 1.0;
        Radius = Nominal_Radius * scale;
        Line_Width = Nominal_LineWidth * scale;
    }
    //BLOCK_END RelativeOrAbsoluteDetail


    VertexOutput vert(VertexInput vertInput)
    {
        UNITY_SETUP_INSTANCE_ID(vertInput);
        VertexOutput o;
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

        // Object_To_World_Dir (#1133)
        float3 Nrm_World_Q1133;
        Nrm_World_Q1133 = normalize((mul((float3x3)UNITY_MATRIX_M, vertInput.normal)));
        
        // Object_To_World_Dir (#1134)
        float3 Tangent_World_Q1134;
        float3 Tangent_World_N_Q1134;
        float Tangent_Length_Q1134;
        Tangent_World_Q1134 = (mul((float3x3)UNITY_MATRIX_M, float3(1,0,0)));
        Tangent_Length_Q1134 = length(Tangent_World_Q1134);
        Tangent_World_N_Q1134 = Tangent_World_Q1134 / Tangent_Length_Q1134;

        float3 Binormal_World_Q1135;
        float3 Binormal_World_N_Q1135;
        float Binormal_Length_Q1135;
        Object_To_World_Dir_B1135(float3(0,1,0),Binormal_World_Q1135,Binormal_World_N_Q1135,Binormal_Length_Q1135);

        float Radius_Q1163;
        float Line_Width_Q1163;
        RelativeOrAbsoluteDetail_B1163(_Radius_,_Line_Width_,_Absolute_Sizes_,Binormal_Length_Q1135,Radius_Q1163,Line_Width_Q1163);

        float3 Dir_Q1168;
        #if defined(_IRIDESCENCE_ENABLE_)
          PickDir_B1168(_Angle_,Tangent_World_N_Q1134,Binormal_World_N_Q1135,Dir_Q1168);
        #else
          Dir_Q1168 = float3(0,0,0);
        #endif

        float Result_Q1138;
        Pick_Radius_B1138(Radius_Q1163,_Radius_Top_Left_,_Radius_Top_Right_,_Radius_Bottom_Left_,_Radius_Bottom_Right_,vertInput.vertex.xyz,Result_Q1138);

        // Divide (#1136)
        float Anisotropy_Q1136 = Tangent_Length_Q1134 / Binormal_Length_Q1135;

        // From_RGBA (#1139)
        float4 Out_Color_Q1139 = float4(Result_Q1138, Line_Width_Q1163, 0, 1);

        float3 New_P_Q1167;
        float2 New_UV_Q1167;
        float Radial_Gradient_Q1167;
        float3 Radial_Dir_Q1167;
        Move_Verts_B1167(Anisotropy_Q1136,vertInput.vertex.xyz,Result_Q1138,New_P_Q1167,New_UV_Q1167,Radial_Gradient_Q1167,Radial_Dir_Q1167);

        float3 Pos_World_Q1123;
        Object_To_World_Pos_B1123(New_P_Q1167,Pos_World_Q1123);

        float Gradient1_Q1164;
        float Gradient2_Q1164;
        #if defined(_SMOOTH_EDGES_)
          Edge_AA_Vertex_B1164(Pos_World_Q1123,vertInput.vertex.xyz,vertInput.normal,_WorldSpaceCameraPos,Radial_Gradient_Q1167,Radial_Dir_Q1167,vertInput.tangent,Gradient1_Q1164,Gradient2_Q1164);
        #else
          Gradient1_Q1164 = 1;
          Gradient2_Q1164 = 1;
        #endif

        float2 Rect_UV_Q1165;
        float4 Rect_Parms_Q1165;
        float2 Scale_XY_Q1165;
        float2 Line_UV_Q1165;
        float2 Color_UV_Info_Q1165;
        Round_Rect_Vertex_B1165(New_UV_Q1167,Result_Q1138,0,Anisotropy_Q1136,Gradient1_Q1164,Gradient2_Q1164,vertInput.normal,float4(1,1,0,0),Rect_UV_Q1165,Rect_Parms_Q1165,Scale_XY_Q1165,Line_UV_Q1165,Color_UV_Info_Q1165);

        float3 Line_Vertex_Q1166;
        #if defined(_LINE_ENABLE_)
          Line_Vertex_B1166(Scale_XY_Q1165,Line_UV_Q1165,_Time.y,_Rate_,_Highlight_Transform_,Line_Vertex_Q1166);
        #else
          Line_Vertex_Q1166 = float3(0,0,0);
        #endif

        // To_XY (#1155)
        float X_Q1155;
        float Y_Q1155;
        X_Q1155 = Color_UV_Info_Q1165.x;
        Y_Q1155 = Color_UV_Info_Q1165.y;

        // From_XYZW (#1154)
        float4 Vec4_Q1154 = float4(X_Q1155, Y_Q1155, Result_Q1138, Line_Width_Q1163);

        float3 Position = Pos_World_Q1123;
        float3 Normal = Nrm_World_Q1133;
        float2 UV = Rect_UV_Q1165;
        float3 Tangent = Line_Vertex_Q1166;
        float3 Binormal = Dir_Q1168;
        float4 Color = Out_Color_Q1139;
        float4 Extra1 = Rect_Parms_Q1165;
        float4 Extra2 = Vec4_Q1154;
        float4 Extra3 = float4(0,0,0,0);

        o.pos = mul(UNITY_MATRIX_VP, float4(Position,1));
        o.posWorld = Position;
        o.normalWorld.xyz = Normal; o.normalWorld.w=1.0;
        o.uv = UV;
        o.tangent.xyz = Tangent; o.tangent.w=1.0;
        o.binormal.xyz = Binormal; o.binormal.w=1.0;
        o.extra1=Extra1;
        o.extra2=Extra2;

        return o;
    }

    //BLOCK_BEGIN Round_Rect_Fragment 1160

    void Round_Rect_Fragment_B1160(
        half Radius,
        half Line_Width,
        half Filter_Width,
        float2 UV,
        half4 Rect_Parms,
        out half InLine    )
    {
        float d = length(max(abs(UV)-Rect_Parms.xy,0.0));
        float dx = max(fwidth(d)*Filter_Width,0.00001);
        InLine = saturate((d+dx*0.5-max(Radius-Line_Width,d-dx*0.5))/dx);
    }
    //BLOCK_END Round_Rect_Fragment

    //BLOCK_BEGIN Smooth_Edges 1162

    void Smooth_Edges_B1162(
        half Filter_Width,
        half4 Rect_Parms,
        out half Inside_Rect    )
    {
        float g = min(Rect_Parms.z,Rect_Parms.w);
        float dgrad = max(fwidth(g)*Filter_Width,0.00001);
        Inside_Rect = saturate(g/dgrad);
    }
    //BLOCK_END Smooth_Edges

    //BLOCK_BEGIN Iridescence 1143

    void Iridescence_B1143(
        half3 Position,
        half3 Normal,
        half2 UV,
        half3 Axis,
        half3 Eye,
        half4 Tint,
        sampler2D Texture,
        bool Reflected,
        half Frequency,
        half Vertical_Offset,
        out half4 Color    )
    {
        
        half3 i = normalize(Position-Eye);
        half3 r = reflect(i,Normal);
        half idota = dot(i,Axis);
        half idotr = dot(i,r);
        
        half x = Reflected ? idotr : idota;
        
        half2 xy;
        xy.x = frac((x*Frequency+1.0)*0.5 + UV.y*Vertical_Offset);
        xy.y = 0.5;
        
        Color = tex2D(Texture,xy);
        Color.rgb*=Tint.rgb;
    }
    //BLOCK_END Iridescence

    //BLOCK_BEGIN Scale_RGB 1146

    void Scale_RGB_B1146(
        half4 Color,
        half Scalar,
        out half4 Result    )
    {
        Result = float4(Scalar,Scalar,Scalar,1) * Color;
    }
    //BLOCK_END Scale_RGB

    //BLOCK_BEGIN Scale_RGB 1144

    void Scale_RGB_B1144(
        half Scalar,
        half4 Color,
        out half4 Result    )
    {
        Result = float4(Scalar,Scalar,Scalar,1) * Color;
    }
    //BLOCK_END Scale_RGB

    //BLOCK_BEGIN Line_Fragment 1157

    void Line_Fragment_B1157(
        half4 Base_Color,
        half4 Highlight_Color,
        half Highlight_Width,
        half3 Line_Vertex,
        half Highlight,
        out half4 Line_Color    )
    {
        half k2 = 1.0-saturate(abs(Line_Vertex.y/Highlight_Width));
        Line_Color = lerp(Base_Color,Highlight_Color,float4(Highlight*k2,Highlight*k2,Highlight*k2,Highlight*k2));
    }
    //BLOCK_END Line_Fragment

    //BLOCK_BEGIN Gradient 1158

    void Gradient_B1158(
        half4 Gradient_Color,
        half4 Top_Left,
        half4 Top_Right,
        half4 Bottom_Left,
        half4 Bottom_Right,
        half2 UV,
        out half4 Result    )
    {
        half3 top = Top_Left.rgb + (Top_Right.rgb - Top_Left.rgb)*UV.x;
        half3 bottom = Bottom_Left.rgb + (Bottom_Right.rgb - Bottom_Left.rgb)*UV.x;
        Result.rgb = Gradient_Color.rgb * (bottom + (top - bottom)*UV.y);
        Result.a = 1.0;
    }
    //BLOCK_END Gradient

    //BLOCK_BEGIN Edge 1153

    void Edge_B1153(
        float4 RectParms,
        half Radius,
        half Line_Width,
        float2 UV,
        half Edge_Width,
        half Edge_Power,
        out half Result    )
    {
        half d = length(max(abs(UV)-RectParms.xy,0.0));
        half edge = 1.0-saturate((1.0-d/(Radius-Line_Width))/Edge_Width);
        Result = pow(edge, Edge_Power);
        
    }
    //BLOCK_END Edge


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

        half Inside_Rect_Q1162;
        #if defined(_SMOOTH_EDGES_)
          Smooth_Edges_B1162(1,fragInput.extra1,Inside_Rect_Q1162);
        #else
          Inside_Rect_Q1162 = 1.0;
        #endif

        // To_XYZW (#1140)
        float X_Q1140;
        float Y_Q1140;
        float Z_Q1140;
        float W_Q1140;
        X_Q1140=fragInput.extra2.x;
        Y_Q1140=fragInput.extra2.y;
        Z_Q1140=fragInput.extra2.z;
        W_Q1140=fragInput.extra2.w;

        half4 Color_Q1143;
        #if defined(_IRIDESCENCE_ENABLE_)
          Iridescence_B1143(fragInput.posWorld,fragInput.normalWorld.xyz,fragInput.uv,fragInput.binormal.xyz,_WorldSpaceCameraPos,_Iridescence_Tint_,_Iridescent_Map_,_Reflected_,_Frequency_,_Vertical_Offset_,Color_Q1143);
        #else
          Color_Q1143 = half4(0,0,0,0);
        #endif

        half4 Result_Q1144;
        Scale_RGB_B1144(_Iridescence_Intensity_,Color_Q1143,Result_Q1144);

        half4 Line_Color_Q1157;
        #if defined(_LINE_ENABLE_)
          Line_Fragment_B1157(_Line_Color_,_Highlight_Color_,_Highlight_Width_,fragInput.tangent.xyz,_Highlight_,Line_Color_Q1157);
        #else
          Line_Color_Q1157 = half4(0,0,0,1);
        #endif

        half Result_Q1153;
        #if defined(_EDGE_ONLY_)
          Edge_B1153(fragInput.extra1,Z_Q1140,W_Q1140,fragInput.uv,_Edge_Width_,_Edge_Power_,Result_Q1153);
        #else
          Result_Q1153 = 1;
        #endif

        // From_XY (#1141)
        float2 Vec2_Q1141 = float2(X_Q1140,Y_Q1140);

        // Multiply (#1161)
        half Product_Q1161 = Inside_Rect_Q1162 * _Fade_Out_;

        half InLine_Q1160;
        #if defined(_LINE_ENABLE_)
          Round_Rect_Fragment_B1160(Z_Q1140,W_Q1140,_Filter_Width_,fragInput.uv,fragInput.extra1,InLine_Q1160);
        #else
          InLine_Q1160 = 0.0;
        #endif

        half4 Result_Q1158;
        #if defined(_GRADIENT_ENABLE_)
          Gradient_B1158(_Gradient_Color_,_Top_Left_,_Top_Right_,_Bottom_Left_,_Bottom_Right_,Vec2_Q1141,Result_Q1158);
        #else
          Result_Q1158 = half4(0,0,0,0);
        #endif

        half4 Result_Q1146;
        Scale_RGB_B1146(Result_Q1158,Result_Q1153,Result_Q1146);

        // Add_Colors (#1145)
        half4 Sum_Q1145 = Result_Q1146 + Result_Q1144;

        // Mix_Colors (#1147)
        half4 Color_At_T_Q1147 = lerp(Line_Color_Q1157, Result_Q1146,float4( _Line_Gradient_Blend_, _Line_Gradient_Blend_, _Line_Gradient_Blend_, _Line_Gradient_Blend_));

        // Add_Colors (#1149)
        half4 Base_And_Iridescent_Q1149;
        Base_And_Iridescent_Q1149 = _Base_Color_ + float4(Sum_Q1145.rgb,0.0);
        
        // Add_Scaled_Color (#1148)
        half4 Sum_Q1148 = Color_At_T_Q1147 + _Iridescence_Edge_Intensity_ * Color_Q1143;

        // Set_Alpha (#1150)
        half4 Result_Q1150 = Sum_Q1148; Result_Q1150.a = 1;

        // Mix_Colors (#1159)
        half4 Color_At_T_Q1159 = lerp(Base_And_Iridescent_Q1149, Result_Q1150,float4( InLine_Q1160, InLine_Q1160, InLine_Q1160, InLine_Q1160));

        // Scale_Color (#1152)
        half4 Result_Q1152 = Product_Q1161 * Color_At_T_Q1159;

        float4 Out_Color = Result_Q1152;
        float Clip_Threshold = 0.001;
        bool To_sRGB = false;

        result = Out_Color;
        return result;
    }

    ENDCG
  }
 }
}
