precision mediump float;

uniform samplerCube uSkybox;
varying vec3 pos;

void main(void)
{    
	//gl_FragColor = vec4(0.0,0.5,0.5,1.0);
    gl_FragColor = textureCube(uSkybox, vec3(pos.x, pos.z, pos.y));
}