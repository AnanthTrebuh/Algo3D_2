
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

	//i reflect
	//m normal
	//n indice
	// R 
	// T = 1 - R

	float ratio = 1.0/ uRefracValue;
	vec3 I = normalize(pos3D.xyz);
	vec3 Re = reflect(I, normalize(N));
	Re = (rMat * vec4(Re, 1.0)).xyz;
	vec4 textRe = vec4(textureCube(uSkybox, normalize(Re.xzy)).rgb, 1.0);
	vec3 Ra = refract(I, normalize(N), ratio);
	Ra = (rMat * vec4(Ra, 1.0)).xyz;
	vec4 textRa = vec4(textureCube(uSkybox, normalize(Ra.xzy)).rgb, 1.0);
	// vec3 R = Re + Ra;

	float c = abs(dot(Ra, normalize(N)));
	float g = sqrt((uRefracValue*uRefracValue) + ( c*c) - 1.0);
    float R = 0.5 * (((g-c)*(g-c))/((g+c)*(g+c))) * ( 1.0 + ((c * (g+c)-1.0) * (c * (g+c)-1.0)) / ((c * (g-c)+1.0) * (c * (g-c)+1.0) ));
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
	
	

	// if(uIsMirror){
	// 	vec3 I = normalize(pos3D).xyz;
	// 	vec3 R = reflect(I, normalize(N));
	// 	R = vec3(rMat * vec4(R, 1.0));
	// 	gl_FragColor = vec4(textureCube(uSkybox, vec3(R.x, R.z, R.y)).rgb, 1.0);
	// }
	// else if (uIsRefrac)
	// {
	// 	float ratio = 0.7;
	// 	vec3 I = normalize(pos3D).xyz;
	// 	vec3 R = refract(I, normalize(N), ratio);
	// 	R = vec3(rMat * vec4(R, 1.0));
	// 	gl_FragColor = vec4(textureCube(uSkybox, vec3(R.x, R.z, R.y)).rgb, 1.0);
	// }
	// else 
	// {
	// 	vec3 col = vec3(0.0,0.6,0.4) * dot(N,normalize(vec3(-pos3D))); // Lambert rendering, eye light source
	// 	gl_FragColor = vec4(col,1.0);
	// }
}




