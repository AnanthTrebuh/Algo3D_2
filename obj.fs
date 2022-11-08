
precision mediump float;

// classic obj
varying vec4 pos3D;
varying vec3 N;

varying mat4 rMat;

uniform bool uIsMirror;
uniform bool uIsRefrac;
uniform samplerCube uSkybox;

// ==============================================
void main(void)
{
	if(uIsMirror){
		vec3 I = normalize(pos3D).xyz;
		vec3 R = reflect(I, normalize(N));
		R = vec3(rMat * vec4(R, 1.0));
		gl_FragColor = vec4(textureCube(uSkybox, vec3(R.x, R.z, R.y)).rgb, 1.0);
	}
	else if (uIsRefrac)
	{
		float ratio = 1.00 / 1.00;
		vec3 I = normalize(pos3D).xyz;
		vec3 R = refract(I, normalize(N), ratio);
		R = vec3(rMat * vec4(R, 1.0));
		gl_FragColor = vec4(textureCube(uSkybox, vec3(R.x, R.z, R.y)).rgb, 1.0);
	}
	else 
	{
		vec3 col = vec3(0.0,0.6,0.4) * dot(N,normalize(vec3(-pos3D))); // Lambert rendering, eye light source
		gl_FragColor = vec4(col,1.0);
	}
}




