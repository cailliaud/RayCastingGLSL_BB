#define M_PI 3.1415926535897932384626433832795
precision mediump float;
//Uniform for the cam.
uniform vec3 cam_origin;
uniform float cam_focal;
uniform float cam_width;
uniform float cam_height;
uniform mat4 cam_rotation;
uniform vec3 cam_direction;
uniform vec2 screenSize;

//Uniform for LIGHT_1.
uniform vec3 light1_center;
uniform vec3 light1_power;
//
//Uniform for LIGHT_2.
uniform vec3 light2_center;
uniform vec3 light2_power;
//
//Uniform for LIGHT_3.
uniform vec3 light3_center;
uniform vec3 light3_power;
//
//Uniform for SPHERE_1.
uniform vec3 sphere1_center;
uniform float sphere1_radius;
uniform vec3 sphere1_kd;
uniform float sphere1_ks;
uniform float sphere1_m;
uniform float sphere1_ni;
//
//Uniform for SPHERE_2.
uniform vec3 sphere2_center;
uniform float sphere2_radius;
uniform vec3 sphere2_kd;
uniform float sphere2_ks;
uniform float sphere2_m;
uniform float sphere2_ni;
//
//Uniform for SPHERE_3.
uniform vec3 sphere3_center;
uniform float sphere3_radius;
uniform vec3 sphere3_kd;
uniform float sphere3_ks;
uniform float sphere3_m;
uniform float sphere3_ni;
//
//Uniform for SPHERE_4.
uniform vec3 sphere4_center;
uniform float sphere4_radius;
uniform vec3 sphere4_kd;
uniform float sphere4_ks;
uniform float sphere4_m;
uniform float sphere4_ni;
//
//Uniform for SPHERE_5.
uniform vec3 sphere5_center;
uniform float sphere5_radius;
uniform vec3 sphere5_kd;
uniform float sphere5_ks;
uniform float sphere5_m;
uniform float sphere5_ni;
//
//Uniform for PLAN_1.
uniform vec3 plan1_normal;
uniform float plan1_shift;
uniform vec3 plan1_kd;
uniform float plan1_ks;
uniform float plan1_m;
uniform float plan1_ni;
//
//Uniform for PLAN_2.
uniform vec3 plan2_normal;
uniform float plan2_shift;
uniform vec3 plan2_kd;
uniform float plan2_ks;
uniform float plan2_m;
uniform float plan2_ni;
//
//Uniform for PLAN_3.
uniform vec3 plan3_normal;
uniform float plan3_shift;
uniform vec3 plan3_kd;
uniform float plan3_ks;
uniform float plan3_m;
uniform float plan3_ni;
//
//Uniform for PLAN_4.
uniform vec3 plan4_normal;
uniform float plan4_shift;
uniform vec3 plan4_kd;
uniform float plan4_ks;
uniform float plan4_m;
uniform float plan4_ni;
//
//Uniform for PLAN_5.
uniform vec3 plan5_normal;
uniform float plan5_shift;
uniform vec3 plan5_kd;
uniform float plan5_ks;
uniform float plan5_m;
uniform float plan5_ni;
//


const int NB_LIGHT = 3;
const int NB_SPHERE = 20;
const int NB_PLAN = 5;


//DEFINITION des STRUCT
struct Ray {
    vec3 O;
    vec3 V;
};

//
struct Material {
    vec3 Kd;
    float Ks;
    float m;
    float ni;
    bool damier;
    float mirror;
};
//
struct Light {
    vec3 O;
    vec3 lum;
};
//
struct Sphere {
    vec3 C;
    float r;
    Material mat;
};
//
struct Plan {
    vec3 N;
    float d;
    Material mat;
};
struct Intersect {
    float t;
    vec3 N;
    Material mat;
};

Light lights[NB_LIGHT];
Sphere spheres[NB_SPHERE];
Plan plans[NB_PLAN];

void createScene(){
    Light light1;
    light1.O = light1_center;
    light1.lum = light1_power;

    Light light2;
    light2.O = light2_center;
    light2.lum = light2_power;

    Light light3;
    light3.O = light3_center;
    light3.lum = light3_power;

    Sphere sphere1;
    sphere1.C = sphere1_center;
    sphere1.r = sphere1_radius;
    Material s1mat;
    s1mat.Kd = sphere1_kd;
    s1mat.Ks = sphere1_ks;
    s1mat.m = sphere1_m;
    s1mat.ni = sphere1_ni;
    s1mat.damier = false;
    sphere1.mat = s1mat;

    Sphere sphere2;
    sphere2.C = sphere2_center;
    sphere2.r = sphere2_radius;
    Material s2mat;
    s2mat.Kd = sphere2_kd;
    s2mat.Ks = sphere2_ks;
    s2mat.m = sphere2_m;
    s2mat.ni = sphere2_ni;
    s2mat.damier = false;
    sphere2.mat = s2mat;

    Sphere sphere3;
    sphere3.C = sphere3_center;
    sphere3.r = sphere3_radius;
    Material s3mat;
    s3mat.Kd = sphere3_kd;
    s3mat.Ks = sphere3_ks;
    s3mat.m = sphere3_m;
    s3mat.ni = sphere3_ni;
    s3mat.damier = false;
    sphere3.mat = s3mat;

    Sphere sphere4;
    sphere4.C = sphere4_center;
    sphere4.r = sphere4_radius;
    Material s4mat;
    s4mat.Kd = sphere4_kd;
    s4mat.Ks = sphere4_ks;
    s4mat.m = sphere4_m;
    s4mat.ni = sphere4_ni;
    s4mat.damier = false;
    sphere4.mat = s4mat;

    Sphere sphere5;
    sphere5.C = sphere5_center;
    sphere5.r = sphere5_radius;
    Material s5mat;
    s5mat.Kd = sphere5_kd;
    s5mat.Ks = sphere5_ks;
    s5mat.m = sphere5_m;
    s5mat.ni = sphere5_ni;
    s5mat.mirror = 0.2;
    s5mat.damier = false;
    sphere5.mat = s5mat;

    Plan plan1;
    plan1.N = plan1_normal;
    plan1.d = plan1_shift;
    Material p1mat;
    p1mat.Kd = plan1_kd;
    p1mat.Ks = plan1_ks;
    p1mat.m = plan1_m;
    p1mat.ni = plan1_ni;
    p1mat.mirror = 0.0;
    p1mat.damier = true;
    plan1.mat = p1mat;

    Plan plan2;
    plan2.N = plan2_normal;
    plan2.d = plan2_shift;
    Material p2mat;
    p2mat.Kd = plan2_kd;
    p2mat.Ks = plan2_ks;
    p2mat.m = plan2_m;
    p2mat.ni = plan2_ni;
    p2mat.mirror = 0.0;
    p2mat.damier = false;
    plan2.mat = p2mat;

    Plan plan3;
    plan3.N = plan3_normal;
    plan3.d = plan3_shift;
    Material p3mat;
    p3mat.Kd = plan3_kd;
    p3mat.Ks = plan3_ks;
    p3mat.m = plan3_m;
    p3mat.ni = plan3_ni;
    p3mat.mirror = 0.0;
    p3mat.damier = false;
    plan3.mat = p3mat;

    Plan plan4;
    plan4.N = plan4_normal;
    plan4.d = plan4_shift;
    Material p4mat;
    p4mat.Kd = plan4_kd;
    p4mat.Ks = plan4_ks;
    p4mat.m = plan4_m;
    p4mat.ni = plan4_ni;
    p4mat.mirror = 0.0;
    p4mat.damier = false;
    plan4.mat = p4mat;

    Plan plan5;
    plan5.N = plan5_normal;
    plan5.d = plan5_shift;
    Material p5mat;
    p5mat.Kd = plan5_kd;
    p5mat.Ks = plan5_ks;
    p5mat.m = plan5_m;
    p5mat.ni = plan5_ni;
    p5mat.mirror = 0.5;
    p5mat.damier = false;
    plan5.mat = p5mat;

    lights[0] = light1;
    lights[1] = light2;
    lights[2] = light3;
    spheres[0] = sphere1;
    spheres[1] = sphere2;
    spheres[2] = sphere3;
    spheres[3] = sphere4;
    spheres[4] = sphere5;
    plans[0] = plan1;
    plans[1] = plan2;
    plans[2] = plan3;
    plans[3] = plan4;
    plans[4] = plan5;
	
	for (int i = 5 ; i< 20 ; i++){
		spheres[i]= Sphere((vec3(30.0,67.0,300.0)/float(i)),20.0/float(i),sphere2.mat);
	}
}



//*****************************Maintenant c'est les fonctions*****************************
/**
* intersectionSphere:
*   Calcule l'intersection entre la Sphere s et le Rayon r
*   et retourne le t min.
*/
float intersectionSphere(Sphere s, Ray r) {
	vec3 o = r.O - s.C;
	float a = dot(r.V,r.V);
	float b = 2.0*dot(r.V,o);
	float c = dot(o,o) - s.r*s.r;
	float delta = b*b -(4.0*a*c);

    if(delta < 0.0){
        return -1.0;
    }else {
        float t1 = (-b-sqrt(delta))/(2.0*a);
        float t2 = (-b+sqrt(delta))/(2.0*a);
        if(t1 < 0.0){
            return t2;
        }
        if(t2 < 0.0){
            return t1;
        }
        return min(t1,t2);
    }
}

/**
* intersectionPlan:
*   Calcule l'intersection entre le plan p et le Rayon r
*   et retourne le t min.
*/

float intersectionPlan(Plan p , Ray r){
     return -(dot(p.N,r.O)+p.d)/(dot(p.N,r.V));
}

/**
* throw(ray):
*   Lance le rayon passé en parametre et retourne "l'impact" dans la scene
*/
Intersect throw(Ray r) {
    float tAux;
    Intersect inter;
    inter.t = -1.0;
    for(int i = 0; i < NB_SPHERE; i++){
        tAux = intersectionSphere(spheres[i], r);
        if (tAux > 0.0 && tAux < inter.t || inter.t == -1.0 && tAux != -1.0 && tAux > 0.0){
            inter.t = tAux;
            inter.mat = spheres[i].mat;
            inter.N = normalize(((r.V * tAux) + r.O) - spheres[i].C); //VECTEUR NORMAL (impact - centre sphere)
        }
    }
    for(int i = 0; i < NB_PLAN; i++){
        tAux = intersectionPlan(plans[i], r);
        if (tAux > 0.0 && tAux < inter.t || inter.t == -1.0 && tAux != -1.0 && tAux > 0.0){
            inter.t = tAux;
            inter.mat = plans[i].mat;
            inter.N = plans[i].N; //VECTEUR NORMAL le meme que le plan
        }
    }
    return inter;
}

/***********************************************************************************************
******************************** Cook-Torrance modele *******************************************
************************************************************************************************/
float getG (vec3 n, vec3 Vi, vec3 V0){
	vec3 h = normalize (Vi+V0);
	float G = min(1.0,min(2.0*dot(n,h)*dot(n,V0)/dot(V0,h),2.0*dot(n,h)*dot(n,Vi)/dot(V0,h) ) );
	return (G);
}
float getDistribution (vec3 n, vec3 Vi, vec3 V0, Material mat){
	vec3 h = normalize (Vi+V0);
	float cosalpha = dot(n,h);
	float D = (1.0 / mat.m*pow(cosalpha,4.0)) * exp(-pow((tan(acos(cosalpha))/mat.m),2.0));
	return (D);
	}
float getFresnel (vec3 n, vec3 Vi, vec3 V0, Material mat){
	vec3 h = normalize (Vi+V0);
	float c = dot(V0,h);
	float g = sqrt( pow(mat.ni,2.0) + pow(c,2.0) - 1.0 );
	float F = 0.5*(pow((g-c),2.0))/(pow((g+c),2.0))*(1.0+((pow(c*(g+c)-1.0,2.0))/(pow(c*(g-c)+1.0,2.0))));
	return (F);
}
//Retourne la BRDF
vec3 microFacette(Light Li, vec3 n, vec3 Vi, vec3 V0, Material mat){
	float costeta =  dot(Vi,n);
	float D  = getDistribution ( n,  Vi,  V0,  mat);
	float F = getFresnel ( n,  Vi,  V0,  mat);
	float G = getG ( n,  Vi,  V0);
	return vec3( Li.lum * ((mat.Kd/M_PI) + (D*F*G / 4.0 * dot(Vi,n)*dot(V0,n)) ) * costeta );
}

/**
* damierBRDF
*   Calcule la couleur en un point donne par rapport a une lumiere, en tenant compte de l'orientation du plan.
*/
vec3 damierBRDF(vec3 i, Light light, Intersect impact, vec3 vi, vec3 v0){
    if(!(impact.mat.mirror > 0.0)){
        if (impact.N.x != 0.0){
            if( mod( floor(i.y/10.0)+floor(i.z/10.0),2.0) == 0.0 ){
                Material blank = impact.mat;
                blank.Kd = vec3(1.0,1.0,1.0);
                return microFacette(light, impact.N , vi, v0, blank);
            }else{
                return microFacette(light, impact.N , vi, v0, impact.mat);
            }
        }else{
            if(impact.N.y != 0.0){
                if( mod( floor(i.x/10.0)+floor(i.z/10.0),2.0) == 0.0 ){
                    Material blank = impact.mat;
                    blank.Kd = impact.mat.Kd*0.8;
                    return microFacette(light, impact.N , vi, v0, blank);
                }else{
                    return microFacette(light, impact.N , vi, v0, impact.mat);
                }
            }else{
                if( mod( floor(i.x/10.0)+floor(i.y/10.0),2.0) == 0.0 ){
                    Material blank = impact.mat;
                    blank.Kd = impact.mat.Kd*0.2;
                    return microFacette(light, impact.N , vi, v0, blank);
                }else{
                    return microFacette(light, impact.N , vi, v0, impact.mat);
                }
            }
        }
    }
}

/**
* blendLights
*   a un point d'impact donne, calcule la somme des BRDF en tenant compte des expositions de chaque lumiere
*/
vec3 blendLights(Intersect impact, Ray r){
    if(impact.t > 0.0){ //t doit etre positif et !=1.0
        Intersect isObscuration;
        vec3 i = r.O + r.V*impact.t;
        vec3 BRDF = vec3(0.0);
        for(int l = 0; l < NB_LIGHT; l++){
            vec3 vi = lights[l].O-i;//vecteur vers la lumière
            Ray toLight;
            toLight.V = normalize(vi);
            toLight.O = i + normalize(vi)/10.0;
            isObscuration = throw(toLight);
            if(isObscuration.t < 0.0 ||     // Si le rayon vers la lumière ne touche aucun élément (< 0.0 ou == -1.0)
            length(isObscuration.t*toLight.V) > length(lights[l].O-toLight.O)){// OU si l'element touché est derriere la lumière
                vec3 v0 = normalize(r.O-i); //Vecteur vers l'origine
                vi = normalize(vi);
                if(impact.mat.damier == true ){
                    BRDF += damierBRDF(i, lights[l], impact, vi, v0);
                }else{
                    BRDF += microFacette(lights[l], impact.N , vi, v0, impact.mat);
                }
            }
        }
    return BRDF;
    }else {
        return vec3(0.8); //Couleur du fond.
    }
}

/**
* reflectedBRDF
*   utilisé lors d'un impact avec un materiau de type mirroir
*/
vec3 reflectedBRDF(Ray ray, Intersect impact){
    Ray newRay;
    newRay.V = reflect(ray.V ,impact.N);
    newRay.O = impact.t*ray.V + ray.O+newRay.V/1000.0;
    Intersect impact2 = throw(newRay);
    return blendLights(impact2, newRay);
}

void main(void){
    //creation de la scene
    createScene();

    //creation du rayon
    Ray ray;
    ray.V =
    normalize(
    vec3(
        cam_width*(gl_FragCoord.x-screenSize.x/2.0)/(screenSize.x/2.0),
        cam_height*(gl_FragCoord.y-screenSize.y/2.0)/(screenSize.y/2.0),
        cam_focal
        )
    );
    ray.O = cam_origin;

    //application des matrices de rotations
    ray.V = (cam_rotation * vec4(ray.V,1.0) ).xyz;

    //lancement du rayon et retour d'un élément intersect au point d'impact
    Intersect impact = throw(ray);
    vec3 BRDF = blendLights(impact, ray);

    //Gestion reflectance
    if (impact.mat.mirror > 0.0){
        vec3 BRDF2;
        BRDF2 = reflectedBRDF(ray, impact);
        BRDF = ( (1.0-impact.mat.mirror)*BRDF + impact.mat.mirror*BRDF2);
    }

    //Voila
    gl_FragColor = vec4(BRDF,1.0);
}
