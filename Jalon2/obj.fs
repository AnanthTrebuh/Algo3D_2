
precision mediump float;

// classic obj
varying vec4 pos3D;
varying vec3 N;

varying mat4 rMat;

uniform bool uIsMirror;
uniform bool uIsRefrac;
uniform samplerCube uSkybox;
uniform float uRefracValue;
uniform bool uIsCookTor;
// ==============================================

vec4 refraction(vec3 I, vec3 N, float ratio){
	vec3 Refraction = refract(I, normalize(N), ratio);
	Refraction = (rMat * vec4(Refraction, 1.0)).xyz;
	return textureCube(uSkybox, Refraction.xzy);
}

vec4 reflection(vec3 I, vec3 N){
	vec3 Reflection = reflect(I, normalize(N));
	Reflection = (rMat * vec4(Reflection, 1.0)).xyz;
	return textureCube(uSkybox, Reflection.xzy);
}

//function to calculate the cook torrance lighting model
vec3 cookTorrance(vec3 I, vec3 N, vec3 V, vec3 L, vec3 Kd, vec3 Ks, float roughness, float metallic, float F0, float F90, float NdotL, float NdotV, float VdotH, float NdotH, float LdotH){
	//calculate the fresnel term
	float F = F0 + (F90 - F0) * (1.0 - VdotH, 5.0)*(1.0 - VdotH, 5.0);
	//calculate the geometric term
	float G = min(1.0, min((2.0 * NdotH * NdotV) / VdotH, (2.0 * NdotH * NdotL) / VdotH));
	//calculate the roughness term
	float D = (roughness * roughness) / (3.14159265359 * (((NdotH * NdotH * (roughness * roughness - 1.0) + 1.0), 2.0)*((NdotH * NdotH * (roughness * roughness - 1.0) + 1.0), 2.0)));
	//calculate the diffuse term
	vec3 diffuse = (1.0 - F) * (1.0 - metallic) * Kd / 3.14159265359;
	//calculate the specular term
	vec3 specular = (F * G * D / (4.0 * NdotL * NdotV) )+ vec3(0,0,0);
	//calculate the final color
	vec3 color = (diffuse + specular) * L * NdotL;
	return color;
}

// else if(uIsCookTor){
	// 	//calculate the view vector
	// 	vec3 V = normalize(-I);
	// 	//calculate the light vector
	// 	vec3 L = normalize(vec3(0.0, 0.0, 1.0));
	// 	//calculate the half vector
	// 	vec3 H = normalize(V + L);
	// 	//calculate the dot products
	// 	float NdotL = max(dot(N, L), 0.0);
	// 	float NdotV = max(dot(N, V), 0.0);
	// 	float VdotH = max(dot(V, H), 0.0);
	// 	float NdotH = max(dot(N, H), 0.0);
	// 	float LdotH = max(dot(L, H), 0.0);
	// 	//calculate the diffuse color
	// 	vec3 Kd = vec3(0.5, 0.5, 0.5);
	// 	//calculate the specular color
	// 	vec3 Ks = vec3(0.5, 0.5, 0.5);
	// 	//calculate the roughness
	// 	float roughness = 0.5;
	// 	//calculate the metallic
	// 	float metallic = 0.5;
	// 	//calculate the fresnel term
	// 	float F0 = 0.04;
	// 	float F90 = 1.0;
	// 	//calculate the color
	// 	vec3 color = cookTorrance(I, N, V, L, Kd, Ks, roughness, metallic, F0, F90, NdotL, NdotV, VdotH, NdotH, LdotH);
	// 	gl_FragColor = vec4(color, 1.0);
	// }

//function to calculate the cook torrance lighting model with beckmann distribution
vec3 cookTorranceBeckmann(vec3 I, vec3 N, vec3 V, vec3 L, vec3 Kd, vec3 Ks, float roughness, float metallic, float F0, float F90, float NdotL, float NdotV, float VdotH, float NdotH, float LdotH){
	//calculate the fresnel term
	float F = F0 + (F90 - F0) * pow(1.0 - VdotH, 5.0);
	//calculate the geometric term
	float G = min(1.0, min((2.0 * NdotH * NdotV) / VdotH, (2.0 * NdotH * NdotL) / VdotH));
	//calculate the roughness term
	float D = exp((NdotH * NdotH - 1.0) / (roughness * roughness * NdotH * NdotH)) / (3.14159265359 * roughness * roughness * NdotH * NdotH * NdotH * NdotH);
	//calculate the diffuse term
	vec3 diffuse = (1.0 - F) * (1.0 - metallic) * Kd / 3.14159265359;
	//calculate the specular term
	vec3 specular = F * G * D / (4.0 * NdotL * NdotV) + vec3(0,0,0) ;
	//calculate the final color
	vec3 color = (diffuse + specular) * L * NdotL;
	return color;
}

float fresnel(vec3 I, vec3 N, float uRefracValue){
	float c = abs(dot(I, normalize(N)));
	float g = sqrt((uRefracValue*uRefracValue) + ( c*c) - 1.0);
	float R = 0.5 * 
			(((g-c)*(g-c))/((g+c)*(g+c))) * 
			( 1.0 + (((c * (g+c)-1.0) * (c * (g+c)-1.0)) / ((c * (g-c)+1.0) * (c * (g-c)+1.0) )));
	return R;
}

void main(void)
{
	float ratio = 1.0/ uRefracValue;
	vec3 I = normalize(pos3D.xyz);
	vec3 O = normalize(-I);
	vec3 M = (I + O) / abs(I + O);

	vec4 textRefrac = uIsRefrac ? refraction(I, N, ratio) : vec4(0,0,0,0);
	vec4 textMirror = uIsMirror ? reflection(I, N) : vec4(0,0,0,0);

	if(uIsMirror && uIsRefrac){
		float R = fresnel(I, N, uRefracValue);
		float T = 1.0 - R;

		gl_FragColor = (textRefrac * R) + (textMirror * T);    
	}
	else if (uIsMirror){
		gl_FragColor = textMirror;
	}
	else if (uIsRefrac){
		gl_FragColor = textRefrac;
	}
	else if(uIsCookTor){
		
		float sigma = 0.2;
		float cosT = dot(normalize(N),normalize(M));
		float theta = acos(cosT);
		float tanT = tan(theta);
		float pi = 3.14159265359;

		float Fr = fresnel(I, M, uRefracValue);
		float D = exp(-(tanT * tanT) / ( 2.0 * sigma  * sigma)) / (pi * sigma * sigma * cosT * cosT * cosT * cosT);
		float G = min(1.0, min((2.0*dot(N,M)*dot(N,O))/dot(O,M),(2.0*dot(N,M)*dot(N,I))/dot(I,M)));

		float Fs = (Fr*D*G)/4.0*abs(dot(I,N))*abs(dot(O,N));

		vec3 tamere = Fs*cosT *vec3(1.0,1.0,1.0);

		vec4 L = textureCube(uSkybox,tamere);


		vec3 col = vec3(0.0,0.6,0.4) * dot(N,normalize(vec3(-pos3D)));
		gl_FragColor = L;
	}
	else {
		vec3 col = vec3(0.0,0.6,0.4) * dot(N,normalize(vec3(-pos3D))); // Lambert rendering, eye light source
		gl_FragColor = vec4(col,1.0);
	}
}

/*=================
//slide 31 
//CT82  F(i,m)D(m)G(i,o,m)
        __________________
		4|i.n|o.n|
// D beckman
*/
