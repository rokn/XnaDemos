sampler s0;
float param;

float4 PixelShaderFunction(float2 coords: TEXCOORD0) : COLOR0
{
	float4 color = tex2D(s0, coords);
	if (coords.y > param)
	{
		color.gb = color.r;
	}
	return color;
}

float4 FrozenShader(float2 coords: TEXCOORD0) : COLOR0
{
	float4 color = tex2D(s0, coords);
	color.b += 3;
	return color;
}

technique Technique1
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
	pass Pass2
	{
		PixelShader = compile ps_2_0 FrozenShader();
	}
}