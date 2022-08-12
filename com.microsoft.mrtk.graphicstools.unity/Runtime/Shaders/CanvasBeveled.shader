// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

Shader "Graphics Tools/Canvas/Beveled" {

Properties {

    [Header(Sun)]
        _Sun_Intensity_("Sun Intensity", Range(0,2)) = 0.75
        _Sun_Theta_("Sun Theta", Range(0,1)) = 0.73
        _Sun_Phi_("Sun Phi", Range(0,1)) = 0.48
        _Indirect_Diffuse_("Indirect Diffuse", Range(0,1)) = 0.51
     
    [Header(Diffuse And Specular)]
        _Albedo_("Albedo", Color) = (1,1,1,1)
        _Specular_("Specular", Range(0,5)) = 0
        _Shininess_("Shininess", Range(0,10)) = 10
        _Sharpness_("Sharpness", Range(0,1)) = 0
        _Subsurface_("Subsurface", Range(0,1)) = 0
     
    [Header(Reflection)]
        _Reflection_("Reflection", Range(0,2)) = 0
        [Toggle(_FRESNEL_DISABLED_)] _Fresnel_Disabled_("Fresnel Disabled", Float) = 0
        _Front_Reflect_("Front Reflect", Range(0,1)) = 0
        _Edge_Reflect_("Edge Reflect", Range(0,1)) = 1
        _Power_("Power", Range(0,10)) = 1
     
    [Header(Sky Environment)]
        [Toggle(_SKY_ENABLED_)] _Sky_Enabled_("Sky Enabled", Float) = 1
        _Sky_Color_("Sky Color", Color) = (0.866667,0.917647,1,1)
        _Horizon_Color_("Horizon Color", Color) = (0.843137,0.87451,1,1)
        _Ground_Color_("Ground Color", Color) = (0.603922,0.611765,0.764706,1)
        _Horizon_Power_("Horizon Power", Range(0,10)) = 1
     
    [Header(Mapped Environment)]
        [Toggle(_MAPPED_ENVIRONMENT_ENABLED_)] _Mapped_Environment_Enabled_("Mapped Environment Enabled", Float) = 0
        [NoScaleOffset] _Reflection_Map_("Reflection Map", Cube) = "" {}
        [NoScaleOffset] _Indirect_Environment_("Indirect Environment", Cube) = "" {}
     
    [Header(Decal Texture)]
        [Toggle(_DECAL_ENABLE_)] _Decal_Enable_("Decal Enable", Float) = 0
        [NoScaleOffset] _Decal_("Decal", 2D) = "" {}
        _Decal_Scale_XY_("Decal Scale XY", Vector) = (1.5,1.5,0,0)
        [Toggle] _Decal_Front_Only_("Decal Front Only", Float) = 1
     
    [Header(Iridescence)]
        [Toggle(_IRIDESCENCE_ENABLED_)] _Iridescence_Enabled_("Iridescence Enabled", Float) = 0
        _Iridescence_Intensity_("Iridescence Intensity", Range(0,1)) = 0
        [NoScaleOffset] _Iridescence_Texture_("Iridescence Texture", 2D) = "" {}
     
    [Header(Depth)]
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("Depth Test", Float) = 4 // "LessEqual"
        [Enum(Microsoft.MixedReality.GraphicsTools.Editor.DepthWrite)] _ZWrite("Depth Write", Float) = 1 // "On"

    [Header(Stencil)]
        _StencilReference("Stencil Reference", Range(0, 255)) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)]_StencilComparison("Stencil Comparison", Int) = 0
        [Enum(UnityEngine.Rendering.StencilOp)]_StencilOperation("Stencil Operation", Int) = 0

    [HideInInspector] _MainTex("Texture", 2D) = "white" {} // Added to avoid UnityUI warnings.
    [HideInInspector] _ClipRect("Clip Rect", Vector) = (-32767.0, -32767.0, 32767.0, 32767.0) // Added to avoid SRP warnings.
    [HideInInspector] _ClipRectRadii("Clip Rect Radii", Vector) = (10.0, 10.0, 10.0, 10.0) // Added to avoid SRP warnings.
}

SubShader {
    Tags{ "RenderType" = "Opaque" }
    Blend Off
    ZTest[_ZTest]
    ZWrite[_ZWrite]
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
    #pragma shader_feature_local _ _DECAL_ENABLE_
    #pragma shader_feature_local _ _IRIDESCENCE_ENABLED_
    #pragma shader_feature_local _ _SKY_ENABLED_
    #pragma shader_feature_local _ _MAPPED_ENVIRONMENT_ENABLED_
    #pragma shader_feature_local _ _FRESNEL_DISABLED_

     #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
     #pragma multi_compile_local _ _UI_CLIP_RECT_ROUNDED _UI_CLIP_RECT_ROUNDED_INDEPENDENT

    #include "UnityCG.cginc"
    #include "GraphicsToolsCommon.hlsl"

CBUFFER_START(UnityPerMaterial)
    half _Sun_Intensity_;
    half _Sun_Theta_;
    half _Sun_Phi_;
    half _Indirect_Diffuse_;
    half4 _Albedo_;
    half _Specular_;
    half _Shininess_;
    half _Sharpness_;
    half _Subsurface_;
    half _Reflection_;
    //bool _Fresnel_Disabled_;
    half _Front_Reflect_;
    half _Edge_Reflect_;
    half _Power_;
    //bool _Sky_Enabled_;
    half4 _Sky_Color_;
    half4 _Horizon_Color_;
    half4 _Ground_Color_;
    half _Horizon_Power_;
    //bool _Mapped_Environment_Enabled_;
    samplerCUBE _Reflection_Map_;
    samplerCUBE _Indirect_Environment_;
    //bool _Decal_Enable_;
    sampler2D _Decal_;
    float2 _Decal_Scale_XY_;
    int _Decal_Front_Only_;
    //bool _Iridescence_Enabled_;
    float _Iridescence_Intensity_;
    sampler2D _Iridescence_Texture_;
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
        float4 binormal : TEXCOORD6;
        float4 extra1 : TEXCOORD4;
      UNITY_VERTEX_OUTPUT_STEREO
    };

    //BLOCK_BEGIN Object_To_World_Pos 13

    void Object_To_World_Pos_B13(
        float3 Pos_Object,
        out float3 Pos_World    )
    {
        Pos_World=(mul(UNITY_MATRIX_M, float4(Pos_Object, 1)));
        
    }
    //BLOCK_END Object_To_World_Pos

    //BLOCK_BEGIN Object_To_World_Normal 24

    void Object_To_World_Normal_B24(
        float3 Nrm_Object,
        out float3 Nrm_World    )
    {
        Nrm_World=UnityObjectToWorldNormal(Nrm_Object);
        
    }
    //BLOCK_END Object_To_World_Normal

    //BLOCK_BEGIN Object_To_World_Dir 31

    void Object_To_World_Dir_B31(
        float3 Dir_Object,
        out float3 Normal_World,
        out float3 Normal_World_N,
        out float Normal_Length    )
    {
        Normal_World = (mul((float3x3)UNITY_MATRIX_M, Dir_Object));
        Normal_Length = length(Normal_World);
        Normal_World_N = Normal_World / Normal_Length;
    }
    //BLOCK_END Object_To_World_Dir

    //BLOCK_BEGIN To_XYZ 33

    void To_XYZ_B33(
        float3 Vec3,
        out float X,
        out float Y,
        out float Z    )
    {
        X=Vec3.x;
        Y=Vec3.y;
        Z=Vec3.z;
        
    }
    //BLOCK_END To_XYZ

    //BLOCK_BEGIN SunDir 64

    void SunDir_B64(
        half Sun_Theta,
        half Sun_Phi,
        out half LightDirX,
        out half LightDirY,
        out half LightDirZ    )
    {
        half theta = Sun_Theta * 2.0 * 3.14159;
        half phi = Sun_Phi * 3.14159;
        LightDirX = cos(phi)*cos(theta);
        LightDirY = sin(phi);
        LightDirZ = cos(phi)*sin(theta);
    }
    //BLOCK_END SunDir


    VertexOutput vert(VertexInput vertInput)
    {
        UNITY_SETUP_INSTANCE_ID(vertInput);
        VertexOutput o;
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

        float3 Pos_World_Q13;
        Object_To_World_Pos_B13(vertInput.vertex.xyz,Pos_World_Q13);

        float3 Nrm_World_Q24;
        Object_To_World_Normal_B24(vertInput.normal,Nrm_World_Q24);

        // Tex_Coords (#56)
        float2 XY_Q56;
        XY_Q56 = (vertInput.uv0-float2(0.5,0.5))*_Decal_Scale_XY_*float2(vertInput.uv2.x/vertInput.uv2.y,1.0) + float2(0.5,0.5);
        
        // Object_To_World_Dir (#22)
        float3 Tangent_World_Q22;
        float3 Tangent_World_N_Q22;
        float Tangent_Length_Q22;
        Tangent_World_Q22 = (mul((float3x3)UNITY_MATRIX_M, float3(1,0,0)));
        Tangent_Length_Q22 = length(Tangent_World_Q22);
        Tangent_World_N_Q22 = Tangent_World_Q22 / Tangent_Length_Q22;

        float3 Normal_World_Q31;
        float3 Normal_World_N_Q31;
        float Normal_Length_Q31;
        Object_To_World_Dir_B31(float3(0,0,1),Normal_World_Q31,Normal_World_N_Q31,Normal_Length_Q31);

        float X_Q33;
        float Y_Q33;
        float Z_Q33;
        To_XYZ_B33(vertInput.vertex.xyz,X_Q33,Y_Q33,Z_Q33);

        half LightDirX_Q64;
        half LightDirY_Q64;
        half LightDirZ_Q64;
        SunDir_B64(_Sun_Theta_,_Sun_Phi_,LightDirX_Q64,LightDirY_Q64,LightDirZ_Q64);

        // Facing (#69)
        float Facing_Q69 = _Decal_Front_Only_ ? (vertInput.normal.z<0.0 ? 1.0 : 0.0) : 1.0;

        // Subtract3 (#32)
        float3 Difference_Q32 = float3(0,0,0) - Normal_World_N_Q31;

        // From_RGBA (#26)
        float4 Out_Color_Q26 = float4(X_Q33, Y_Q33, Z_Q33, 1);

        // From_XYZW (#36)
        float4 Vec4_Q36 = float4(LightDirX_Q64, LightDirY_Q64, LightDirZ_Q64, Facing_Q69);

        float3 Position = Pos_World_Q13;
        float3 Normal = Nrm_World_Q24;
        float2 UV = XY_Q56;
        float3 Tangent = Tangent_World_N_Q22;
        float3 Binormal = Difference_Q32;
        float4 Color = Out_Color_Q26;
        float4 Extra1 = Vec4_Q36;
        float4 Extra2 = float4(0,0,0,0);
        float4 Extra3 = float4(0,0,0,0);

        o.pos = mul(UNITY_MATRIX_VP, float4(Position,1));
#ifdef UNITY_UI_CLIP_RECT
        o.posLocal = vertInput.vertex.xyz;
#endif
        o.posWorld = Position;
        o.normalWorld.xyz = Normal; o.normalWorld.w=1.0;
        o.uv = UV;
        o.tangent.xyz = Tangent; o.tangent.w=1.0;
        o.binormal.xyz = Binormal; o.binormal.w=1.0;
        o.extra1=Extra1;

        return o;
    }

    //BLOCK_BEGIN Fragment_Main 68

    void Fragment_Main_B68(
        half Sun_Intensity,
        half3 Normal,
        half4 Albedo,
        half Transmission,
        half Fresnel_Reflect,
        half Shininess,
        half3 Incident,
        half Indirect_Diffuse,
        half Specular,
        half Reflection,
        half4 Reflection_Sample,
        half4 Indirect_Sample,
        half Sharpness,
        half SSS,
        half Subsurface,
        half4 Iridescence,
        half3 LightDir,
        out half4 Result    )
    {
        half NdotL = max(dot(LightDir,Normal),0.0);
        half3 R = reflect(Incident,Normal);
        half RdotL = max(0.0,dot(R,LightDir));
        half specular = pow(RdotL,Shininess);
        specular = lerp(specular,smoothstep(0.495*Sharpness,1.0-0.495*Sharpness,specular),Sharpness);
        Result = ((Sun_Intensity*NdotL + Indirect_Sample * Indirect_Diffuse)*(1.0 + SSS * Subsurface)) * Albedo * Transmission + (Sun_Intensity*specular*Specular + Fresnel_Reflect * Reflection*Reflection_Sample) + Iridescence;
    }
    //BLOCK_END Fragment_Main

    //BLOCK_BEGIN Fast_Fresnel 66

    void Fast_Fresnel_B66(
        half Front_Reflect,
        half Edge_Reflect,
        half Power,
        half3 Normal,
        half3 Incident,
        out half Transmit,
        out half Reflect    )
    {
        half d = max(-dot(Incident,Normal),0);
        Reflect = Front_Reflect+(Edge_Reflect-Front_Reflect)*pow(1-d,Power);
        Transmit=1-Reflect;
    }
    //BLOCK_END Fast_Fresnel

    //BLOCK_BEGIN SSS 41

    void SSS_B41(
        half3 ButtonN,
        half3 Normal,
        half3 Incident,
        out half Result    )
    {
        half NdotI = abs(dot(Normal,Incident));
        half BdotI = abs(dot(ButtonN,Incident));
        Result = (abs(NdotI-BdotI));
    }
    //BLOCK_END SSS

    //BLOCK_BEGIN Scale_Color 45

    void Scale_Color_B45(
        half4 Color,
        half Scalar,
        out half4 Result    )
    {
        Result = Scalar * Color;
    }
    //BLOCK_END Scale_Color

    //BLOCK_BEGIN Sky_Environment 54

    half4 SampleEnv_Bid54(half3 D, half4 S, half4 H, half4 G, half exponent)
    {
        half k = pow(abs(D.y),exponent);
        half4 C;
        if (D.y>0.0) {
            C=lerp(H,S,float4(k,k,k,k));
        } else {
            C=lerp(H,G,float4(k,k,k,k));    
        }
        return C;
    }
    
    void Sky_Environment_B54(
        half3 Normal,
        half3 Reflected,
        half4 Sky_Color,
        half4 Horizon_Color,
        half4 Ground_Color,
        half Horizon_Power,
        out half4 Reflected_Color,
        out half4 Indirect_Color    )
    {
        // main code goes here
        Reflected_Color = SampleEnv_Bid54(Reflected,Sky_Color,Horizon_Color,Ground_Color,Horizon_Power);
        Indirect_Color = lerp(Ground_Color,Sky_Color,float4(Normal.y*0.5+0.5,Normal.y*0.5+0.5,Normal.y*0.5+0.5,Normal.y*0.5+0.5));
        
    }
    //BLOCK_END Sky_Environment

    //BLOCK_BEGIN Remap_Range 50

    void Remap_Range_B50(
        half In_Min,
        half In_Max,
        half Out_Min,
        half Out_Max,
        half In,
        out half Out    )
    {
        Out = lerp(Out_Min,Out_Max,clamp((In-In_Min)/(In_Max-In_Min),0,1));
        
    }
    //BLOCK_END Remap_Range

    //BLOCK_BEGIN Code 51

    void Code_B51(
        half X,
        out half Result    )
    {
        Result = (acos(X)/3.14159-0.5)*2;
    }
    //BLOCK_END Code


    half4 frag(VertexOutput fragInput) : SV_Target
    {
#if defined(UNITY_UI_CLIP_RECT)
        clip(GTUnityUIClipRect(fragInput.posLocal.xy, _ClipRect, _ClipRectRadii) - 0.5);
#endif
        half4 result;

        // Normalize3 (#62)
        half3 Normalized_Q62 = normalize(fragInput.normalWorld.xyz);

        // Incident3 (#61)
        half3 Incident_Q61 = normalize(fragInput.posWorld - _WorldSpaceCameraPos);

        half Result_Q41;
        SSS_B41(fragInput.binormal.xyz,Normalized_Q62,Incident_Q61,Result_Q41);

        // To_XYZW (#44)
        half X_Q44;
        half Y_Q44;
        half Z_Q44;
        half W_Q44;
        X_Q44=fragInput.extra1.x;
        Y_Q44=fragInput.extra1.y;
        Z_Q44=fragInput.extra1.z;
        W_Q44=fragInput.extra1.w;

        // Color_Texture (#42)
        half4 Color_Q42;
        #if defined(_DECAL_ENABLE_)
          Color_Q42 = tex2D(_Decal_,fragInput.uv);
        #else
          Color_Q42 = half4(0,0,0,0);
        #endif

        // Reflect (#60)
        half3 Reflected_Q60 = reflect(Incident_Q61, Normalized_Q62);

        // Normalize3 (#53)
        half3 Normalized_Q53 = normalize(fragInput.tangent.xyz);

        half Transmit_Q66;
        half Reflect_Q66;
        #if defined(_FRESNEL_DISABLED_)
          Transmit_Q66 = 1.0;
          Reflect_Q66 = 1.0;
        #else
          Fast_Fresnel_B66(_Front_Reflect_,_Edge_Reflect_,_Power_,Normalized_Q62,Incident_Q61,Transmit_Q66,Reflect_Q66);
        #endif

        // From_XYZ (#63)
        half3 Vec3_Q63 = float3(X_Q44,Y_Q44,Z_Q44);

        half4 Result_Q45;
        Scale_Color_B45(Color_Q42,W_Q44,Result_Q45);

        // Mapped_Environment (#58)
        half4 Reflected_Color_Q58;
        half4 Indirect_Diffuse_Q58;
        #if defined(_MAPPED_ENVIRONMENT_ENABLED_)
          Reflected_Color_Q58 = texCUBE(_Reflection_Map_,Reflected_Q60);
          Indirect_Diffuse_Q58 = texCUBE(_Indirect_Environment_,Reflected_Q60);
        #else
          Reflected_Color_Q58 = half4(0,0,0,1);
          Indirect_Diffuse_Q58 = half4(0,0,0,1);
        #endif

        half4 Reflected_Color_Q54;
        half4 Indirect_Color_Q54;
        #if defined(_SKY_ENABLED_)
          Sky_Environment_B54(Normalized_Q62,Reflected_Q60,_Sky_Color_,_Horizon_Color_,_Ground_Color_,_Horizon_Power_,Reflected_Color_Q54,Indirect_Color_Q54);
        #else
          Reflected_Color_Q54 = half4(0,0,0,1);
          Indirect_Color_Q54 = half4(0,0,0,1);
        #endif

        // DotProduct3 (#52)
        half Dot_Q52 = dot(Incident_Q61, Normalized_Q53);

        // Blend_Over (#46)
        half4 Result_Q46 = Result_Q45 + (1.0 - Result_Q45.a) * _Albedo_;

        // Add_Colors (#29)
        half4 Sum_Q29 = Reflected_Color_Q58 + Reflected_Color_Q54;

        // Add_Colors (#30)
        half4 Sum_Q30 = Indirect_Diffuse_Q58 + Indirect_Color_Q54;

        half Result_Q51;
        Code_B51(Dot_Q52,Result_Q51);

        half Out_Q50;
        Remap_Range_B50(-1,1,0,1,Result_Q51,Out_Q50);

        // From_XY (#49)
        half2 Vec2_Q49 = float2(Out_Q50,0.5);

        // Color_Texture (#48)
        half4 Color_Q48;
        #if defined(_IRIDESCENCE_ENABLED_)
          Color_Q48 = tex2D(_Iridescence_Texture_,Vec2_Q49);
        #else
          Color_Q48 = half4(0,0,0,0);
        #endif

        // Scale_Color (#47)
        half4 Result_Q47 = _Iridescence_Intensity_ * Color_Q48;

        half4 Result_Q68;
        Fragment_Main_B68(_Sun_Intensity_,Normalized_Q62,Result_Q46,Transmit_Q66,Reflect_Q66,_Shininess_,Incident_Q61,_Indirect_Diffuse_,_Specular_,_Reflection_,Sum_Q29,Sum_Q30,_Sharpness_,Result_Q41,_Subsurface_,Result_Q47,Vec3_Q63,Result_Q68);

        // Set_Alpha (#57)
        half4 Result_Q57 = Result_Q68; Result_Q57.a = 1;

        float4 Out_Color = Result_Q57;
        float Clip_Threshold = 0.001;
        bool To_sRGB = false;

        result = Out_Color;
        return result;
    }

    ENDCG
  }
 }
}
