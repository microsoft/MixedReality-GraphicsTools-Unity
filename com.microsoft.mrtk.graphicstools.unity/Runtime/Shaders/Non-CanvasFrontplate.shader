// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

Shader "Graphics Tools/Non-Canvas/Frontplate" {

Properties {

    [Header(Round Rect)]
        _Radius_("Radius", Range(0,0.5)) = 0.01
        _Line_Width_("Line Width", Range(0,1)) = 0.001
        [Toggle] _Relative_To_Height_("Relative To Height", Float) = 0
        _Filter_Width_("Filter Width", Range(0,4)) = 1.5
        _Edge_Color_("Edge Color", Color) = (0.53,0.53,0.53,1)
     
    [Header(Fade)]
        _Fade_Out_("Fade Out", Range(0,1)) = 1
     
    [Header(Antialiasing)]
        [Toggle] _Smooth_Edges_("Smooth Edges", Float) = 1
     
    [Header(Blob)]
        [Toggle] _Blob_Enable_("Blob Enable", Float) = 1
        [PerRendererData] _Blob_Position_("Blob Position",Vector) = (0,0,0,1)
        _Blob_Intensity_("Blob Intensity", Range(0,3)) = 0.5
        _Blob_Near_Size_("Blob Near Size", Range(0,1)) = 0.032
        _Blob_Far_Size_("Blob Far Size", Range(0,1)) = 0.048
        _Blob_Near_Distance_("Blob Near Distance", Range(0,1)) = 0.008
        _Blob_Far_Distance_("Blob Far Distance", Range(0,1)) = 0.064
        _Blob_Fade_Length_("Blob Fade Length", Range(0,1)) = 0.04
        _Blob_Inner_Fade_("Blob Inner Fade", Range(0.001,1)) = 0.01
        [PerRendererData] _Blob_Pulse_("Blob Pulse",Range(0,1)) = 0
        [PerRendererData] _Blob_Fade_("Blob Fade",Range(0,1)) = 1
        _Blob_Pulse_Max_Size_("Blob Pulse Max Size", Range(0,1)) = 0.05
     
    [Header(Blob 2)]
        [Toggle] _Blob_Enable_2_("Blob Enable 2", Float) = 1
        [PerRendererData] _Blob_Position_2_("Blob Position 2",Vector) = (0,0,0,1)
        _Blob_Near_Size_2_("Blob Near Size 2", Range(0,1)) = 0.008
        _Blob_Inner_Fade_2_("Blob Inner Fade 2", Range(0,1)) = 0.1
        [PerRendererData] _Blob_Pulse_2_("Blob Pulse 2",Range(0,1)) = 0
        [PerRendererData] _Blob_Fade_2_("Blob Fade 2",Range(0,1)) = 1
     
    [Header(Gaze)]
        _Gaze_Intensity_("Gaze Intensity", Range(0,1)) = 0.8
        [PerRendererData] _Gaze_Focus_("Gaze Focus",Range(0,1)) = 0.0
        [PerRendererData] _Pinched_("Pinched",Float) = 0.0
     
    [Header(Blob Texture)]
        [NoScaleOffset] _Blob_Texture_("Blob Texture", 2D) = "" {}
     
    [Header(Selection)]
        _Selection_Fuzz_("Selection Fuzz", Range(0,1)) = 0.5
        _Selected_("Selected", Range(0,1)) = 0
        _Selection_Fade_("Selection Fade", Range(0,1)) = 0.2
        _Selection_Fade_Size_("Selection Fade Size", Range(0,1)) = 0.0
        _Selected_Distance_("Selected Distance", Range(0,1)) = 0.08
        _Selected_Fade_Length_("Selected Fade Length", Range(0,1)) = 0.08
     
    [Header(Proximity)]
        _Proximity_Max_Intensity_("Proximity Max Intensity", Range(0,1)) = 0.45
        _Proximity_Far_Distance_("Proximity Far Distance", Range(0,2)) = 0.16
        _Proximity_Near_Radius_("Proximity Near Radius", Range(0,2)) = 0.016
        _Proximity_Anisotropy_("Proximity Anisotropy", Range(0,1)) = 1
     
    [Header(Global)]
        [PerRendererData] _Use_Global_Left_Index_("Use Global Left Index",Float) = 1
        [PerRendererData] _Use_Global_Right_Index_("Use Global Right Index",Float) = 1
     

    [Header(Stencil)]
        _StencilReference("Stencil Reference", Range(0, 255)) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)]_StencilComparison("Stencil Comparison", Int) = 0
        [Enum(UnityEngine.Rendering.StencilOp)]_StencilOperation("Stencil Operation", Int) = 0
}

SubShader {
    Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
    Blend One One
    Cull Off
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
    float _Radius_;
    float _Line_Width_;
    int _Relative_To_Height_;
    float _Filter_Width_;
    half4 _Edge_Color_;
    half _Fade_Out_;
    int _Smooth_Edges_;
    int _Blob_Enable_;
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
    int _Blob_Enable_2_;
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
    int _Use_Global_Left_Index_;
    int _Use_Global_Right_Index_;

CBUFFER_END

    float4 Global_Left_Index_Tip_Position;
    float4 Global_Right_Index_Tip_Position;

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
        half4 normalWorld : TEXCOORD5;
        float2 uv : TEXCOORD0;
        float3 posWorld : TEXCOORD7;
        float4 tangent : TANGENT;
        float4 extra1 : TEXCOORD4;
        float4 extra2 : TEXCOORD3;
        float4 extra3 : TEXCOORD2;
      UNITY_VERTEX_OUTPUT_STEREO
    };

    //BLOCK_BEGIN Blob_Vertex 1239

    void Blob_Vertex_B1239(
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
        float4 Vx_Color,
        float2 UV,
        float3 Face_Center,
        float2 Face_Size,
        float2 In_UV,
        float Blob_Fade_Length,
        float Selection_Fade,
        float Selection_Fade_Size,
        float Inner_Fade,
        float Blob_Pulse,
        float Blob_Fade,
        float Blob_Enabled,
        float DistanceOffset,
        out float3 Out_Position,
        out float2 Out_UV,
        out float3 Blob_Info,
        out float2 Blob_Relative_UV    )
    {
        
        float blobSize, fadeIn;
        float3 Hit_Position;
        Blob_Info = float3(0.0,0.0,0.0);
        
        float Hit_Distance = dot(Blob_Position-Face_Center, Normal) + DistanceOffset*Blob_Far_Distance;
        Hit_Position = Blob_Position - Hit_Distance * Normal;
        
        float absD = abs(Hit_Distance);
        float lerpVal = clamp((absD-Blob_Near_Distance)/(Blob_Far_Distance-Blob_Near_Distance),0.0,1.0);
        fadeIn = 1.0-clamp((absD-Blob_Far_Distance)/Blob_Fade_Length,0.0,1.0);
        
        float innerFade = 1.0-clamp(-Hit_Distance/Inner_Fade,0.0,1.0);
        
        //compute blob size
        float farClip = saturate(1.0-step(Blob_Far_Distance+Blob_Fade_Length,absD));
        float size = lerp(Blob_Near_Size,Blob_Far_Size,lerpVal)*farClip;
        blobSize = lerp(size,Selection_Fade_Size,Selection_Fade)*innerFade*Blob_Enabled;
        Blob_Info.x = lerpVal*0.5+0.5;
            
        Blob_Info.y = fadeIn*Intensity*(1.0-Selection_Fade)*Blob_Fade;
        Blob_Info.x *= (1.0-Blob_Pulse);
        
        //compute blob position
        float3 delta = Hit_Position - Face_Center;
        float2 blobCenterXY = float2(dot(delta,Tangent),dot(delta,Bitangent));
        
        float2 quadUVin = 2.0*UV-1.0;  // remap to (-.5,.5)
        float2 blobXY = blobCenterXY+quadUVin*blobSize;
        
        //keep the quad within the face
        float2 blobClipped = clamp(blobXY,-Face_Size*0.5,Face_Size*0.5);
        float2 blobUV = (blobClipped-blobCenterXY)/max(blobSize,0.0001)*2.0;
        
        float3 blobCorner = Face_Center + blobClipped.x*Tangent + blobClipped.y*Bitangent;
        
        //blend using VxColor.r=1 for blob quad, 0 otherwise
        Out_Position = lerp(Position,blobCorner,Vx_Color.rrr);
        Out_UV = lerp(In_UV,blobUV,Vx_Color.rr);
        Blob_Relative_UV = blobClipped/Face_Size.y;
    }
    //BLOCK_END Blob_Vertex

    //BLOCK_BEGIN Round_Rect_Vertex 1235

    void Round_Rect_Vertex_B1235(
        float2 UV,
        float3 Tangent,
        float3 Binormal,
        float Radius,
        float Anisotropy,
        float2 Blob_Center_UV,
        out float2 Rect_UV,
        out float2 Scale_XY,
        out float4 Rect_Parms    )
    {
        Scale_XY = float2(Anisotropy,1.0);
        Rect_UV = (UV - float2(0.5,0.5)) * Scale_XY;
        Rect_Parms.xy = Scale_XY*0.5-float2(Radius,Radius);
        Rect_Parms.zw = Blob_Center_UV;
    }
    //BLOCK_END Round_Rect_Vertex

    //BLOCK_BEGIN Proximity_Vertex 1232

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
    
    void Proximity_Vertex_B1232(
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

    //BLOCK_BEGIN Object_To_World_Pos 1211

    void Object_To_World_Pos_B1211(
        float3 Pos_Object,
        out float3 Pos_World    )
    {
        Pos_World=(mul(UNITY_MATRIX_M, float4(Pos_Object, 1)));
        
    }
    //BLOCK_END Object_To_World_Pos

    //BLOCK_BEGIN Choose_Blob 1226

    void Choose_Blob_B1226(
        float4 Vx_Color,
        float3 Position1,
        float3 Position2,
        bool Blob_Enable_1,
        bool Blob_Enable_2,
        float Near_Size_1,
        float Near_Size_2,
        float Blob_Inner_Fade_1,
        float Blob_Inner_Fade_2,
        float Blob_Pulse_1,
        float Blob_Pulse_2,
        float Blob_Fade_1,
        float Blob_Fade_2,
        out float3 Position,
        out float Near_Size,
        out float Inner_Fade,
        out float Blob_Enable,
        out float Fade,
        out float Pulse    )
    {
        Position = Position1*(1.0-Vx_Color.g)+Vx_Color.g*Position2;
        
        float b1 = Blob_Enable_1 ? 1.0 : 0.0;
        float b2 = Blob_Enable_2 ? 1.0 : 0.0;
        Blob_Enable = b1+(b2-b1)*Vx_Color.g;
        
        Pulse = Blob_Pulse_1*(1.0-Vx_Color.g)+Vx_Color.g*Blob_Pulse_2;
        Fade = Blob_Fade_1*(1.0-Vx_Color.g)+Vx_Color.g*Blob_Fade_2;
        Near_Size = Near_Size_1*(1.0-Vx_Color.g)+Vx_Color.g*Near_Size_2;
        Inner_Fade = Blob_Inner_Fade_1*(1.0-Vx_Color.g)+Vx_Color.g*Blob_Inner_Fade_2;
    }
    //BLOCK_END Choose_Blob

    //BLOCK_BEGIN Move_Verts 1231

    void Move_Verts_B1231(
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

    //BLOCK_BEGIN Object_To_World_Dir 1213

    void Object_To_World_Dir_B1213(
        float3 Dir_Object,
        out float3 Binormal_World    )
    {
        Binormal_World = (mul((float3x3)UNITY_MATRIX_M, Dir_Object));
        
    }
    //BLOCK_END Object_To_World_Dir

    //BLOCK_BEGIN Proximity_Visibility 1254

    void Proximity_Visibility_B1254(
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
        
        //float3 objectOriginInWorld = (mul(UNITY_MATRIX_M, float4(float3(0.0,0.0,0.0), 1)));
        float3 delta1 = blob1 - Face_Center;
        float3 delta2 = blob2 - Face_Center;
        
        float dist1 = dot(delta1,delta1);
        float dist2 = dot(delta2,delta2);
        
        float nearestProxDist = sqrt(min(dist1,dist2));
        
        Width = (1.0 - step(boxMaxSize+Proximity_Radius,nearestProxDist))*(1.0-step(Proximity_Far_Distance,min(d1,d2))*(1.0-step(0.0001,Selection)));
        Width = max(Gaze, Width);
    }
    //BLOCK_END Proximity_Visibility

    //BLOCK_BEGIN Selection_Vertex 1230

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
    
    void Selection_Vertex_B1230(
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


    VertexOutput vert(VertexInput vertInput)
    {
        UNITY_SETUP_INSTANCE_ID(vertInput);
        VertexOutput o;
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

        // Pack_For_Vertex (#1228)
        float3 Vec3_Q1228 = float3(float2(0,0).x,float2(0,0).y,vertInput.color.r);

        // Object_To_World_Dir (#1223)
        float3 Nrm_World_Q1223;
        Nrm_World_Q1223 = normalize((mul((float3x3)UNITY_MATRIX_M, vertInput.normal)));
        
        // Object_To_World_Pos (#1229)
        float3 Face_Center_Q1229;
        Face_Center_Q1229=(mul(UNITY_MATRIX_M, float4(float3(0,0,0), 1)));
        
        // Object_To_World_Dir (#1212)
        float3 Tangent_World_Q1212;
        Tangent_World_Q1212 = (mul((float3x3)UNITY_MATRIX_M, vertInput.tangent));
        
        float3 Binormal_World_Q1213;
        Object_To_World_Dir_B1213((normalize(cross(vertInput.normal,vertInput.tangent))),Binormal_World_Q1213);

        // Anisotropy (#1220)
        float Anisotropy_Q1220=length(Tangent_World_Q1212)/length(Binormal_World_Q1213);

        // Conditional (#1241)
        float3 Result_Q1241;
        Result_Q1241 = _Use_Global_Left_Index_ ? Global_Left_Index_Tip_Position.xyz : _Blob_Position_;
        
        // Conditional (#1242)
        float3 Result_Q1242;
        Result_Q1242 = _Use_Global_Right_Index_ ? Global_Right_Index_Tip_Position.xyz : _Blob_Position_2_;
        
        // Lerp (#1257)
        float Value_At_T_Q1257=lerp(_Blob_Near_Size_,_Blob_Pulse_Max_Size_,_Blob_Pulse_);

        // Lerp (#1258)
        float Value_At_T_Q1258=lerp(_Blob_Near_Size_2_,_Blob_Pulse_Max_Size_,_Blob_Pulse_2_);

        // Multiply (#1244)
        float Product_Q1244 = _Gaze_Intensity_ * _Gaze_Focus_;

        // Step (#1245)
        float Step_Q1245 = step(0.0001, Product_Q1244);

        // Normalize3 (#1214)
        float3 Tangent_World_N_Q1214 = normalize(Tangent_World_Q1212);

        // Normalize3 (#1215)
        float3 Binormal_World_N_Q1215 = normalize(Binormal_World_Q1213);

        float3 Position_Q1226;
        float Near_Size_Q1226;
        float Inner_Fade_Q1226;
        float Blob_Enable_Q1226;
        float Fade_Q1226;
        float Pulse_Q1226;
        Choose_Blob_B1226(vertInput.color,Result_Q1241,Result_Q1242,_Blob_Enable_,_Blob_Enable_2_,Value_At_T_Q1257,Value_At_T_Q1258,_Blob_Inner_Fade_,_Blob_Inner_Fade_2_,_Blob_Pulse_,_Blob_Pulse_2_,_Blob_Fade_,_Blob_Fade_2_,Position_Q1226,Near_Size_Q1226,Inner_Fade_Q1226,Blob_Enable_Q1226,Fade_Q1226,Pulse_Q1226);

        // Face_Size (#1234)
        float2 Face_Size_Q1234;
        float ScaleY_Q1234;
        Face_Size_Q1234 = float2(length(Tangent_World_Q1212),length(Binormal_World_Q1213));
        ScaleY_Q1234 = Face_Size_Q1234.y;
        
        // Scale_Radius_And_Width (#1237)
        float Out_Radius_Q1237;
        float Out_Line_Width_Q1237;
        Out_Radius_Q1237 = _Relative_To_Height_ ? _Radius_ : _Radius_ / ScaleY_Q1234;
        Out_Line_Width_Q1237 = _Relative_To_Height_ ? _Line_Width_ : _Line_Width_ / ScaleY_Q1234;

        float Show_Selection_Q1230;
        Selection_Vertex_B1230(Result_Q1241,Result_Q1242,Face_Center_Q1229,Face_Size_Q1234,Nrm_World_Q1223,Tangent_World_N_Q1214,Binormal_World_N_Q1215,_Selection_Fuzz_,_Selected_,_Selected_Distance_,_Selected_Fade_Length_,float3(0,0,-1),Show_Selection_Q1230);

        // Max (#1240)
        float MaxAB_Q1240=max(Show_Selection_Q1230,Product_Q1244);

        float Width_Q1254;
        Proximity_Visibility_B1254(Show_Selection_Q1230,Result_Q1241,Result_Q1242,_Proximity_Far_Distance_,_Proximity_Near_Radius_,Face_Center_Q1229,Nrm_World_Q1223,Face_Size_Q1234,Step_Q1245,Width_Q1254);

        float3 New_P_Q1231;
        float2 New_UV_Q1231;
        Move_Verts_B1231(vertInput.uv0,Out_Radius_Q1237,Anisotropy_Q1220,Out_Line_Width_Q1237,Width_Q1254,New_P_Q1231,New_UV_Q1231);

        float3 Pos_World_Q1211;
        Object_To_World_Pos_B1211(New_P_Q1231,Pos_World_Q1211);

        float3 Out_Position_Q1239;
        float2 Out_UV_Q1239;
        float3 Blob_Info_Q1239;
        float2 Blob_Relative_UV_Q1239;
        Blob_Vertex_B1239(Pos_World_Q1211,Nrm_World_Q1223,Tangent_World_N_Q1214,Binormal_World_N_Q1215,Position_Q1226,_Blob_Intensity_,Near_Size_Q1226,_Blob_Far_Size_,_Blob_Near_Distance_,_Blob_Far_Distance_,vertInput.color,vertInput.uv0,Face_Center_Q1229,Face_Size_Q1234,New_UV_Q1231,_Blob_Fade_Length_,_Selection_Fade_,_Selection_Fade_Size_,Inner_Fade_Q1226,Pulse_Q1226,Fade_Q1226,Blob_Enable_Q1226,0.0,Out_Position_Q1239,Out_UV_Q1239,Blob_Info_Q1239,Blob_Relative_UV_Q1239);

        float2 Rect_UV_Q1235;
        float2 Scale_XY_Q1235;
        float4 Rect_Parms_Q1235;
        Round_Rect_Vertex_B1235(New_UV_Q1231,Tangent_World_Q1212,Binormal_World_Q1213,Out_Radius_Q1237,Anisotropy_Q1220,Blob_Relative_UV_Q1239,Rect_UV_Q1235,Scale_XY_Q1235,Rect_Parms_Q1235);

        float4 Extra_Q1232;
        float Distance_To_Face_Q1232;
        float Distance_Fade1_Q1232;
        float Distance_Fade2_Q1232;
        Proximity_Vertex_B1232(Result_Q1241,Result_Q1242,Face_Center_Q1229,Pos_World_Q1211,_Proximity_Far_Distance_,1.0,_Proximity_Anisotropy_,Nrm_World_Q1223,Tangent_World_N_Q1214,Binormal_World_N_Q1215,Extra_Q1232,Distance_To_Face_Q1232,Distance_Fade1_Q1232,Distance_Fade2_Q1232);

        // From_XYZW (#1236)
        float4 Vec4_Q1236 = float4(MaxAB_Q1240, Distance_Fade1_Q1232, Distance_Fade2_Q1232, Out_Radius_Q1237);

        float3 Position = Out_Position_Q1239;
        float3 Normal = Vec3_Q1228;
        float2 UV = Out_UV_Q1239;
        float3 Tangent = Blob_Info_Q1239;
        float3 Binormal = float3(0,0,0);
        float4 Color = float4(1,1,1,1);
        float4 Extra1 = Rect_Parms_Q1235;
        float4 Extra2 = Extra_Q1232;
        float4 Extra3 = Vec4_Q1236;

        o.pos = mul(UNITY_MATRIX_VP, float4(Position,1));
        o.posWorld = Position;
        o.normalWorld.xyz = Normal; o.normalWorld.w=1.0;
        o.uv = UV;
        o.tangent.xyz = Tangent; o.tangent.w=1.0;
        o.extra1=Extra1;
        o.extra2=Extra2;
        o.extra3=Extra3;

        return o;
    }

    //BLOCK_BEGIN Scale_Color 1253

    void Scale_Color_B1253(
        half4 Color,
        half Scalar,
        out half4 Result    )
    {
        Result = Scalar * Color;
    }
    //BLOCK_END Scale_Color

    //BLOCK_BEGIN Scale_RGB 1249

    void Scale_RGB_B1249(
        half4 Color,
        half Scalar,
        out half4 Result    )
    {
        Result = float4(Scalar,Scalar,Scalar,1) * Color;
    }
    //BLOCK_END Scale_RGB

    //BLOCK_BEGIN Proximity_Fragment 1250

    void Proximity_Fragment_B1250(
        half Proximity_Max_Intensity,
        half Proximity_Near_Radius,
        half4 Deltas,
        half Show_Selection,
        half Distance_Fade1,
        half Distance_Fade2,
        half Strength,
        out half Proximity    )
    {
        float proximity1 = (1.0-saturate(length(Deltas.xy)/Proximity_Near_Radius))*Distance_Fade1;
        float proximity2 = (1.0-saturate(length(Deltas.zw)/Proximity_Near_Radius))*Distance_Fade2;
        
        Proximity = Strength * (Proximity_Max_Intensity * max(proximity1, proximity2) *(1.0-Show_Selection)+Show_Selection);
        
    }
    //BLOCK_END Proximity_Fragment

    //BLOCK_BEGIN Blob_Fragment 1255

    void Blob_Fragment_B1255(
        float2 UV,
        float3 Blob_Info,
        sampler2D Blob_Texture,
        out half4 Blob_Color    )
    {
        float k = dot(UV,UV);
        Blob_Color = Blob_Info.y * tex2D(Blob_Texture,float2(float2(sqrt(k),Blob_Info.x).x,1.0-float2(sqrt(k),Blob_Info.x).y))*(1.0-saturate(k));
    }
    //BLOCK_END Blob_Fragment

    //BLOCK_BEGIN Round_Rect_Fragment 1260

    void Round_Rect_Fragment_B1260(
        half Radius,
        half4 Line_Color,
        half Filter_Width,
        half Line_Visibility,
        half4 Fill_Color,
        bool Smooth_Edges,
        half4 Rect_Parms,
        out half Inside_Rect    )
    {
        half d = length(max(abs(Rect_Parms.zw)-Rect_Parms.xy,0.0));
        half dx = max(fwidth(d)*Filter_Width,0.00001);
        
        Inside_Rect = Smooth_Edges ? saturate((Radius-d)/dx) : 1.0-step(Radius,d);
        
    }
    //BLOCK_END Round_Rect_Fragment


    half4 frag(VertexOutput fragInput) : SV_Target
    {
        ClipAgainstPrimitive(fragInput.posWorld);

        half4 result;

        // Is_Quad (#1252)
        half Is_Quad_Q1252;
        Is_Quad_Q1252=fragInput.normalWorld.xyz.z;
        
        half4 Blob_Color_Q1255;
        Blob_Fragment_B1255(fragInput.uv,fragInput.tangent.xyz,_Blob_Texture_,Blob_Color_Q1255);

        // To_XYZW (#1251)
        half X_Q1251;
        half Y_Q1251;
        half Z_Q1251;
        half W_Q1251;
        X_Q1251=fragInput.extra3.x;
        Y_Q1251=fragInput.extra3.y;
        Z_Q1251=fragInput.extra3.z;
        W_Q1251=fragInput.extra3.w;

        half Proximity_Q1250;
        Proximity_Fragment_B1250(_Proximity_Max_Intensity_,_Proximity_Near_Radius_,fragInput.extra2,X_Q1251,Y_Q1251,Z_Q1251,1.0,Proximity_Q1250);

        half Inside_Rect_Q1260;
        Round_Rect_Fragment_B1260(W_Q1251,half4(1,1,1,1),_Filter_Width_,1,half4(0,0,0,0),_Smooth_Edges_,fragInput.extra1,Inside_Rect_Q1260);

        half4 Result_Q1249;
        Scale_RGB_B1249(_Edge_Color_,Proximity_Q1250,Result_Q1249);

        // Scale_Color (#1246)
        half4 Result_Q1246 = Inside_Rect_Q1260 * Blob_Color_Q1255;

        // Mix_Colors (#1247)
        half4 Color_At_T_Q1247 = lerp(Result_Q1249, Result_Q1246,float4( Is_Quad_Q1252, Is_Quad_Q1252, Is_Quad_Q1252, Is_Quad_Q1252));

        half4 Result_Q1253;
        Scale_Color_B1253(Color_At_T_Q1247,_Fade_Out_,Result_Q1253);

        float4 Out_Color = Result_Q1253;
        float Clip_Threshold = 0.001;
        bool To_sRGB = false;

        result = Out_Color;
        return result;
    }

    ENDCG
  }
 }
}
