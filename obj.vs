attribute vec3 aVertexPosition;
attribute vec3 aVertexNormal;

uniform bool uIsMirror;

uniform mat4 uRMatrix;
uniform mat4 uMVMatrix;
uniform mat4 uPMatrix;

varying vec4 pos3D;
varying vec3 N;
varying vec4 rMat;

//mirror obj
varying vec3 vPosMirror;

mat4 transpose(mat4 m) {
    return mat4(m[0][0], m[1][0], m[2][0], m[3][0],
                m[0][1], m[1][1], m[2][1], m[3][1],
                m[0][2], m[1][2], m[2][2], m[3][2],
                m[0][3], m[1][3], m[2][3], m[3][3] );
}

void main(void) 
{
	pos3D = uMVMatrix * vec4(aVertexPosition,1.0);
	N = vec3(uRMatrix * vec4(aVertexNormal,1.0));
	gl_Position = uPMatrix * pos3D;
	rMat = vec4(transpose(uRMatrix));

	// N = mat3(transpose(inverse(uMVMatrix))) * aVertexNormal;
	// vPos = vec3(model * vec4(aVertexPosition, 1.0));
	// gl_Position = uPMatrix * uMVMatrix * vec4(vPos,1.0);
}
