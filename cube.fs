precision mediump float;

uniform samplerCube uSkybox;
varying vec3 pos;

void main(void)
{    
    gl_FragColor = textureCube(uSkybox, vec3(pos.x, pos.z, pos.y));
}