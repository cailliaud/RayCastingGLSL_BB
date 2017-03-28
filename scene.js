/**
 * Created by ambie on 09/03/2017.
 */
var SPHERE_NB = 0;
var PLAN_NB = 0;
var LIGHT_NB = 0;

function Scene() {
    this.spheres = [];
    this.plans = [];
    this.lights = [];
    this.mainCam = new Cam();
}

 /* Definition of the camera
 * origin: origin of the camera
 * direction: direction pointing by the camera, facing Z by default
 * focal: the focal distance, 50 by default
 */
function Cam(){
    this.origin = vec3.create([0.0,0.0,0.0]);
    this.direction = vec3.create([0.0,0.0,1.0]);
    this.focal = 50.0;
    this.width = 36.0;

    this.rotation = mat4.create(this.rotation);
    // this.height = 18.0; set to 20.25 for 16:9 emulation
    this.height = 20.25;

    gl.useProgram(shaderProgram);
    this.originLocation = gl.getUniformLocation(shaderProgram, "cam_origin");
    this.focalLocation = gl.getUniformLocation(shaderProgram, "cam_focal");
    this.widthLocation = gl.getUniformLocation(shaderProgram, "cam_width");
    this.heightLocation = gl.getUniformLocation(shaderProgram, "cam_height");
    this.directionLocation = gl.getUniformLocation(shaderProgram, "cam_direction");
    this.rotationLocation = gl.getUniformLocation(shaderProgram, "cam_rotation");
    this.updateUniform = function () {
        gl.uniform3f(this.originLocation,this.origin[0],this.origin[1],this.origin[2]);
        gl.uniform1f(this.focalLocation,this.focal);
        gl.uniform1f(this.widthLocation,this.width);
        gl.uniform1f(this.heightLocation,this.height);
        gl.uniform3f(this.directionLocation,this.direction[0],this.direction[1],this.direction[2]);
        gl.uniformMatrix4fv(this.rotationLocation, false ,this.rotation);
    };
   
}

/**
 * The material is a subObject of any plan or sphere.
 * @param kd: a vec3 for the RGB color of the element.
 * @param ks: the specular part, from 0 to 1.
 * @param m: the roughness of the material
 * @param ni: the rate of microfacets not facing
 * @constructor
 */
function Material(kd, ks, m, ni) {
    this.kd = vec3.create(kd);
    this.ks = ks;
    this.m = m;
    this.ni = ni;
    this.createUniform = function (shape) {
        gl.useProgram(shaderProgram);
        if (shape.hasOwnProperty('center')){
            this.kdLocation = gl.getUniformLocation(shaderProgram, "sphere"+shape.id+"_kd");
            this.ksLocation = gl.getUniformLocation(shaderProgram, "sphere"+shape.id+"_ks");
            this.mLocation = gl.getUniformLocation(shaderProgram, "sphere"+shape.id+"_m");
            this.niLocation = gl.getUniformLocation(shaderProgram, "sphere"+shape.id+"_ni");
        }
        if (shape.hasOwnProperty('normal')){
            this.kdLocation = gl.getUniformLocation(shaderProgram, "plan"+shape.id+"_kd");
            this.ksLocation = gl.getUniformLocation(shaderProgram, "plan"+shape.id+"_ks");
            this.mLocation = gl.getUniformLocation(shaderProgram, "plan"+shape.id+"_m");
            this.niLocation = gl.getUniformLocation(shaderProgram, "plan"+shape.id+"_ni");
        }
    };
    this.updateUniform = function () {
        gl.uniform3f(this.kdLocation,this.kd[0],this.kd[1],this.kd[2]);
        gl.uniform1f(this.ksLocation,this.ks);
        gl.uniform1f(this.mLocation,this.m);
        gl.uniform1f(this.niLocation,this.ni);
    };
}


/**
 * Definition of a sphere.
 * center: a vec3 Float32array
 * @constructor
 */
function Sphere(center, radius, mat) {
    this.center = vec3.create(center);
    this.radius = radius;
    this.mat = mat;
    this.id = ++SPHERE_NB;

    gl.useProgram(shaderProgram);
    this.centerLocation = gl.getUniformLocation(shaderProgram, "sphere"+this.id+"_center");
    this.radiusLocation = gl.getUniformLocation(shaderProgram, "sphere"+this.id+"_radius");
    this.mat.createUniform(this);
    this.updateUniform = function () {
        gl.uniform3f(this.centerLocation,this.center[0],this.center[1],this.center[2]);
        gl.uniform1f(this.radiusLocation,this.radius);
        this.mat.updateUniform();
    };
    this.updateUniform();

    this.move = function (direction){
        this.center[0] += direction[0];
        this.center[1] += direction[1];
        this.center[2] += direction[2];
        gl.uniform3f(this.centerLocation,this.center[0],this.center[1],this.center[2]);
    }
}



/**
 * Definition for the plan.
 * normal: the normal of the plan, same as y axis by default
 * shift: the shift of the plan form the origin
 * @constructor
 */
function Plan(normal, shift, mat) {
    this.normal = vec3.create(normal);
    this.shift = shift;
    this.mat = mat;
    this.id = ++PLAN_NB;

    gl.useProgram(shaderProgram);
    this.normalLocation = gl.getUniformLocation(shaderProgram, "plan"+this.id+"_normal");
    this.shiftLocation = gl.getUniformLocation(shaderProgram, "plan"+this.id+"_shift");
    this.mat.createUniform(this);
    this.updateUniform = function () {
        gl.uniform3f(this.normalLocation, this.normal[0], this.normal[1], this.normal[2]);
        gl.uniform1f(this.shiftLocation, this.shift);
        this.mat.updateUniform();
    };
    this.updateUniform();
}

/**
 * Definition for the plan.
 * center: a vec3 Float32array
 * power: a vec3 Float32array for the RGB power (can be over 1)
 * @constructor
 */
function Light(pos,pow) {
    this.center = vec3.create(pos);
    this.power = vec3.create(pow);
    this.id = ++LIGHT_NB;

    gl.useProgram(shaderProgram);
    this.centerLocation = gl.getUniformLocation(shaderProgram, "light"+this.id+"_center");
    this.powerLocation = gl.getUniformLocation(shaderProgram, "light"+this.id+"_power");
    this.updateUniform = function () {
        gl.uniform3f(this.centerLocation, this.center[0], this.center[1], this.center[2]);
        gl.uniform3f(this.powerLocation, this.power[0], this.power[1], this.power[2]);
    };
    this.updateUniform();

    this.move = function (direction){
        this.center[0] += direction[0];
        this.center[1] += direction[1];
        this.center[2] += direction[2];
        gl.uniform3f(this.centerLocation,this.center[0],this.center[1],this.center[2]);
    }
}


