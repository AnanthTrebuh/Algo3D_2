
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
void main(void)
{
	float ratio = 1.0/ uRefracValue;
	vec3 I = normalize(pos3D.xyz);
	vec3 Reflection = reflect(I, normalize(N));
	Reflection = (rMat * vec4(Reflection, 1.0)).xyz;
	vec4 textRe = vec4(0,0,0,0);
	textRe = textureCube(uSkybox, Reflection.xzy);
	vec3 Refraction = refract(I, normalize(N), ratio);
	Refraction = (rMat * vec4(Refraction, 1.0)).xyz;
	vec4 textRa = vec4(0,0,0,0);
	textRa = textureCube(uSkybox, Refraction.xzy);

	float c = abs(dot(I, normalize(N)));
	float g = sqrt((uRefracValue*uRefracValue) + ( c*c) - 1.0);
    float R = 0.5 * 
			(((g-c)*(g-c))/((g+c)*(g+c))) * 
			( 1.0 + (((c * (g+c)-1.0) * (c * (g+c)-1.0)) / ((c * (g-c)+1.0) * (c * (g-c)+1.0) )));
	float T = 1.0 - R;

	if(uIsMirror && uIsRefrac){
		textRa = textRa * R;
		textRe = textRe * T;
		gl_FragColor = textRa  + textRe;    
	}
	else if (uIsMirror){
		gl_FragColor = textRe;
	}
	else if (uIsRefrac){
		gl_FragColor = textRa;
	}
	else {
		vec3 col = vec3(0.0,0.6,0.4) * dot(N,normalize(vec3(-pos3D))); // Lambert rendering, eye light source
		gl_FragColor = vec4(col,1.0);
	}
}




