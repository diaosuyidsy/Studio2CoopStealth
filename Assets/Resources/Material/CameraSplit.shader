Shader "Custom/CameraSplit"
{
	SubShader
	{
		// Apply this shader before drawing geometry
		Tags { "Queue" = "Geometry-500" }

		ColorMask 0
		ZWrite On
		Pass
		{
		// Intentionally blank
	 }
	}
}
