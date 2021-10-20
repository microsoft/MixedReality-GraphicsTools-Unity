
Shader "CanvasButtonBgGlow" {

Properties {

    [Header(Rounded Rectangle)]
        _Bevel_Radius_("Bevel Radius", Range(0,1)) = .05
        _Line_Width_("Line Width", Range(0,1)) = 0.03
     
    [Header(Animation)]
        _Motion_("Motion", Range(0,1)) = 0.0
        _Max_Intensity_("Max Intensity", Range(0,1)) = 0.5
        _Intensity_Fade_In_Exponent_("Intensity Fade In Exponent", Range(0,5)) = 2
        _Outer_Fuzz_Start_("Outer Fuzz Start", Range(0,1)) = 0.002
        _Outer_Fuzz_End_("Outer Fuzz End", Range(0,1)) = 0.001
     
    [Header(Color)]
        _Color_("Color", Color) = (1,1,1,1)
        _Inner_Color_("Inner Color", Color) = (1,1,1,1)
        _Blend_Exponent_("Blend Exponent", Range(0,9)) = 1
     
    [Header(Inner Transition)]
        _Falloff_("Falloff", Range(0,5)) = 1.0
        _Bias_("Bias", Range(0,1)) = 0.5
     


}

SubShader {
    Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
    Blend One One
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

    #include "UnityCG.cginc"

CBUFFER_START(UnityPerMaterial)
    half _Bevel_Radius_;
    half _Line_Width_;
    half _Motion_;
    half _Max_Intensity_;
    half _Intensity_Fade_In_Exponent_;
    half _Outer_Fuzz_Start_;
    half _Outer_Fuzz_End_;
    half4 _Color_;
    half4 _Inner_Color_;
    half _Blend_Exponent_;
    half _Falloff_;
    half _Bias_;
CBUFFER_END


    struct VertexInput {
        float4 vertex : POSITION;
        float2 uv0 : TEXCOORD0;
        float2 uv2 : TEXCOORD2;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct VertexOutput {
        float4 pos : SV_POSITION;
        half4 normalWorld : TEXCOORD5;
        float2 uv : TEXCOORD0;
      UNITY_VERTEX_OUTPUT_STEREO
    };


    //BLOCK_BEGIN Object_To_World_Pos 403

    void Object_To_World_Pos_B403(
        float3 Pos_Object,
        out float3 Pos_World    )
    {
        Pos_World=(mul(unity_ObjectToWorld, float4(Pos_Object, 1)));
        
    }
    //BLOCK_END Object_To_World_Pos

    //BLOCK_BEGIN ScaleUVs 408

    void ScaleUVs_B408(
        float2 UV,
        float SizeX,
        float SizeY,
        out float2 XY    )
    {
        XY = (UV - float2(0.5,0.5))*float2(SizeX/SizeY,1.0);
        
    }
    //BLOCK_END ScaleUVs

    //BLOCK_BEGIN Conditional_Vec3 411

    void Conditional_Vec3_B411(
        bool Which,
        float3 If_False,
        float3 If_True,
        out float3 Result    )
    {
        Result = Which ? If_True : If_False;
        
    }
    //BLOCK_END Conditional_Vec3


    VertexOutput vert(VertexInput vertInput)
    {
        UNITY_SETUP_INSTANCE_ID(vertInput);
        VertexOutput o;
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

        // To_XY (#433)
        float X_Q433;
        float Y_Q433;
        X_Q433 = vertInput.uv2.x;
        Y_Q433 = vertInput.uv2.y;

        // Max (#420)
        half MaxAB_Q420=max(0,_Motion_);

        // From_XYZ (#414)
        float3 Vec3_Q414 = float3(X_Q433,Y_Q433,0);

        float2 XY_Q408;
        ScaleUVs_B408(vertInput.uv0,X_Q433,Y_Q433,XY_Q408);

        // Greater_Than (#419)
        bool Greater_Than_Q419;
        Greater_Than_Q419 = MaxAB_Q420 > 0;
        
        float3 Result_Q411;
        Conditional_Vec3_B411(Greater_Than_Q419,float3(0,0,0),vertInput.vertex.xyz,Result_Q411);

        float3 Pos_World_Q403;
        Object_To_World_Pos_B403(Result_Q411,Pos_World_Q403);

        float3 Position = Pos_World_Q403;
        float3 Normal = Vec3_Q414;
        float2 UV = XY_Q408;
        float3 Tangent = float3(0,0,0);
        float3 Binormal = float3(0,0,0);
        float4 Color = float4(1,1,1,1);

        o.pos = mul(UNITY_MATRIX_VP, float4(Position,1));
        o.normalWorld.xyz = Normal; o.normalWorld.w=1.0;
        o.uv = UV;

        return o;
    }

    //BLOCK_BEGIN Power 428

    void Power_B428(
        half Base,
        half Exponent,
        out half Power    )
    {
        Power = pow(Base, Exponent);
        
    }
    //BLOCK_END Power

    //BLOCK_BEGIN Ease_Transition 423

    float Bias_Bid423(float b, float v) {
      return pow(v,log(clamp(b,0.001,0.999))/log(0.5));
    }
    
    void Ease_Transition_B423(
        half Falloff,
        half Bias,
        half F,
        out half Result    )
    {
        Result = pow(Bias_Bid423(Bias,F),Falloff);
        
    }
    //BLOCK_END Ease_Transition

    //BLOCK_BEGIN Round_Rect_Fragment 448

    void Round_Rect_Fragment_B448(
        half Radius,
        half Line_Width,
        half Anisotropy,
        float2 UV,
        half Outer_Fuzz,
        half Max_Outer_Fuzz,
        out half Rect_Distance,
        out half Inner_Distance    )
    {
        half2 center = half2(Anisotropy*0.5-Radius,0.5-Radius);
        
        half d = length(max(abs(UV)-center,0.0));
        //half dx = max(fwidth(d)*Filter_Width,0.00001);
        
        //Inside_Rect = saturate((Radius-d)/dx);
        //Inside_Line = Inside_Rect *(1.0-saturate((Radius-Line_Width-d)/dx));
        
        half radius = Radius - Max_Outer_Fuzz;
        Inner_Distance = saturate(1.0-(radius-d)/Line_Width);
        Rect_Distance = saturate(1.0-(d-radius)/Outer_Fuzz)*Inner_Distance;
        
    }
    //BLOCK_END Round_Rect_Fragment

    //BLOCK_BEGIN To_XYZ 418

    void To_XYZ_B418(
        half3 Vec3,
        out half X,
        out half Y,
        out half Z    )
    {
        X=Vec3.x;
        Y=Vec3.y;
        Z=Vec3.z;
        
    }
    //BLOCK_END To_XYZ


    half4 frag(VertexOutput fragInput) : SV_Target
    {
        half4 result;

        // Max (#420)
        half MaxAB_Q420=max(0,_Motion_);

        half X_Q418;
        half Y_Q418;
        half Z_Q418;
        To_XYZ_B418(fragInput.normalWorld.xyz,X_Q418,Y_Q418,Z_Q418);

        // Sqrt (#430)
        half Sqrt_F_Q430 = sqrt(MaxAB_Q420);

        half Power_Q417;
        Power_B428(MaxAB_Q420,_Intensity_Fade_In_Exponent_,Power_Q417);

        // Divide (#443)
        half Quotient_Q443 = X_Q418 / Y_Q418;

        // Lerp (#426)
        half Value_At_T_Q426=lerp(_Outer_Fuzz_Start_,_Outer_Fuzz_End_,Sqrt_F_Q430);

        // Multiply (#416)
        half Product_Q416 = _Max_Intensity_ * Power_Q417;

        half Rect_Distance_Q448;
        half Inner_Distance_Q448;
        Round_Rect_Fragment_B448(_Bevel_Radius_,_Line_Width_,Quotient_Q443,fragInput.uv,Value_At_T_Q426,_Outer_Fuzz_Start_,Rect_Distance_Q448,Inner_Distance_Q448);

        half Power_Q428;
        Power_B428(Inner_Distance_Q448,_Blend_Exponent_,Power_Q428);

        half Result_Q423;
        Ease_Transition_B423(_Falloff_,_Bias_,Rect_Distance_Q448,Result_Q423);

        // Mix_Colors (#424)
        half4 Color_At_T_Q424 = lerp(_Inner_Color_, _Color_,float4( Power_Q428, Power_Q428, Power_Q428, Power_Q428));

        // Multiply (#415)
        half Product_Q415 = Result_Q423 * Product_Q416;

        // Scale_Color (#431)
        half4 Result_Q431 = Product_Q415 * Color_At_T_Q424;

        float4 Out_Color = Result_Q431;
        float Clip_Threshold = 0;

        result = Out_Color;
        return result;
    }

    ENDCG
  }
 }
}
