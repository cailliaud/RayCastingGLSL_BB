
// =====================================================
var gl;
var shadersLoaded = 0;
var vertShaderTxt;
var fragShaderTxt;
var shaderProgram;
var vertexBuffer;
var sphere = null;
var backgroundColor = [0.2,0.2,0.2];

function Sphere(x,y,z,rad,mat){
	this.x = x;
	this.y = y;
	this.z = z;
	this.rad = rad;

	this.mat = mat;
}

function Materiau (r,g,b, ks, ni , m){
	this.r = r;
	this.g = g;
	this.b = b;
	this.ks = ks;
	this.ni = ni;
	this.m = m;
}
function Light(x,y,z,p){
	this.x = x;
	this.y = y;
	this.z = z;
	this.p = p;

}

function showInfoSphere(){
	alert("x:"+sphere.x+" y:"+sphere.y+" z:"+sphere.z+  " \n rouge:"+sphere.r+" vert:"+sphere.g+" bleu:"+sphere.b);
}

// =====================================================
function webGLStart() {
	//Instanciation d'un objet sphere
	mat = new Materiau(1.0,1.0,1.0,3.0,1.3,0.6);
	
	sphere = new Sphere(0.0,0.0,150.0,15.0, mat);
	
	// Instanciation d'un objet lumière
	light = new Light (120.0,120.0,0.0,3.0);
	
	var canvas = document.getElementById("WebGL-test");
	initGL(canvas);
	initBuffers();
	loadShaders('shader');

	gl.clearColor(0.0, 0.0, 0.0, 1.0);
	
	refreshInfo();
	
	
	
	drawScene();
}


function refreshInfo() {
	

	
	document.getElementById("posX").value= sphere.x.toFixed(1);
	document.getElementById("posY").value= sphere.y.toFixed(1);
	document.getElementById("posZ").value= sphere.z.toFixed(1);
	document.getElementById("radius").value= sphere.rad.toFixed(1);
	document.getElementById("red").value= sphere.mat.r.toFixed(1);
	document.getElementById("green").value= sphere.mat.g.toFixed(1);
	document.getElementById("blue").value= sphere.mat.b.toFixed(1);
	document.getElementById("Ks").value= sphere.mat.ks.toFixed(1);
	document.getElementById("M").value= sphere.mat.m.toFixed(1);
	document.getElementById("Ni").value= sphere.mat.ni.toFixed(1);
	
	document.getElementById("posXLight").value= light.x.toFixed(1);
	document.getElementById("posYLight").value= light.y.toFixed(1);
	document.getElementById("posZLight").value= light.z.toFixed(1);
	document.getElementById("PuissanceLight").value= light.p.toFixed(1);
	


	
	document.getElementById("outputXLight").value= light.x.toFixed(1);
	document.getElementById("outputYLight").value= light.y.toFixed(1);
	document.getElementById("outputZLight").value= light.z.toFixed(1);
	
	document.getElementById("outputXSphere").value= sphere.x.toFixed(1);
	document.getElementById("outputYSphere").value= sphere.y.toFixed(1);
	document.getElementById("outputZSphere").value= sphere.z.toFixed(1);
	document.getElementById("outputradius").value= sphere.rad.toFixed(1);
	document.getElementById("outputKs").value= sphere.mat.ks.toFixed(1);
	document.getElementById("outputM").value= sphere.mat.m.toFixed(1);
	document.getElementById("outputNi").value= sphere.mat.ni.toFixed(1);
	
	
	document.getElementById("outputRedSphere").value= sphere.mat.r.toFixed(1);
	document.getElementById("outputGreenSphere").value= sphere.mat.g.toFixed(1);
	document.getElementById("outputBlueSphere").value= sphere.mat.b.toFixed(1);
	document.getElementById("outputPuissanceLight").value= light.p.toFixed(1);




}

// =====================================================
function initGL(canvas)
{
	try {
		gl = canvas.getContext("experimental-webgl");
		gl.viewportWidth = canvas.width;
		gl.viewportHeight = canvas.height;
		gl.viewport(0, 0, canvas.width, canvas.height);
	} catch (e) {}
	if (!gl) {
		alert("Could not initialise WebGL");
	}
}

// =====================================================
function initBuffers() {
	vertexBuffer = gl.createBuffer();
	gl.bindBuffer(gl.ARRAY_BUFFER, vertexBuffer);
	vertices = [
							-1.0, -1.0,
							-1.0,  1.0,
							 1.0,  1.0,
							 1.0, -1.0
							];
	gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(vertices), gl.STATIC_DRAW);
	vertexBuffer.itemSize = 2;
	vertexBuffer.numItems = 4;
}


// =====================================================
function loadShaders(shader) {
	loadShaderText(shader,'.vs');
	loadShaderText(shader,'.fs');
}

// =====================================================
function loadShaderText(filename,ext) {   // technique car lecture asynchrone...
  var xhttp = new XMLHttpRequest();
  xhttp.onreadystatechange = function() {
    if (xhttp.readyState == 4 && xhttp.status == 200) {
			if(ext=='.vs') { vertShaderTxt = xhttp.responseText; shadersLoaded ++; }
			if(ext=='.fs') { fragShaderTxt = xhttp.responseText; shadersLoaded ++; }
			if(shadersLoaded==2) {
				initShaders(vertShaderTxt,fragShaderTxt);
				shadersLoaded=0;
			}
    }
  }
  xhttp.open("GET", filename+ext, true);
  xhttp.send();
}

// =====================================================
function initShaders(vShaderTxt,fShaderTxt) {

	vshader = gl.createShader(gl.VERTEX_SHADER);
	gl.shaderSource(vshader, vShaderTxt);
	gl.compileShader(vshader);
	if (!gl.getShaderParameter(vshader, gl.COMPILE_STATUS)) {
		alert(gl.getShaderInfoLog(vshader));
		return null;
	}

	fshader = gl.createShader(gl.FRAGMENT_SHADER);
	gl.shaderSource(fshader, fShaderTxt);
	gl.compileShader(fshader);
	if (!gl.getShaderParameter(fshader, gl.COMPILE_STATUS)) {
		alert(gl.getShaderInfoLog(fshader));
		return null;
	}

	shaderProgram = gl.createProgram();
	gl.attachShader(shaderProgram, vshader);
	gl.attachShader(shaderProgram, fshader);
	


	gl.linkProgram(shaderProgram);

	if (!gl.getProgramParameter(shaderProgram, gl.LINK_STATUS)) {
		alert("Could not initialise shaders");
	}

	gl.useProgram(shaderProgram);
	
	shaderProgram.sphereCenter = gl.getUniformLocation(shaderProgram, "sphereCenter");
	shaderProgram.sphereRadius = gl.getUniformLocation(shaderProgram, "sphereRadius");
	shaderProgram.sphereColor = gl.getUniformLocation(shaderProgram, "sphereColor");
	shaderProgram.sphereKs = gl.getUniformLocation(shaderProgram, "sphereKs");
	shaderProgram.sphereM = gl.getUniformLocation(shaderProgram, "sphereM");
	shaderProgram.sphereNi = gl.getUniformLocation(shaderProgram, "sphereNi");
	
	
	shaderProgram.backgroundColor = gl.getUniformLocation(shaderProgram, "backgroundColor");
	
	// Envoi des variables aux shaders
	shaderProgram.lumiereCenter = gl.getUniformLocation(shaderProgram, "lumiereCenter");
	shaderProgram.lumierePuissance = gl.getUniformLocation(shaderProgram, "lumierePuissance");

	
	gl.uniform3f(shaderProgram.backgroundColor,backgroundColor[0],backgroundColor[1],backgroundColor[2]);
		
	shaderProgram.vertexPositionAttribute = gl.getAttribLocation(shaderProgram, "vertexPosition");
	gl.enableVertexAttribArray(shaderProgram.vertexPositionAttribute);
	
	setUniform();
	
	drawScene();
}


function setUniform(){
	gl.uniform3f(shaderProgram.sphereCenter, sphere.x, sphere.y, sphere.z);
	gl.uniform1f(shaderProgram.sphereRadius, sphere.rad);
	gl.uniform3f(shaderProgram.sphereColor, sphere.mat.r, sphere.mat.g, sphere.mat.b);
	gl.uniform1f(shaderProgram.sphereKs, sphere.mat.ks);
	gl.uniform1f(shaderProgram.sphereM, sphere.mat.m);
	gl.uniform1f(shaderProgram.sphereNi, sphere.mat.ni);
	
	
	gl.uniform3f(shaderProgram.lumiereCenter, light.x, light.y, light.z);
	gl.uniform1f(shaderProgram.lumierePuissance, light.p );
}


// =====================================================
function drawScene() {
	gl.clear(gl.COLOR_BUFFER_BIT);

	gl.bindBuffer(gl.ARRAY_BUFFER, vertexBuffer);
	gl.vertexAttribPointer(shaderProgram.vertexPositionAttribute,
      vertexBuffer.itemSize, gl.FLOAT, false, 0, 0);

	gl.drawArrays(gl.TRIANGLE_FAN, 0, vertexBuffer.numItems);
}


// =====================================================
function redraw() {
	sphere.x= parseFloat(document.getElementById("posX").value); 
	sphere.y= parseFloat(document.getElementById("posY").value); 
	sphere.z= parseFloat(document.getElementById("posZ").value);
	sphere.rad = parseFloat(document.getElementById("radius").value);
	sphere.mat.r= parseFloat(document.getElementById("red").value); 
	sphere.mat.g = parseFloat(document.getElementById("green").value); 
	sphere.mat.b = parseFloat(document.getElementById("blue").value); 
	sphere.mat.ks = parseFloat(document.getElementById("Ks").value); 
	sphere.mat.m = parseFloat(document.getElementById("M").value); 
	sphere.mat.ni = parseFloat(document.getElementById("Ni").value); 
	
	
	
	
	light.x= parseFloat(document.getElementById("posXLight").value); 
	light.y= parseFloat(document.getElementById("posYLight").value); 
	light.z= parseFloat(document.getElementById("posZLight").value);
	light.p= parseFloat(document.getElementById("PuissanceLight").value); 

	
	setUniform();
	drawScene();
	
	refreshInfo();



}





function wait(ms){
   var start = new Date().getTime();
   var end = start;
   while(end < start + ms) {
     end = new Date().getTime();
  }
}
