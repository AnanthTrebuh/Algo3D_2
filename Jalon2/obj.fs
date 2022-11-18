
precision mediump float;

// classic obj
varying vec4 pos3D;
varying vec3 N;

varying mat4 rMat;

uniform bool uIsMirror;
uniform bool uIsRefrac;
uniform samplerCube uSkybox;
uniform float uRefracValue;
uniform float uSigmaValue;
uniform bool uIsCookTor;
uniform vec3 uLightSource;

// ==============================================

vec4 refraction(vec3 I, vec3 N, float ratio){
	vec3 Refraction = refract(I, normalize(N), ratio);
	Refraction = vec3(rMat * vec4(Refraction, 1.0));
	return textureCube(uSkybox, Refraction.xzy);
}

vec4 reflection(vec3 I, vec3 N){
	vec3 Reflection = reflect(I, normalize(N));
	Reflection = vec3(rMat * vec4(Reflection, 1.0));
	return textureCube(uSkybox, Reflection.xzy);
}

// //function t

float fresnel(vec3 I, vec3 N, float uRefracValue){
	float c = abs(dot(I, normalize(N)));
	float g = sqrt((uRefracValue*uRefracValue) + ( c*c) - 1.0);
	float R = 0.5 * 
			(((g-c)*(g-c))/((g+c)*(g+c))) * 
			( 1.0 + (((c * (g+c)-1.0) * (c * (g+c)-1.0)) / ((c * (g-c)+1.0) * (c * (g-c)+1.0) )));
	return R;
}

float beckmann(float cosT, float sigma, float pi){
	float sigma2 = sigma * sigma;
	float costT2 = cosT * cosT;
	float costT4 = costT2 * costT2;
	float sinT2 = 1.0 - costT2;
	float tanT2 = sinT2 / costT2;

	float D1 = 1.0 / (pi * sigma2 * costT4);
	float D2 = exp(-tanT2 / (2.0 * sigma2));
	float D = D1 * D2;
	return D;
}

float masking(float NdotM,float NdotI, float NdotO, float OdotM, float IdotM){
	float G1 = 2.0 * NdotM * NdotI / OdotM;
	float G2 = 2.0 * NdotM * NdotO / OdotM;
	float G = min(1.0, min(G1, G2));
	return G;
}

void main(void)
{
	float ratio = 1.0/ uRefracValue;
	vec3 I = normalize(pos3D.xyz);

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
		vec3 lightSource = uLightSource;

		vec3 Kd = vec3(0.8);
		vec3 Ks = vec3(1.0);
		vec3 i = normalize(lightSource - pos3D.xyz);
		vec3 o = normalize(-pos3D.xyz);
		vec3 M = normalize(i + o);

		float cosT = dot(normalize(N),normalize(M));
		float pi = 3.14159265359;

		float NdotM = dot(normalize(N),normalize(M));
		float NdotI = dot(normalize(N),normalize(i));
		float NdotO = dot(normalize(N),normalize(o));
		float OdotM = dot(normalize(o),normalize(M));
		float IdotM = dot(normalize(i),normalize(M));

		float Fr = fresnel(I, M, uRefracValue);
		float D = beckmann(cosT, uSigmaValue, pi);
		float G = masking(NdotM,NdotI, NdotO, OdotM, IdotM);

		float L1 = (Fr * D * G) / (4.0 * NdotI * NdotO);
		vec3 L2 = Kd/pi + Ks * L1;
		vec3 speculaire = Ks * (Fr * D * G) / (4.0 * NdotI * NdotO);
		vec3 diffuse = Kd/pi * (1.0 - Fr);
		vec3 fr = diffuse + speculaire;
		vec3 L = vec3(0.0,2.5,2.5) * fr * cosT;

		gl_FragColor = vec4(L,1.0);
	}
	else {
		vec3 col = vec3(0.0,0.6,0.4) * dot(N,normalize(vec3(-pos3D))); // Lambert rendering, eye light source
		gl_FragColor = vec4(col,1.0);
	}
}
