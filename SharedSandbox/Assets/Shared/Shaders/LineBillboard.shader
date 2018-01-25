
/*
Unity CG shader reference
https://docs.unity3d.com/455/Documentation/Manual/SL-Reference.html

Unity CG struct semantics
https://docs.unity3d.com/Manual/SL-VertexProgramInputs.html

CG functions reference
http://developer.download.nvidia.com/CgTutorial/cg_tutorial_appendix_e.html

Geometry shader tutorial
https://takinginitiative.wordpress.com/2011/01/12/directx10-tutorial-9-the-geometry-shader/
*/

Shader "RC3/LineBillboard"
{
	Properties
	{
		_texture("Texture", 2D) = ""{}
		_u0("U0", Range(0.0, 1.0)) = 0.0
		_u1("U1", Range(0.0, 1.0)) = 1.0
        _width("Width", Range(0.0, 1.0)) = 1.0
	}

	SubShader
	{
		Pass
		{
			Tags{ "RenderType" = "Opaque" "Queue" = "Transparent" "IgnoreProjector" = "True" }
			Blend SrcAlpha OneMinusSrcAlpha // typical transparency
			LOD 100

			CGPROGRAM

			#pragma target 5.0
			#pragma vertex Vert
			#pragma geometry Geom
			#pragma fragment Frag

			#include "UnityCG.cginc"

			/*
			Variables
			*/

			sampler2D _texture;
			float _u0;
			float _u1;
			float _width;

			/*
			Data types
			*/

			struct VertData
			{
				float4 pos : POSITION;
				float2 uv0 : TEXCOORD0;
			};

			struct GeomData
			{
				float4 pos : POSITION;
				float2 uv0 : TEXCOORD0;
			};

			struct FragData
			{
				float4 pos : POSITION;
				float2 uv0 : TEXCOORD0;
			};

			/*
			Helper functions
			*/

			float ramp(float t, float t0, float t1)
			{
				return saturate((t - t0) / (t1 - t0));
			}

			/*
			Main
			*/

			// Vertex shader
			GeomData Vert(VertData v)
			{
				GeomData g;
				g.pos = mul(unity_ObjectToWorld, v.pos);
				g.uv0 = ramp(v.uv0.x, _u0, _u1);
				return g;
			}


			// Geometry shader
			[maxvertexcount(4)]
			void Geom(line GeomData g[2], inout TriangleStream<FragData> stream)
			{
				float3 cam = _WorldSpaceCameraPos;

				float3 p0 = g[0].pos;
				float3 p1 = g[1].pos;

				float3 x = p1 - p0;
				float3 y0 = normalize(cross(x, cam - p0)) * _width;
				float3 y1 = normalize(cross(x, cam - p1)) * _width;

				// append to stream
				FragData f;
				float4x4 vp = UNITY_MATRIX_VP;

				f.uv0 = g[0].uv0;

				f.pos = mul(vp, float4(p0 - y0, 1.0));
				stream.Append(f);

				f.pos = mul(vp, float4(p0 + y0, 1.0));
				stream.Append(f);

				f.uv0 = g[1].uv0;

				f.pos = mul(vp, float4(p1 - y1, 1.0));
				stream.Append(f);

				f.pos = mul(vp, float4(p1 + y1, 1.0));
				stream.Append(f);
			}


			// Fragment shader
			float4 Frag(FragData f) : COLOR
			{
				return tex2D(_texture, f.uv0);
			}

			ENDCG
		}
	}
}
