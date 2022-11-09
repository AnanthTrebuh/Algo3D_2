
// =====================================================
var gl;

// =====================================================
var mvMatrix = mat4.create();
var pMatrix = mat4.create();
var rotMatrix = mat4.create();
var distCENTER;
var textureCube;
var isMirror = false;
var isRefrac = false;
// =====================================================

var OBJ1 = null;
var PLANE = null;
var CUBE = null;

var objet = "sphere.obj";
var sliderValue = 0.5;


// =====================================================
// OBJET 3D, lecture fichier obj
// =====================================================

class objmesh {

	// --------------------------------------------
	constructor(objFname) {
		this.objName = objFname;
		this.shaderName = 'obj';
		this.loaded = -1;
		this.shader = null;
		this.mesh = null;
		this.isMirror = this.isMirror;
		this.isRefrac = this.isRefrac;
		
		loadObjFile(this);
		loadShaders(this);
	}

	// --------------------------------------------
	setShadersParams() {
		gl.useProgram(this.shader);

		this.shader.vAttrib = gl.getAttribLocation(this.shader, "aVertexPosition");
		gl.enableVertexAttribArray(this.shader.vAttrib);
		gl.bindBuffer(gl.ARRAY_BUFFER, this.mesh.vertexBuffer);
		gl.vertexAttribPointer(this.shader.vAttrib, this.mesh.vertexBuffer.itemSize, gl.FLOAT, false, 0, 0);

		this.shader.nAttrib = gl.getAttribLocation(this.shader, "aVertexNormal");
		gl.enableVertexAttribArray(this.shader.nAttrib);
		gl.bindBuffer(gl.ARRAY_BUFFER, this.mesh.normalBuffer);
		gl.vertexAttribPointer(this.shader.nAttrib, this.mesh.vertexBuffer.itemSize, gl.FLOAT, false, 0, 0);

		gl.bindTexture(gl.TEXTURE_CUBE_MAP, textureCube);

		this.shader.uSkyboxUniform = gl.getUniformLocation(this.shader, "uSkybox");
		gl.bindTexture(gl.TEXTURE_CUBE_MAP, textureCube);
		gl.uniform1i(this.shader.uSkyboxUniform, 0);

		this.shader.rMatrixUniform = gl.getUniformLocation(this.shader, "uRMatrix");
		this.shader.mvMatrixUniform = gl.getUniformLocation(this.shader, "uMVMatrix");
		this.shader.pMatrixUniform = gl.getUniformLocation(this.shader, "uPMatrix");

		this.shader.isMirrorUniform = gl.getUniformLocation(this.shader, "uIsMirror");
		this.shader.isRefracUniform = gl.getUniformLocation(this.shader, "uIsRefrac");
		this.shader.refractValue = gl.getUniformLocation(this.shader, "uRefracValue");
	}
	
	// --------------------------------------------
	setMatrixUniforms() {
		mat4.identity(mvMatrix);
		mat4.translate(mvMatrix, distCENTER);
		mat4.multiply(mvMatrix, rotMatrix);
		gl.uniformMatrix4fv(this.shader.rMatrixUniform, false, rotMatrix);
		gl.uniformMatrix4fv(this.shader.mvMatrixUniform, false, mvMatrix);
		gl.uniformMatrix4fv(this.shader.pMatrixUniform, false, pMatrix);
		gl.uniform1i(this.shader.isMirrorUniform, this.isMirror);
		gl.uniform1i(this.shader.isRefracUniform, this.isRefrac);
		gl.uniform1f(this.shader.refractValue, sliderValue);
	}
	
	// --------------------------------------------
	draw() {
		if(this.shader && this.loaded==4 && this.mesh != null) {
			this.setShadersParams();
			this.setMatrixUniforms();
			gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, this.mesh.indexBuffer);
			gl.drawElements(gl.TRIANGLES, this.mesh.indexBuffer.numItems, gl.UNSIGNED_SHORT, 0);
		}
	}
}

class cube {

	// --------------------------------------------
	constructor() {
		this.shaderName = 'cube';
		this.loaded = -1;
		this.shader = null;
		this.mesh = null;
		this.vbuffer = null;
		// this.cBuffer = null;
		this.iBuffer = null;
		this.initAll();
		loadShaders(this);
	}

	initAll() {
		var size = 15
		var vertices = [
			-size, -size, -size, // 0
			size, -size, -size, // 1
			size, size, -size, // 2
			-size, size, -size, // 3
			-size, -size, size, // 4 
			size, -size, size, // 5
			size, size, size, // 6
			-size, size, size, // 7
		]

		var indices = [
			3, 2, 6, 	3, 6, 7, /* top */ 
			0, 4, 5, 	0, 5, 1, /* bottom */

			3, 7, 4, 	3, 4, 0, /* left */ 
			0, 1, 2, 	0, 2, 3, /* front */ 
			4, 6, 5, 	4, 7, 6, /* back */ 
			1, 5, 6, 	1, 6, 2 /* right */ 
		]

		// Create a texture.
		gl.generateMipmap(gl.TEXTURE_CUBE_MAP);
		gl.texParameteri(gl.TEXTURE_CUBE_MAP, gl.TEXTURE_MIN_FILTER, gl.LINEAR_MIPMAP_LINEAR);

		// load on buffer
		// vertecies
		this.vBuffer = gl.createBuffer()
		gl.bindBuffer(gl.ARRAY_BUFFER, this.vBuffer)
		gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(vertices), gl.STATIC_DRAW)
		this.vBuffer.itemSize = 3
		this.vBuffer.numItems = 8

		// indexs
		this.iBuffer = gl.createBuffer()
		gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, this.iBuffer)
		gl.bufferData(gl.ELEMENT_ARRAY_BUFFER, new Uint16Array(indices), gl.STATIC_DRAW)
		this.iBuffer.itemSize = 1
		this.iBuffer.numItems = 36


	}

	// --------------------------------------------
	setShadersParams() {
		gl.useProgram(this.shader)

		this.shader.vAttrib = gl.getAttribLocation(this.shader, "aVertexPosition")
		gl.enableVertexAttribArray(this.shader.vAttrib)
		gl.bindBuffer(gl.ARRAY_BUFFER, this.vBuffer)
		gl.vertexAttribPointer(this.shader.vAttrib, this.vBuffer.itemSize, gl.FLOAT, false, 0, 0)

		this.shader.uSkyboxUniform = gl.getUniformLocation(this.shader, "uSkybox")
		//gl.activeTexture(gl.TEXTURE_CUBE_MAP); // genere des warnings
		gl.bindTexture(gl.TEXTURE_CUBE_MAP, textureCube);
		gl.uniform1i(this.shader.uSkyboxUniform, 0);

		this.shader.pMatrixUniform = gl.getUniformLocation(this.shader, "uPMatrix")
		this.shader.mvMatrixUniform = gl.getUniformLocation(this.shader, "uMVMatrix")

		mat4.identity(mvMatrix)
		mat4.translate(mvMatrix, distCENTER)
		mat4.multiply(mvMatrix, rotMatrix)

		gl.uniformMatrix4fv(this.shader.pMatrixUniform, false, pMatrix)
		gl.uniformMatrix4fv(this.shader.mvMatrixUniform, false, mvMatrix)
	}

	// --------------------------------------------
	setMatrixUniforms() {
		mat4.identity(mvMatrix);
		mat4.translate(mvMatrix, distCENTER);
		mat4.multiply(mvMatrix, rotMatrix);
		gl.uniformMatrix4fv(this.shader.mvMatrixUniform, false, mvMatrix);
		gl.uniformMatrix4fv(this.shader.pMatrixUniform, false, pMatrix);
	}

	// --------------------------------------------
	draw() {
        if (this.shader && this.loaded == 4) {
			this.setShadersParams()
			this.setMatrixUniforms()
			gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, this.iBuffer)
			gl.drawElements(gl.TRIANGLES, this.iBuffer.numItems , gl.UNSIGNED_SHORT, 0)
        }
    }
}

// =====================================================
// PLAN 3D, Support géométrique
// =====================================================

class plane {
	
	// --------------------------------------------
	constructor() {
		this.shaderName='plane';
		this.loaded=-1;
		this.shader=null;
		this.initAll();
	}
		
	// --------------------------------------------
	initAll() {
		var size=1.0;
		var vertices = [
			-size, -size, 0.,
			 size, -size, 0,
			 size, size, 0.,
			-size, size, 0.
		];

		var texcoords = [
			0.0,0.0,
			0.0,1.0,
			1.0,1.0,
			1.0,0.0
		];

		this.vBuffer = gl.createBuffer();
		gl.bindBuffer(gl.ARRAY_BUFFER, this.vBuffer);
		gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(vertices), gl.STATIC_DRAW);
		this.vBuffer.itemSize = 3;
		this.vBuffer.numItems = 4;

		this.tBuffer = gl.createBuffer();
		gl.bindBuffer(gl.ARRAY_BUFFER, this.tBuffer);
		gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(texcoords), gl.STATIC_DRAW);
		this.tBuffer.itemSize = 2;
		this.tBuffer.numItems = 4;

		loadShaders(this);
	}
	
	
	// --------------------------------------------
	setShadersParams() {
		gl.useProgram(this.shader);

		this.shader.vAttrib = gl.getAttribLocation(this.shader, "aVertexPosition");
		gl.enableVertexAttribArray(this.shader.vAttrib);
		gl.bindBuffer(gl.ARRAY_BUFFER, this.vBuffer);
		gl.vertexAttribPointer(this.shader.vAttrib, this.vBuffer.itemSize, gl.FLOAT, false, 0, 0);

		this.shader.tAttrib = gl.getAttribLocation(this.shader, "aTexCoords");
		gl.enableVertexAttribArray(this.shader.tAttrib);
		gl.bindBuffer(gl.ARRAY_BUFFER, this.tBuffer);
		gl.vertexAttribPointer(this.shader.tAttrib,this.tBuffer.itemSize, gl.FLOAT, false, 0, 0);

		this.shader.pMatrixUniform = gl.getUniformLocation(this.shader, "uPMatrix");
		this.shader.mvMatrixUniform = gl.getUniformLocation(this.shader, "uMVMatrix");

		mat4.identity(mvMatrix);
		mat4.translate(mvMatrix, distCENTER);
		mat4.multiply(mvMatrix, rotMatrix);

		gl.uniformMatrix4fv(this.shader.pMatrixUniform, false, pMatrix);
		gl.uniformMatrix4fv(this.shader.mvMatrixUniform, false, mvMatrix);
	}

	// --------------------------------------------
	draw() {
		if(this.shader && this.loaded==4) {		
			this.setShadersParams();
			
			gl.drawArrays(gl.TRIANGLE_FAN, 0, this.vBuffer.numItems);
			gl.drawArrays(gl.LINE_LOOP, 0, this.vBuffer.numItems);
		}
	}

}


// =====================================================
// FONCTIONS GENERALES, INITIALISATIONS
// =====================================================



// =====================================================
function initGL(canvas)
{
	try {
		gl = canvas.getContext("experimental-webgl");
		gl.viewportWidth = canvas.width;
		gl.viewportHeight = canvas.height;
		gl.viewport(0, 0, canvas.width, canvas.height);

		gl.clearColor(0.7, 0.7, 0.7, 1.0);
		gl.enable(gl.DEPTH_TEST);
		gl.enable(gl.CULL_FACE);
		gl.cullFace(gl.BACK); 
	} catch (e) {}
	if (!gl) {
		console.log("Could not initialise WebGL");
	}
}


// =====================================================
loadObjFile = function(OBJ3D)
{
	var xhttp = new XMLHttpRequest();

	xhttp.onreadystatechange = function() {
		if (xhttp.readyState == 4 && xhttp.status == 200) {
			var tmpMesh = new OBJ.Mesh(xhttp.responseText);
			OBJ.initMeshBuffers(gl,tmpMesh);
			OBJ3D.mesh=tmpMesh;
		}
	}



	xhttp.open("GET", OBJ3D.objName, true);
	xhttp.send();
}



// =====================================================
function loadShaders(Obj3D) {
	loadShaderText(Obj3D,'.vs');
	loadShaderText(Obj3D,'.fs');
}

// =====================================================
function loadShaderText(Obj3D,ext) {   // lecture asynchrone...
  var xhttp = new XMLHttpRequest();
  
  xhttp.onreadystatechange = function() {
	if (xhttp.readyState == 4 && xhttp.status == 200) {
		if(ext=='.vs') { Obj3D.vsTxt = xhttp.responseText; Obj3D.loaded ++; }
		if(ext=='.fs') { Obj3D.fsTxt = xhttp.responseText; Obj3D.loaded ++; }
		if(Obj3D.loaded==2) {
			Obj3D.loaded ++;
			compileShaders(Obj3D);
			Obj3D.loaded ++;
		}
	}
  }
  
  Obj3D.loaded = 0;
  xhttp.open("GET", Obj3D.shaderName+ext, true);
  xhttp.send();
}

// =====================================================
function compileShaders(Obj3D)
{
	Obj3D.vshader = gl.createShader(gl.VERTEX_SHADER);
	gl.shaderSource(Obj3D.vshader, Obj3D.vsTxt);
	gl.compileShader(Obj3D.vshader);
	if (!gl.getShaderParameter(Obj3D.vshader, gl.COMPILE_STATUS)) {
		console.log("Vertex Shader FAILED... "+Obj3D.shaderName+".vs");
		console.log(gl.getShaderInfoLog(Obj3D.vshader));
	}

	Obj3D.fshader = gl.createShader(gl.FRAGMENT_SHADER);
	gl.shaderSource(Obj3D.fshader, Obj3D.fsTxt);
	gl.compileShader(Obj3D.fshader);
	if (!gl.getShaderParameter(Obj3D.fshader, gl.COMPILE_STATUS)) {
		console.log("Fragment Shader FAILED... "+Obj3D.shaderName+".fs");
		console.log(gl.getShaderInfoLog(Obj3D.fshader));
	}

	Obj3D.shader = gl.createProgram();
	gl.attachShader(Obj3D.shader, Obj3D.vshader);
	gl.attachShader(Obj3D.shader, Obj3D.fshader);
	gl.linkProgram(Obj3D.shader);
	if (!gl.getProgramParameter(Obj3D.shader, gl.LINK_STATUS)) {
		console.log("Could not initialise shaders");
		console.log(gl.getShaderInfoLog(Obj3D.shader));
	}
}

function loadTextures(path) {
    const faces = [
        {
            target: gl.TEXTURE_CUBE_MAP_POSITIVE_X,
            url: path + "right.jpg",
        },
        {
            target: gl.TEXTURE_CUBE_MAP_NEGATIVE_X,
            url: path + "left.jpg",
        },
        {
            target: gl.TEXTURE_CUBE_MAP_POSITIVE_Y,
            url: path + "top.jpg",
        },
        {
            target: gl.TEXTURE_CUBE_MAP_NEGATIVE_Y,
            url: path + "bottom.jpg",
        },
        {
            target: gl.TEXTURE_CUBE_MAP_POSITIVE_Z,
            url: path + "front.jpg",
        },
        {
            target: gl.TEXTURE_CUBE_MAP_NEGATIVE_Z,
            url: path + "back.jpg",
        },
    ]

    var CurrtextureCube = gl.createTexture()
    gl.bindTexture(gl.TEXTURE_CUBE_MAP, CurrtextureCube)
	faces.forEach((face) => {
        const { target, url } = face

        // Upload the canvas to the cubemap face.
        const level = 0
        const internalFormat = gl.RGBA
        const width = 512
        const height = 512
        const format = gl.RGBA
        const type = gl.UNSIGNED_BYTE

        // setup each face so it's immediately renderable
        gl.texImage2D(target, level, internalFormat, width, height, 0, format, type, null)

        // Asynchronously load an image
        const image = new Image()
        image.src = url
        image.onload = () => {
            // Now that the image has loaded upload it to the texture.
            gl.bindTexture(gl.TEXTURE_CUBE_MAP, CurrtextureCube)
            // gl.texParameteri(gl.TEXTURE_CUBE_MAP, gl.TEXTURE_MAG_FILTER, gl.LINEAR);
            // gl.texParameteri(gl.TEXTURE_CUBE_MAP, gl.TEXTURE_MIN_FILTER, gl.LINEAR);
            // gl.texParameteri(gl.TEXTURE_CUBE_MAP, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE);
            // gl.texParameteri(gl.TEXTURE_CUBE_MAP, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE);
            gl.texImage2D(target, level, internalFormat, format, type, image)
            gl.generateMipmap(gl.TEXTURE_CUBE_MAP)
        }
    })
    return CurrtextureCube
}

// =====================================================
function webGLStart() {
	
	var canvas = document.getElementById("WebGL-test");
	document.getElementById("mirrorSwitch").checked=false;
	document.getElementById("refractSwitch").checked=false;

	canvas.onmousedown = handleMouseDown;
	document.onmouseup = handleMouseUp;
	document.onmousemove = handleMouseMove;
	canvas.onwheel = handleMouseWheel;

	initGL(canvas);

	textures = {
        Water: loadTextures("./skybox/Water/"),
        Space: loadTextures("./skybox/Space/"),
        RedSpace: loadTextures("./skybox/RedSpace/"),
		Mountain: loadTextures("./skybox/Mountain/"),
    }
    textureCube = textures.Water
	gl.bindTexture(gl.TEXTURE_CUBE_MAP, textureCube);

	mat4.perspective(45, gl.viewportWidth / gl.viewportHeight, 0.1, 100.0, pMatrix);
	mat4.identity(rotMatrix);
	mat4.rotate(rotMatrix, rotX, [1, 0, 0]);
	mat4.rotate(rotMatrix, rotY, [0, 0, 1]);

	distCENTER = vec3.create([0,-0.2,-3]);
	
	PLANE = new plane();
	CUBE = new cube();
	OBJ1 = new objmesh(objet);
	//OBJ2 = new objmesh('porsche.obj');
	
	tick();
}

// =====================================================
function drawScene() {
	console.log("drawScene");
	gl.clear(gl.COLOR_BUFFER_BIT);
	//PLANE.draw();
	CUBE.draw();
	OBJ1.draw();
	//OBJ2.draw();
}

function setMirror() {
	    // this.isRefrac =false;
		OBJ1.isMirror = !OBJ1.isMirror;
		// document.getElementById("refractSwitch").checked=false;

		// this.isMir = !this.isMir;
		// OBJ1.isMirror = this.isMir;
		console.log("Mirror : "+this.isMir);
}

function setRefrac() {
		// this.isMir = false;
		OBJ1.isRefrac = !OBJ1.isRefrac;
		// document.getElementById("mirrorSwitch").checked=false;
		// this.isRefrac = !this.isRefrac;
		// OBJ1.isRefrac = this.isRefrac;
		
		console.log("Refrac : "+this.isRefrac);
}

function changeImage(numImage) {
	console.log(numImage)
	textureCube = textures[numImage];
}

function getSliderVal() {
	sliderValue =  document.getElementById("slider").value/100;
	console.log("sliderValue : "+sliderValue);
}
var slider = document.getElementById("myRange");

var output = document.getElementById("demo");
// var sliderValue = slider.value;
// output.innerHTML = slider.value; // Display the default slider value

// Update the current slider value (each time you drag the slider handle)
// slider.oninput = function() {
//  	sliderValue = slider.value;
// 	console.log(sliderValue);
// }
