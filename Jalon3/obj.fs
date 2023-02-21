
precision mediump float;
const float pi = 3.14159265359;

// classic obj
varying vec4 pos3D;
varying vec3 N;

varying mat4 rMat;

uniform bool uIsMirror;
uniform bool uIsRefrac;
uniform bool uIsCookTor;
uniform bool uIsSample;
uniform bool uIsDepoli;

uniform samplerCube uSkybox;

uniform float uRefracValue;
uniform float uSigmaValue;

uniform int uNbSample;
uniform float uLumino;

uniform vec3 uLightSource;
uniform vec3 uColorObj;

// ==============================================

vec4 refraction(vec3 I, vec3 N, float ratio){
	vec3 Refraction = refract(I, N, ratio);
	Refraction = vec3(rMat * vec4(Refraction, 1.0));
	return textureCube(uSkybox, Refraction.xzy);
}

vec4 reflection(vec3 I, vec3 N){
	vec3 Reflection = reflect(I, N);
	Reflection = vec3(rMat * vec4(Reflection, 1.0));
	return textureCube(uSkybox, Reflection.xzy);
}

float ddot(vec3 a, vec3 b){
	return max(0.0,dot(a,b));
}


float fresnel(vec3 I, vec3 N, float uRefracValue){
	float c = abs(dot(I, N));
	float g = sqrt((uRefracValue*uRefracValue) + ( c*c) - 1.0);
	float R = 0.5 * 
			(((g-c)*(g-c))/((g+c)*(g+c))) * 
			( 1.0 + (((c * (g+c)-1.0) * (c * (g+c)-1.0)) / ((c * (g-c)+1.0) * (c * (g-c)+1.0) )));
	return R;
}

float beckmann(float cosT, float sigma){
	float sigma2 = sigma * sigma;
	float cosT2 = cosT * cosT;
	float cosT4 = cosT2 * cosT2;
	float sinT2 = 1.0 - cosT2;
	float tanT2 = sinT2 / cosT2;

	float D1 = 1.0 / (pi * sigma2 * cosT4);
	float D2 = exp(-tanT2 / sigma2);
	float D = D1 * D2;
	return D;
}

float masking(float NdotM,float NdotI, float NdotO, float OdotM, float IdotM){
	float G1 = 2.0 * NdotM * NdotI / IdotM;
	float G2 = 2.0 * NdotM * NdotO / OdotM;
	float G = min(1.0, min(G1, G2));
	return G;
}

float rand(vec2 co){
    return fract(sin(dot(co, vec2(12.9898, 78.233))) * 43758.5453);
}

vec3 randM(int i){
	vec2 ran = (rMat*(vec4(gl_FragCoord.xy,0.,0.)+1.0)).xy;

	float ksi1 = rand(ran + 2.*float(i));
	float ksi2 = rand(ran + 2.*float(i) + 1.);

	float phi = 2.0 * pi * ksi2;
	float theta = atan(sqrt(-uSigmaValue*uSigmaValue * log(1.0-ksi1)));

	float x = sin(theta) * cos(phi);
	float y = sin(theta) * sin(phi);
	float z = cos(theta);

	vec3 m =  vec3(x,y,z);
	return m;
}

mat3 transpose(mat3 m) {
    return mat3(m[0][0], m[1][0], m[2][0],
                m[0][1], m[1][1], m[2][1],
                m[0][2], m[1][2], m[2][2]);
}

vec3 cookTorrance(vec3 Nn, vec3 o){
	vec3 lightSource = uLightSource;

	vec3 Kd = uColorObj;
	vec3 i = normalize(lightSource - pos3D.xyz);
	vec3 M = normalize(i + o);

	float NdotM = ddot(Nn, M); 
	float cosT = NdotM;
	float NdotI = ddot(Nn, i); 
	float NdotO = ddot(Nn, o); 
	float OdotM = ddot(o, M); 
	float IdotM = ddot(i, M); 

	float F = fresnel(i, M, uRefracValue);
	float D = beckmann(cosT, uSigmaValue);
	float G = masking(NdotM,NdotI, NdotO, OdotM, IdotM);

	vec3 brdf = vec3((F * D * G) / (4.0 * NdotI * NdotO));
	vec3 diffuse = Kd/pi * (1.0 - F);
	vec3 Fr = diffuse + brdf;
	vec3 L = (2.0) * Fr * NdotI;
	return L;
}

mat3 getMatRotM(vec3 Nn){
	vec3 iN = vec3(1.0,0.0,0.0);
	if(dot(normalize(iN), Nn) > 0.9){
		iN = vec3(0.0,1.0,0.0);
	}
	vec3 jN = normalize(cross(Nn, iN));
	iN = normalize(cross(Nn, jN));

	mat3 matRot = mat3(iN, jN, Nn);
	return matRot;
}

vec3 echantillonageImportance(vec3 Nn, vec3 o){	
	
	mat3 matRot = getMatRotM(Nn);
	int nbIter = 0;
	vec3 Lo = vec3(0.0);
	vec3 m;

	for(int j = 0; j<100; j++){
		if(nbIter>uNbSample) break;

		m = randM(j);
		m = matRot * m;
		m = normalize(m);

		vec3 i = reflect(-o, m);

		float NdotM = ddot(Nn, m); 
		float NdotI = ddot(Nn, i); 
		float NdotO = ddot(Nn, o); 
		float OdotM = ddot(o, m); 
		float IdotM = ddot(i, m); 

		if(NdotI < 0.0001 || NdotO < 0.0001 || NdotM < 0.0001 || OdotM < 0.0001 || IdotM < 0.0001){
			continue;
		}
		
		float F = fresnel(i, m, uRefracValue);
		float D = beckmann(NdotM, uSigmaValue);
		float G = masking(NdotM,NdotI, NdotO, OdotM, IdotM);

		float pdf = D * NdotM;
		
		float brdf = (F * D * G) / (4.0 * NdotI * NdotO);

		i = vec3(rMat * vec4(i, 1.0));
		vec3 Li = textureCube(uSkybox, i.xzy).xyz;
		Lo += Li * brdf * NdotI / pdf;

		nbIter++; 
	}

	Lo /= float(nbIter);
	return Lo;
}
void main(void)
{
	float ratio = 1.0 / uRefracValue;
	vec3 I = normalize(pos3D.xyz);
	vec3 Nn = normalize(N);
	vec3 o = normalize(-pos3D.xyz);

	vec4 textRefrac = refraction(I, Nn, ratio);;
	vec4 textMirror = reflection(I, Nn);

	if(uIsMirror && uIsRefrac){
		float R = fresnel(I, Nn, uRefracValue);
		float T = 1.0 - R;
		gl_FragColor = (textRefrac * T) + (textMirror * R);    
	}
	else if (uIsMirror){
		if(uIsDepoli){
			gl_FragColor = vec4(0.0,0.0,0.0,1.0);
		}else{
			gl_FragColor = textMirror;			
		}
	}
	else if(uIsCookTor){
		vec3 L = cookTorrance(Nn,o);
		gl_FragColor = vec4(L,1.0);
	}
	else if (uIsSample){
		vec3 Lo = echantillonageImportance(Nn, o);
		gl_FragColor = vec4(Lo*uLumino, 1.0);
	}
	else {
		vec3 col = uColorObj * dot(Nn,normalize(vec3(-pos3D))); // Lambert rendering, eye light source
		gl_FragColor = vec4(col,1.0);
	}
}
