// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

Shader "Graphics Tools/Non-Canvas/Slate" {

Properties {

    [Header(Cursors)]
        _TouchCursor_Color_("TouchCursor_Color", Color) = (0.2,0.44,0.78,1) // Touch cursors are a circle drawn with this color
        _TouchCursor_Scale_("TouchCursor_Scale", Range(0,10)) = 0.5         // 1 is 10% of minimum dimension
        _TouchCursor_Position_("TouchCursor_Position", Vector) = (0,0,0,0)  // from lower left (-0.5, -0.5) to upper right (0.5, 0.5)
        _TouchCursor_Visibility_("TouchCursor_Visibility", Range(0,1)) = 0  // Controls opacitiy now, but could control other transition animations in the future

        [NoScaleOffset] _EyeCursor_Texture_("EyeCursor_Texture", 2D) = "" {}// Eye cursors are based on an image texture (to support pre-rendered blur)
        _EyeCursor_Scale_("EyeCursor_Scale", Range(0,10)) = 0.5             // 1 is 100% of minimum dimension
        _EyeCursor_Position_("EyeCursor_Position", Vector) = (0,0,0,0)      // from lower left (-0.5, -0.5) to upper right (0.5, 0.5)
        _EyeCursor_Visibility_("EyeCursor_Visibility", Range(0,1)) = 0      // Controls opacitiy now, but could control other transition animations in the future
        _EyeCursor_MaxAlpha_("EyeCursor_MaxAlpha", Range(0,1)) = 0.3        // When eye cursor has fully appeared, what is the alpha?

        _SlateAspectRatio_("SlateAspectRatio", Range(0.25,4)) = 1            // width/height of display surface

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
        _Iridescence_Edge_Intensity_("Iridescence Edge Intensity", Range(0,1)) = 1
        _Iridescence_Tint_("Iridescence Tint", Color) = (1,1,1,1)
        [NoScaleOffset] _Iridescent_Map_("Iridescent Map", 2D) = "" {}
        _Angle_("Angle", Range(-90,90)) = -45
        [Toggle] _Reflected_("Reflected", Float) = 1
        _Frequency_("Frequency", Range(0,10)) = 1
        _Vertical_Offset_("Vertical Offset", Range(0,2)) = 0
     
    [Header(Slate Map)]
        [NoScaleOffset] _MainTex("MainTex", 2D) = "" {}
        _Black_("Black", Range(0,1)) = 0.08
        _U_Transform("U Transform", Vector) = (1,0,0,0)
        _V_Transform("V Transform", Vector) = (0,1,0,0)
        _U_Offset("U Offset", Float) = 0
        _V_Offset("V Offset", Float) = 0.0
     
    [Header(Fade)]
        _Fade_Out_("Fade Out", Range(0,1)) = 1
     
    [Header(Antialiasing)]
        [Toggle(_SMOOTH_EDGES_)] _Smooth_Edges_("Smooth Edges", Float) = 0
     

    [Header(Stencil)]
        _StencilReference("Stencil Reference", Range(0, 255)) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)]_StencilComparison("Stencil Comparison", Int) = 0
        [Enum(UnityEngine.Rendering.StencilOp)]_StencilOperation("Stencil Operation", Int) = 0
}

SubShader {
    Tags{ "RenderType" = "Opaque" }
    Blend Off
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
    #pragma multi_compile _ _SMOOTH_EDGES_
    #pragma multi_compile _ _LINE_ENABLE_
    #pragma multi_compile _ _IRIDESCENCE_ENABLE_

    #include "UnityCG.cginc"

CBUFFER_START(UnityPerMaterial)

    float4 _TouchCursor_Color_;
    float _TouchCursor_Scale_;
    float2 _TouchCursor_Position_;
    float _TouchCursor_Visibility_;

    sampler2D _EyeCursor_Texture_;
    float _EyeCursor_Scale_;
    float2 _EyeCursor_Position_;
    float _EyeCursor_Visibility_;
    float _EyeCursor_MaxAlpha_;

    float _SlateAspectRatio_;
    
    float _Radius_;
    //bool _Line_Enable_;
    float _Line_Width_;
    bool _Absolute_Sizes_;
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
    half _Iridescence_Edge_Intensity_;
    half4 _Iridescence_Tint_;
    sampler2D _Iridescent_Map_;
    float _Angle_;
    bool _Reflected_;
    float _Frequency_;
    float _Vertical_Offset_;
    sampler2D _MainTex;
    half _Black_;
    float2 _U_Transform;
    float2 _V_Transform;
    float _U_Offset;
    float _V_Offset;
    half _Fade_Out_;
    //bool _Smooth_Edges_;

CBUFFER_END


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

    //BLOCK_BEGIN Object_To_World_Pos 717

    void Object_To_World_Pos_B717(
        float3 Pos_Object,
        out float3 Pos_World    )
    {
        Pos_World=(mul(UNITY_MATRIX_M, float4(Pos_Object, 1)));
        
    }
    //BLOCK_END Object_To_World_Pos

    //BLOCK_BEGIN Round_Rect_Vertex 759

    void Round_Rect_Vertex_B759(
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

    //BLOCK_BEGIN Line_Vertex 760

    void Line_Vertex_B760(
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

    //BLOCK_BEGIN PickDir 762

    void PickDir_B762(
        float Degrees,
        float3 DirX,
        float3 DirY,
        out float3 Dir    )
    {
        float a = Degrees*3.14159/180.0;
        Dir = cos(a)*DirX+sin(a)*DirY;
    }
    //BLOCK_END PickDir

    //BLOCK_BEGIN Move_Verts 761

    void Move_Verts_B761(
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

    //BLOCK_BEGIN Pick_Radius 732

    void Pick_Radius_B732(
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

    //BLOCK_BEGIN Edge_AA_Vertex 758

    void Edge_AA_Vertex_B758(
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

    //BLOCK_BEGIN Object_To_World_Dir 729

    void Object_To_World_Dir_B729(
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

    //BLOCK_BEGIN RelativeOrAbsoluteDetail 757

    void RelativeOrAbsoluteDetail_B757(
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

        // Object_To_World_Dir (#727)
        float3 Nrm_World_Q727;
        Nrm_World_Q727 = normalize((mul((float3x3)UNITY_MATRIX_M, vertInput.normal)));
        
        // Object_To_World_Dir (#728)
        float3 Tangent_World_Q728;
        float3 Tangent_World_N_Q728;
        float Tangent_Length_Q728;
        Tangent_World_Q728 = (mul((float3x3)UNITY_MATRIX_M, float3(1,0,0)));
        Tangent_Length_Q728 = length(Tangent_World_Q728);
        Tangent_World_N_Q728 = Tangent_World_Q728 / Tangent_Length_Q728;

        float3 Binormal_World_Q729;
        float3 Binormal_World_N_Q729;
        float Binormal_Length_Q729;
        Object_To_World_Dir_B729(float3(0,1,0),Binormal_World_Q729,Binormal_World_N_Q729,Binormal_Length_Q729);

        float Radius_Q757;
        float Line_Width_Q757;
        RelativeOrAbsoluteDetail_B757(_Radius_,_Line_Width_,_Absolute_Sizes_,Binormal_Length_Q729,Radius_Q757,Line_Width_Q757);

        float3 Dir_Q762;
        #if defined(_IRIDESCENCE_ENABLE_)
          PickDir_B762(_Angle_,Tangent_World_N_Q728,Binormal_World_N_Q729,Dir_Q762);
        #else
          Dir_Q762 = float3(0,0,0);
        #endif

        float Result_Q732;
        Pick_Radius_B732(Radius_Q757,_Radius_Top_Left_,_Radius_Top_Right_,_Radius_Bottom_Left_,_Radius_Bottom_Right_,vertInput.vertex.xyz,Result_Q732);

        // Divide (#730)
        float Anisotropy_Q730 = Tangent_Length_Q728 / Binormal_Length_Q729;

        // From_RGBA (#733)
        float4 Out_Color_Q733 = float4(Result_Q732, Line_Width_Q757, 0, 1);

        float3 New_P_Q761;
        float2 New_UV_Q761;
        float Radial_Gradient_Q761;
        float3 Radial_Dir_Q761;
        Move_Verts_B761(Anisotropy_Q730,vertInput.vertex.xyz,Result_Q732,New_P_Q761,New_UV_Q761,Radial_Gradient_Q761,Radial_Dir_Q761);

        float3 Pos_World_Q717;
        Object_To_World_Pos_B717(New_P_Q761,Pos_World_Q717);

        float Gradient1_Q758;
        float Gradient2_Q758;
        #if defined(_SMOOTH_EDGES_)
          Edge_AA_Vertex_B758(Pos_World_Q717,vertInput.vertex.xyz,vertInput.normal,_WorldSpaceCameraPos,Radial_Gradient_Q761,Radial_Dir_Q761,vertInput.tangent,Gradient1_Q758,Gradient2_Q758);
        #else
          Gradient1_Q758 = 1;
          Gradient2_Q758 = 1;
        #endif

        float2 Rect_UV_Q759;
        float4 Rect_Parms_Q759;
        float2 Scale_XY_Q759;
        float2 Line_UV_Q759;
        float2 Color_UV_Info_Q759;
        Round_Rect_Vertex_B759(New_UV_Q761,Result_Q732,0,Anisotropy_Q730,Gradient1_Q758,Gradient2_Q758,vertInput.normal,float4(1,1,0,0),Rect_UV_Q759,Rect_Parms_Q759,Scale_XY_Q759,Line_UV_Q759,Color_UV_Info_Q759);

        float3 Line_Vertex_Q760;
        #if defined(_LINE_ENABLE_)
          Line_Vertex_B760(Scale_XY_Q759,Line_UV_Q759,_Time.y,_Rate_,_Highlight_Transform_,Line_Vertex_Q760);
        #else
          Line_Vertex_Q760 = float3(0,0,0);
        #endif

        // TransformSlateUV (#749)
        float X_Q749;
        float Y_Q749;
        X_Q749 = dot(Color_UV_Info_Q759, _U_Transform) + _U_Offset;
        Y_Q749 = dot(Color_UV_Info_Q759, _V_Transform) + _V_Offset;

        // From_XYZW (#748)
        float4 Vec4_Q748 = float4(X_Q749, Y_Q749, Result_Q732, Line_Width_Q757);

        float3 Position = Pos_World_Q717;
        float3 Normal = Nrm_World_Q727;
        float2 UV = Rect_UV_Q759;
        float3 Tangent = Line_Vertex_Q760;
        float3 Binormal = Dir_Q762;
        float4 Color = Out_Color_Q733;
        float4 Extra1 = Rect_Parms_Q759;
        float4 Extra2 = Vec4_Q748;
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

    //BLOCK_BEGIN Round_Rect_Fragment 754

    void Round_Rect_Fragment_B754(
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

    //BLOCK_BEGIN Smooth_Edges 756

    void Smooth_Edges_B756(
        half Filter_Width,
        half4 Rect_Parms,
        out half Inside_Rect    )
    {
        float g = min(Rect_Parms.z,Rect_Parms.w);
        float dgrad = max(fwidth(g)*Filter_Width,0.00001);
        Inside_Rect = saturate(g/dgrad);
    }
    //BLOCK_END Smooth_Edges

    //BLOCK_BEGIN Line_Fragment 751

    void Line_Fragment_B751(
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

    //BLOCK_BEGIN Iridescence 737

    void Iridescence_B737(
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

    // Ported from Unity Shader Graph generated code
    void Unity_Blend_Lighten_float4(float4 Base, float4 Blend, out float4 Out, float Opacity)
    {
        Out = max(Blend, Base);
        Out = lerp(Base, Out, Opacity);
    }

    // Ported from Unity Shader Graph generated code
    void Unity_Blend_Multiply_float4(float4 Base, float4 Blend, out float4 Out, float Opacity)
    {
        Out = Base * Blend;
        Out = lerp(Base, Out, Opacity);
    }

    // Ported from Unity Shader Graph generated code
    void Unity_Ellipse_float(float2 UV, float Width, float Height, out float Out)
    {
    #if defined(SHADER_STAGE_RAY_TRACING)
        Out = saturate((1.0 - length((UV * 2 - 1) / float2(Width, Height))) * FLT_MAX);
    #else
        float d = length((UV * 2 - 1) / float2(Width, Height));
        Out = saturate((1 - d) / fwidth(d));
    #endif
    }

    // Scale normalized based on output dimensions. Only reduce scale, don't expand it.
    void AdjustScaleByAspectRatio(float scale, float aspectRatio, out float AdjustedWidth, out float AdjustedHeight)
    {
        AdjustedWidth = aspectRatio <= 1 ? scale : scale / aspectRatio;
        AdjustedHeight = aspectRatio <= 1 ? scale * aspectRatio : scale;
    }

    half4 frag(VertexOutput fragInput) : SV_Target
    {
        half4 result;

        half Inside_Rect_Q756;
        #if defined(_SMOOTH_EDGES_)
          Smooth_Edges_B756(1,fragInput.extra1,Inside_Rect_Q756);
        #else
          Inside_Rect_Q756 = 1.0;
        #endif

        // To_XYZW (#734)
        float X_Q734;
        float Y_Q734;
        float Z_Q734;
        float W_Q734;
        X_Q734=fragInput.extra2.x;
        Y_Q734=fragInput.extra2.y;
        Z_Q734=fragInput.extra2.z;
        W_Q734=fragInput.extra2.w;

        half4 Line_Color_Q751;
        #if defined(_LINE_ENABLE_)
          Line_Fragment_B751(_Line_Color_,_Highlight_Color_,_Highlight_Width_,fragInput.tangent.xyz,_Highlight_,Line_Color_Q751);
        #else
          Line_Color_Q751 = half4(0,0,0,1);
        #endif

        half4 Color_Q737;
        #if defined(_IRIDESCENCE_ENABLE_)
          Iridescence_B737(fragInput.posWorld,fragInput.normalWorld.xyz,fragInput.uv,fragInput.binormal.xyz,_WorldSpaceCameraPos,_Iridescence_Tint_,_Iridescent_Map_,_Reflected_,_Frequency_,_Vertical_Offset_,Color_Q737);
        #else
          Color_Q737 = half4(0,0,0,0);
        #endif

        // Multiply (#755)
        half Product_Q755 = Inside_Rect_Q756 * _Fade_Out_;

        half InLine_Q754;
        #if defined(_LINE_ENABLE_)
          Round_Rect_Fragment_B754(Z_Q734,W_Q734,_Filter_Width_,fragInput.uv,fragInput.extra1,InLine_Q754);
        #else
          InLine_Q754 = 0.0;
        #endif

        // Add_Scaled_Color (#742)
        half4 Sum_Q742 = Line_Color_Q751 + _Iridescence_Edge_Intensity_ * Color_Q737;

        // From_XY (#735)
        float2 Vec2_Q735 = float2(X_Q734,Y_Q734);

        // Set_Alpha (#744)
        half4 Result_Q744 = Sum_Q742; Result_Q744.a = 1;

        // SlateMap (#770)
        half4 Color_Q770;
        Color_Q770 = tex2D(_MainTex,Vec2_Q735);
        Color_Q770.rgb = Color_Q770.rgb*(1.0-_Black_)+_Black_;

        // EYE CURSOR ===

        float2 uvForCursors = float2(fragInput.extra2.x - 0.5, _V_Transform.y * (fragInput.extra2.y - 1) - 0.5);

        float AdjustedScaleX, AdjustedScaleY;
        AdjustScaleByAspectRatio(_EyeCursor_Scale_, _SlateAspectRatio_, AdjustedScaleX, AdjustedScaleY);
        float2 adjustedEyeScale = float2(AdjustedScaleX, AdjustedScaleY);

        // adjust eye cursor UVs based on desired position and scale
        float2 scaleToTiling = float2(1, 1) / adjustedEyeScale;
        float2 adjustedEyePosition = float2(-1, -1) / adjustedEyeScale * _EyeCursor_Position_;
        float2 clampedUV = clamp(scaleToTiling * uvForCursors + 0.5 + adjustedEyePosition, 0, 1); // don't repeat texture, just extend the edge over the entire slate
        half4 eyeCursorColor = tex2D(_EyeCursor_Texture_, clampedUV);

        // eye cursor compositing
        Unity_Blend_Multiply_float4(Color_Q770, eyeCursorColor, Color_Q770, _EyeCursor_Visibility_ * _EyeCursor_MaxAlpha_);

        // TOUCH CURSOR ===

        // Adjust UV for cursor position, scale, and aspect ratio
        float AdjustedWidth, AdjustedHeight;
        float adjustedTouchScale = _TouchCursor_Scale_ * 0.1f; // 1 is 10% of minimum dimension
        AdjustScaleByAspectRatio(adjustedTouchScale, _SlateAspectRatio_, AdjustedWidth, AdjustedHeight);
        float2 touchCursorUV = float2(_TouchCursor_Position_.x, _TouchCursor_Position_.y) * -1 + uvForCursors + 0.5;

        // draw ellipse (monochromatic)
        float touchCursorBW;
        Unity_Ellipse_float(touchCursorUV, AdjustedWidth, AdjustedHeight, touchCursorBW);
        touchCursorBW = 1 - touchCursorBW; // invert ellipse colors to black ellipse on white background

        // tint ellipse to touch cursor color
        float4 touchCursorOutput;
        Unity_Blend_Lighten_float4(touchCursorBW, _TouchCursor_Color_, touchCursorOutput, 1);

        // touch cursor compositing
        Unity_Blend_Multiply_float4(Color_Q770, touchCursorOutput, Color_Q770, _TouchCursor_Visibility_); // using multiply blend, but design could explore other looks

        // Mix_Colors (#753)
        half4 Color_At_T_Q753 = lerp(Color_Q770, Result_Q744, float4(InLine_Q754, InLine_Q754, InLine_Q754, InLine_Q754));

        // Scale_Color (#746)
        half4 Result_Q746 = Product_Q755 * Color_At_T_Q753;

        float4 Out_Color = Result_Q746;
        float Clip_Threshold = 0.001;
        bool To_sRGB = false;

        result = Out_Color;
        return result;
    }

    ENDCG
  }
 }
}
