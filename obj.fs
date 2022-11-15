
precision mediump float;

// classic obj
varying vec4 pos3D;
varying vec3 N;

varying mat4 rMat;

uniform bool uIsMirror;
uniform bool uIsRefrac;
uniform samplerCube uSkybox;
uniform float uRefracValue;
// ==============================================

vec4 refraction(vec3 I, vec3 N, float ratio){
	vec3 Refraction = refract(I, N, ratio);
	Refraction = (rMat * vec4(Refraction, 1.0)).xyz;
	return textureCube(uSkybox, Refraction.xzy);
}

vec4 reflection(vec3 I, vec3 N){
	vec3 Reflection = reflect(I, N);
	Reflection = (rMat * vec4(Reflection, 1.0)).xyz;
	return textureCube(uSkybox, Reflection.xzy);
}

void main(void)
{
	float ratio = 1.0/ uRefracValue;
	vec3 I = normalize(pos3D.xyz);
	vec3 Normal = normalize(N);

	vec4 textRefrac = uIsRefrac ? refraction(I, N, ratio) : vec4(0,0,0,0);
	vec4 textMirror = uIsMirror ? reflection(I, N) : vec4(0,0,0,0);

	if(uIsMirror && uIsRefrac){
		float c = abs(dot(I, normalize(N)));
		float g = sqrt((uRefracValue*uRefracValue) + ( c*c) - 1.0);
		float R = 0.5 * 
				(((g-c)*(g-c))/((g+c)*(g+c))) * 
				( 1.0 + (((c * (g+c)-1.0) * (c * (g+c)-1.0)) / ((c * (g-c)+1.0) * (c * (g-c)+1.0) )));
		float T = 1.0 - R;

		gl_FragColor = ((textRefrac * R) + (textMirror * T));    
	}
	else if (uIsMirror){
		gl_FragColor = textMirror;
	}
	else if (uIsRefrac){
		gl_FragColor = textRefrac;
	}
	else {
		vec3 col = vec3(0.0,0.6,0.4) * dot(N,normalize(vec3(-pos3D))); // Lambert rendering, eye light source
		gl_FragColor = vec4(col,1.0);
	}
}




