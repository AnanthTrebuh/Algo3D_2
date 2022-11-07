
precision mediump float;

// classic obj
varying vec4 pos3D;
varying vec3 N;

// mirror obk
varying vec3 vNormal;
varying vec3 vPos;

uniform bool uIsMirror;
uniform vec3 uCameraPos;
uniform samplerCube uSkybox;

// ==============================================
void main(void)
{

	if(uIsMirror){
		vec3 I = normalize(VPos - uCameraPos);
		vec3 R = reflect(I, normalize(VNormal));
		gl_FragColor = vec4(texture(skybox, R).rgb, 1.0);
	}
	else
	{
		vec3 col = vec3(0.8,0.4,0.4) * dot(N,normalize(vec3(-pos3D))); // Lambert rendering, eye light source
		gl_FragColor = vec4(col,1.0);
	}
}




