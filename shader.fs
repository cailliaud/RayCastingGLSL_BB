#define M_PI 3.1415926535897932384626433832795
precision mediump float;


/*
Indexing of Arrays, Vectors and Matrices
Definition:
constant-index-expressions are a superset of constant-expressions. Constant-index-expressions can include loop indices as defined in Appendix A section 4.
The following are constant-index-expressions:
• Constant expressions
• Loop indices as defined in section 4
• Expressions composed of both of the above
When used as an index, a constant-index-expression must have integral type.
*/

//attribut du plan image
const float screenWidth = 	720.0;
const float screenHeight = 480.0;
const float focal = 50.0; //50.0

/**
* Algorythme Mode:
* 0: Japanese flag mode
* 1: Lambertian shading
* 2: Modified Phong shading
* 3: Microfacette shading
*/
int algorythm = 3;

// Information Sphère 
const int nbS = 4;
uniform vec3 sphereCenter;
uniform float sphereRadius;
uniform vec3 sphereColor;
uniform vec3 backgroundColor;


// Information lumière
uniform vec3 lumiereCenter;
uniform float lumierePuissance;
uniform float sphereKs;
uniform float sphereM;
uniform float sphereNi;


//#### RAYON ####
/**
* Defini un rayon.
* ori : son point d'origine
* v : sa direction
* t : distance en v avant impact
*/
struct Ray{
	vec3 ori;
	vec3 v;
	float t;
};
	
//#### LIGHT ####	
/**
* Defini une source de lumiere.
* c : son centre
* p : sa puissance (r/g/b)
*/
struct Light{
		vec3 c;	//Center of the light
		float p;	//Power of the light (spectrum)
	};

//#### 	MATERIAU ####
/**
* Kd : couleur du matériau
* Ks : valeur de la composante de specularité
* ni : indice de refractance du matériau (0 et 3.0)
* m : indice de rugosité du matériau (0 et 1.5)
*/
struct Materiau {
	vec3 Kd;	
	float Ks;
	// ni est l'indice de refractance
	float ni;
	// m est la rugosité (utilisé comme étant n pr phong)
	float m;
};


//#### SPHERE ####
/**
* Defini une sphere
* c : son centre
* r : son rayon
* mat : @see MyMaterial
*/
struct Sphere{
		vec3 c;
		float r;
		Materiau mat;
	};
	
//#### PLAN ####
/**
* Definition d'un plan
* Ax + By + Cz + D = 0
* norm : normal du plan
* h : hauteur
*/
struct Plan{
	vec3 norm;
	float h;

};

//############### Intersection algorythm ##############
/**
* Fonction qui test l'impact entre une sphere et un rayon
* @param Sphere s: sphere a tester
* @param Ray r: rayon lancé
*/
float intersectSphere(Sphere s, Ray r){
	//"RayCasting"
	vec3 o = r.ori - s.c;
	float a = dot(r.v,r.v);
	float b = 2.0*dot(o,r.v);
	float c = dot(o,o) - s.r*s.r;
	float delta = b*b -(4.0*a*c);
	
	//Selection de T selon DELTA
	if(delta < 0.0){
		return -1.0;
	}else{
		if(delta == 0.0){
			return -b/2.0*a;
		}else{
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
}

/**
* Permet de lancer le rayon pour chaque sphere @see intersectSphere();
*/
bool intersection(Sphere[nbS] spheres, Ray r, out float tMin, out Sphere s){
	float tAux;
	tMin = -1.0;
	for (int i = 0; i < nbS; i++){
		tAux = intersectSphere(spheres[i], r);
		if (tMin == -1.0 && tAux > 0.0 || tAux > 0.0 && tAux < tMin){
			tMin = tAux;
			s = spheres[i];
		}
	}
	
	return tMin>=0.0;
}

/**
* Fonction Intersection Plan 
*/ 
bool intersectPlan(Plan p , Ray r, out float tMin){
	 //"RayCasting"
	 tMin = -(dot(p.norm,r.ori)+p.h)/(dot(p.norm,r.v));
     return tMin>=0.0;

}


//############### ALGORYTHM 1 ##############
//############# Lambertian Shading for Sphere  ########
void lambertianShading(Light light, vec3 n, vec3 vi, Sphere sphereHit){
	float theta = dot(vi, n);
	if(theta > 0.0){ 	//Couleur coté ECLAIRE de la sphere
		gl_FragColor = vec4(light.p*sphereHit.mat.Kd/M_PI*theta,1.0);
	}
	else{ 				//Couleur coté SOMBRE de la sphere
		gl_FragColor = vec4(0.0,0.0,0.0,1.0);
	}
}


//############### ALGORYTHM 2 ##############
//############# Phong Shading  #############
void phong(Light light, Sphere sphere ,  vec3 Vi ,  vec3 V0 ,  vec3 Normale){
    float costeta,cosalpha ;
    vec3 h;
	costeta =  dot(Vi,Normale);
	if(costeta < 0.0) {
		costeta = 0.0;
	}
	h= normalize(Vi+V0);
	cosalpha = dot(Normale,h);
	if (cosalpha <0.0) {
		cosalpha= 0.0;
	}
	vec3 RGB = vec3(  (sphere.mat.Kd/M_PI) + ( (sphere.mat.Ks*(2.0+sphere.mat.m) /(2.0*M_PI) )*(pow(cosalpha,sphere.mat.m) )) * costeta * light.p );

	gl_FragColor = vec4(RGB,1.0);
}

//############### ALGORYTHM 3 ##############
//############# MicroFacettes  #############

/**
*Fonction pour obtenir  G :l'ombrage et le masque d'une facette 
* Voir page 5 sur 18 de la publication de Cook-Torrance de 1982 BRDF
* L = Vi (light)
* V = V0 (viewer)
*/
float getG (vec3 n, vec3 Vi, vec3 V0){
	vec3 h = normalize (Vi+V0);
	float G = min(1.0,min(2.0*dot(n,h)*dot(n,V0)/dot(V0,h),2.0*dot(n,h)*dot(n,Vi)/dot(V0,h) ) );
	return (G);
}



/**
* Fonction pour obtenir la distribution de Beckmann
* voir Page 6 sur 18 de la publication de Cook-Torrance de 1982 BRDF
*/
float getDistribution (vec3 n, vec3 Vi, vec3 V0, Sphere s){
	vec3 h = normalize (Vi+V0);
	float cosalpha = dot(n,h);
	float D = (1.0 / s.mat.m*pow(cosalpha,4.0)) * exp(-pow((tan(acos(cosalpha))/s.mat.m),2.0));
	return (D);
	}
	
/**
* Fonction pour obtenir Fresnel 
*/
float getFresnel (vec3 n, vec3 Vi, vec3 V0, Sphere s){
	vec3 h = normalize (Vi+V0);
	float c = dot(V0,h);
	float g = sqrt( pow(s.mat.ni,2.0) + pow(c,2.0) - 1.0 );
	
	float F = 0.5*(pow((g-c),2.0))/(pow((g+c),2.0))*(1.0+((pow(c*(g+c)-1.0,2.0))/(pow(c*(g-c)+1.0,2.0))));
	return (F);
}

/**
* Main microfacete
*/
void microFacette (Light Li, vec3 n, vec3 Vi, vec3 V0, Sphere s){
	float costeta =  dot(Vi,n);
	float D  =  getDistribution ( n,  Vi,  V0,  s);
	float F =  getFresnel ( n,  Vi,  V0,  s);
	float G = getG ( n,  Vi,  V0);
	vec3 RGB = vec3( Li.p * ((s.mat.Kd/M_PI) + (D*F*G / 4.0 * dot(Vi,n)*dot(V0,n)) ) * costeta );
	 gl_FragColor = vec4(RGB,1.0);
}

//############### ALGORYTHM PLan ##############
//############# Lambertian Shading for Plan  ########
void lambertianShadingPlan(Light light, vec3 n, vec3 vi, vec3 Kd){
	float theta = dot(vi, n);
	if(theta > 0.0){ 	//Couleur coté ECLAIRE de la sphere
		gl_FragColor = vec4(light.p*Kd/M_PI*theta,1.0);
	}
	else{ 				//Couleur coté SOMBRE de la sphere
		gl_FragColor = vec4(0.0,0.0,0.0,1.0);
	}
}
	
void main(void){

	//#### DEFINITION DE LA LUMIERE ####
	Light light;
	light.c =lumiereCenter;
	light.p = lumierePuissance;
	
	
	
	//#### DEFINITION DES SPHERES
	
	Sphere spheres[nbS];
	
	//Sphere principale
	Sphere sphere;
	sphere.c = sphereCenter;
	sphere.r = sphereRadius;
	sphere.mat.Kd = sphereColor;
	sphere.mat.Ks = sphereKs;
	sphere.mat.m = sphereM;
	sphere.mat.ni = sphereNi;
	
	Sphere sphere2;
	sphere2.c = sphere.c +vec3(-5.0,-5.0,-15.0);
	sphere2.r = sphere.r;
	sphere2.mat.Kd = sphere.mat.Kd * vec3(0.0,0.0,1.0);
	sphere2.mat.Ks = sphere.mat.Ks;
	sphere2.mat.ni = sphere.mat.ni;
	sphere2.mat.m = sphere.mat.m;

	Sphere sphere3;
	sphere3.c = sphere.c + vec3(13.0, 13.0, +25.0);
	sphere3.r = sphere.r;
	sphere3.mat.Kd = sphere.mat.Kd * vec3(0.0,0.5,0.0);
	sphere3.mat.Ks = sphere.mat.Ks;
	sphere3.mat.ni = sphere.mat.ni;
	sphere3.mat.m = sphere.mat.m;
	
	Sphere sphere4;
	sphere4.c = sphere.c + vec3(-35.0, 5.0, 7.0);
	sphere4.r = sphere.r;
	sphere4.mat.Kd = sphere.mat.Kd * vec3(1.0,1.0,0.0);
	sphere4.mat.Ks = sphere.mat.Ks;
	sphere4.mat.ni = sphere.mat.ni;
	sphere4.mat.m = sphere.mat.m;
	
	spheres[0] = sphere;	
	spheres[1] = sphere2;
	spheres[2] = sphere3;	
	spheres[3] = sphere4;
	
	//#### DEFINITION DU RAYON
	Ray ray;
	ray.ori = vec3(0, 0, 0);


	ray.v = normalize(
				vec3(
			18.0*(gl_FragCoord.x-screenWidth/2.0)/(screenWidth/2.0),
			12.0*(gl_FragCoord.y-screenHeight/2.0)/(screenHeight/2.0),
			focal
			
			)
		);
		
	//####DEFINTION DU PLAN
	Plan plan;
	plan.norm = normalize(vec3(0.0,1.0,0.0));
	plan.h = 20.0;

	
		
	//#### DEBUT DU RAY CASTING ####
	Sphere sphereHit;
	float tPlan,tSphere;
	bool bSphere = intersection(spheres, ray, tSphere, sphereHit);
	bool bPlan= intersectPlan(plan,ray, tPlan);
	vec3 i = vec3(-1.0);
	if (bSphere && tSphere<tPlan || bSphere && !bPlan){
		i = (ray.v * tSphere) + ray.ori; 
		// Calcul des vecteurs à l'impact
		vec3 vi = normalize(light.c - i); 		//VECTEUR VERS LA LUMIERE
		vec3 n = normalize(i - sphereHit.c); 	//VECTEUR NORMAL A LA SPHERE
		vec3 v0 = normalize(ray.ori-i); // Vecteur vers l'origine
	
		if(algorythm==0){gl_FragColor = vec4(sphereHit.mat.Kd,1.0);}
		if(algorythm==1){lambertianShading(light, n, vi, sphereHit);}
		if(algorythm==2){phong(light, sphereHit, vi, v0, n);}
		if(algorythm==3){microFacette (light, n, vi, v0, sphereHit);}
	
	}
	else if (bPlan)  {
		i = (ray.v * tPlan) + ray.ori; 
		vec3 vi = normalize(light.c - i); 		
		vec3 n = normalize( plan.norm); 	
		vec3 v0 = normalize(ray.ori-i); 
		if (mod(abs(i.x-20.0),20.0)<= 10.0 && mod(abs(i.z-0.0),20.0)<= 10.0) {
			lambertianShadingPlan( light,  n,  vi, vec3(0.1,0.1,0.3));
			// gl_FragColor = vec4(0.1,0.1,0.3,1.0);
			}
		else {
			lambertianShadingPlan( light,  n,  vi, vec3(0.1,0.1,0.5));
			// gl_FragColor = vec4(0.1,0.1,0.5,1.0);
		}
	}else {
		gl_FragColor = vec4(0.7,0.7,0.7,1.0);
	}
	
	
	vec3 vi = normalize(light.c - i);
	Ray toLight;
	toLight.v = vi;
	toLight.ori = i+vi;
	bool obstacle = intersection(spheres, toLight , tSphere, sphereHit);
	if(obstacle){
		gl_FragColor = vec4(0.0,0.0,0.0,1.0);
	}
	
	
		
	
	
	
 }