// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

Shader "Graphics Tools/Non-Canvas/Beveled" {

Properties {

    [Header(Round Rect)]
        _Radius_("Radius", Range(0,0.5)) = 0.15
        _Bevel_Front_("Bevel Front", Range(0,1)) = 0.035
        _Bevel_Front_Stretch_("Bevel Front Stretch", Range(0,1)) = 0
        _Bevel_Back_("Bevel Back", Range(0,1)) = 0.03
        _Bevel_Back_Stretch_("Bevel Back Stretch", Range(0,1)) = 0
     
    [Header(Radii Multipliers)]
        _Radius_Top_Left_("Radius Top Left", Range(0,1)) = 1
        _Radius_Top_Right_("Radius Top Right", Range(0,1)) = 1.0
        _Radius_Bottom_Left_("Radius Bottom Left", Range(0,1)) = 1.0
        _Radius_Bottom_Right_("Radius Bottom Right", Range(0,1)) = 1.0
     
    [Header(Sun)]
        _Sun_Intensity_("Sun Intensity", Range(0,2)) = 1
        _Sun_Theta_("Sun Theta", Range(0,1)) = 0.9
        _Sun_Phi_("Sun Phi", Range(0,1)) = 0.4
        _Indirect_Diffuse_("Indirect Diffuse", Range(0,1)) = 1
     
    [Header(Diffuse And Specular)]
        _Albedo_("Albedo", Color) = (0.0117647,0.505882,0.996078,1)
        _Specular_("Specular", Range(0,5)) = 0
        _Shininess_("Shininess", Range(0,10)) = 10
        _Sharpness_("Sharpness", Range(0,1)) = 0
        _Subsurface_("Subsurface", Range(0,1)) = 0
     
    [Header(Reflection)]
        _Reflection_("Reflection", Range(0,2)) = 0.75
        [Toggle(_FRESNEL_ENABLED_)] _Fresnel_Enabled_("Fresnel Enabled", Float) = 1
        _Front_Reflect_("Front Reflect", Range(0,1)) = 0
        _Edge_Reflect_("Edge Reflect", Range(0,1)) = 1
        _Power_("Power", Range(0,10)) = 1
     
    [Header(Sky Environment)]
        [Toggle(_SKY_ENABLED_)] _Sky_Enabled_("Sky Enabled", Float) = 1
        _Sky_Color_("Sky Color", Color) = (0.0117647,0.960784,0.996078,1)
        _Horizon_Color_("Horizon Color", Color) = (0.0117647,0.333333,0.996078,1)
        _Ground_Color_("Ground Color", Color) = (0,0.254902,0.980392,1)
        _Horizon_Power_("Horizon Power", Range(0,10)) = 1
     
    [Header(Mapped Environment)]
        [Toggle(_ENV_ENABLE_)] _Env_Enable_("Env Enable", Float) = 0
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
    #pragma shader_feature_local _ _ENV_ENABLE_
    #pragma shader_feature_local _ _DECAL_ENABLE_
    #pragma shader_feature_local _ _IRIDESCENCE_ENABLED_
    #pragma shader_feature_local _ _FRESNEL_ENABLED_
    #pragma shader_feature_local _ _SKY_ENABLED_
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
    float _Bevel_Front_;
    float _Bevel_Front_Stretch_;
    float _Bevel_Back_;
    float _Bevel_Back_Stretch_;
    float _Radius_Top_Left_;
    float _Radius_Top_Right_;
    float _Radius_Bottom_Left_;
    float _Radius_Bottom_Right_;
    float _Sun_Intensity_;
    float _Sun_Theta_;
    float _Sun_Phi_;
    float _Indirect_Diffuse_;
    half4 _Albedo_;
    half _Specular_;
    half _Shininess_;
    half _Sharpness_;
    half _Subsurface_;
    half _Reflection_;
    //bool _Fresnel_Enabled_;
    half _Front_Reflect_;
    half _Edge_Reflect_;
    half _Power_;
    //bool _Sky_Enabled_;
    half4 _Sky_Color_;
    half4 _Horizon_Color_;
    half4 _Ground_Color_;
    half _Horizon_Power_;
    //bool _Env_Enable_;
    samplerCUBE _Reflection_Map_;
    samplerCUBE _Indirect_Environment_;
    //bool _Decal_Enable_;
    sampler2D _Decal_;
    float2 _Decal_Scale_XY_;
    int _Decal_Front_Only_;
    //bool _Iridescence_Enabled_;
    half _Iridescence_Intensity_;
    sampler2D _Iridescence_Texture_;

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
      UNITY_VERTEX_OUTPUT_STEREO
    };

    //BLOCK_BEGIN Move_Verts 1422

    void Move_Verts_B1422(
        float Anisotropy,
        float3 P,
        float Radius,
        float Bevel,
        float3 Normal_Object,
        float ScaleZ,
        float Stretch,
        out float3 New_P,
        out float2 New_UV,
        out float Radial_Gradient,
        out float3 Radial_Dir,
        out float3 New_Normal    )
    {
        float2 UV = P.xy * 2 + 0.5;
        float2 center = saturate(UV);
        float2 delta = UV - center;
        float deltad = (length(delta)*2);
        float f = (Bevel+(Radius-Bevel)*Stretch)/Radius;
        float innerd = saturate(deltad*2);
        float outerd = saturate(deltad*2-1);
        float bevelAngle = outerd*3.14159*0.5;
        float sinb = sin(bevelAngle);
        float cosb = cos(bevelAngle);
        float beveld = (1-f)*innerd + f * sinb;
        float br = outerd;
        float2 r2 = 2.0 * float2(Radius / Anisotropy, Radius);
        
        float dir = P.z<0.0001 ? 1.0 : -1.0;
        
        New_UV = center + r2 * ((0.5-center)+normalize(delta+float2(0.0,0.000001))*beveld*0.5);
        New_P = float3(New_UV - 0.5, P.z+dir*(1-cosb)*Bevel*ScaleZ);
                
        Radial_Gradient = saturate((deltad-0.5)*2);
        Radial_Dir = float3(delta * r2, 0.0);
        
        float3 beveledNormal = cosb*Normal_Object + sinb*float3(delta.x,delta.y,0.0);
        New_Normal = Normal_Object.z==0 ? Normal_Object : beveledNormal;
    }
    //BLOCK_END Move_Verts

    //BLOCK_BEGIN SunDir 1410

    void SunDir_B1410(
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

    //BLOCK_BEGIN Pick_Radius 1398

    void Pick_Radius_B1398(
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


    VertexOutput vert(VertexInput vertInput)
    {
        UNITY_SETUP_INSTANCE_ID(vertInput);
        VertexOutput o;
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

        // Tex_Coords (#1400)
        float2 XY_Q1400;
        XY_Q1400 = (vertInput.uv0-float2(0.5,0.5))*_Decal_Scale_XY_ + float2(0.5,0.5);
        
        // Object_To_World_Dir (#1389)
        float3 Tangent_World_Q1389;
        float3 Tangent_World_N_Q1389;
        float Tangent_Length_Q1389;
        Tangent_World_Q1389 = (mul((float3x3)UNITY_MATRIX_M, float3(1,0,0)));
        Tangent_Length_Q1389 = length(Tangent_World_Q1389);
        Tangent_World_N_Q1389 = Tangent_World_Q1389 / Tangent_Length_Q1389;

        // Object_To_World_Dir (#1419)
        float3 Normal_World_Q1419;
        float3 Normal_World_N_Q1419;
        float Normal_Length_Q1419;
        Normal_World_Q1419 = (mul((float3x3)UNITY_MATRIX_M, float3(0,0,1)));
        Normal_Length_Q1419 = length(Normal_World_Q1419);
        Normal_World_N_Q1419 = Normal_World_Q1419 / Normal_Length_Q1419;

        // To_XYZ (#1420)
        float X_Q1420;
        float Y_Q1420;
        float Z_Q1420;
        X_Q1420=vertInput.vertex.xyz.x;
        Y_Q1420=vertInput.vertex.xyz.y;
        Z_Q1420=vertInput.vertex.xyz.z;

        half LightDirX_Q1410;
        half LightDirY_Q1410;
        half LightDirZ_Q1410;
        SunDir_B1410(_Sun_Theta_,_Sun_Phi_,LightDirX_Q1410,LightDirY_Q1410,LightDirZ_Q1410);

        float Result_Q1398;
        Pick_Radius_B1398(_Radius_,_Radius_Top_Left_,_Radius_Top_Right_,_Radius_Bottom_Left_,_Radius_Bottom_Right_,vertInput.vertex.xyz,Result_Q1398);

        // Object_To_World_Dir (#1418)
        float3 Binormal_World_Q1418;
        float3 Binormal_World_N_Q1418;
        float Binormal_Length_Q1418;
        Binormal_World_Q1418 = (mul((float3x3)UNITY_MATRIX_M, float3(0,1,0)));
        Binormal_Length_Q1418 = length(Binormal_World_Q1418);
        Binormal_World_N_Q1418 = Binormal_World_Q1418 / Binormal_Length_Q1418;

        // Greater_Than (#1415)
        bool Greater_Than_Q1415 = Z_Q1420 > 0;

        // Subtract3 (#1397)
        float3 Difference_Q1397 = float3(0,0,0) - Normal_World_N_Q1419;

        // From_RGBA (#1392)
        float4 Out_Color_Q1392 = float4(X_Q1420, Y_Q1420, Z_Q1420, 1);

        // Divide (#1390)
        float Anisotropy_Q1390 = Tangent_Length_Q1389 / Binormal_Length_Q1418;

        // Choose_Bevel (#1411)
        float Result_Q1411 = Greater_Than_Q1415 ? _Bevel_Back_ : _Bevel_Front_;

        // Divide (#1396)
        float Anisotropy_Q1396 = Binormal_Length_Q1418 / Normal_Length_Q1419;

        // Choose_Stretch (#1412)
        float Result_Q1412 = Greater_Than_Q1415 ? _Bevel_Back_Stretch_ : _Bevel_Front_Stretch_;

        float3 New_P_Q1422;
        float2 New_UV_Q1422;
        float Radial_Gradient_Q1422;
        float3 Radial_Dir_Q1422;
        float3 New_Normal_Q1422;
        Move_Verts_B1422(Anisotropy_Q1390,vertInput.vertex.xyz,Result_Q1398,Result_Q1411,vertInput.normal,Anisotropy_Q1396,Result_Q1412,New_P_Q1422,New_UV_Q1422,Radial_Gradient_Q1422,Radial_Dir_Q1422,New_Normal_Q1422);

        // Facing (#1421)
        float Facing_Q1421 = _Decal_Front_Only_ ? (New_Normal_Q1422.z<0.0 ? 1.0 : 0.0) : 1.0;

        // Object_To_World_Pos (#1416)
        float3 Pos_World_Q1416=(mul(UNITY_MATRIX_M, float4(New_P_Q1422, 1)));

        // Object_To_World_Normal (#1417)
        float3 Nrm_World_Q1417=UnityObjectToWorldNormal(New_Normal_Q1422);

        // From_XYZW (#1401)
        float4 Vec4_Q1401 = float4(LightDirX_Q1410, LightDirY_Q1410, LightDirZ_Q1410, Facing_Q1421);

        float3 Position = Pos_World_Q1416;
        float3 Normal = Nrm_World_Q1417;
        float2 UV = XY_Q1400;
        float3 Tangent = Tangent_World_N_Q1389;
        float3 Binormal = Difference_Q1397;
        float4 Color = Out_Color_Q1392;
        float4 Extra1 = Vec4_Q1401;
        float4 Extra2 = float4(0,0,0,0);
        float4 Extra3 = float4(0,0,0,0);

        o.pos = mul(UNITY_MATRIX_VP, float4(Position,1));
        o.posWorld = Position;
        o.normalWorld.xyz = Normal; o.normalWorld.w=1.0;
        o.uv = UV;
        o.tangent.xyz = Tangent; o.tangent.w=1.0;
        o.binormal.xyz = Binormal; o.binormal.w=1.0;
        o.extra1=Extra1;

        return o;
    }

    //BLOCK_BEGIN Fragment_Main 1425

    void Fragment_Main_B1425(
        half Sun_Intensity,
        half Indirect_Diffuse,
        half3 Normal,
        half4 Albedo,
        half Fresnel_Reflect,
        half Specular,
        half Shininess,
        half Sharpness,
        half Subsurface,
        half3 Incident,
        half Reflection,
        half SSS,
        half3 Reflected,
        half4 Light_Dir,
        half4 Reflection_Sample,
        half4 Indirect_Sample,
        half4 Iridescence,
        out half4 Result    )
    {
        half3 lightDir = Light_Dir.xyz;
        half NdotL = max(dot(lightDir,Normal),0.0);
        half RdotL = max(0.0,dot(Reflected, lightDir));
        half specular = pow(RdotL,Shininess);
        specular = lerp(specular,smoothstep(0.495*Sharpness,1.0-0.495*Sharpness,specular),Sharpness);
        Result.rgb = ((Sun_Intensity*NdotL + Indirect_Sample.rgb * Indirect_Diffuse)*(1.0 + SSS * Subsurface)) * (1.0-Fresnel_Reflect) * Albedo.rgb + (Sun_Intensity*specular + Fresnel_Reflect * Reflection*Reflection_Sample.rgb) + Iridescence.rgb;
        Result.a = 1.0;
    }
    //BLOCK_END Fragment_Main

    //BLOCK_BEGIN Fast_Fresnel 1409

    void Fast_Fresnel_B1409(
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
        Transmit = 1.0 - Reflect;
    }
    //BLOCK_END Fast_Fresnel

    //BLOCK_BEGIN Iridescence 1428

    void Iridescence_B1428(
        half Intensity,
        sampler2D Texture,
        half3 Incident,
        half3 Tangent,
        out half4 Iridescence    )
    {
        half IdotT = dot(Incident, Tangent);
        float angle = (acos(IdotT)/3.14159-1.0);
        Iridescence = tex2D(Texture,half2(angle, 0.5))*Intensity;
    }
    //BLOCK_END Iridescence

    //BLOCK_BEGIN Sky_Environment 1426

    half4 SampleSky(half3 D, half4 S, half4 H, half4 G, half exponent)
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
    
    //BLOCK_END Sky_Environment


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

        // Normalize3 (#1414)
        half3 NormalN_Q1414 = normalize(fragInput.normalWorld.xyz);

        // Incident3 (#1406)
        half3 Incident_Q1406 = normalize(fragInput.posWorld - _WorldSpaceCameraPos);

        // SSS (#1407)
        half Result_Q1407 = abs(abs(dot(NormalN_Q1414,Incident_Q1406)) - abs(dot(fragInput.binormal.xyz,Incident_Q1406)));

        // Reflect (#1408)
        half3 Reflected_Q1408 = reflect(Incident_Q1406, NormalN_Q1414);

        // Color_Texture (#1429)
        half4 Decal_Q1429;
        #if defined(_DECAL_ENABLE_)
          Decal_Q1429 = tex2D(_Decal_,fragInput.uv) * fragInput.extra1.w;
        #else
          Decal_Q1429 = half4(0,0,0,0);
        #endif

        // Mapped_Environment (#1413)
        half4 Reflected_Color_Q1413;
        half4 Indirect_Diffuse_Q1413;
        #if defined(_ENV_ENABLE_)
          Reflected_Color_Q1413 = texCUBE(_Reflection_Map_,Reflected_Q1408);
          Indirect_Diffuse_Q1413 = texCUBE(_Indirect_Environment_,Reflected_Q1408);
        #else
          Reflected_Color_Q1413 = half4(0,0,0,1);
          Indirect_Diffuse_Q1413 = half4(0,0,0,1);
        #endif

        // Sky_Environment (#1426)
        half4 Reflected_Color_Q1426;
        half4 Indirect_Color_Q1426;
        #if defined(_SKY_ENABLED_)
          Reflected_Color_Q1426 = SampleSky(Reflected_Q1408,_Sky_Color_,_Horizon_Color_,_Ground_Color_,_Horizon_Power_);
          Indirect_Color_Q1426 = lerp(_Ground_Color_,_Sky_Color_,float4(NormalN_Q1414.y*0.5+0.5,NormalN_Q1414.y*0.5+0.5,NormalN_Q1414.y*0.5+0.5,NormalN_Q1414.y*0.5+0.5));
        #else
          Reflected_Color_Q1426 = half4(0,0,0,1);
          Indirect_Color_Q1426 = half4(0,0,0,1);
        #endif

        // Normalize3 (#1427)
        half3 TangentN_Q1427 = normalize(fragInput.tangent.xyz);

        // Blend_Over (#1430)
        half4 Decaled_Albedo_Q1430 = Decal_Q1429 + (1.0 - Decal_Q1429.a) * _Albedo_;

        half Transmit_Q1409;
        half Reflect_Q1409;
        #if defined(_FRESNEL_ENABLED_)
          Fast_Fresnel_B1409(_Front_Reflect_,_Edge_Reflect_,_Power_,NormalN_Q1414,Incident_Q1406,Transmit_Q1409,Reflect_Q1409);
        #else
          Transmit_Q1409 = 1;
          Reflect_Q1409 = 0;
        #endif

        // Add_Colors (#1424)
        half4 Reflections_Q1424 = Reflected_Color_Q1413 + Reflected_Color_Q1426;

        // Add_Colors (#1423)
        half4 Indirect_Q1423 = Indirect_Diffuse_Q1413 + Indirect_Color_Q1426;

        half4 Iridescence_Q1428;
        #if defined(_IRIDESCENCE_ENABLED_)
          Iridescence_B1428(_Iridescence_Intensity_,_Iridescence_Texture_,Incident_Q1406,TangentN_Q1427,Iridescence_Q1428);
        #else
          Iridescence_Q1428 = half4(0,0,0,0);
        #endif

        half4 Result_Q1425;
        Fragment_Main_B1425(_Sun_Intensity_,_Indirect_Diffuse_,NormalN_Q1414,Decaled_Albedo_Q1430,Reflect_Q1409,_Specular_,_Shininess_,_Sharpness_,_Subsurface_,Incident_Q1406,_Reflection_,Result_Q1407,Reflected_Q1408,fragInput.extra1,Reflections_Q1424,Indirect_Q1423,Iridescence_Q1428,Result_Q1425);

        float4 Out_Color = Result_Q1425;
        float Clip_Threshold = 0.001;
        bool To_sRGB = false;

        result = Out_Color;
        return result;
    }

    ENDCG
  }
 }
}
