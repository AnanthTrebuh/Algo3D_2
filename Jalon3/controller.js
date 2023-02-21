function setMirror() {
	isMirror = !isMirror;
	isCookTor = false;
	isSample = false;
	// isDepoli = false;
	OBJ1.isMirror = !OBJ1.isMirror;
	OBJ1.isCookTor = false;
	OBJ1.isSample = false;
	OBJ1.isDepoli = false;
	document.getElementById("cookTorSwitch").checked=false;
	document.getElementById("echantillonnage").checked=false;
	if(!isMirror) {
		document.getElementById("refractSwitch").checked=false;
		document.getElementById("mirrorSwitchOpt").checked=false;
		isRefrac = false;
		OBJ1.isRefrac = false;
		isDepoli = false;
	}
}

/*=========================*/

function setRefrac() {
	isRefrac = !isRefrac;
	isCookTor = false;
	isSample = false;
	isDepoli = false;
	OBJ1.isRefrac = !OBJ1.isRefrac;
	OBJ1.isCookTor = false;
	OBJ1.isSample = false;
	OBJ1.isDepoli = false;
	document.getElementById("cookTorSwitch").checked=false;
	document.getElementById("echantillonnage").checked=false;
	document.getElementById("mirrorSwitchOpt").checked=false;
	if(isRefrac){
		document.getElementById("mirrorSwitch").checked=true;
		isMirror = true;
		OBJ1.isMirror = true;
	}
}

/*=========================*/

function setCookTor() {
	isCookTor = !isCookTor;
	isRefrac = false;
	isMirror = false;
	isSample = false;
	isDepoli = false;
	OBJ1.isCookTor = !OBJ1.isCookTor;
	OBJ1.isRefrac = false;
	OBJ1.isMirror = false;
	OBJ1.isSample = false;
	OBJ1.isDepoli = false;
	document.getElementById("mirrorSwitch").checked=false;
	document.getElementById("refractSwitch").checked=false;
	document.getElementById("echantillonnage").checked=false;
	document.getElementById("mirrorSwitchOpt").checked=false;
}

/*=========================*/

function setEchantillonnage(){
	isSample = ! isSample;
	isCookTor = false;
	isRefrac = false;
	isMirror = false;
	isDepoli = false;
	OBJ1.isSample = !OBJ1.isSample;
	OBJ1.isCookTor = false;
	OBJ1.isRefrac = false;
	OBJ1.isMirror = false;
	OBJ1.isDepoli = false;
	document.getElementById("mirrorSwitch").checked=false;
	document.getElementById("refractSwitch").checked=false;
	document.getElementById("cookTorSwitch").checked=false;
	document.getElementById("mirrorSwitchOpt").checked=false;
}

/*=========================*/

function changeImage(numImage) {
	textureCube = textures[numImage];
}

/*=========================*/

function getSliderVal() {
	sliderValue =  document.getElementById("slider").value;
	document.getElementById("refract").innerHTML=sliderValue;
}

/*=========================*/

function getSigmaVal(){
	sigmaValue = document.getElementById("sigma").value;
	document.getElementById("sigmaVal").innerHTML=sigmaValue;
}

/*=========================*/

function getSampleVal(){
	nbSample = document.getElementById("sample").value;
	document.getElementById("sampleVal").innerHTML=nbSample;
}

/*=========================*/

function getLuminoVal(){
	lumino = document.getElementById("lumino").value;
	document.getElementById("luminoVal").innerHTML=lumino;
}

/*=========================*/

function changeObj(object){
	objet = 'obj/'+object+'.obj';
	OBJ1 = new objmesh(objet);
	OBJ1.draw();
}

/*=========================*/

function getColorVal(){
	var colorTemp = hexToRgb(document.getElementById("colorchooser").value);
	color = [colorTemp.r/255,colorTemp.g/255,colorTemp.b/255];
}

/*=========================*/

/*Permet de récupérer le code Hex pour le tranformer en rgb*/
function hexToRgb(hex) {
	var result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
	return result ? {
	  r: parseInt(result[1], 16),
	  g: parseInt(result[2], 16),
	  b: parseInt(result[3], 16)
	} : null;
}

/*=========================*/

function setDepoli(){
	console.log("depoli");
	isDepoli = !isDepoli;
	OBJ1.isDepoli = !OBJ1.isDepoli;
}