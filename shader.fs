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
uniform vec3 backgroundColor;


// Information lumière
uniform vec3 lumiereCenter;
uniform float lumierePuissance;

// #####Récupération Sphere 1 #####
uniform vec3 sphere1_Center;
uniform float sphere1_Radius;
uniform vec3 sphere1_Color;
uniform float sphere1_Ks;
uniform float sphere1_M;
uniform float sphere1_Ni;

// #####Récupération Sphere 2 #####
uniform vec3 sphere2_Center;
uniform float sphere2_Radius;
uniform vec3 sphere2_Color;
uniform float sphere2_Ks;
uniform float sphere2_M;
uniform float sphere2_Ni;


// #####Récupération Sphere 3 #####
uniform vec3 sphere3_Center;
uniform float sphere3_Radius;
uniform vec3 sphere3_Color;
uniform float sphere3_Ks;
uniform float sphere3_M;
uniform float sphere3_Ni;

// #####Récupération Sphere 4 #####
uniform vec3 sphere4_Center;
uniform float sphere4_Radius;
uniform vec3 sphere4_Color;
uniform float sphere4_Ks;
uniform float sphere4_M;
uniform float sphere4_Ni;


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
	int type;

};

//############### Intersection algorythm ##############
/**
* Fonction qui test l'impact entre une sphere et un rayon
* @param Sphere s: sphere à tester
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
* return -1.0 si pas d'intersection et out sphere sera null
*/
float intersectionAllSpheres(Sphere[nbS] spheres, Ray r,  out Sphere s){
	float tAux;
	float tMin = -1.0;
	for (int i = 0; i < nbS; i++){
		tAux = intersectSphere(spheres[i], r);
		if (tMin == -1.0 && tAux > 0.0 || tAux > 0.0 && tAux < tMin){
			tMin = tAux;
			s = spheres[i];
		}
	}
	return tMin;
	
}

/**
* Fonction Intersection Plan 
* return -1.0 si pas d'intersection
*/ 
float intersectPlan(Plan p , Ray r){
	 float t = -(dot(p.norm,r.ori)+p.h)/(dot(p.norm,r.v));
	 if (t<0.0) {return -1.0;}
	 else{
	 return t;}


}

float intersectionAllPlan (Plan[nbS] plans, Ray r,  out Plan p){
	float tAux;
	float tMin = -1.0;
	for (int i = 0; i < nbS; i++){
		tAux = intersectPlan(plans[i], r);
		if (tMin == -1.0 && tAux > 0.0 || tAux > 0.0 && tAux < tMin){
			tMin = tAux;
			p = plans[i];
		}
	}
	return tMin;
	
}		

//############### ALGORYTHM 1 ##############
//############# Lambertian Shading for Sphere  ########
vec4 lambertianShading(Light light, vec3 n, vec3 vi, Sphere sphereHit){
	float theta = dot(vi, n);
	if(theta > 0.0){ 	//Couleur coté ECLAIRE de la sphere
		return vec4(light.p*sphereHit.mat.Kd/M_PI*theta,1.0);
	}
	else{ 				//Couleur coté SOMBRE de la sphere
		return vec4(0.0,0.0,0.0,1.0);
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
vec4 microFacette (Light Li, vec3 n, vec3 Vi, vec3 V0, Sphere s){
	float costeta =  dot(Vi,n);
	float D  =  getDistribution ( n,  Vi,  V0,  s);
	float F =  getFresnel ( n,  Vi,  V0,  s);
	float G = getG ( n,  Vi,  V0);
	vec3 RGB = vec3( Li.p * ((s.mat.Kd/M_PI) + (D*F*G / 4.0 * dot(Vi,n)*dot(V0,n)) ) * costeta );
	return vec4(RGB,1.0);
}

//############### ALGORYTHM PLan ##############
//############# Lambertian Shading for Plan  ########
vec4 lambertianShadingPlan(Light light, vec3 n, vec3 vi, vec3 Kd){
	float theta = dot(vi, n);
	if(theta > 0.0){ 	//Couleur coté ECLAIRE de la sphere
		return vec4(light.p*Kd/M_PI*theta,1.0);
	}
	else{ 				//Couleur coté SOMBRE de la sphere
		return vec4(0.0,0.0,0.0,1.0);
	}
}
	


/**
* Fonction Intersection Scene
*/
float intersectScene (Sphere[nbS] spheres, Plan[nbS] plans , Ray r ,Light light, out vec4 BRDF ){
	float tPlan;
	float tSphere;
	float  tMin;
	Sphere sphereHit;
	Plan planHit;
	vec3 i;
	vec3 vi;
	vec3 n;
	vec3 vO;
	tSphere = intersectionAllSpheres(spheres, r, sphereHit);
	tPlan = intersectionAllPlan(plans,r,planHit);
	
	if ((tSphere>-1.0 && tPlan==-1.0) || (tSphere > -1.0 && tPlan > -1.0 && tSphere<tPlan)  ){
		i = (r.v * tSphere) + r.ori; 
		// Calcul des vecteurs à l'impact
		vi = normalize(light.c - i); 	
		n = normalize(i - sphereHit.c); 	
		vO = normalize(r.ori-i); 
		BRDF= microFacette(light, n, vi, vO, sphereHit);
		return tSphere;
	}
	if ((tSphere > -1.0 && tPlan > -1.0  && tSphere>tPlan) || (tSphere==-1.0 && tPlan>-1.0) ){
		i = (r.v * tPlan) + r.ori; 
		 vi = normalize(light.c - i); 		
		 n = normalize( planHit.norm); 	
		 vO = normalize(r.ori-i); 
		
		bool testDamier;
		if (planHit.type ==1){testDamier	=mod(floor(i.x/10.0) + floor (i.z/10.0), 2.0) == 0.0;}
		if (planHit.type ==2){testDamier	=mod(floor(i.x/10.0) + floor (i.y/10.0), 2.0) == 0.0;}
		if (planHit.type ==3){testDamier	=mod(floor(i.y/10.0) + floor (i.z/10.0), 2.0) == 0.0;}
		if (testDamier ){
			BRDF=lambertianShadingPlan( light,  n,  vi, vec3(0.1,0.1,0.2));
			
			}
		else {
			BRDF=lambertianShadingPlan( light,  n,  vi, vec3(0.1,0.1,0.3));
			
		}
		return tPlan;
	}
	
	BRDF= vec4(0.7,0.7,0.7,1.0);
	return -1.0;
		
}
			

void shadow (float t, Ray ray, Sphere[nbS] spheres, Light light ){
	Ray toLight;
	vec3 point_i;
	vec3 vi;
	float tMin;
	Sphere sphereHit;
	
	point_i = (ray.v * t) + ray.ori; 
	vi = normalize(light.c - point_i);
	toLight.v = vi;
	toLight.ori = point_i+vi; 
	tMin = intersectionAllSpheres(spheres, toLight , sphereHit);
	if(tMin>-1.0){
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
	sphere.c = sphere1_Center;
	sphere.r = sphere1_Radius;
	sphere.mat.Kd = sphere1_Color;
	sphere.mat.Ks = sphere1_Ks;
	sphere.mat.m = sphere1_M;
	sphere.mat.ni = sphere1_Ni;
	
	Sphere sphere2;
	sphere2.c = sphere2_Center;
	sphere2.r = sphere2_Radius;
	sphere2.mat.Kd = sphere2_Color;
	sphere2.mat.Ks = sphere2_Ks;
	sphere2.mat.m = sphere2_M;
	sphere2.mat.ni = sphere2_Ni;

	Sphere sphere3;
	sphere3.c = sphere3_Center;
	sphere3.r = sphere3_Radius;
	sphere3.mat.Kd = sphere3_Color;
	sphere3.mat.Ks = sphere3_Ks;
	sphere3.mat.m = sphere3_M;
	sphere3.mat.ni = sphere3_Ni;
	
	Sphere sphere4;
	sphere4.c = sphere4_Center;
	sphere4.r = sphere4_Radius;
	sphere4.mat.Kd = sphere4_Color;
	sphere4.mat.Ks = sphere4_Ks;
	sphere4.mat.m = sphere4_M;
	sphere4.mat.ni = sphere4_Ni;
	
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
		
	//####DEFINTION DES PLANS
	
	// planché
	Plan plans[nbS];
	Plan plan;
	plan.norm = normalize(vec3(0.0,1.0,0.0));
	plan.h = 20.0;
	plan.type= 1;

	// mur au fond
	Plan plan2;
	plan2.norm = normalize(vec3(0.0,0.0,-1.0));
	plan2.h = 600.0;
	plan2.type =2;
	
	// mur de droite
	Plan plan3;
	plan3.norm = normalize(vec3(-1.0,0.0,0.0));
	plan3.h = 100.0;
	plan3.type =3;
	
	// mur de gauche
	Plan plan4;
	plan4.norm = normalize(vec3(1.0,0.0,0.0));
	plan4.h = 100.0;
	plan4.type =3;
	
	plans[0] = plan;	
	plans[1] = plan2;
	plans[2] = plan3;	
	plans[3] = plan4;
		
	
	
	vec4 BRDF;
	float tMin ;
	tMin = intersectScene ( spheres,  plans , ray ,light,  BRDF );
	gl_FragColor = BRDF;
	shadow (tMin, ray, spheres,  light );
	
	
		
	
	
	
 }