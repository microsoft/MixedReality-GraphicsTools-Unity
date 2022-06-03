// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

Shader "Graphics Tools/Non-Canvas/Slate Proximity" {

Properties {

    [Header(Shape)]
        [Toggle(_ROUNDED_)] _Rounded_("Rounded", Float) = 1
        _Radius_("Radius", Range(0,1)) = 0.01
     
    [Header(Blob)]
        [PerRendererData] _Blob_Position_("Blob Position",Vector) = (0,0,0,1)
        _Blob_Intensity_("Blob Intensity", Range(0,3)) = 1
        _Blob_Near_Size_("Blob Near Size", Range(0,1)) = 0.02
        _Blob_Far_Size_("Blob Far Size", Range(0,1)) = 0.05
        _Blob_Near_Distance_("Blob Near Distance", Range(0,1)) = 0.0
        _Blob_Far_Distance_("Blob Far Distance", Range(0,1)) = 0.08
        _Blob_Fade_Length_("Blob Fade Length", Range(0,1)) = 0.08
        _Blob_Inner_Fade_("Blob Inner Fade", Range(0.01,1)) = 0.1
        [PerRendererData] _Blob_Pulse_("Blob Pulse",Range(0,1)) = 0
        [PerRendererData] _Blob_Fade_("Blob Fade",Range(0,1)) = 1
     
    [Header(Blob Texture)]
        [NoScaleOffset] _Blob_Texture_("Blob Texture", 2D) = "" {}
     
    [Header(Blob 2)]
        [PerRendererData] _Blob_Position_2_("Blob Position 2",Vector) = (0,0,0,1)
        _Blob_Near_Size_2_("Blob Near Size 2", Float) = 0.02
        _Blob_Inner_Fade_2_("Blob Inner Fade 2", Range(0,1)) = 0.1
        [PerRendererData] _Blob_Pulse_2_("Blob Pulse 2",Range(0,1)) = 0
        [PerRendererData] _Blob_Fade_2_("Blob Fade 2",Range(0,1)) = 1
     
    [Header(Global)]
        [PerRendererData] _Use_Global_Left_Index_("Use Global Left Index",Float) = 1
        [PerRendererData] _Use_Global_Right_Index_("Use Global Right Index",Float) = 1
     

}

SubShader {
    Tags{ "RenderType" = "AlphaTest" "Queue" = "AlphaTest"}
    Blend One OneMinusSrcAlpha
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
    #pragma multi_compile_local _ _ROUNDED_

    #include "UnityCG.cginc"

CBUFFER_START(UnityPerMaterial)
    //bool _Rounded_;
    float _Radius_;
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
    sampler2D _Blob_Texture_;
    float3 _Blob_Position_2_;
    float _Blob_Near_Size_2_;
    float _Blob_Inner_Fade_2_;
    float _Blob_Pulse_2_;
    float _Blob_Fade_2_;
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
        float4 extra1 : TEXCOORD4;
      UNITY_VERTEX_OUTPUT_STEREO
    };

    //BLOCK_BEGIN Blob_Vertex 985

    void Blob_Vertex_B985(
        float3 Face_Center,
        float3 Normal,
        float3 Tangent,
        float3 Bitangent,
        float Intensity,
        float Blob_Far_Size,
        float Blob_Near_Distance,
        float Blob_Far_Distance,
        float Blob_Fade_Length,
        float4 Vx_Color,
        float2 UV,
        float3 Blob_Position,
        float Blob_Near_Size,
        float Inner_Fade,
        float Blob_Enabled,
        float Fade,
        float Pulse,
        float2 Face_Size,
        float Radius,
        out float3 Out_Position,
        out float2 Out_UV,
        out float3 Blob_Info,
        out float4 Clip_Info    )
    {
        
        float Hit_Distance = dot(Blob_Position-Face_Center, Normal);
        float3 Hit_Position = Blob_Position - Hit_Distance * Normal;
        
        float absD = (Hit_Distance);
        float lerpVal = clamp((absD-Blob_Near_Distance)/(Blob_Far_Distance-Blob_Near_Distance),0.0,1.0);
        float fadeIn = 1.0-clamp((absD-Blob_Far_Distance)/Blob_Fade_Length,0.0,1.0);
        
        //compute blob position & uv
        float3 delta = Hit_Position - Face_Center;
        float2 blobCenterXY = float2(dot(delta,Tangent),dot(delta,Bitangent));
        
        float innerFade = 1.0-clamp(-Hit_Distance/Inner_Fade,0.0,1.0);
        
        float size = lerp(Blob_Near_Size,Blob_Far_Size,lerpVal)*Blob_Enabled*step(0.001,fadeIn);
        
        float2 quadUVin = UV-0.5; // remap to (-.5,.5)
        float2 blobXY = blobCenterXY+quadUVin*size;
        //keep the quad within the face
        float2 blobClipped = clamp(blobXY,-Face_Size*0.5,Face_Size*0.5);
        float2 blobUV = (blobClipped-blobCenterXY)/max(size,0.0001)*2.0;
        
        float3 blobCorner = Face_Center + blobClipped.x*Tangent + blobClipped.y*Bitangent;
        
        Out_Position = blobCorner;
        Out_UV = blobUV;
        Blob_Info = float3((lerpVal*0.5+0.5)*(1.0-Pulse),Intensity*fadeIn*Fade*innerFade,0.0);
        Clip_Info = float4(blobClipped.x,blobClipped.y,Face_Size.x*0.5-Radius,Face_Size.y*0.5-Radius);
    }
    //BLOCK_END Blob_Vertex

    //BLOCK_BEGIN Object_To_World_Pos 964

    void Object_To_World_Pos_B964(
        float3 Pos_Object,
        out float3 Pos_World    )
    {
        Pos_World=(mul(UNITY_MATRIX_M, float4(Pos_Object, 1)));
        
    }
    //BLOCK_END Object_To_World_Pos

    //BLOCK_BEGIN Choose_Blob 986

    void Choose_Blob_B986(
        float4 Vx_Color,
        float3 Position1,
        float3 Position2,
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
        out float Fade,
        out float Pulse    )
    {
        Position = Position1*(1.0-Vx_Color.g)+Vx_Color.g*Position2;
        Pulse = Blob_Pulse_1*(1.0-Vx_Color.g)+Vx_Color.g*Blob_Pulse_2;
        Fade = Blob_Fade_1*(1.0-Vx_Color.g)+Vx_Color.g*Blob_Fade_2;
        Near_Size = Near_Size_1*(1.0-Vx_Color.g)+Vx_Color.g*Near_Size_2;
        Inner_Fade = Blob_Inner_Fade_1*(1.0-Vx_Color.g)+Vx_Color.g*Blob_Inner_Fade_2;
    }
    //BLOCK_END Choose_Blob

    //BLOCK_BEGIN Object_To_World_Dir 977

    void Object_To_World_Dir_B977(
        float3 Nrm_Object,
        out float3 Nrm_World    )
    {
        Nrm_World=(mul((float3x3)UNITY_MATRIX_M, Nrm_Object));
    }
    //BLOCK_END Object_To_World_Dir

    //BLOCK_BEGIN Object_To_World_Dir 968

    void Object_To_World_Dir_B968(
        float3 Dir_Object,
        out float3 Dir_World    )
    {
        Dir_World=(mul((float3x3)UNITY_MATRIX_M, Dir_Object));
    }
    //BLOCK_END Object_To_World_Dir


    VertexOutput vert(VertexInput vertInput)
    {
        UNITY_SETUP_INSTANCE_ID(vertInput);
        VertexOutput o;
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

        float3 Pos_World_Q964;
        Object_To_World_Pos_B964(float3(0,0,0),Pos_World_Q964);

        float3 Nrm_World_Q977;
        Object_To_World_Dir_B977(vertInput.normal,Nrm_World_Q977);

        float3 Dir_World_Q968;
        Object_To_World_Dir_B968(vertInput.tangent,Dir_World_Q968);

        float3 Dir_World_Q969;
        Object_To_World_Dir_B968((normalize(cross(vertInput.normal,vertInput.tangent))),Dir_World_Q969);

        // Conditional (#978)
        float3 Result_Q978;
        Result_Q978 = _Use_Global_Left_Index_ ? Global_Left_Index_Tip_Position.xyz : _Blob_Position_;
        
        // Conditional (#980)
        float3 Result_Q980;
        Result_Q980 = _Use_Global_Right_Index_ ? Global_Right_Index_Tip_Position.xyz : _Blob_Position_2_;
        
        // Length3 (#974)
        float Length_Q974 = length(Dir_World_Q968);

        // Length3 (#973)
        float Length_Q973 = length(Dir_World_Q969);

        // Normalize3 (#963)
        float3 Normalized_Q963 = normalize(Nrm_World_Q977);

        // Normalize3 (#961)
        float3 Normalized_Q961 = normalize(Dir_World_Q968);

        // Normalize3 (#962)
        float3 Normalized_Q962 = normalize(Dir_World_Q969);

        float3 Position_Q986;
        float Near_Size_Q986;
        float Inner_Fade_Q986;
        float Fade_Q986;
        float Pulse_Q986;
        Choose_Blob_B986(vertInput.color,Result_Q978,Result_Q980,_Blob_Near_Size_,_Blob_Near_Size_2_,_Blob_Inner_Fade_,_Blob_Inner_Fade_2_,_Blob_Pulse_,_Blob_Pulse_2_,_Blob_Fade_,_Blob_Fade_2_,Position_Q986,Near_Size_Q986,Inner_Fade_Q986,Fade_Q986,Pulse_Q986);

        // From_XY (#975)
        float2 Vec2_Q975 = float2(Length_Q974,Length_Q973);

        float3 Out_Position_Q985;
        float2 Out_UV_Q985;
        float3 Blob_Info_Q985;
        float4 Clip_Info_Q985;
        Blob_Vertex_B985(Pos_World_Q964,Normalized_Q963,Normalized_Q961,Normalized_Q962,_Blob_Intensity_,_Blob_Far_Size_,_Blob_Near_Distance_,_Blob_Far_Distance_,_Blob_Fade_Length_,vertInput.color,vertInput.uv0,Position_Q986,Near_Size_Q986,Inner_Fade_Q986,1.0,Fade_Q986,Pulse_Q986,Vec2_Q975,_Radius_,Out_Position_Q985,Out_UV_Q985,Blob_Info_Q985,Clip_Info_Q985);

        float3 Position = Out_Position_Q985;
        float2 UV = Out_UV_Q985;
        float3 Normal = Blob_Info_Q985;
        float3 Tangent = float3(0,0,0);
        float3 Binormal = float3(0,0,0);
        float4 Color = float4(1,1,1,1);
        float4 Extra1 = Clip_Info_Q985;

        o.pos = mul(UNITY_MATRIX_VP, float4(Position,1));
        o.normalWorld.xyz = Normal; o.normalWorld.w=1.0;
        o.uv = UV;
        o.extra1=Extra1;

        return o;
    }

    //BLOCK_BEGIN Blob_Fragment 982

    void Blob_Fragment_B982(
        sampler2D Blob_Texture,
        float3 Blob_Info,
        float2 UV,
        float Clip,
        out float4 Blob_Color    )
    {
        float k = dot(UV,UV);
        Blob_Color = (Clip * Blob_Info.y) * tex2D(Blob_Texture,float2(float2(sqrt(k),Blob_Info.x).x,1.0-float2(sqrt(k),Blob_Info.x).y))*(1.0-saturate(k));
    }
    //BLOCK_END Blob_Fragment

    //BLOCK_BEGIN ClipToRoundedRect 984

    void ClipToRoundedRect_B984(
        float4 ClipInfo,
        half Radius,
        out half Result    )
    {
        half d = length(max(abs(ClipInfo.xy)-ClipInfo.zw,0));
        half fw = max(fwidth(d),0.0001);
        Result = 1.0-saturate((d-Radius)/fw);
    }
    //BLOCK_END ClipToRoundedRect


    half4 frag(VertexOutput fragInput) : SV_Target
    {
        half4 result;

        half Result_Q984;
        #if defined(_ROUNDED_)
          ClipToRoundedRect_B984(fragInput.extra1,_Radius_,Result_Q984);
        #else
          Result_Q984 = 1.0;
        #endif

        float4 Blob_Color_Q982;
        Blob_Fragment_B982(_Blob_Texture_,fragInput.normalWorld.xyz,fragInput.uv,Result_Q984,Blob_Color_Q982);

        float4 Out_Color = Blob_Color_Q982;
        float Clip_Threshold = 0;
        bool To_sRGB = false;

        result = Out_Color;
        return result;
    }

    ENDCG
  }
 }
}
