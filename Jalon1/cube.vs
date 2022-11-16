attribute vec3 aVertexPosition;
// attribute vec4 aColor;

uniform mat4 uMVMatrix;
uniform mat4 uPMatrix;

varying vec3 pos;

void main(void)
{

    pos = aVertexPosition;
	gl_Position = uPMatrix * uMVMatrix * vec4(aVertexPosition, 1.0);
}  