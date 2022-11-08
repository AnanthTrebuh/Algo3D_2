
precision mediump float;

// classic obj
varying vec4 pos3D;
varying vec3 N;

varying vec4 rMat;

uniform bool uIsMirror;
uniform vec3 uCameraPos;
uniform samplerCube uSkybox;

// ==============================================
void main(void)
{
	if(uIsMirror){
		vec3 I = normalize(pos3D).xyz;
		vec3 r = reflect(I, normalize(N));
		r = vec3(rMat * vec4(r, 1.0));
		gl_FragColor = vec4(textureCube(uSkybox, vec3(r.x, r.z, r.y)).rgb, 1.0);
	}
	else
	{
		vec3 col = vec3(0.0,0.6,0.4) * dot(N,normalize(vec3(-pos3D))); // Lambert rendering, eye light source
		gl_FragColor = vec4(col,1.0);
	}
}




