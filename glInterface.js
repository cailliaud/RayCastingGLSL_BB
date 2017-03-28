/**
 * Created by ambie on 09/03/2017.
 */

//#######################//
var gl;
var shadersLoaded = 0;
var vertShaderTxt;
var fragShaderTxt;
var shaderProgram;
var vertexBuffer;
var canvas;
//#######################//

function webGLStart() {
    canvas = document.getElementById("WebGL-test");
    initGL(canvas);
    initBuffers();
    loadShaders('shader');
}

function initGL(canvas) {
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
function initBuffers() {
    vertexBuffer = gl.createBuffer();
    gl.bindBuffer(gl.ARRAY_BUFFER, vertexBuffer);
    var vertices = [
        -1.0, -1.0,
        -1.0, 1.0,
        1.0, 1.0,
        1.0, -1.0
    ];
    gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(vertices), gl.STATIC_DRAW);
    vertexBuffer.itemSize = 2;
    vertexBuffer.numItems = 4;
}
function loadShaders(shader) {
    loadShaderText(shader,'.vs');
    loadShaderText(shader,'.fs');
}
function loadShaderText(filename,ext) {
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
    };
    xhttp.open("GET", filename+ext, true);
    xhttp.send();
}
function initShaders(vShaderTxt,fShaderTxt) {
    var vshader = gl.createShader(gl.VERTEX_SHADER);
    gl.shaderSource(vshader, vShaderTxt);
    gl.compileShader(vshader);
    if (!gl.getShaderParameter(vshader, gl.COMPILE_STATUS)) {
        alert(gl.getShaderInfoLog(vshader));
        return null;
    }

    var fshader = gl.createShader(gl.FRAGMENT_SHADER);
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

    shaderProgram.screenSize = gl.getUniformLocation(shaderProgram, "screenSize");
    gl.uniform2f(shaderProgram.screenSize, canvas.width, canvas.height);

    initialiseComponent();

    shaderProgram.vertexPositionAttribute = gl.getAttribLocation(shaderProgram, "vertexPosition");
    gl.enableVertexAttribArray(shaderProgram.vertexPositionAttribute);

    shaderProgram.color = gl.getUniformLocation(shaderProgram, "cam_origin");

    drawScene();
}

function drawScene() {
    gl.clear(gl.COLOR_BUFFER_BIT);
    gl.bindBuffer(gl.ARRAY_BUFFER, vertexBuffer);
    gl.vertexAttribPointer(shaderProgram.vertexPositionAttribute,
        vertexBuffer.itemSize, gl.FLOAT, false, 0, 0);
    gl.drawArrays(gl.TRIANGLE_FAN, 0, vertexBuffer.numItems);
}




