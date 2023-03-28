// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

Shader "Graphics Tools/Canvas/Frontplate" {

Properties {

    [Header(Round Rect)]
        _Radius_("Radius", Float) = 0.3125
        _Line_Width_("Line Width", Float) = .031
        [Toggle] _Relative_To_Height_("Relative To Height", Float) = 1
        _Fixed_Unit_Multiplier_("Fixed Unit Multiplier", Float) = 1000
        _Filter_Width_("Filter Width", Range(0,4)) = 1.5
        _Edge_Color_("Edge Color", Color) = (0.53,0.53,0.53,1)
     
    [Header(Fade)]
        _Fade_Out_("Fade Out", Range(0,1)) = 1
     
    [Header(Blob)]
        [Toggle(_BLOB_ENABLE_)] _Blob_Enable_("Blob Enable", Float) = 0
        _Blob_Position_("Blob Position", Vector) = (.5, 0, 0.2, 1)
        _Blob_Intensity_("Blob Intensity", Range(0,3)) = 0.34
        _Blob_Near_Size_("Blob Near Size", Range(0,1)) = 0.025
        _Blob_Far_Size_("Blob Far Size", Range(0,1)) = 0.05
        _Blob_Near_Distance_("Blob Near Distance", Range(0,1)) = 0
        _Blob_Far_Distance_("Blob Far Distance", Range(0,1)) = 0.08
        _Blob_Fade_Length_("Blob Fade Length", Range(0,1)) = 0.08
        _Blob_Inner_Fade_("Blob Inner Fade", Range(0.001,1)) = 0.01
        _Blob_Pulse_("Blob Pulse", Range(0,1)) = 0
        _Blob_Fade_("Blob Fade", Range(0,1)) = 1
        _Blob_Pulse_Max_Size_("Blob Pulse Max Size", Range(0,1)) = 0.05
     
    [Header(Blob 2)]
        [Toggle(_BLOB_ENABLE_2_)] _Blob_Enable_2_("Blob Enable 2", Float) = 0
        _Blob_Position_2_("Blob Position 2", Vector) = (0, 0, 0, 1)
        _Blob_Near_Size_2_("Blob Near Size 2", Range(0,1)) = 0.025
        _Blob_Inner_Fade_2_("Blob Inner Fade 2", Range(0,1)) = 0.1
        _Blob_Pulse_2_("Blob Pulse 2", Range(0,1)) = 0
        _Blob_Fade_2_("Blob Fade 2", Range(0,1)) = 1
     
    [Header(Gaze)]
        _Gaze_Intensity_("Gaze Intensity", Range(0,1)) = 0.7
        _Gaze_Focus_("Gaze Focus", Range(0,1)) = 0
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
        [Toggle] _Use_Global_Left_Index_("Use Global Left Index", Float) = 1
        [Toggle] _Use_Global_Right_Index_("Use Global Right Index", Float) = 1
     

    [Header(Blending)]
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Source Blend", Float) = 1       // "One"
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Destination Blend", Float) = 1  // "One"

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
    Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
    Blend[_SrcBlend][_DstBlend]
    Cull Off
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
    #pragma shader_feature_local _ _BLOB_ENABLE_
    #pragma shader_feature_local _ _BLOB_ENABLE_2_
    #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
    #pragma multi_compile_local _ _UI_CLIP_RECT_ROUNDED _UI_CLIP_RECT_ROUNDED_INDEPENDENT
    //#pragma multi_compile_local _ _CLIPPING_PLANE _CLIPPING_SPHERE _CLIPPING_BOX

    #include "UnityCG.cginc"
    #include "GraphicsToolsCommon.hlsl"

CBUFFER_START(UnityPerMaterial)
    float _Radius_;
    float _Line_Width_;
    int _Relative_To_Height_;
    float _Fixed_Unit_Multiplier_;
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
    int _Use_Global_Left_Index_;
    int _Use_Global_Right_Index_;
    // #if defined(UNITY_UI_CLIP_RECT)
    float4 _ClipRect;
    // #if defined(_UI_CLIP_RECT_ROUNDED) || defined(_UI_CLIP_RECT_ROUNDED_INDEPENDENT)
    float4 _ClipRectRadii;

CBUFFER_END

    float4 Global_Left_Index_Tip_Position;
    float4 Global_Right_Index_Tip_Position;

    struct VertexInput {
        float4 vertex : POSITION;
        half3 normal : NORMAL;
        float2 uv0 : TEXCOORD0;
        float2 uv2 : TEXCOORD2;
        float4 tangent : TANGENT;
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
        float4 tangent : TANGENT;
        float4 binormal : TEXCOORD6;
        float4 vertexColor : COLOR;
        float4 extra1 : TEXCOORD4;
        float4 extra2 : TEXCOORD3;
        float4 extra3 : TEXCOORD2;
      UNITY_VERTEX_OUTPUT_STEREO
    };

    //BLOCK_BEGIN Blob_Vertex 280

    void Blob_Vertex_B280(
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

    //BLOCK_BEGIN Round_Rect_Vertex 278

    void Round_Rect_Vertex_B278(
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

    //BLOCK_BEGIN Proximity_Vertex 296

    float2 ProjectProximity(
        float3 blobPosition,
        float3 position,
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
    
    void Proximity_Vertex_B296(
        float3 Blob_Position,
        float3 Blob_Position_2,
        float3 Position,
        float Proximity_Far_Distance,
        float Relative_Scale,
        float Proximity_Anisotropy,
        float3 Normal,
        float3 Tangent,
        float3 Binormal,
        float Blob_Enable,
        float Blob_Enable_2,
        out float4 Extra,
        out float Distance_Fade1,
        out float Distance_Fade2    )
    {
        float distz1,distz2;
        Extra.xy = ProjectProximity(Blob_Position,Position,Normal,Tangent*Proximity_Anisotropy,Binormal,distz1)/Relative_Scale;
        Extra.zw = ProjectProximity(Blob_Position_2,Position,Normal,Tangent*Proximity_Anisotropy,Binormal,distz2)/Relative_Scale;
        
        Distance_Fade1 = (1.0 - saturate(distz1/Proximity_Far_Distance))*Blob_Enable;
        Distance_Fade2 = (1.0 - saturate(distz2/Proximity_Far_Distance))*Blob_Enable_2;
        
    }
    //BLOCK_END Proximity_Vertex

    //BLOCK_BEGIN Blob_Vertex 286

    void Blob_Vertex_B286(
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

    //BLOCK_BEGIN Proximity_Visibility 269

    void Proximity_Visibility_B269(
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

    //BLOCK_BEGIN Permutation_To_FloatBool 290

    void Permutation_To_FloatBool_B290(
        out float Enabled    )
    {
        Enabled = 1.0;
    }
    //BLOCK_END Permutation_To_FloatBool

    //BLOCK_BEGIN Selection_Vertex 297

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
    //return saturate(abs(blobCenterXY.y)/faceSize.y);
        return selectPulse.x * selectPulse.y * fadeIn;
    }
    
    void Selection_Vertex_B297(
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
        float Blob_Enable,
        float Blob_Enable_2,
        out float Show_Selection    )
    {
        float select1 = computeSelection(Blob_Position,Normal,Tangent,Bitangent,Face_Center,Face_Size,Selection_Fuzz,Far_Distance,Fade_Length)*Blob_Enable;
        float select2 = computeSelection(Blob_Position_2,Normal,Tangent,Bitangent,Face_Center,Face_Size,Selection_Fuzz,Far_Distance,Fade_Length)*Blob_Enable_2;
        Show_Selection = lerp(max(select1,select2),1.0,Selected);
    }
    //BLOCK_END Selection_Vertex

    //BLOCK_BEGIN Object_To_World_Dir 248

    void Object_To_World_Dir_B248(
        float3 Dir_Object,
        out float3 Binormal_World    )
    {
        Binormal_World = (mul((float3x3)UNITY_MATRIX_M, Dir_Object));
        
    }
    //BLOCK_END Object_To_World_Dir

    //BLOCK_BEGIN GeneralScale 314

    void GeneralScale_B314(
        out float MinGeneralScale    )
    {
        float scaleX = length((mul((float3x3)UNITY_MATRIX_M, float3(1,0,0))));
        float scaleY = length((mul((float3x3)UNITY_MATRIX_M, float3(0,1,0))));
        float scaleZ = length((mul((float3x3)UNITY_MATRIX_M, float3(0,0,1))));
        MinGeneralScale = min(min(scaleX,scaleY),scaleZ);
    }
    //BLOCK_END GeneralScale


    VertexOutput vert(VertexInput vertInput)
    {
        UNITY_SETUP_INSTANCE_ID(vertInput);
        VertexOutput o;
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

        // Object_To_World_Pos (#293)
        float3 Pos_World_Q293;
        Pos_World_Q293=(mul(UNITY_MATRIX_M, float4(vertInput.vertex.xyz, 1)));
        
        // Object_To_World_Dir (#300)
        float3 Nrm_World_Q300;
        Nrm_World_Q300 = normalize((mul((float3x3)UNITY_MATRIX_M, vertInput.normal)));
        
        // Conditional (#261)
        float3 Result_Q261;
        Result_Q261 = _Use_Global_Left_Index_ ? Global_Left_Index_Tip_Position.xyz : _Blob_Position_;
        
        // Lerp (#271)
        float Value_At_T_Q271=lerp(_Blob_Near_Size_,_Blob_Pulse_Max_Size_,_Blob_Pulse_);

        // Anisotropy (#288)
        float Anisotropy_Q288=vertInput.uv2.x/vertInput.uv2.y;

        // Conditional (#262)
        float3 Result_Q262;
        Result_Q262 = _Use_Global_Right_Index_ ? Global_Right_Index_Tip_Position.xyz : _Blob_Position_2_;
        
        float Enabled_Q290;
        #if defined(_BLOB_ENABLE_)
          Permutation_To_FloatBool_B290(Enabled_Q290);
        #else
          Enabled_Q290 = 0;
        #endif

        float Enabled_Q291;
        #if defined(_BLOB_ENABLE_2_)
          Permutation_To_FloatBool_B290(Enabled_Q291);
        #else
          Enabled_Q291 = 0;
        #endif

        // Lerp (#272)
        float Value_At_T_Q272=lerp(_Blob_Near_Size_2_,_Blob_Pulse_Max_Size_,_Blob_Pulse_2_);

        // Object_To_World_Dir (#247)
        float3 Tangent_World_Q247;
        Tangent_World_Q247 = (mul((float3x3)UNITY_MATRIX_M, vertInput.tangent));
        
        float3 Binormal_World_Q248;
        Object_To_World_Dir_B248((normalize(cross(vertInput.normal,vertInput.tangent))),Binormal_World_Q248);

        float MinGeneralScale_Q314;
        GeneralScale_B314(MinGeneralScale_Q314);

        // Multiply (#263)
        float Product_Q263 = _Gaze_Intensity_ * _Gaze_Focus_;

        // Normalize3 (#249)
        float3 Tangent_World_N_Q249 = normalize(Tangent_World_Q247);

        // Normalize3 (#250)
        float3 Binormal_World_N_Q250 = normalize(Binormal_World_Q248);

        // Face_Size (#305)
        float2 Face_Size_Q305;
        float ScaleY_Q305;
        Face_Size_Q305 = vertInput.uv2*MinGeneralScale_Q314;
        ScaleY_Q305 = vertInput.uv2.y;
        
        // Step (#264)
        float Step_Q264 = step(0.0001, Product_Q263);

        float4 Extra_Q296;
        float Distance_Fade1_Q296;
        float Distance_Fade2_Q296;
        Proximity_Vertex_B296(Result_Q261,Result_Q262,Pos_World_Q293,_Proximity_Far_Distance_,1.0,_Proximity_Anisotropy_,Nrm_World_Q300,Tangent_World_N_Q249,Binormal_World_N_Q250,Enabled_Q290,Enabled_Q291,Extra_Q296,Distance_Fade1_Q296,Distance_Fade2_Q296);

        // Scale_Radius_And_Width (#289)
        float Out_Radius_Q289;
        float Out_Line_Width_Q289;
        Out_Radius_Q289 = _Relative_To_Height_ ? _Radius_ : _Radius_ * _Fixed_Unit_Multiplier_ / ScaleY_Q305;
        Out_Line_Width_Q289 = _Relative_To_Height_ ? _Line_Width_ : _Line_Width_ * _Fixed_Unit_Multiplier_ / ScaleY_Q305;

        // FaceCenter (#299)
        float3 Face_Center_Q299;
        //Face_Center_Q299=(mul(UNITY_MATRIX_M, float4(Pos_Object, 1)));
        Face_Center_Q299 = Pos_World_Q293 + (0.5-vertInput.uv0.x)*Face_Size_Q305.x*Tangent_World_N_Q249 - (0.5-vertInput.uv0.y)*Face_Size_Q305.y*Binormal_World_N_Q250;

        float Show_Selection_Q297;
        Selection_Vertex_B297(Result_Q261,Result_Q262,Face_Center_Q299,Face_Size_Q305,Nrm_World_Q300,Tangent_World_N_Q249,Binormal_World_N_Q250,_Selection_Fuzz_,_Selected_,_Selected_Distance_,_Selected_Fade_Length_,float3(0,0,-1),Enabled_Q290,Enabled_Q291,Show_Selection_Q297);

        float2 Out_UV_Q280;
        float3 Blob_Info_Q280;
        #if defined(_BLOB_ENABLE_)
          Blob_Vertex_B280(Pos_World_Q293,Nrm_World_Q300,Tangent_World_N_Q249,Binormal_World_N_Q250,Result_Q261,_Blob_Intensity_,Value_At_T_Q271,_Blob_Far_Size_,_Blob_Near_Distance_,_Blob_Far_Distance_,vertInput.uv0,Face_Center_Q299,Face_Size_Q305,_Blob_Fade_Length_,_Selection_Fade_,_Selection_Fade_Size_,_Blob_Inner_Fade_,_Blob_Pulse_,_Blob_Fade_,Out_UV_Q280,Blob_Info_Q280);
        #else
          Out_UV_Q280 = float2(0,0);
          Blob_Info_Q280 = float3(0,0,0);
        #endif

        float4 Rect_Parms_Q278;
        Round_Rect_Vertex_B278(vertInput.uv0,Out_Radius_Q289,Anisotropy_Q288,Rect_Parms_Q278);

        float4 Blob_Info_Q286;
        #if defined(_BLOB_ENABLE_2_)
          Blob_Vertex_B286(Pos_World_Q293,Nrm_World_Q300,Tangent_World_N_Q249,Binormal_World_N_Q250,Result_Q262,_Blob_Intensity_,Value_At_T_Q272,_Blob_Far_Size_,_Blob_Near_Distance_,_Blob_Far_Distance_,vertInput.uv0,Face_Center_Q299,Face_Size_Q305,_Blob_Fade_Length_,_Selection_Fade_,_Selection_Fade_Size_,_Blob_Inner_Fade_2_,_Blob_Pulse_2_,_Blob_Fade_2_,Blob_Info_Q286);
        #else
          Blob_Info_Q286 = float4(0,0,0,0);
        #endif

        float Width_Q269;
        Proximity_Visibility_B269(Show_Selection_Q297,Result_Q261,Result_Q262,_Proximity_Far_Distance_,_Proximity_Near_Radius_,Face_Center_Q299,Nrm_World_Q300,Face_Size_Q305,Step_Q264,Width_Q269);

        // Multiply (#279)
        float Product_Q279 = Out_Line_Width_Q289 * Width_Q269;

        // Max (#260)
        float MaxAB_Q260=max(Show_Selection_Q297,Product_Q263);

        // Visibility (#287)
        float3 Result_Q287 = (Width_Q269+Blob_Info_Q280.y+Blob_Info_Q286.y)>0.0 ? Pos_World_Q293 : float3(0.0,0.0,0.0);

        // From_XYZ (#275)
        float3 Vec3_Q275 = float3(Out_Radius_Q289,Product_Q279,Out_Line_Width_Q289);

        // From_XYZ (#281)
        float3 Vec3_Q281 = float3(MaxAB_Q260,Distance_Fade1_Q296,Distance_Fade2_Q296);

        float3 Position = Result_Q287;
        float3 Normal = Vec3_Q275;
        float2 UV = Out_UV_Q280;
        float3 Tangent = Blob_Info_Q280;
        float3 Binormal = Vec3_Q281;
        float4 Color = vertInput.color;
        float4 Extra1 = Rect_Parms_Q278;
        float4 Extra2 = Extra_Q296;
        float4 Extra3 = Blob_Info_Q286;

        o.pos = mul(UNITY_MATRIX_VP, float4(Position,1));
        o.posWorld = Position;
#ifdef UNITY_UI_CLIP_RECT
        o.posLocal = vertInput.vertex.xyz;
#endif
        o.normalWorld.xyz = Normal; o.normalWorld.w=1.0;
        o.uv = UV;
        o.tangent.xyz = Tangent; o.tangent.w=1.0;
        o.binormal.xyz = Binormal; o.binormal.w=1.0;
        o.vertexColor = Color;
        o.extra1=Extra1;
        o.extra2=Extra2;
        o.extra3=Extra3;

        return o;
    }

    //BLOCK_BEGIN Blob_Fragment 284

    void Blob_Fragment_B284(
        float2 UV,
        float3 Blob_Info,
        sampler2D Blob_Texture,
        out half4 Blob_Color    )
    {
        float k = dot(UV,UV);
        Blob_Color = Blob_Info.y * tex2D(Blob_Texture,float2(float2(sqrt(k),Blob_Info.x).x,1.0-float2(sqrt(k),Blob_Info.x).y))*(1.0-saturate(k));
    }
    //BLOCK_END Blob_Fragment

    //BLOCK_BEGIN Blob_Fragment 283

    void Blob_Fragment_B283(
        sampler2D Blob_Texture,
        float4 Blob_Info,
        out half4 Blob_Color    )
    {
        float k = dot(Blob_Info.zw,Blob_Info.zw);
        Blob_Color = Blob_Info.y * tex2D(Blob_Texture,float2(float2(sqrt(k),Blob_Info.x).x,1.0-float2(sqrt(k),Blob_Info.x).y))*(1.0-saturate(k));
    }
    //BLOCK_END Blob_Fragment

    //BLOCK_BEGIN Scale_RGB 267

    void Scale_RGB_B267(
        half4 Color,
        half Scalar,
        out half4 Result    )
    {
        Result = float4(Scalar,Scalar,Scalar,1) * Color;
    }
    //BLOCK_END Scale_RGB

    //BLOCK_BEGIN Round_Rect_Fragment 277

    void Round_Rect_Fragment_B277(
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

    //BLOCK_BEGIN Proximity_Fragment 295

    void Proximity_Fragment_B295(
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
        ClipAgainstPrimitive(fragInput.posWorld);

    #ifdef UNITY_UI_CLIP_RECT
        clip(GTUnityUIClipRect(fragInput.posLocal.xy, _ClipRect, _ClipRectRadii) - 0.5);
    #endif
        half4 result;

        half4 Blob_Color_Q284;
        #if defined(_BLOB_ENABLE_)
          Blob_Fragment_B284(fragInput.uv,fragInput.tangent.xyz,_Blob_Texture_,Blob_Color_Q284);
        #else
          Blob_Color_Q284 = half4(0,0,0,0);
        #endif

        half4 Blob_Color_Q283;
        #if defined(_BLOB_ENABLE_2_)
          Blob_Fragment_B283(_Blob_Texture_,fragInput.extra3,Blob_Color_Q283);
        #else
          Blob_Color_Q283 = half4(0,0,0,0);
        #endif

        // To_XYZ (#276)
        half X_Q276;
        half Y_Q276;
        half Z_Q276;
        X_Q276=fragInput.normalWorld.xyz.x;
        Y_Q276=fragInput.normalWorld.xyz.y;
        Z_Q276=fragInput.normalWorld.xyz.z;
        
        // To_XYZ (#282)
        half X_Q282;
        half Y_Q282;
        half Z_Q282;
        X_Q282=fragInput.binormal.xyz.x;
        Y_Q282=fragInput.binormal.xyz.y;
        Z_Q282=fragInput.binormal.xyz.z;
        
        half Inside_Line_Q277;
        half Inside_Rect_Q277;
        Round_Rect_Fragment_B277(X_Q276,Y_Q276,_Filter_Width_,fragInput.extra1,Inside_Line_Q277,Inside_Rect_Q277);

        half Proximity_Q295;
        Proximity_Fragment_B295(_Proximity_Max_Intensity_,_Proximity_Near_Radius_,fragInput.extra2,X_Q282,Y_Q282,Z_Q282,Proximity_Q295);

        // Multiply (#273)
        half Product_Q273 = Inside_Rect_Q277 * _Fade_Out_;

        // Multiply (#274)
        half Product_Q274 = Proximity_Q295 * Inside_Line_Q277;

        half4 Result_Q267;
        Scale_RGB_B267(_Edge_Color_,Product_Q274,Result_Q267);

        // Add_Colors (#285)
        half4 Sum_Q285 = Blob_Color_Q284 + Blob_Color_Q283 + Result_Q267;

        // Scale_Color (#265)
        half4 Result_Q265 = Product_Q273 * Sum_Q285;

        // Multiply_Colors (#308)
        half4 Product_Q308 = fragInput.vertexColor * Result_Q265;

        half4 Out_Color = Product_Q308;
        half Clip_Threshold = 0.001;
        bool To_sRGB = false;

        result = Out_Color;
        return result;
    }

    ENDCG
  }
 }
}
