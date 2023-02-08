// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

Shader "Graphics Tools/Canvas/Backplate" {

Properties {

    [Header(Round Rect)]
        _Base_Color_("Base Color", Color) = (0,0,0,1)
        [Toggle(_LINE_DISABLED_)] _Line_Disabled_("Line Disabled", Float) = 0
        _Line_Width_("Line Width", Range(0,10)) = 1
        _Line_Color_("Line Color", Color) = (0,0,0,1)
        _Filter_Width_("Filter Width", Range(0,4)) = 1
     
    [Header(Line Highlight)]
        _Rate_("Rate", Range(0,1)) = 0
        _Highlight_Color_("Highlight Color", Color) = (0.98,0.98,0.98,1)
        _Highlight_Width_("Highlight Width", Range(0,2)) = 0.25
        _Highlight_Transform_("Highlight Transform", Vector) = (1, 1, 0, 0)
        _Highlight_("Highlight", Range(0,1)) = 1
     
    [Header(Iridescence)]
        [Toggle(_IRIDESCENCE_ENABLE_)] _Iridescence_Enable_("Iridescence Enable", Float) = 1
        _Iridescence_Intensity_("Iridescence Intensity", Range(0,1)) = 0
        _Iridescence_Edge_Intensity_("Iridescence Edge Intensity", Range(0,1)) = 0.56
        _Iridescence_Tint_("Iridescence Tint", Color) = (1,1,1,1)
        [NoScaleOffset] _Iridescent_Map_("Iridescent Map", 2D) = "" {}
        _Frequency_("Frequency", Range(0,10)) = 1
        _Vertical_Offset_("Vertical Offset", Range(0,2)) = 0
        _Orthographic_Distance_("Orthographic Distance", Float) = 400
     
    [Header(Gradient)]
        [Toggle(_GRADIENT_DISABLED_)] _Gradient_Disabled_("Gradient Disabled", Float) = 0
        _Gradient_Color_("Gradient Color", Color) = (0.631373,0.631373,0.631373,1)
        _Top_Left_("Top Left", Color) = (1,0.690196,0.976471,1)
        _Top_Right_("Top Right", Color) = (0.0,0.33,0.88,1)
        _Bottom_Left_("Bottom Left", Color) = (0.0,0.33,0.88,1)
        _Bottom_Right_("Bottom Right", Color) = (1,1,1,1)
        [Toggle(_EDGE_ONLY_)] _Edge_Only_("Edge Only", Float) = 0
        _Line_Gradient_Blend_("Line Gradient Blend", Range(0,1)) = 0.36
     
    [Header(Fade)]
        _Fade_Out_("Fade Out", Range(0,1)) = 1
     
    [Header(Antialiasing)]
        [Toggle(_SMOOTH_EDGES_)] _Smooth_Edges_("Smooth Edges", Float) = 0
     
    [Header(Occlusion)]
        _Occluded_Intensity_("Occluded Intensity", Range(0,1)) = 1
        [NoScaleOffset] _OccludedTex("OccludedTex", 2D) = "" {}
        _OccludedColor("OccludedColor", Color) = (0,0.5,1,1)
        _GridScale("GridScale", Float) = 0.02
     

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

    [HideInInspector] _MainTex("Texture", 2D) = "white" {} // Added to avoid UnityUI warnings.
    [HideInInspector] _ClipRect("Clip Rect", Vector) = (-32767.0, -32767.0, 32767.0, 32767.0) // Added to avoid SRP warnings.
    [HideInInspector] _ClipRectRadii("Clip Rect Radii", Vector) = (10.0, 10.0, 10.0, 10.0) // Added to avoid SRP warnings.
}

SubShader {
    Tags{ "RenderType" = "Opaque" }
    Blend[_SrcBlend][_DstBlend],[_SrcBlendAlpha][_DstBlendAlpha]
    Tags {"DisableBatching" = "True"}
    Stencil
    {
        Ref[_StencilReference]
        Comp[_StencilComparison]
        Pass[_StencilOperation]
    }

    LOD 100

    CGINCLUDE

    #pragma target 4.0
    #pragma shader_feature_local _ _IRIDESCENCE_ENABLE_
    #pragma shader_feature_local _ _LINE_DISABLED_
    #pragma shader_feature_local _ _GRADIENT_DISABLED_
    #pragma shader_feature_local _ _EDGE_ONLY_
    #pragma shader_feature_local _ _SMOOTH_EDGES_

    #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
    #pragma multi_compile_local _ _UI_CLIP_RECT_ROUNDED _UI_CLIP_RECT_ROUNDED_INDEPENDENT
    //#pragma multi_compile_local _ _CLIPPING_PLANE _CLIPPING_SPHERE _CLIPPING_BOX

    #include "UnityCG.cginc"
    #include "GraphicsToolsCommon.hlsl"

CBUFFER_START(UnityPerMaterial)
    half4 _Base_Color_;
    //bool _Line_Disabled_;
    float _Line_Width_;
    half4 _Line_Color_;
    half _Filter_Width_;
    float _Rate_;
    half4 _Highlight_Color_;
    half _Highlight_Width_;
    float4 _Highlight_Transform_;
    half _Highlight_;
    //bool _Iridescence_Enable_;
    half _Iridescence_Intensity_;
    half _Iridescence_Edge_Intensity_;
    half4 _Iridescence_Tint_;
    sampler2D _Iridescent_Map_;
    float _Frequency_;
    float _Vertical_Offset_;
    float _Orthographic_Distance_;
    //bool _Gradient_Disabled_;
    half4 _Gradient_Color_;
    half4 _Top_Left_;
    half4 _Top_Right_;
    half4 _Bottom_Left_;
    half4 _Bottom_Right_;
    //bool _Edge_Only_;
    half _Line_Gradient_Blend_;
    float _Fade_Out_;
    //bool _Smooth_Edges_;
    half _Occluded_Intensity_;
    sampler2D _OccludedTex;
    half4 _OccludedColor;
    float _GridScale;
    // #if defined(UNITY_UI_CLIP_RECT)
    float4 _ClipRect;
    // #if defined(_UI_CLIP_RECT_ROUNDED) || defined(_UI_CLIP_RECT_ROUNDED_INDEPENDENT)
    float4 _ClipRectRadii;

CBUFFER_END


    struct VertexInput {
        float4 vertex : POSITION;
        half3 normal : NORMAL;
        float2 uv0 : TEXCOORD0;
        float2 uv2 : TEXCOORD2;
        float2 uv3 : TEXCOORD3;
        float4 tangent : TANGENT;
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
        float3 posWorld : TEXCOORD7;
        float4 tangent : TANGENT;
        float4 vertexColor : COLOR;
        float4 extra1 : TEXCOORD4;
        float4 extra2 : TEXCOORD3;
        float4 extra3 : TEXCOORD2;
      UNITY_VERTEX_OUTPUT_STEREO
    };

    //BLOCK_BEGIN Round_Rect_Vertex 31

    void Round_Rect_Vertex_B31(
        float2 UV,
        float Radius,
        float Anisotropy,
        out float2 Rect_UV,
        out float4 Rect_Parms,
        out float2 Scale_XY,
        out float2 Line_UV    )
    {
        Scale_XY = float2(Anisotropy,1.0);
        Line_UV = (UV - float2(0.5,0.5));
        Rect_UV = Line_UV * Scale_XY;
        Rect_Parms.xy = Scale_XY*0.5-float2(Radius,Radius);
        Rect_Parms.z = 0.0;
        Rect_Parms.w = 0.0;
        
    }
    //BLOCK_END Round_Rect_Vertex

    //BLOCK_BEGIN Gradient 45

    void Gradient_B45(
        half4 Gradient_Color,
        half4 Top_Left,
        half4 Top_Right,
        half4 Bottom_Left,
        half4 Bottom_Right,
        half2 UV,
        out half3 Result    )
    {
        half3 top = Top_Left.rgb + (Top_Right.rgb - Top_Left.rgb)*UV.x;
        half3 bottom = Bottom_Left.rgb + (Bottom_Right.rgb - Bottom_Left.rgb)*UV.x;
        Result = Gradient_Color.rgb * (bottom + (top - bottom)*UV.y);
        
    }
    //BLOCK_END Gradient

    //BLOCK_BEGIN Line_Vertex 37

    void Line_Vertex_B37(
        float2 Scale_XY,
        float2 UV,
        float Time,
        float Rate,
        float4 Highlight_Transform,
        out float4 Line_Vertex    )
    {
        float angle2 = (Rate*Time) * 2.0 * 3.1416;
        float sinAngle2 = sin(angle2);
        float cosAngle2 = cos(angle2);
        float2 xformUV = UV * Highlight_Transform.xy + Highlight_Transform.zw;
        Line_Vertex.x = 0.0;
        Line_Vertex.y = 0.0;
        Line_Vertex.z = cosAngle2*xformUV.x-sinAngle2*xformUV.y;
        Line_Vertex.w = 0.0; //sinAngle2*xformUV.x+cosAngle2*xformUV.y;
    }
    //BLOCK_END Line_Vertex

    //BLOCK_BEGIN RelativeOrAbsoluteDetail 28

    void RelativeOrAbsoluteDetail_B28(
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

    //BLOCK_BEGIN Edge_AA_Vertex 64

    void Edge_AA_Vertex_B64(
        float3 Position_World,
        float3 Normal_Object,
        float3 Eye,
        float3 Tangent,
        out float Gradient1,
        out float Gradient2    )
    {
        float3 I = (Eye-Position_World);
        float3 T = UnityObjectToWorldNormal(Tangent);
        float g = (dot(T,I)<=0.0) ? 0.0 : 1.0;
        if (Normal_Object.z==0) { // edge
            Gradient1 = Tangent.z>0.0 ? g : 1.0;
            Gradient2 = Tangent.z>0.0 ? 1.0 : g;
        } else {
            Gradient1 = (unity_OrthoParams.w) ? (Tangent.z==0 ? 0 : 1) : g;
            Gradient2 = 1.0;
        }
    }
    //BLOCK_END Edge_AA_Vertex


    VertexOutput vert(VertexInput vertInput)
    {
        UNITY_SETUP_INSTANCE_ID(vertInput);
        VertexOutput o;
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

        // Object_To_World_Pos (#29)
        float3 Pos_World_Q29=(mul(UNITY_MATRIX_M, float4(vertInput.vertex.xyz*float3(float2(1,1).x, float2(1,1).y, 1.0), 1)));

        // Object_To_World_Dir (#32)
        float3 Nrm_World_Q32;
        #if defined(_IRIDESCENCE_ENABLE_)
          Nrm_World_Q32 = normalize((mul((float3x3)UNITY_MATRIX_M, vertInput.normal)));
          
        #else
          Nrm_World_Q32 = float3(0,0,1);
        #endif

        // To_XY (#26)
        float X_Q26;
        float Y_Q26;
        X_Q26 = vertInput.uv3.x;
        Y_Q26 = vertInput.uv3.y;

        half3 Result_Q45;
        #if defined(_EDGE_ONLY_)
          Gradient_B45(_Gradient_Color_,_Top_Left_,_Top_Right_,_Bottom_Left_,_Bottom_Right_,vertInput.uv0,Result_Q45);
        #else
          Result_Q45 = half3(0,0,0);
        #endif

        // Permutation_To_Bool (#47)
        bool Bool_Q47;
        #if defined(_GRADIENT_DISABLED_)
          Bool_Q47 = true;
        #else
          Bool_Q47 = false;
        #endif

        // Iridescence_View_Origin (#84)
        float3 Ir_Eye_Q84 = (unity_OrthoParams.w) ? (mul(UNITY_MATRIX_M, float4(float3(0,0,-_Orthographic_Distance_), 1))) : _WorldSpaceCameraPos;

        // To_XY (#23)
        float X_Q23;
        float Y_Q23;
        X_Q23 = vertInput.uv0.x;
        Y_Q23 = vertInput.uv0.y;

        // To_XYZ (#78)
        float X_Q78;
        float Y_Q78;
        float Z_Q78;
        X_Q78=vertInput.vertex.xyz.x;
        Y_Q78=vertInput.vertex.xyz.y;
        Z_Q78=vertInput.vertex.xyz.z;
        
        float Gradient1_Q64;
        float Gradient2_Q64;
        #if defined(_SMOOTH_EDGES_)
          Edge_AA_Vertex_B64(Pos_World_Q29,vertInput.normal,_WorldSpaceCameraPos,vertInput.tangent,Gradient1_Q64,Gradient2_Q64);
        #else
          Gradient1_Q64 = 1;
          Gradient2_Q64 = 1;
        #endif

        // To_XY (#24)
        float X_Q24;
        float Y_Q24;
        X_Q24 = vertInput.uv2.x;
        Y_Q24 = vertInput.uv2.y;

        // To_RGBA (#100)
        float R_Q100;
        float G_Q100;
        float B_Q100;
        float A_Q100;
        R_Q100=vertInput.color.r; G_Q100=vertInput.color.g; B_Q100=vertInput.color.b; A_Q100=vertInput.color.a;

        // Conditional_Vec3 (#48)
        float3 Result_Q48 = Bool_Q47 ? float3(0,0,0) : Result_Q45;

        // From_XYZW (#59)
        float4 Vec4_Q59 = float4(X_Q78, Y_Q78, Gradient1_Q64, Gradient2_Q64);

        // Divide (#18)
        float Anisotropy_Q18 = X_Q24 / Y_Q24;

        // Multiply (#101)
        float Product_Q101 = A_Q100 * _Fade_Out_;

        float Radius_Q28;
        float Line_Width_Q28;
        RelativeOrAbsoluteDetail_B28(0.0,_Line_Width_,true,Y_Q24,Radius_Q28,Line_Width_Q28);

        float2 Rect_UV_Q31;
        float4 Rect_Parms_Q31;
        float2 Scale_XY_Q31;
        float2 Line_UV_Q31;
        Round_Rect_Vertex_B31(vertInput.uv0,X_Q26,Anisotropy_Q18,Rect_UV_Q31,Rect_Parms_Q31,Scale_XY_Q31,Line_UV_Q31);

        // From_Rgb_A (#85)
        float4 Result_Q85;
        Result_Q85.rgb = Ir_Eye_Q84;
        Result_Q85.a = Product_Q101;

        // From_XYZW (#22)
        float4 Vec4_Q22 = float4(X_Q23, Y_Q23, X_Q26, Line_Width_Q28);

        float4 Line_Vertex_Q37;
        #if defined(_LINE_DISABLED_)
          Line_Vertex_Q37 = float4(0,0,0,0);
        #else
          Line_Vertex_B37(Scale_XY_Q31,Line_UV_Q31,_Time.y,_Rate_,_Highlight_Transform_,Line_Vertex_Q37);
        #endif

        // Add4 (#30)
        float4 Sum4_Q30 = Rect_Parms_Q31 + Line_Vertex_Q37;

        float3 Position = Pos_World_Q29;
        float3 Normal = Nrm_World_Q32;
        float2 UV = Rect_UV_Q31;
        float3 Tangent = Result_Q48;
        float3 Binormal = float3(0,0,0);
        float4 Color = Result_Q85;
        float4 Extra1 = Sum4_Q30;
        float4 Extra2 = Vec4_Q22;
        float4 Extra3 = Vec4_Q59;

        o.pos = mul(UNITY_MATRIX_VP, float4(Position,1));
#ifdef UNITY_UI_CLIP_RECT
        o.posLocal = vertInput.vertex.xyz;
#endif
        o.posWorld = Position;
        o.normalWorld.xyz = Normal; o.normalWorld.w=1.0;
        o.uv = UV;
        o.tangent.xyz = Tangent; o.tangent.w=1.0;
        o.vertexColor = Color;
        o.extra1=Extra1;
        o.extra2=Extra2;
        o.extra3=Extra3;

        return o;
    }

    ENDCG

    Pass
    {
        Name "Default"
    ZWrite[_ZWrite]
    ZTest[_ZTest]

        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag

    //BLOCK_BEGIN Smooth_Edges 62

    void Smooth_Edges_B62(
        half Filter_Width,
        half4 Rect_Parms,
        out half Inside_Rect    )
    {
        float g = min(Rect_Parms.z,Rect_Parms.w);
        float dgrad = max(fwidth(g)*Filter_Width,0.001);
        Inside_Rect = saturate(g/dgrad);
    }
    //BLOCK_END Smooth_Edges

    //BLOCK_BEGIN Round_Rect_Fragment 63

    void Round_Rect_Fragment_B63(
        half Radius,
        half Line_Width,
        half Filter_Width,
        float2 UV,
        float4 Rect_Parms,
        out half InLine    )
    {
        half d = length(max(abs(UV)-Rect_Parms.xy,0.0));
        half dx = max(fwidth(d)*Filter_Width,0.001);
        InLine = saturate((d+dx*0.5-max(Radius-Line_Width,d-dx*0.5))/dx);
        
    }
    //BLOCK_END Round_Rect_Fragment

    //BLOCK_BEGIN Iridescence 77

    void Iridescence_B77(
        half3 Position,
        half3 Normal,
        half2 UV,
        half3 Eye,
        half4 Tint,
        sampler2D Texture,
        half Frequency,
        half Vertical_Offset,
        out half3 Color    )
    {
        half3 i = normalize(Position-Eye);
        half3 r = reflect(i,Normal);
        half x = dot(i,r);
        
        half2 xy;
        xy.x = frac((x*Frequency+1.0)*0.5 + UV.y*Vertical_Offset);
        xy.y = 0.5;
        
        Color = tex2D(Texture,xy).rgb;
        Color *= Tint.rgb;
    }
    //BLOCK_END Iridescence

    //BLOCK_BEGIN Line_Fragment 36

    void Line_Fragment_B36(
        half4 Base_Color,
        half4 Highlight_Color,
        half Highlight_Width,
        half4 Line_Vertex,
        half Highlight,
        out half4 Line_Color    )
    {
        half k2 = 1.0-saturate(abs(Line_Vertex.z/Highlight_Width));
        Line_Color = lerp(Base_Color,Highlight_Color,float4(Highlight*k2,Highlight*k2,Highlight*k2,Highlight*k2));
    }
    //BLOCK_END Line_Fragment

    //BLOCK_BEGIN Gradient 57

    void Gradient_B57(
        half4 Gradient_Color,
        half4 Top_Left,
        half4 Top_Right,
        half4 Bottom_Left,
        half4 Bottom_Right,
        half2 UV,
        out half3 Gradient    )
    {
        half3 top = Top_Left.rgb + (Top_Right.rgb - Top_Left.rgb)*UV.x;
        half3 bottom = Bottom_Left.rgb + (Bottom_Right.rgb - Bottom_Left.rgb)*UV.x;
        Gradient = Gradient_Color.rgb * (bottom + (top - bottom)*UV.y);
    }
    //BLOCK_END Gradient


    half4 frag(VertexOutput fragInput) : SV_Target
    {
        ClipAgainstPrimitive(fragInput.posWorld);

    #ifdef UNITY_UI_CLIP_RECT
        clip(GTUnityUIClipRect(fragInput.posLocal.xy, _ClipRect, _ClipRectRadii) - 0.5);
    #endif
        half4 result;

        half Inside_Rect_Q62;
        #if defined(_SMOOTH_EDGES_)
          Smooth_Edges_B62(1,fragInput.extra3,Inside_Rect_Q62);
        #else
          Inside_Rect_Q62 = 1.0;
        #endif

        // To_RGBA (#96)
        float R_Q96;
        float G_Q96;
        float B_Q96;
        float A_Q96;
        R_Q96=fragInput.vertexColor.r; G_Q96=fragInput.vertexColor.g; B_Q96=fragInput.vertexColor.b; A_Q96=fragInput.vertexColor.a;

        // To_XYZW (#19)
        float X_Q19;
        float Y_Q19;
        float Z_Q19;
        float W_Q19;
        X_Q19=fragInput.extra2.x;
        Y_Q19=fragInput.extra2.y;
        Z_Q19=fragInput.extra2.z;
        W_Q19=fragInput.extra2.w;

        half4 Line_Color_Q36;
        #if defined(_LINE_DISABLED_)
          Line_Color_Q36 = half4(0,0,0,1);
        #else
          Line_Fragment_B36(_Line_Color_,_Highlight_Color_,_Highlight_Width_,fragInput.extra1,_Highlight_,Line_Color_Q36);
        #endif

        // From_XYZ (#95)
        float3 Vec3_Q95 = float3(R_Q96,G_Q96,B_Q96);

        // Permutation_To_Bool (#46)
        bool Bool_Q46;
        #if defined(_GRADIENT_DISABLED_)
          Bool_Q46 = true;
        #else
          Bool_Q46 = false;
        #endif

        // From_XY (#20)
        float2 Vec2_Q20 = float2(X_Q19,Y_Q19);

        // Multiply (#92)
        half Product_Q92 = Inside_Rect_Q62 * A_Q96;

        half InLine_Q63;
        #if defined(_LINE_DISABLED_)
          InLine_Q63 = 0.0;
        #else
          Round_Rect_Fragment_B63(Z_Q19,W_Q19,_Filter_Width_,fragInput.uv,fragInput.extra1,InLine_Q63);
        #endif

        half3 Color_Q77;
        #if defined(_IRIDESCENCE_ENABLE_)
          Iridescence_B77(fragInput.posWorld,fragInput.normalWorld.xyz,fragInput.uv,Vec3_Q95,_Iridescence_Tint_,_Iridescent_Map_,_Frequency_,_Vertical_Offset_,Color_Q77);
        #else
          Color_Q77 = half3(0,0,0);
        #endif

        // Scale3 (#52)
        half3 Result_Q52 = _Iridescence_Intensity_ * Color_Q77;

        half3 Gradient_Q57;
        #if defined(_EDGE_ONLY_)
          Gradient_Q57 = half3(0,0,0);
        #else
          Gradient_B57(_Gradient_Color_,_Top_Left_,_Top_Right_,_Bottom_Left_,_Bottom_Right_,Vec2_Q20,Gradient_Q57);
        #endif

        // Add_Colors (#58)
        half3 Interior_Gradient_Q58;
        half3 Edge_Gradient_Q58;
        Interior_Gradient_Q58 = Bool_Q46 ? half3(0,0,0) : Gradient_Q57.rgb;
        Edge_Gradient_Q58 = Bool_Q46 ? half3(0,0,0) : fragInput.tangent.xyz + Gradient_Q57.rgb;

        // Mix_Colors (#49)
        half3 Color_At_T_Q49 = Line_Color_Q36.rgb + (Edge_Gradient_Q58-Line_Color_Q36.rgb)*_Line_Gradient_Blend_;

        // Add3 (#50)
        half3 Sum3_Q50 = Interior_Gradient_Q58 + Result_Q52;

        // Add_Scaled_Color (#53)
        half3 Sum_Q53 = Color_At_T_Q49 + _Iridescence_Edge_Intensity_ * Color_Q77;

        // Add_Colors (#51)
        half3 Base_And_Iridescent_Q51 = _Base_Color_.rgb + Sum3_Q50;

        // Mix3 (#54)
        half3 V_T_Q54 = lerp(Base_And_Iridescent_Q51,Sum_Q53,float3(InLine_Q63,InLine_Q63,InLine_Q63));

        // Set_Alpha (#55)
        half4 Result_Q55 = half4(V_T_Q54, 1);

        // Scale_Color (#60)
        half4 Result_Q60 = Product_Q92 * Result_Q55;

        half4 Out_Color = Result_Q60;
        half Clip_Threshold = 0.001;
        bool To_sRGB = false;

        result = Out_Color;
        return result;
    }

    ENDCG
  }

    Pass
    {
        Name "Occluded"
        Tags { "LightMode" = "UIOccluded" }
        ZTest Greater

        CGPROGRAM
        #pragma vertex vert
        #pragma fragment secondFragment

        //BLOCK_BEGIN Smooth_Edges 62

        void Smooth_Edges_B62(
            half Filter_Width,
            half4 Rect_Parms,
            out half Inside_Rect        )
        {
            float g = min(Rect_Parms.z,Rect_Parms.w);
            float dgrad = max(fwidth(g)*Filter_Width,0.001);
            Inside_Rect = saturate(g/dgrad);
        }
        //BLOCK_END Smooth_Edges

        //BLOCK_BEGIN Round_Rect_Fragment 63

        void Round_Rect_Fragment_B63(
            half Radius,
            half Line_Width,
            half Filter_Width,
            float2 UV,
            float4 Rect_Parms,
            out half InLine        )
        {
            half d = length(max(abs(UV)-Rect_Parms.xy,0.0));
            half dx = max(fwidth(d)*Filter_Width,0.001);
            InLine = saturate((d+dx*0.5-max(Radius-Line_Width,d-dx*0.5))/dx);
            
        }
        //BLOCK_END Round_Rect_Fragment

        //BLOCK_BEGIN Iridescence 77

        void Iridescence_B77(
            half3 Position,
            half3 Normal,
            half2 UV,
            half3 Eye,
            half4 Tint,
            sampler2D Texture,
            half Frequency,
            half Vertical_Offset,
            out half3 Color        )
        {
            half3 i = normalize(Position-Eye);
            half3 r = reflect(i,Normal);
            half x = dot(i,r);
            
            half2 xy;
            xy.x = frac((x*Frequency+1.0)*0.5 + UV.y*Vertical_Offset);
            xy.y = 0.5;
            
            Color = tex2D(Texture,xy).rgb;
            Color *= Tint.rgb;
        }
        //BLOCK_END Iridescence

        //BLOCK_BEGIN Line_Fragment 36

        void Line_Fragment_B36(
            half4 Base_Color,
            half4 Highlight_Color,
            half Highlight_Width,
            half4 Line_Vertex,
            half Highlight,
            out half4 Line_Color        )
        {
            half k2 = 1.0-saturate(abs(Line_Vertex.z/Highlight_Width));
            Line_Color = lerp(Base_Color,Highlight_Color,float4(Highlight*k2,Highlight*k2,Highlight*k2,Highlight*k2));
        }
        //BLOCK_END Line_Fragment

        //BLOCK_BEGIN Gradient 57

        void Gradient_B57(
            half4 Gradient_Color,
            half4 Top_Left,
            half4 Top_Right,
            half4 Bottom_Left,
            half4 Bottom_Right,
            half2 UV,
            out half3 Gradient        )
        {
            half3 top = Top_Left.rgb + (Top_Right.rgb - Top_Left.rgb)*UV.x;
            half3 bottom = Bottom_Left.rgb + (Bottom_Right.rgb - Bottom_Left.rgb)*UV.x;
            Gradient = Gradient_Color.rgb * (bottom + (top - bottom)*UV.y);
        }
        //BLOCK_END Gradient


        half4 secondFragment(VertexOutput fragInput) : SV_Target
        {
        ClipAgainstPrimitive(fragInput.posWorld);

    #ifdef UNITY_UI_CLIP_RECT
        clip(GTUnityUIClipRect(fragInput.posLocal.xy, _ClipRect, _ClipRectRadii) - 0.5);
    #endif
        half4 result;

        half Inside_Rect_Q62;
        #if defined(_SMOOTH_EDGES_)
          Smooth_Edges_B62(1,fragInput.extra3,Inside_Rect_Q62);
        #else
          Inside_Rect_Q62 = 1.0;
        #endif

        // To_RGBA (#96)
        float R_Q96;
        float G_Q96;
        float B_Q96;
        float A_Q96;
        R_Q96=fragInput.vertexColor.r; G_Q96=fragInput.vertexColor.g; B_Q96=fragInput.vertexColor.b; A_Q96=fragInput.vertexColor.a;

        // To_XYZW (#19)
        float X_Q19;
        float Y_Q19;
        float Z_Q19;
        float W_Q19;
        X_Q19=fragInput.extra2.x;
        Y_Q19=fragInput.extra2.y;
        Z_Q19=fragInput.extra2.z;
        W_Q19=fragInput.extra2.w;

        half4 Line_Color_Q36;
        #if defined(_LINE_DISABLED_)
          Line_Color_Q36 = half4(0,0,0,1);
        #else
          Line_Fragment_B36(_Line_Color_,_Highlight_Color_,_Highlight_Width_,fragInput.extra1,_Highlight_,Line_Color_Q36);
        #endif

        // From_XYZ (#95)
        float3 Vec3_Q95 = float3(R_Q96,G_Q96,B_Q96);

        // To_XYZW (#66)
        float X_Q66;
        float Y_Q66;
        float Z_Q66;
        float W_Q66;
        X_Q66=fragInput.extra3.x;
        Y_Q66=fragInput.extra3.y;
        Z_Q66=fragInput.extra3.z;
        W_Q66=fragInput.extra3.w;

        // Permutation_To_Bool (#46)
        bool Bool_Q46;
        #if defined(_GRADIENT_DISABLED_)
          Bool_Q46 = true;
        #else
          Bool_Q46 = false;
        #endif

        // From_XY (#20)
        float2 Vec2_Q20 = float2(X_Q19,Y_Q19);

        // Multiply (#98)
        half Product_Q98 = Inside_Rect_Q62 * A_Q96;

        half InLine_Q63;
        #if defined(_LINE_DISABLED_)
          InLine_Q63 = 0.0;
        #else
          Round_Rect_Fragment_B63(Z_Q19,W_Q19,_Filter_Width_,fragInput.uv,fragInput.extra1,InLine_Q63);
        #endif

        half3 Color_Q77;
        #if defined(_IRIDESCENCE_ENABLE_)
          Iridescence_B77(fragInput.posWorld,fragInput.normalWorld.xyz,fragInput.uv,Vec3_Q95,_Iridescence_Tint_,_Iridescent_Map_,_Frequency_,_Vertical_Offset_,Color_Q77);
        #else
          Color_Q77 = half3(0,0,0);
        #endif

        // From_XY (#67)
        float2 Vec2_Q67 = float2(X_Q66,Y_Q66);

        // Scale3 (#52)
        half3 Result_Q52 = _Iridescence_Intensity_ * Color_Q77;

        half3 Gradient_Q57;
        #if defined(_EDGE_ONLY_)
          Gradient_Q57 = half3(0,0,0);
        #else
          Gradient_B57(_Gradient_Color_,_Top_Left_,_Top_Right_,_Bottom_Left_,_Bottom_Right_,Vec2_Q20,Gradient_Q57);
        #endif

        // Scale2 (#73)
        float2 Result_Q73 = Vec2_Q67 * _GridScale;

        // Add_Colors (#58)
        half3 Interior_Gradient_Q58;
        half3 Edge_Gradient_Q58;
        Interior_Gradient_Q58 = Bool_Q46 ? half3(0,0,0) : Gradient_Q57.rgb;
        Edge_Gradient_Q58 = Bool_Q46 ? half3(0,0,0) : fragInput.tangent.xyz + Gradient_Q57.rgb;

        // Color_Texture (#69)
        half4 Color_Q69 = tex2D(_OccludedTex,Result_Q73);

        // Mix_Colors (#49)
        half3 Color_At_T_Q49 = Line_Color_Q36.rgb + (Edge_Gradient_Q58-Line_Color_Q36.rgb)*_Line_Gradient_Blend_;

        // Add3 (#50)
        half3 Sum3_Q50 = Interior_Gradient_Q58 + Result_Q52;

        // Multiply_Colors (#72)
        half3 Product_Q72 = _OccludedColor.rgb * Color_Q69.rgb;

        // Add_Scaled_Color (#53)
        half3 Sum_Q53 = Color_At_T_Q49 + _Iridescence_Edge_Intensity_ * Color_Q77;

        // Add_Colors (#51)
        half3 Base_And_Iridescent_Q51 = _Base_Color_.rgb + Sum3_Q50;

        // Mix3 (#54)
        half3 V_T_Q54 = lerp(Base_And_Iridescent_Q51,Sum_Q53,float3(InLine_Q63,InLine_Q63,InLine_Q63));

        // Scale3 (#79)
        half3 Result_Q79 = _Occluded_Intensity_ * V_T_Q54;

        // Add3 (#75)
        half3 Sum3_Q75 = Result_Q79 + Product_Q72;

        // Set_Alpha (#71)
        half4 Result_Q71 = half4(Sum3_Q75, 1);

        // Scale_Color (#68)
        half4 Result_Q68 = Product_Q98 * Result_Q71;

        half4 Out_Color = Result_Q68;


        result = Out_Color;
        return result;
        }
        ENDCG
    }

 }
}
