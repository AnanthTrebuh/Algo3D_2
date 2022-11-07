attribute vec3 aVertexPosition;
attribute vec3 aVertexNormal;

uniform bool uIsMirror;

uniform mat4 uRMatrix;
uniform mat4 uMVMatrix;
uniform mat4 uPMatrix;

varying vec4 pos3D;
varying vec3 N;

//mirror obj
varying vec3 vPosMirror;

void main(void) 
{
	//pos3D = uMVMatrix * vec4(aVertexPosition,1.0);
	//N = vec3(uRMatrix * vec4(aVertexNormal,1.0));
	//gl_Position = uPMatrix * pos3D;

	N = mat3(transpose(inverse(uMVMatrix))) * aVertexNormal;
	vPos = vec3(model * vec4(aVertexPosition, 1.0));
	gl_Position = uPMatrix * uMVMatrix * vec4(vPos,1.0);
}
