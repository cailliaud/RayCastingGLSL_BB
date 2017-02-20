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


//#### SPHERE ####
struct Sphere{
		vec3 c;
		float r;
		vec3 Kd;
		float n;
		float Ks;
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
	gl_FragColor = vec4(sphere.Kd*puissance/M_PI*theta,1.0);
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
	vec3 RGB = vec3(  (sphere.Kd/M_PI) + ( (sphere.Ks*(2.0+sphere.n) /(2.0*M_PI) )*(pow(cosalpha,sphere.n) )) * costeta * light.p );
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
	sphere.Kd = sphereColor;
	sphere.Ks = sphereKs;
	sphere.n = sphereN;
	

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
				Phong( light, sphere , vi , v0 , n);
			}
			else{
				gl_FragColor = vec4(0.0,0.0,0.0,1.0);
			}
		}
	}
	
 }