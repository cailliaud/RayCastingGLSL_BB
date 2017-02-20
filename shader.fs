#define M_PI 3.1415926535897932384626433832795

precision mediump float;

//attribut du plan image
const float screenWidth = 	720.0;
const float screenHeight = 480.0;
const float focal = 50.0;

// Information Sphère 
uniform vec3 sphereCenter;
uniform float sphereRadius;
uniform vec3 sphereColor;
uniform vec3 backgroundColor;
uniform int antialiasing;

// Information lumière
uniform vec3 lumiereCenter;
uniform float lumierePuissance;
uniform float sphereKs;
uniform float sphereN;


//#### RAYON ####
struct Ray{
	vec3 ori;
	float t;
	vec3 v;
};
	
//#### LIGHT ####	
struct Light{
		vec3 c;	//Center of the light
		float p;	//Power of the light (spectrum)
	};

//#### 	MATERIAU ####
struct Materiau {
	vec3 Kd;
	
	float Ks;
	// ni est l'indice de refractance
	float ni;
	// m est la rugosité (utilisé comme étant n pr phong)
	float m;
};

//#### SPHERE ####
struct Sphere{
		vec3 c;
		float r;
		Materiau mat;
	};
	


float getTMin(float a, float b, float c){
	float delta = b*b-(4.0*a*c);	
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

// Fonction couleur Lambert
void Lambert (float puissance, Sphere sphere, float theta){
	gl_FragColor = vec4(sphere.mat.Kd*puissance/M_PI*theta,1.0);
}

// Fonction couleur Phong
void Phong( Light light, Sphere sphere ,  vec3 Vi ,  vec3 V0 ,  vec3 Normale){
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

// Fonction pour obtenir  G :l'ombrage et le masque d'une facette 
// Voir page 5 sur 18 de la publication de Cook-Torrance de 1982 BRDF
// L = Vi (light)
// V = V0 (viewer)
float getG (vec3 n, vec3 Vi, vec3 V0){
	vec3 h = normalize (Vi+V0);
	float G = min(1.0,min(2.0*dot(n,h)*dot(n,V0)/dot(V0,h),2.0*dot(n,h)*dot(n,Vi)/dot(V0,h) ) );
	return (G);
}



// Fonction pour obtenir la distribution de Beckmann
// voir Page 6 sur 18 de la publication de Cook-Torrance de 1982 BRDF
float getDistribution (vec3 n, vec3 Vi, vec3 V0, Sphere s){
	vec3 h = normalize (Vi+V0);
	float cosalpha = dot(n,h);
	float D = (1.0 / s.mat.m*pow(cosalpha,4.0)) * exp(-pow((tan(acos(cosalpha))/s.mat.m),2.0));
	return (D);
	}
	
// Fonction pour obtenir Fresnel 
float getFresnel (vec3 n, vec3 Vi, vec3 V0, Sphere s){
	vec3 h = normalize (Vi+V0);
	float c = dot(V0,h);
	float g = sqrt( pow(s.mat.ni,2.0) + pow(c,2.0) - 1.0 );
	
	float F = 0.5*(pow((g-c),2.0))/(pow((g+c),2.0))*(1.0+((pow(c*(g+c)-1.0,2.0))/(pow(c*(g-c)+1.0,2.0))));
	return (F);
}

void microFacette (Light Li, vec3 n, vec3 Vi, vec3 V0, Sphere s){
	float costeta =  dot(Vi,n);
	float D  =  getDistribution ( n,  Vi,  V0,  s);
	float F =  getFresnel ( n,  Vi,  V0,  s);
	float G = getG ( n,  Vi,  V0);
	vec3 RGB = vec3( Li.p * ((s.mat.Kd/M_PI) + s.mat.Ks*(D*F*G / 4.0 * dot(Vi,n)*dot(V0,n)) ) * costeta );
	gl_FragColor = vec4(RGB,1.0);
}


	
void main(void){

	//#### DEFINITION DE LA LUMIERE ####
	Light light;
	light.c =lumiereCenter;
	light.p = lumierePuissance;
	
	
	//#### DEFINITION DE LA SPHERE
	Sphere sphere;
	sphere.c = sphereCenter;
	sphere.r = sphereRadius;
	sphere.mat.Kd = sphereColor;
	sphere.mat.Ks = sphereKs;
	sphere.mat.m = sphereN;
	sphere.mat.ni = 0.5;
	

	Ray ray;
	ray.ori = vec3(0, 0, 0);


	ray.v = normalize(
				vec3(
			18.0*(gl_FragCoord.x-screenWidth/2.0)/(screenWidth/2.0),
			12.0*(gl_FragCoord.y-screenHeight/2.0)/(screenHeight/2.0),
			focal
			
			)
		);
		
		
		
	//#### DEBUT DU RAY CASTING ####
	float a = ray.v.x*ray.v.x + ray.v.y*ray.v.y + ray.v.z*ray.v.z;
	float b = -2.0*((ray.v.x*sphere.c.x)+(ray.v.y*sphere.c.y)+(ray.v.z*sphere.c.z));
	float c = sphere.c.x*sphere.c.x + sphere.c.y*sphere.c.y + sphere.c.z*sphere.c.z - sphere.r*sphere.r;
	float delta = b*b-(4.0*a*c);
	if(delta < 0.0){
		gl_FragColor = vec4(backgroundColor,1.0);
	}else{
		ray.t = getTMin(a,b,c);
		vec3 i = (ray.v*ray.t)+ray.ori;
		vec3 vi = normalize(light.c-i); //VECTEUR VERS LA LUMIERE
		vec3 n = normalize(i-sphere.c); //VECTEUR NORMAL A LA SPHERE
		vec3 v0 = normalize((0.0,0.0,0.0,0.0)-i); // Vecteur vers l'origine
		float theta = dot(vi, n);
		if(ray.t > 0.0){
			if(theta > 0.0){
				//Lambert(light.p, sphere,theta);
				//Phong( light, sphere , vi , v0 , n);
				microFacette ( light,  n,  vi,  v0, sphere);
			}
			else{
				gl_FragColor = vec4(0.0,0.0,0.0,1.0);
			}
		}
	}
	
 }