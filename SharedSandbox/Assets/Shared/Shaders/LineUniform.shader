/*
Unity shader reference
https://docs.unity3d.com/455/Documentation/Manual/SL-Reference.html

CG functions reference
http://developer.download.nvidia.com/CgTutorial/cg_tutorial_appendix_e.html
*/

Shader "RC3/LineUniform"
{
	Properties
	{
		_Color0("Color0", Color) = (1.0, 1.0, 1.0, 1.0)
		_Color1("Color1", Color) = (0.0, 0.0, 0.0, 1.0)
		_Width("Width", Range(0.0, 10.0)) = 3.0
		_Range("Range", Range(0.0, 100.0)) = 10.0
	}

	SubShader
	{
		Pass
		{
			Tags{ "RenderType" = "Opaque" }
			LOD 100

			CGPROGRAM

			#pragma target 5.0
			#pragma vertex VS_Main
			#pragma geometry GS_Main
			#pragma fragment FS_Main
			#include "UnityCG.cginc"

			/*
			Data structures
			*/

			struct VS_Input
			{
				float4 pos : POSITION;
				// float2 tex0 : TEXCOORD0;
			};

			struct GS_Input
			{
				float4 pos : POSITION;
				// float2 tex0 : TEXCOORD0;
			};

			struct FS_Input
			{
				float4 pos : POSITION;
				float4 col : COLOR;
				// float2 tex0 : TEXCOORD0;
			};

			/*
			Variables
			*/

			float4 _Color0;
			float4 _Color1;
			float _Width;
			float _Range;

			static float _Aspect = _ScreenParams.x / _ScreenParams.y;
			static float _AspectInv = 1.0 / _Aspect;

			/*
			Shader programs
			*/

			// Vertex shader
			GS_Input VS_Main(VS_Input v)
			{
				GS_Input g;
				g.pos = mul(unity_ObjectToWorld, v.pos);
				// g.tex0 = v.tex0;

				return g;
			}


			// Geometry shader
			[maxvertexcount(4)]
			void GS_Main(line GS_Input p[2], inout TriangleStream<FS_Input> triStream)
			{
				float4x4 vp = UNITY_MATRIX_VP;

				// project to screen space
				// transformed point is in homogeneous coords (xw, yw, zw, w)
				float4 p0 = mul(vp, p[0].pos);
				float4 p1 = mul(vp, p[1].pos);

				// remove perspective distortion
				p0 /= p0.w;
				p1 /= p1.w;

				// create offset vectors
				float2 pn0 = p0.xy;
				float2 pn1 = p1.xy;

				// correct for aspect ratio
				pn0.x *= _Aspect;
				pn1.x *= _Aspect;
		
				float2 dx = normalize(pn1 - pn0) * (_Width / _ScreenParams.y); // constant line width
				float2 dy = float2(-dx.y, dx.x); // perp ccw

				// inverse correct aspect
				dx.x *= _AspectInv;
				dy.x *= _AspectInv;	

				// lerp factors
				float t0 = smoothstep(0.0, _Range, length(p[0].pos));
				float t1 = smoothstep(0.0, _Range, length(p[1].pos));

				// append to stream
				FS_Input f;

				f.col = lerp(_Color0, _Color1, t0);

				f.pos = p0 + float4(-dx + dy, 0.0, 0.0);
				triStream.Append(f);

				f.pos = p0 - float4(dx + dy, 0.0, 0.0);
				triStream.Append(f);

				f.col = lerp(_Color0, _Color1, t1);

				f.pos = p1 + float4(dx + dy, 0.0, 0.0);
				triStream.Append(f);

				f.pos = p1 - float4(-dx + dy, 0.0, 0.0);
				triStream.Append(f);
			}


			// Fragment shader
			float4 FS_Main(FS_Input f) : COLOR
			{
				return f.col;
			}

		ENDCG
	}
	}
}