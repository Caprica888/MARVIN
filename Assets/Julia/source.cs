using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class source : MonoBehaviour {

    private double airAttenuation = 0.001; //In inverse meters, 1 is made up someone change it

    private ParticleSystem sourceParticles;
    private Light sourceLight;
    
    private bool[] particleTypes = new bool[ 3 ]; //If the particle emits a particle then its index will be true. Order: Alpha, Beta, Gamma
    private double[] particleEnergies = new double[ 3 ]; //In keV

    public string radioNuke = "";
    public Color gammaColour;
    public Color betaColour;

    public double sourceActivity; //in mCi
    private double weight = 50; //Kg and it's a place holder

    private Boolean debug = true;

    // Use this for initialization
    void Start () {

       
        sourceParticles = GetComponentInChildren<ParticleSystem>();
        sourceLight = GetComponentInChildren<Light>();

        //Init arrays
        for ( int i = 0 ; i < particleTypes.Length ; i++ ) {

            particleTypes[i] = false;
            particleEnergies[i] = 0;

        }

        switch( radioNuke ) {

            case "Co-60":

                particleTypes[2] = true;
                particleEnergies[2] = 1332.5;

                break;

        }

        sourceParticles.enableEmission = false;
        sourceLight.enabled = false;

        if ( particleTypes[ 0 ] ) {

            sourceLight.enabled = true;
        
        }



        //Currently there's only one particle system so it can only display gamma or beta, rn gamma takes president
        if ( particleTypes[ 2 ] ) { 

            //Gamma
            //Travels at speed of light
            //Smallest

            sourceParticles.startSize = 0.01f; //Made up
            sourceParticles.startColor = gammaColour;

        }
        else if ( particleTypes[ 1 ] ) {

            //Beta
            //Electron
            //Bigger and faster but doesnt travel at speed of light

            sourceParticles.startSize = 0.05f; //Also made up
            sourceParticles.startColor = betaColour;

        }

        if ( particleTypes[1] || particleTypes[2] ) {

            sourceParticles.enableEmission = true;
            sourceParticles.Play();

        }

    }
	
	// Update is called once per frame
	void Update () {

        Vector3[] doseReceptors = findReceptorLocations();

        double activity = 0;

        for ( int i = 0 ; i < doseReceptors.Length ; i++ ) {

            activity += getAttenuatedActivity( sourceActivity * 37000000 , transform.position , doseReceptors[i]); //37000000 is mCi to Bq


        }

        //Check to see if array length is 0, we dont want to divide by 0 later on
        if ( doseReceptors.Length != 0 ) {


            double averageActivity = activity / doseReceptors.Length; //in units s^-1
            double doseRate = 0;

            for ( int i = 0 ; i < particleTypes.Length ; i++ ) {

                if ( particleTypes[ i ] ) {

                    //averageActivity * particleEnergies[ i ] yields keV/s
                    //keV/s / 6241506479963235 yields j/s, conversion factor
                    //Dividing that by weight yields j/kg*s, and a Sv=j/kg
                    doseRate += ( averageActivity * particleEnergies[ i ] * 1000 * 3600 ) / ( 6241506479963235 * weight ); //Yields mSv/hr
                    
                }

            }
            
            updateControllerDoseRate(doseRate);

        }

    }

    //Trig functions here incase I need to replace them with approximations for speed
    private float cos( float angle ) {

        return Mathf.Cos(angle);

    }


    private float sin(float angle) {

        return Mathf.Sin(angle);

    }

    private float asin(float angle) {

        return Mathf.Asin(angle);

    }

    private float acos(float angle) {

        return Mathf.Acos(angle);

    }

    private float sqrt( float number) {

        return Mathf.Sqrt(number);

    }
    
    //Rotates the point around center
    private Vector3 rotateAbout( Vector3 center , Vector3 point , Vector3 angles) {

        return Quaternion.Euler( angles ) * ( point - center ) + center;

    }
    

    //Will return 2 points if line intersects box, will return 1 point if it 'knicks' the box, or return 0 points. Well the array length will always be 2 but they're just filled with zero vectors
    private Vector3[] lineBoxIntersection(Vector3 origin , Vector3 destination ,Vector3 boxPosistion , Vector3 size , Vector3 angles ) {

        Vector3[] points = new Vector3[ 2 ];
        Vector3[] planes = new Vector3[ 6 ];

        //Seperate box into 6 planes
        planes[0] = linePlaneIntersection(origin , destination , boxPosistion + new Vector3(0 , ( size.y / 2 ) , 0) , new Vector2(size.x , size.z) , angles + new Vector3( 90 , 0 , 0 ) ); //Top
        planes[1] = linePlaneIntersection(origin , destination , boxPosistion + new Vector3(0 , -( size.y / 2 ) , 0) , new Vector2(size.x , size.z) , angles + new Vector3(90 , 0 , 0)); //Bottom
        planes[2] = linePlaneIntersection(origin , destination , boxPosistion + new Vector3(0 , 0 , ( size.z / 2 ) ) , new Vector2(size.x , size.y) , angles + new Vector3(0 , 0 , 0)); //Front
        planes[3] = linePlaneIntersection(origin , destination , boxPosistion + new Vector3(0 , 0 , -( size.z / 2 ) ) , new Vector2(size.x , size.y) , angles + new Vector3(0 , 0 , 0)); //Back
        planes[4] = linePlaneIntersection(origin , destination , boxPosistion + new Vector3( ( size.x / 2 ) , 0 , 0) , new Vector2(size.z , size.y) , angles + new Vector3(0 , 90 , 0)); //Left side
        planes[5] = linePlaneIntersection(origin , destination , boxPosistion + new Vector3( ( size.x / 2 ) , 0 , 0) , new Vector2(size.z , size.y) , angles + new Vector3(0 , 90 , 0)); //Right side

        points[0] = Vector3.zero;
        points[1] = Vector3.zero;

        int k = 0;

        for ( int i = 0 ; i < planes.Length ; i++ ) {

            if ( planes[ i ] != Vector3.zero ) {

                points[k] = planes[i];

                k++;

                if ( k == 2 ) {

                    return points;

                }

            }

        }

        return points;

    }

    //Will return the point where a line passes through a plane, if there is no intersection then return zero vector3
    private Vector3 linePlaneIntersection(Vector3 origin , Vector3 destination , Vector3 planePosistion , Vector2 planeSize , Vector3 rotation) {

        Vector3 intersectionPoint = Vector3.zero;

        //A is origin, B is destination, C is center of plane, I is intersection point
        float ab = Vector3.Distance(origin , destination);
        float ac = Vector3.Distance(planePosistion , origin);
        float bc = Vector3.Distance(planePosistion , destination);

        //AB^2=CB^2+AC^2-2(AC)(BC)cos(omega)
        float omega = acos( ( ( ab * ab ) - ( bc * bc ) - ( ac * ac ) ) / ( -2 * ac * bc ) );

        //Sine rule
        float ci = ( sin(omega) * ac * bc ) / ab;
        float alpha = asin(ci / ac);
        float ai = cos(alpha) * ac;

        //Complicated trapazoid diagram, just trust me maths
        float deltaX = sqrt(((ai*ai)+(ci*ci)-(ac*ac)) / 2);
        
        if ( deltaX < ( planeSize.x / 2 ) ) {
            
            //P-thag rule
            float deltaY = ( ci * ci ) - ( deltaX * deltaX );

            if ( deltaY < ( planeSize.y / 2 ) ) {

                //If these conditions are met then the line passes through the plane
                intersectionPoint = planePosistion + new Vector3(deltaX , deltaY , 0);

                //Rotates point about plane posistion
                intersectionPoint =rotateAbout( planePosistion , intersectionPoint , rotation );

            }

        }

        return intersectionPoint;

    }

    private double attenuate( double initialAcitvity , double attenuationConstant , double distance) {
        
        return initialAcitvity * Mathf.Exp( ( float )( -attenuationConstant * distance ) );


    }

    //Work in progress my dudes
    private double getAttenuatedActivity( double initialActivity , Vector3 origin , Vector3 destination ) {

        double attenuatedActivity = initialActivity;

        GameObject[] shields = GameObject.FindGameObjectsWithTag( "Shielding" );
        GameObject shield;


        for ( int i = 0 ; i < shields.Length ; i++ ) {

            shield = shields[i];

            //Someone help me find the size of a box ):
            Vector3[] points = lineBoxIntersection(origin , destination , shield.transform.position , shield.transform.localScale , shield.transform.rotation.eulerAngles );

            if ( points[0] != Vector3.zero && points[1] != Vector3.zero ) {
                
                //This is our thickness
                float thickness = Vector3.Distance(points[0] , points[1]);

                Vector3 closestPoint = points[0];
                Vector3 furthestPoint = points[1];

                if ( Vector3.Distance( origin , points[ 1 ] ) < Vector3.Distance(origin , points[0]) ) {

                    closestPoint = points[1];
                    furthestPoint = points[0];

                }

                float airAttenuationDistance = Vector3.Distance(origin , closestPoint);

                attenuatedActivity = attenuate(attenuatedActivity , airAttenuation , airAttenuationDistance);

                float materialAttenuationConstant = 10;//Please change later to match material

                attenuatedActivity = attenuate(attenuatedActivity, materialAttenuationConstant , thickness);
                origin = furthestPoint;

            }

        }

        attenuatedActivity = attenuate(attenuatedActivity , airAttenuation , Vector3.Distance(origin , destination));

        return attenuatedActivity;

    }

    //Get's posistion of game objects tagged with 'DoseReceptor'
    private Vector3[] findReceptorLocations() {

        GameObject[] doseReceptors = GameObject.FindGameObjectsWithTag("DoseReceptor");

        Vector3[] points = new Vector3[doseReceptors.Length];

        for ( int i = 0 ; i < points.Length ; i++ ) {

            points[i] = doseReceptors[i].transform.position;
            
        }

        return points;

    }

    private TextMesh textMesh = null;
    private double lastDoseRate = 0;

    int updateClicks = 40;
    int updateClick = 0;

    private void updateControllerDoseRate( double doseRate ) {


        //Only update if dose has changed
        if ( doseRate != lastDoseRate && updateClick > updateClicks ) {

            //If its null find the text mesh in the game
            if ( textMesh == null ) {

                textMesh = GameObject.Find("Controller text").GetComponent<TextMesh>(); //Name of plane on controller

            }

            if ( textMesh != null ) {

                textMesh.text = Math.Round(doseRate , 2) + "\n" + "mSv/h"; //Rounds to two decimals and updates text

            }

            doseRate = lastDoseRate;
            updateClick = 0;

        }

        updateClick++;

    }

}
