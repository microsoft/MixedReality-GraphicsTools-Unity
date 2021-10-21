// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/// <summary>
/// Note, this shader is generated from a tool and is not formated for user readability.
/// </summary>

Shader "Graphics Tools/Canvas Frontplate" {

Properties {
    [HideInInspector] _MainTex("Texture", 2D) = "white" {} // Added to avoid UnityUI warnings.

    [Header(Round Rect)]
        _Radius_("Radius", Range(0,0.5)) = 0.3125
        _Line_Width_("Line Width", Range(0,1)) = .031
        _Filter_Width_("Filter Width", Range(0,4)) = 1.5
        _Edge_Color_("Edge Color", Color) = (0.53,0.53,0.53,1)
     
    [Header(Fade)]
        _Fade_Out_("Fade Out", Range(0,1)) = 1
     
    [Header(Blob)]
        [Toggle(_BLOB_ENABLE_)] _Blob_Enable_("Blob Enable", Float) = 1
        [PerRendererData] _Blob_Position_("Blob Position",Vector) = (.5,0,0.2,1)
        _Blob_Intensity_("Blob Intensity", Range(0,3)) = 0.34
        _Blob_Near_Size_("Blob Near Size", Range(0,1)) = 0.025
        _Blob_Far_Size_("Blob Far Size", Range(0,1)) = 0.05
        _Blob_Near_Distance_("Blob Near Distance", Range(0,1)) = 0
        _Blob_Far_Distance_("Blob Far Distance", Range(0,1)) = 0.08
        _Blob_Fade_Length_("Blob Fade Length", Range(0,1)) = 0.08
        _Blob_Inner_Fade_("Blob Inner Fade", Range(0.001,1)) = 0.01
        [PerRendererData] _Blob_Pulse_("Blob Pulse",Range(0,1)) = 0
        [PerRendererData] _Blob_Fade_("Blob Fade",Range(0,1)) = 1
        _Blob_Pulse_Max_Size_("Blob Pulse Max Size", Range(0,1)) = 0.05
     
    [Header(Blob 2)]
        [Toggle(_BLOB_ENABLE_2_)] _Blob_Enable_2_("Blob Enable 2", Float) = 0
        [PerRendererData] _Blob_Position_2_("Blob Position 2",Vector) = (10,10.1,-0.6,1)
        _Blob_Near_Size_2_("Blob Near Size 2", Range(0,1)) = 0.025
        _Blob_Inner_Fade_2_("Blob Inner Fade 2", Range(0,1)) = 0.1
        [PerRendererData] _Blob_Pulse_2_("Blob Pulse 2",Range(0,1)) = 0
        [PerRendererData] _Blob_Fade_2_("Blob Fade 2",Range(0,1)) = 1
     
    [Header(Gaze)]
        _Gaze_Intensity_("Gaze Intensity", Range(0,1)) = 0
        _Gaze_Focus_("Gaze Focus", Range(0,1)) = 0.0
        [PerRendererData] _Pinched_("Pinched",Float) = 0.0
     
    [Header(Blob Texture)]
        [NoScaleOffset] _Blob_Texture_("Blob Texture", 2D) = "" {}
     
    [Header(Selection)]
        _Selection_Fuzz_("Selection Fuzz", Range(0,1)) = 0.5
        _Selected_("Selected", Range(0,1)) = 0
        _Selection_Fade_("Selection Fade", Range(0,1)) = 0
        _Selection_Fade_Size_("Selection Fade Size", Range(0,1)) = 0.3
        _Selected_Distance_("Selected Distance", Range(0,1)) = 0.08
        _Selected_Fade_Length_("Selected Fade Length", Range(0,1)) = 0.08
     
    [Header(Proximity)]
        _Proximity_Max_Intensity_("Proximity Max Intensity", Range(0,1)) = 0.45
        _Proximity_Far_Distance_("Proximity Far Distance", Range(0,2)) = 0.16
        _Proximity_Near_Radius_("Proximity Near Radius", Range(0,2)) = .03
        _Proximity_Anisotropy_("Proximity Anisotropy", Range(0,1)) = 1
     
    [Header(Global)]
        [PerRendererData] _Use_Global_Left_Index_("Use Global Left Index",Float) = 1
        [PerRendererData] _Use_Global_Right_Index_("Use Global Right Index",Float) = 1
     


}

SubShader {
    Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
    Blend One One
    Cull Off
    ZWrite Off
    Tags {"DisableBatching" = "True"}

    LOD 100


    Pass

    {

    CGPROGRAM

    #pragma vertex vert
    #pragma fragment frag
    #pragma multi_compile_instancing
    #pragma target 4.0
    #pragma multi_compile _ _BLOB_ENABLE_
    #pragma multi_compile _ _BLOB_ENABLE_2_

    #include "UnityCG.cginc"

CBUFFER_START(UnityPerMaterial)
    float _Radius_;
    float _Line_Width_;
    float _Filter_Width_;
    half4 _Edge_Color_;
    half _Fade_Out_;
    //bool _Blob_Enable_;
    float3 _Blob_Position_;
    float _Blob_Intensity_;
    float _Blob_Near_Size_;
    float _Blob_Far_Size_;
    float _Blob_Near_Distance_;
    float _Blob_Far_Distance_;
    float _Blob_Fade_Length_;
    float _Blob_Inner_Fade_;
    float _Blob_Pulse_;
    float _Blob_Fade_;
    float _Blob_Pulse_Max_Size_;
    //bool _Blob_Enable_2_;
    float3 _Blob_Position_2_;
    float _Blob_Near_Size_2_;
    float _Blob_Inner_Fade_2_;
    float _Blob_Pulse_2_;
    float _Blob_Fade_2_;
    float _Gaze_Intensity_;
    float _Gaze_Focus_;
    float _Pinched_;
    sampler2D _Blob_Texture_;
    float _Selection_Fuzz_;
    float _Selected_;
    float _Selection_Fade_;
    float _Selection_Fade_Size_;
    float _Selected_Distance_;
    float _Selected_Fade_Length_;
    half _Proximity_Max_Intensity_;
    float _Proximity_Far_Distance_;
    half _Proximity_Near_Radius_;
    float _Proximity_Anisotropy_;
    bool _Use_Global_Left_Index_;
    bool _Use_Global_Right_Index_;
    float4 Global_Left_Index_Tip_Position;
    float4 Global_Right_Index_Tip_Position;
CBUFFER_END


    struct VertexInput {
        float4 vertex : POSITION;
        half3 normal : NORMAL;
        float2 uv0 : TEXCOORD0;
        float2 uv2 : TEXCOORD2;
        float4 tangent : TANGENT;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct VertexOutput {
        float4 pos : SV_POSITION;
        half4 normalWorld : TEXCOORD5;
        float2 uv : TEXCOORD0;
        float4 tangent : TANGENT;
        float4 binormal : TEXCOORD6;
        float4 extra1 : TEXCOORD4;
        float4 extra2 : TEXCOORD3;
        float4 extra3 : TEXCOORD2;
      UNITY_VERTEX_OUTPUT_STEREO
    };


    //BLOCK_BEGIN Blob_Vertex 112

    void Blob_Vertex_B112(
        float3 Position,
        float3 Normal,
        float3 Tangent,
        float3 Bitangent,
        float3 Blob_Position,
        float Intensity,
        float Blob_Near_Size,
        float Blob_Far_Size,
        float Blob_Near_Distance,
        float Blob_Far_Distance,
        float2 UV,
        float3 Face_Center,
        float2 Face_Size,
        float Blob_Fade_Length,
        float Selection_Fade,
        float Selection_Fade_Size,
        float Inner_Fade,
        float Blob_Pulse,
        float Blob_Fade,
        out float2 Out_UV,
        out float3 Blob_Info    )
    {
        
        float blobSize, fadeIn;
        float3 Hit_Position;
        Blob_Info = float3(0.0,0.0,0.0);
        
        float3 delta = Blob_Position - Face_Center;
        float Hit_Distance = dot(delta, Normal);
        Hit_Position = Blob_Position - Hit_Distance * Normal;
        
        float absD = abs(Hit_Distance);
        float lerpVal = clamp((absD-Blob_Near_Distance)/(Blob_Far_Distance-Blob_Near_Distance),0.0,1.0);
        fadeIn = 1.0-clamp((absD-Blob_Far_Distance)/Blob_Fade_Length,0.0,1.0);
        
        float innerFade = 1.0-clamp(-Hit_Distance/Inner_Fade,0.0,1.0);
        
        //compute blob size
        float farClip = saturate(1.0-step(Blob_Far_Distance+Blob_Fade_Length,absD));
        float size = lerp(Blob_Near_Size,Blob_Far_Size,lerpVal)*farClip;
        blobSize = lerp(size,Selection_Fade_Size,Selection_Fade)*innerFade;
        Blob_Info.x = lerpVal*0.5+0.5;
            
        Blob_Info.y = fadeIn*Intensity*(1.0-Selection_Fade)*Blob_Fade;
        Blob_Info.x *= (1.0-Blob_Pulse);
        
        //compute blob position
        float2 blobCenterXY = float2(dot(delta,Tangent),dot(delta,Bitangent));
        
        Out_UV = ((float2(UV.x-0.5,0.5-UV.y))*Face_Size-blobCenterXY)/max(blobSize,0.0001)*2.0;
        
    }
    //BLOCK_END Blob_Vertex

    //BLOCK_BEGIN Round_Rect_Vertex 107

    void Round_Rect_Vertex_B107(
        float2 UV,
        float Radius,
        float Anisotropy,
        out float4 Rect_Parms    )
    {
        float2 Scale_XY = float2(Anisotropy,1.0);
        Rect_Parms.xy = Scale_XY*0.5-float2(Radius,Radius);
        Rect_Parms.zw = (UV - float2(0.5,0.5)) * Scale_XY;
    }
    //BLOCK_END Round_Rect_Vertex

    //BLOCK_BEGIN Proximity_Vertex 80

    float2 ProjectProximity(
        float3 blobPosition,
        float3 position,
        float3 center,
        float3 dir,
        float3 xdir,
        float3 ydir,
        out float vdistance
    )
    {
        float3 delta = blobPosition - position;
        float2 xy = float2(dot(delta,xdir),dot(delta,ydir));
        vdistance = abs(dot(delta,dir));
        return xy;
    }
    
    void Proximity_Vertex_B80(
        float3 Blob_Position,
        float3 Blob_Position_2,
        float3 Face_Center,
        float3 Position,
        float Proximity_Far_Distance,
        float Relative_Scale,
        float Proximity_Anisotropy,
        float3 Normal,
        float3 Tangent,
        float3 Binormal,
        out float4 Extra,
        out float Distance_To_Face,
        out float Distance_Fade1,
        out float Distance_Fade2    )
    {
        //float3 Active_Face_Dir_X = normalize(cross(Active_Face_Dir,Up));
        //float3 Active_Face_Dir_X = normalize(float3(Active_Face_Dir.y-Active_Face_Dir.z,Active_Face_Dir.z-Active_Face_Dir.x,Active_Face_Dir.x-Active_Face_Dir.y));
        //float3 Active_Face_Dir_Y = cross(Active_Face_Dir,Active_Face_Dir_X);
        
        float distz1,distz2;
        Extra.xy = ProjectProximity(Blob_Position,Position,Face_Center,Normal,Tangent*Proximity_Anisotropy,Binormal,distz1)/Relative_Scale;
        Extra.zw = ProjectProximity(Blob_Position_2,Position,Face_Center,Normal,Tangent*Proximity_Anisotropy,Binormal,distz2)/Relative_Scale;
        
        Distance_To_Face = dot(Normal,Position-Face_Center);
        Distance_Fade1 = 1.0 - saturate(distz1/Proximity_Far_Distance);
        Distance_Fade2 = 1.0 - saturate(distz2/Proximity_Far_Distance);
        
    }
    //BLOCK_END Proximity_Vertex

    //BLOCK_BEGIN Blob_Vertex 120

    void Blob_Vertex_B120(
        float3 Position,
        float3 Normal,
        float3 Tangent,
        float3 Bitangent,
        float3 Blob_Position,
        float Intensity,
        float Blob_Near_Size,
        float Blob_Far_Size,
        float Blob_Near_Distance,
        float Blob_Far_Distance,
        float2 UV,
        float3 Face_Center,
        float2 Face_Size,
        float Blob_Fade_Length,
        float Selection_Fade,
        float Selection_Fade_Size,
        float Inner_Fade,
        float Blob_Pulse,
        float Blob_Fade,
        out float4 Blob_Info    )
    {
        
        float blobSize, fadeIn;
        float3 Hit_Position;
        
        float3 delta = Blob_Position - Face_Center;
        float Hit_Distance = dot(delta, Normal);
        Hit_Position = Blob_Position - Hit_Distance * Normal;
        
        float absD = abs(Hit_Distance);
        float lerpVal = clamp((absD-Blob_Near_Distance)/(Blob_Far_Distance-Blob_Near_Distance),0.0,1.0);
        fadeIn = 1.0-clamp((absD-Blob_Far_Distance)/Blob_Fade_Length,0.0,1.0);
        
        float innerFade = 1.0-clamp(-Hit_Distance/Inner_Fade,0.0,1.0);
        
        //compute blob size
        float farClip = saturate(1.0-step(Blob_Far_Distance+Blob_Fade_Length,absD));
        float size = lerp(Blob_Near_Size,Blob_Far_Size,lerpVal)*farClip;
        blobSize = lerp(size,Selection_Fade_Size,Selection_Fade)*innerFade;
        Blob_Info.x = lerpVal*0.5+0.5;
            
        Blob_Info.y = fadeIn*Intensity*(1.0-Selection_Fade)*Blob_Fade;
        Blob_Info.x *= (1.0-Blob_Pulse);
        
        //compute blob position
        float2 blobCenterXY = float2(dot(delta,Tangent),dot(delta,Bitangent));
        
        Blob_Info.zw = ((float2(UV.x-0.5,0.5-UV.y))*Face_Size-blobCenterXY)/max(blobSize,0.0001)*2.0;
        
    }
    //BLOCK_END Blob_Vertex

    //BLOCK_BEGIN Proximity_Visibility 96

    void Proximity_Visibility_B96(
        float Selection,
        float3 Proximity_Center,
        float3 Proximity_Center_2,
        float Proximity_Far_Distance,
        float Proximity_Radius,
        float3 Face_Center,
        float3 Normal,
        float2 Face_Size,
        float Gaze,
        out float Width    )
    {
        //make all edges invisible if no proximity or selection visible
        float boxMaxSize = length(Face_Size)*0.5;
        
        float d1 = dot(Proximity_Center-Face_Center, Normal);
        float3 blob1 = Proximity_Center - d1 * Normal;
        
        float d2 = dot(Proximity_Center_2-Face_Center, Normal);
        float3 blob2 = Proximity_Center_2 - d2 * Normal;
        
        //float3 objectOriginInWorld = (mul(unity_ObjectToWorld, float4(float3(0.0,0.0,0.0), 1)));
        float3 delta1 = blob1 - Face_Center;
        float3 delta2 = blob2 - Face_Center;
        
        float dist1 = dot(delta1,delta1);
        float dist2 = dot(delta2,delta2);
        
        float nearestProxDist = sqrt(min(dist1,dist2));
        
        Width = (1.0 - step(boxMaxSize+Proximity_Radius,nearestProxDist))*(1.0-step(Proximity_Far_Distance,min(d1,d2))*(1.0-step(0.0001,Selection)));
        Width = max(Gaze, Width);
    }
    //BLOCK_END Proximity_Visibility

    //BLOCK_BEGIN Object_To_World_Pos 60

    void Object_To_World_Pos_B60(
        float3 Pos_Object,
        out float3 Pos_World    )
    {
        Pos_World=(mul(unity_ObjectToWorld, float4(Pos_Object, 1)));
        
    }
    //BLOCK_END Object_To_World_Pos

    //BLOCK_BEGIN Selection_Vertex 78

    float2 ramp2(float2 start, float2 end, float2 x)
    {
       return clamp((x-start)/(end-start),float2(0.0,0.0),float2(1.0,1.0));
    }
    
    float computeSelection(
        float3 blobPosition,
        float3 normal,
        float3 tangent,
        float3 bitangent,
        float3 faceCenter,
        float2 faceSize,
        float selectionFuzz,
        float farDistance,
        float fadeLength
    )
    {
        float3 delta = blobPosition - faceCenter;
        float absD = abs(dot(delta,normal));
        float fadeIn = 1.0-clamp((absD-farDistance)/fadeLength,0.0,1.0);
        
        float2 blobCenterXY = float2(dot(delta,tangent),dot(delta,bitangent));
    
        float2 innerFace = faceSize * (1.0-selectionFuzz) * 0.5;
        float2 selectPulse = ramp2(-faceSize*0.5,-innerFace,blobCenterXY)-ramp2(innerFace,faceSize*0.5,blobCenterXY);
    
        return selectPulse.x * selectPulse.y * fadeIn;
    }
    
    void Selection_Vertex_B78(
        float3 Blob_Position,
        float3 Blob_Position_2,
        float3 Face_Center,
        float2 Face_Size,
        float3 Normal,
        float3 Tangent,
        float3 Bitangent,
        float Selection_Fuzz,
        float Selected,
        float Far_Distance,
        float Fade_Length,
        float3 Active_Face_Dir,
        out float Show_Selection    )
    {
        float select1 = computeSelection(Blob_Position,Normal,Tangent,Bitangent,Face_Center,Face_Size,Selection_Fuzz,Far_Distance,Fade_Length);
        float select2 = computeSelection(Blob_Position_2,Normal,Tangent,Bitangent,Face_Center,Face_Size,Selection_Fuzz,Far_Distance,Fade_Length);
        
        Show_Selection = lerp(max(select1,select2),1.0,Selected);
    }
    //BLOCK_END Selection_Vertex

    //BLOCK_BEGIN Object_To_World_Dir 62

    void Object_To_World_Dir_B62(
        float3 Dir_Object,
        out float3 Binormal_World    )
    {
        Binormal_World = (mul((float3x3)unity_ObjectToWorld, Dir_Object));
        
    }
    //BLOCK_END Object_To_World_Dir

    //BLOCK_BEGIN Move_Verts 79

    void Move_Verts_B79(
        float2 UV,
        float Radius,
        float Anisotropy,
        float Line_Width,
        float Visible,
        out float3 New_P,
        out float2 New_UV    )
    {
        
        float2 xy = 2 * UV - float2(0.5,0.5);
        float2 center = saturate(xy);
        
        float2 delta = 2 * (xy - center);
        float deltaLength = length(delta);
        
        float2 aniso = float2(1.0 / Anisotropy, 1.0);
        center = (center-float2(0.5,0.5))*(1.0-2.0*Radius*aniso);
        
        New_UV = float2((2.0-2.0*deltaLength)*Visible,0.0);
        
        float deltaRadius =  (Radius - Line_Width * New_UV.x);
        
        New_P.xy = (center + deltaRadius / deltaLength *aniso * delta);
        New_P.z = 0.0;
        
    }
    //BLOCK_END Move_Verts


    VertexOutput vert(VertexInput vertInput)
    {
        UNITY_SETUP_INSTANCE_ID(vertInput);
        VertexOutput o;
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

        // Object_To_World_Pos (#100)
        float3 Face_Center_Q100;
        Face_Center_Q100=(mul(unity_ObjectToWorld, float4(vertInput.vertex.xyz, 1)));
        
        // Object_To_World_Dir (#72)
        float3 Nrm_World_Q72;
        Nrm_World_Q72 = normalize((mul((float3x3)unity_ObjectToWorld, vertInput.normal)));
        
        // Conditional (#87)
        float3 Result_Q87;
        Result_Q87 = _Use_Global_Left_Index_ ? Global_Left_Index_Tip_Position.xyz : _Blob_Position_;
        
        // Lerp (#98)
        float Value_At_T_Q98=lerp(_Blob_Near_Size_,_Blob_Pulse_Max_Size_,_Blob_Pulse_);

        // Object_To_World_Pos (#77)
        float3 Face_Center_Q77;
        Face_Center_Q77=(mul(unity_ObjectToWorld, float4(float3(0,0,0), 1)));
        
        // Anisotropy (#69)
        float Anisotropy_Q69=vertInput.uv2.x/vertInput.uv2.y;

        // Conditional (#88)
        float3 Result_Q88;
        Result_Q88 = _Use_Global_Right_Index_ ? Global_Right_Index_Tip_Position.xyz : _Blob_Position_2_;
        
        // Lerp (#99)
        float Value_At_T_Q99=lerp(_Blob_Near_Size_2_,_Blob_Pulse_Max_Size_,_Blob_Pulse_2_);

        // Object_To_World_Dir (#61)
        float3 Tangent_World_Q61;
        Tangent_World_Q61 = (mul((float3x3)unity_ObjectToWorld, vertInput.tangent));
        
        float3 Binormal_World_Q62;
        Object_To_World_Dir_B62((normalize(cross(vertInput.normal,vertInput.tangent))),Binormal_World_Q62);

        // Multiply (#89)
        float Product_Q89 = _Gaze_Intensity_ * _Gaze_Focus_;

        // Normalize3 (#63)
        float3 Tangent_World_N_Q63 = normalize(Tangent_World_Q61);

        // Normalize3 (#64)
        float3 Binormal_World_N_Q64 = normalize(Binormal_World_Q62);

        // Face_Size (#82)
        float2 Face_Size_Q82;
        float ScaleY_Q82;
        Face_Size_Q82 = float2(length(Tangent_World_Q61),length(Binormal_World_Q62));
        ScaleY_Q82 = Face_Size_Q82.y;
        
        float Show_Selection_Q78;
        Selection_Vertex_B78(Result_Q87,Result_Q88,Face_Center_Q77,Face_Size_Q82,Nrm_World_Q72,Tangent_World_N_Q63,Binormal_World_N_Q64,_Selection_Fuzz_,_Selected_,_Selected_Distance_,_Selected_Fade_Length_,float3(0,0,-1),Show_Selection_Q78);

        // Step (#90)
        float Step_Q90 = step(0.0001, Product_Q89);

        float2 Out_UV_Q112;
        float3 Blob_Info_Q112;
        #if defined(_BLOB_ENABLE_)
          Blob_Vertex_B112(Face_Center_Q100,Nrm_World_Q72,Tangent_World_N_Q63,Binormal_World_N_Q64,Result_Q87,_Blob_Intensity_,Value_At_T_Q98,_Blob_Far_Size_,_Blob_Near_Distance_,_Blob_Far_Distance_,vertInput.uv0,Face_Center_Q77,Face_Size_Q82,_Blob_Fade_Length_,_Selection_Fade_,_Selection_Fade_Size_,_Blob_Inner_Fade_,_Blob_Pulse_,_Blob_Fade_,Out_UV_Q112,Blob_Info_Q112);
        #else
          Out_UV_Q112 = float2(0,0);
          Blob_Info_Q112 = float3(0,0,0);
        #endif

        float4 Blob_Info_Q120;
        #if defined(_BLOB_ENABLE_2_)
          Blob_Vertex_B120(Face_Center_Q100,Nrm_World_Q72,Tangent_World_N_Q63,Binormal_World_N_Q64,Result_Q88,_Blob_Intensity_,Value_At_T_Q99,_Blob_Far_Size_,_Blob_Near_Distance_,_Blob_Far_Distance_,vertInput.uv0,Face_Center_Q77,Face_Size_Q82,_Blob_Fade_Length_,_Selection_Fade_,_Selection_Fade_Size_,_Blob_Inner_Fade_2_,_Blob_Pulse_2_,_Blob_Fade_2_,Blob_Info_Q120);
        #else
          Blob_Info_Q120 = float4(0,0,0,0);
        #endif

        float Width_Q96;
        Proximity_Visibility_B96(Show_Selection_Q78,Result_Q87,Result_Q88,_Proximity_Far_Distance_,_Proximity_Near_Radius_,Face_Center_Q77,Nrm_World_Q72,Face_Size_Q82,Step_Q90,Width_Q96);

        // Scale_Radius_And_Width (#83)
        float Out_Radius_Q83;
        float Out_Line_Width_Q83;
        Out_Radius_Q83 = true ? _Radius_ : _Radius_ / ScaleY_Q82;
        Out_Line_Width_Q83 = true ? _Line_Width_ : _Line_Width_ / ScaleY_Q82;

        // Multiply (#108)
        float Product_Q108 = Out_Line_Width_Q83 * Width_Q96;

        // Max (#85)
        float MaxAB_Q85=max(Show_Selection_Q78,Product_Q89);

        float3 New_P_Q79;
        float2 New_UV_Q79;
        Move_Verts_B79(vertInput.uv0,Out_Radius_Q83,Anisotropy_Q69,Out_Line_Width_Q83,Width_Q96,New_P_Q79,New_UV_Q79);

        // Visibility (#121)
        float3 Result_Q121 = (Width_Q96+Blob_Info_Q112.y+Blob_Info_Q120.y)>0.0 ? Face_Center_Q100 : float3(0.0,0.0,0.0);

        // From_XYZ (#103)
        float3 Vec3_Q103 = float3(Out_Radius_Q83,Product_Q108,0);

        float4 Rect_Parms_Q107;
        Round_Rect_Vertex_B107(vertInput.uv0,Out_Radius_Q83,Anisotropy_Q69,Rect_Parms_Q107);

        float3 Pos_World_Q60;
        Object_To_World_Pos_B60(New_P_Q79,Pos_World_Q60);

        float4 Extra_Q80;
        float Distance_To_Face_Q80;
        float Distance_Fade1_Q80;
        float Distance_Fade2_Q80;
        Proximity_Vertex_B80(Result_Q87,Result_Q88,Face_Center_Q77,Pos_World_Q60,_Proximity_Far_Distance_,1.0,_Proximity_Anisotropy_,Nrm_World_Q72,Tangent_World_N_Q63,Binormal_World_N_Q64,Extra_Q80,Distance_To_Face_Q80,Distance_Fade1_Q80,Distance_Fade2_Q80);

        // From_XYZ (#113)
        float3 Vec3_Q113 = float3(MaxAB_Q85,Distance_Fade1_Q80,Distance_Fade2_Q80);

        float3 Position = Result_Q121;
        float3 Normal = Vec3_Q103;
        float2 UV = Out_UV_Q112;
        float3 Tangent = Blob_Info_Q112;
        float3 Binormal = Vec3_Q113;
        float4 Color = float4(1,1,1,1);
        float4 Extra1 = Rect_Parms_Q107;
        float4 Extra2 = Extra_Q80;
        float4 Extra3 = Blob_Info_Q120;

        o.pos = mul(UNITY_MATRIX_VP, float4(Position,1));
        o.normalWorld.xyz = Normal; o.normalWorld.w=1.0;
        o.uv = UV;
        o.tangent.xyz = Tangent; o.tangent.w=1.0;
        o.binormal.xyz = Binormal; o.binormal.w=1.0;
        o.extra1=Extra1;
        o.extra2=Extra2;
        o.extra3=Extra3;

        return o;
    }

    //BLOCK_BEGIN Blob_Fragment 118

    void Blob_Fragment_B118(
        float2 UV,
        float3 Blob_Info,
        sampler2D Blob_Texture,
        out half4 Blob_Color    )
    {
        float k = dot(UV,UV);
        Blob_Color = Blob_Info.y * tex2D(Blob_Texture,float2(float2(sqrt(k),Blob_Info.x).x,1.0-float2(sqrt(k),Blob_Info.x).y))*(1.0-saturate(k));
    }
    //BLOCK_END Blob_Fragment

    //BLOCK_BEGIN Blob_Fragment 117

    void Blob_Fragment_B117(
        sampler2D Blob_Texture,
        float4 Blob_Info,
        out half4 Blob_Color    )
    {
        float k = dot(Blob_Info.zw,Blob_Info.zw);
        Blob_Color = Blob_Info.y * tex2D(Blob_Texture,float2(float2(sqrt(k),Blob_Info.x).x,1.0-float2(sqrt(k),Blob_Info.x).y))*(1.0-saturate(k));
    }
    //BLOCK_END Blob_Fragment

    //BLOCK_BEGIN Scale_RGB 93

    void Scale_RGB_B93(
        half4 Color,
        half Scalar,
        out half4 Result    )
    {
        Result = float4(Scalar,Scalar,Scalar,1) * Color;
    }
    //BLOCK_END Scale_RGB

    //BLOCK_BEGIN Round_Rect_Fragment 106

    void Round_Rect_Fragment_B106(
        half Radius,
        half Line_Width,
        half Filter_Width,
        half4 Rect_Parms,
        out half Inside_Line,
        out half Inside_Rect    )
    {
        half d = length(max(abs(Rect_Parms.zw)-Rect_Parms.xy,0.0));
        half dx = max(fwidth(d)*Filter_Width,0.00001);
        
        Inside_Line = 1.0-saturate((Radius-d-Line_Width)/dx);
        Inside_Rect = saturate((Radius-d)/dx);
        
    }
    //BLOCK_END Round_Rect_Fragment

    //BLOCK_BEGIN Proximity_Fragment 115

    void Proximity_Fragment_B115(
        half Proximity_Max_Intensity,
        half Proximity_Near_Radius,
        half4 Deltas,
        half Show_Selection,
        half Distance_Fade1,
        half Distance_Fade2,
        out half Proximity    )
    {
        float proximity1 = (1.0-saturate(length(Deltas.xy)/Proximity_Near_Radius))*Distance_Fade1;
        float proximity2 = (1.0-saturate(length(Deltas.zw)/Proximity_Near_Radius))*Distance_Fade2;
        
        Proximity = (Proximity_Max_Intensity * max(proximity1, proximity2) *(1.0-Show_Selection)+Show_Selection);
        
    }
    //BLOCK_END Proximity_Fragment


    half4 frag(VertexOutput fragInput) : SV_Target
    {
        half4 result;

        half4 Blob_Color_Q118;
        #if defined(_BLOB_ENABLE_)
          Blob_Fragment_B118(fragInput.uv,fragInput.tangent.xyz,_Blob_Texture_,Blob_Color_Q118);
        #else
          Blob_Color_Q118 = half4(0,0,0,0);
        #endif

        half4 Blob_Color_Q117;
        #if defined(_BLOB_ENABLE_2_)
          Blob_Fragment_B117(_Blob_Texture_,fragInput.extra3,Blob_Color_Q117);
        #else
          Blob_Color_Q117 = half4(0,0,0,0);
        #endif

        half Inside_Line_Q106;
        half Inside_Rect_Q106;
        Round_Rect_Fragment_B106(_Radius_,_Line_Width_,_Filter_Width_,fragInput.extra1,Inside_Line_Q106,Inside_Rect_Q106);

        // To_XYZ (#114)
        half X_Q114;
        half Y_Q114;
        half Z_Q114;
        X_Q114=fragInput.binormal.xyz.x;
        Y_Q114=fragInput.binormal.xyz.y;
        Z_Q114=fragInput.binormal.xyz.z;
        
        // Multiply (#101)
        half Product_Q101 = Inside_Rect_Q106 * _Fade_Out_;

        half Proximity_Q115;
        Proximity_Fragment_B115(_Proximity_Max_Intensity_,_Proximity_Near_Radius_,fragInput.extra2,X_Q114,Y_Q114,Z_Q114,Proximity_Q115);

        // Multiply (#102)
        half Product_Q102 = Proximity_Q115 * Inside_Line_Q106;

        half4 Result_Q93;
        Scale_RGB_B93(_Edge_Color_,Product_Q102,Result_Q93);

        // Add_Colors (#119)
        half4 Sum_Q119 = Blob_Color_Q118 + Blob_Color_Q117 + Result_Q93;

        // Scale_Color (#91)
        half4 Result_Q91 = Product_Q101 * Sum_Q119;

        float4 Out_Color = Result_Q91;
        float Clip_Threshold = 0.001;
        bool To_sRGB = false;

        result = Out_Color;
        return result;
    }

    ENDCG
  }
 }
}
