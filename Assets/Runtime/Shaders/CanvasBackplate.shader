// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/// <summary>
/// Note, this shader is generated from a tool and is not formated for user readability.
/// </summary>

Shader "Graphics Tools/Canvas Backplate" {

Properties {

    [Header(Round Rect)]
        _Line_Width_("Line Width", Range(0,10)) = 1
        _Filter_Width_("Filter Width", Range(0,4)) = 1
        _Base_Color_("Base Color", Color) = (0,0,0,1)
        _Line_Color_("Line Color", Color) = (0,0,0,1)
     
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
        _Angle_("Angle", Range(-90,90)) = -45
        [Toggle] _Reflected_("Reflected", Float) = 1
        _Frequency_("Frequency", Range(0,10)) = 1
        _Vertical_Offset_("Vertical Offset", Range(0,2)) = 0
     
    [Header(Gradient)]
        _Gradient_Color_("Gradient Color", Color) = (0.631373,0.631373,0.631373,1)
        _Top_Left_("Top Left", Color) = (1,0.690196,0.976471,1)
        _Top_Right_("Top Right", Color) = (0.0,0.33,0.88,1)
        _Bottom_Left_("Bottom Left", Color) = (0.0,0.33,0.88,1)
        _Bottom_Right_("Bottom Right", Color) = (1,1,1,1)
        [Toggle(_EDGE_ONLY_)] _Edge_Only_("Edge Only", Float) = 0
        _Edge_Width_("Edge Width", Range(0,1)) = 0.5
        _Edge_Power_("Edge Power", Range(0,10)) = 2.0
        _Line_Gradient_Blend_("Line Gradient Blend", Range(0,1)) = 0.36
     
    [Header(Fade)]
        _Fade_Out_("Fade Out", Range(0,1)) = 1
     


}

SubShader {
    Tags{ "RenderType" = "Opaque" }
    Blend Off
    Tags {"DisableBatching" = "True"}

    LOD 100


    Pass

    {

    CGPROGRAM

    #pragma vertex vert
    #pragma fragment frag
    #pragma multi_compile_instancing
    #pragma target 4.0
    #pragma multi_compile _ _IRIDESCENCE_ENABLE_
    #pragma multi_compile _ _EDGE_ONLY_

    #include "UnityCG.cginc"

    float _Line_Width_;
    float _Filter_Width_;
    float4 _Base_Color_;
    float4 _Line_Color_;
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
    bool _Reflected_;
    float _Frequency_;
    float _Vertical_Offset_;
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




    struct VertexInput {
        float4 vertex : POSITION;
        half3 normal : NORMAL;
        float2 uv0 : TEXCOORD0;
        float2 uv2 : TEXCOORD2;
        float2 uv3 : TEXCOORD3;
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


    //BLOCK_BEGIN Object_To_World_Pos 143

    void Object_To_World_Pos_B143(
        float3 Pos_Object,
        float2 ScaleXY,
        out float3 Pos_World    )
    {
        Pos_World=(mul(unity_ObjectToWorld, float4(Pos_Object*float3(ScaleXY.x, ScaleXY.y, 1.0), 1)));
        
    }
    //BLOCK_END Object_To_World_Pos

    //BLOCK_BEGIN Round_Rect_Vertex 136

    void Round_Rect_Vertex_B136(
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

    //BLOCK_BEGIN Line_Vertex 112

    void Line_Vertex_B112(
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

    //BLOCK_BEGIN PickDir 113

    void PickDir_B113(
        float Degrees,
        float3 DirX,
        float3 DirY,
        out float3 Dir    )
    {
        // main code goes here
        float a = Degrees*3.14159/180.0;
        Dir = cos(a)*DirX+sin(a)*DirY;
        
    }
    //BLOCK_END PickDir

    //BLOCK_BEGIN Object_To_World_Dir 109

    void Object_To_World_Dir_B109(
        float3 Dir_Object,
        out float3 Binormal_World,
        out float3 Binormal_World_N,
        out float Binormal_Length    )
    {
        Binormal_World = (mul((float3x3)unity_ObjectToWorld, Dir_Object));
        Binormal_Length = length(Binormal_World);
        Binormal_World_N = Binormal_World / Binormal_Length;
    }
    //BLOCK_END Object_To_World_Dir

    //BLOCK_BEGIN RelativeOrAbsoluteDetail 120

    void RelativeOrAbsoluteDetail_B120(
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


        float3 Pos_World_Q143;
        Object_To_World_Pos_B143(vertInput.vertex.xyz,float2(1,1),Pos_World_Q143);

        // Object_To_World_Dir (#105)
        float3 Nrm_World_Q105;
        Nrm_World_Q105 = normalize((mul((float3x3)unity_ObjectToWorld, vertInput.normal)));
        
        // To_XY (#160)
        float X_Q160;
        float Y_Q160;
        X_Q160 = vertInput.uv3.x;
        Y_Q160 = vertInput.uv3.y;

        // Object_To_World_Dir (#108)
        float3 Tangent_World_Q108;
        float3 Tangent_World_N_Q108;
        float Tangent_Length_Q108;
        Tangent_World_Q108 = (mul((float3x3)unity_ObjectToWorld, float3(1,0,0)));
        Tangent_Length_Q108 = length(Tangent_World_Q108);
        Tangent_World_N_Q108 = Tangent_World_Q108 / Tangent_Length_Q108;

        float3 Binormal_World_Q109;
        float3 Binormal_World_N_Q109;
        float Binormal_Length_Q109;
        Object_To_World_Dir_B109(float3(0,1,0),Binormal_World_Q109,Binormal_World_N_Q109,Binormal_Length_Q109);

        // To_XY (#151)
        float X_Q151;
        float Y_Q151;
        X_Q151 = vertInput.uv2.x;
        Y_Q151 = vertInput.uv2.y;

        float3 Dir_Q113;
        PickDir_B113(_Angle_,Tangent_World_N_Q108,Binormal_World_N_Q109,Dir_Q113);

        // Divide (#110)
        float Anisotropy_Q110 = X_Q151 / Y_Q151;

        float Radius_Q120;
        float Line_Width_Q120;
        RelativeOrAbsoluteDetail_B120(0.0,_Line_Width_,true,Y_Q151,Radius_Q120,Line_Width_Q120);

        float2 Rect_UV_Q136;
        float4 Rect_Parms_Q136;
        float2 Scale_XY_Q136;
        float2 Line_UV_Q136;
        float2 Color_UV_Info_Q136;
        Round_Rect_Vertex_B136(vertInput.uv0,X_Q160,0,Anisotropy_Q110,1,1,vertInput.normal,float4(1,1,0,0),Rect_UV_Q136,Rect_Parms_Q136,Scale_XY_Q136,Line_UV_Q136,Color_UV_Info_Q136);

        float3 Line_Vertex_Q112;
        Line_Vertex_B112(Scale_XY_Q136,Line_UV_Q136,_Time.y,_Rate_,_Highlight_Transform_,Line_Vertex_Q112);

        // To_XY (#138)
        float X_Q138;
        float Y_Q138;
        X_Q138 = Color_UV_Info_Q136.x;
        Y_Q138 = Color_UV_Info_Q136.y;

        // From_XYZW (#137)
        float4 Vec4_Q137 = float4(X_Q138, Y_Q138, X_Q160, Line_Width_Q120);

        float3 Position = Pos_World_Q143;
        float3 Normal = Nrm_World_Q105;
        float2 UV = Rect_UV_Q136;
        float3 Tangent = Line_Vertex_Q112;
        float3 Binormal = Dir_Q113;
        float4 Color = float4(1,1,1,1);
        float4 Extra1 = Rect_Parms_Q136;
        float4 Extra2 = Vec4_Q137;
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

    //BLOCK_BEGIN FastLinearTosRGB 132

    void FastLinearTosRGB_B132(
        half4 Linear,
        out half4 sRGB    )
    {
        sRGB.rgb = sqrt(saturate(Linear.rgb));
        sRGB.a = Linear.a;
        
    }
    //BLOCK_END FastLinearTosRGB

    //BLOCK_BEGIN Round_Rect_Fragment 111

    void Round_Rect_Fragment_B111(
        half Radius,
        half Line_Width,
        half4 Line_Color,
        half Filter_Width,
        float2 UV,
        half Line_Visibility,
        float4 Rect_Parms,
        half4 Fill_Color,
        out half4 Color    )
    {
        half d = length(max(abs(UV)-Rect_Parms.xy,0.0));
        half dx = max(fwidth(d)*Filter_Width,0.00001);
        
        half inner = saturate((d+dx*0.5-max(Radius-Line_Width,d-dx*0.5))/dx);
        
        Color = saturate(lerp(Fill_Color, Line_Color,float4( inner, inner, inner, inner)));
        
    }
    //BLOCK_END Round_Rect_Fragment

    //BLOCK_BEGIN Iridescence 122

    void Iridescence_B122(
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

    //BLOCK_BEGIN Scale_RGB 125

    void Scale_RGB_B125(
        half4 Color,
        half Scalar,
        out half4 Result    )
    {
        Result = float4(Scalar,Scalar,Scalar,1) * Color;
    }
    //BLOCK_END Scale_RGB

    //BLOCK_BEGIN Scale_RGB 123

    void Scale_RGB_B123(
        half Scalar,
        half4 Color,
        out half4 Result    )
    {
        Result = float4(Scalar,Scalar,Scalar,1) * Color;
    }
    //BLOCK_END Scale_RGB

    //BLOCK_BEGIN Line_Fragment 140

    void Line_Fragment_B140(
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

    //BLOCK_BEGIN Edge 135

    void Edge_B135(
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

    //BLOCK_BEGIN Gradient 134

    void Gradient_B134(
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


    half4 frag(VertexOutput fragInput) : SV_Target
    {
        half4 result;

        // To_XYZW (#117)
        float X_Q117;
        float Y_Q117;
        float Z_Q117;
        float W_Q117;
        X_Q117=fragInput.extra2.x;
        Y_Q117=fragInput.extra2.y;
        Z_Q117=fragInput.extra2.z;
        W_Q117=fragInput.extra2.w;

        half4 Color_Q122;
        #if defined(_IRIDESCENCE_ENABLE_)
          Iridescence_B122(fragInput.posWorld,fragInput.normalWorld.xyz,fragInput.uv,fragInput.binormal.xyz,_WorldSpaceCameraPos,_Iridescence_Tint_,_Iridescent_Map_,_Reflected_,_Frequency_,_Vertical_Offset_,Color_Q122);
        #else
          Color_Q122 = half4(0,0,0,1);
        #endif

        half4 Result_Q123;
        Scale_RGB_B123(_Iridescence_Intensity_,Color_Q122,Result_Q123);

        half4 Line_Color_Q140;
        Line_Fragment_B140(_Line_Color_,_Highlight_Color_,_Highlight_Width_,fragInput.tangent.xyz,_Highlight_,Line_Color_Q140);

        half Result_Q135;
        #if defined(_EDGE_ONLY_)
          Edge_B135(fragInput.extra1,Z_Q117,W_Q117,fragInput.uv,_Edge_Width_,_Edge_Power_,Result_Q135);
        #else
          Result_Q135 = 1;
        #endif

        // From_XY (#118)
        float2 Vec2_Q118 = float2(X_Q117,Y_Q117);

        half4 Result_Q134;
        Gradient_B134(_Gradient_Color_,_Top_Left_,_Top_Right_,_Bottom_Left_,_Bottom_Right_,Vec2_Q118,Result_Q134);

        // FastsRGBtoLinear (#127)
        half4 Linear_Q127;
        Linear_Q127.rgb = saturate(Result_Q134.rgb*Result_Q134.rgb);
        Linear_Q127.a=Result_Q134.a;
        
        half4 Result_Q125;
        Scale_RGB_B125(Linear_Q127,Result_Q135,Result_Q125);

        // Add_Colors (#124)
        half4 Sum_Q124 = Result_Q125 + Result_Q123;

        // Mix_Colors (#126)
        half4 Color_At_T_Q126 = lerp(Line_Color_Q140, Result_Q125,float4( _Line_Gradient_Blend_, _Line_Gradient_Blend_, _Line_Gradient_Blend_, _Line_Gradient_Blend_));

        // Add_Colors (#129)
        half4 Base_And_Iridescent_Q129;
        Base_And_Iridescent_Q129 = _Base_Color_ + float4(Sum_Q124.rgb,0.0);
        
        // Add_Scaled_Color (#128)
        half4 Sum_Q128 = Color_At_T_Q126 + _Iridescence_Edge_Intensity_ * Color_Q122;

        // Set_Alpha (#130)
        half4 Result_Q130 = Sum_Q128; Result_Q130.a = 1;

        half4 Color_Q111;
        Round_Rect_Fragment_B111(Z_Q117,W_Q117,Result_Q130,_Filter_Width_,fragInput.uv,1,fragInput.extra1,Base_And_Iridescent_Q129,Color_Q111);

        // Scale_Color (#133)
        half4 Result_Q133 = _Fade_Out_ * Color_Q111;

        half4 sRGB_Q132;
        FastLinearTosRGB_B132(Result_Q133,sRGB_Q132);

        // Set_Alpha (#154)
        half4 Result_Q154 = sRGB_Q132; Result_Q154.a = 1;

        float4 Out_Color = Result_Q154;
        float Clip_Threshold = 0.001;
        bool To_sRGB = false;

        result = Out_Color;
        return result;
    }

    ENDCG
  }
 }
}
