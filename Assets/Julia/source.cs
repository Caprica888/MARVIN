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

 
    

    //Will return 2 points if line intersects box, will return 1 point if it 'knicks' the box, or return 0 points. Well the array length will always be 2 but they're just filled with zero vectors
    private Vector3[] lineBoxIntersection(Vector3 origin , Vector3 destination , GameObject gameObject ) {
       
        RaycastHit hitA;
        RaycastHit hitB;
        Vector3 startA = Vector3.zero;
        Vector3 startB = Vector3.zero;


        //Checks to see if the source hit one face
        if ( Physics.Raycast(origin , ( ( origin - destination ) * -1f ).normalized , out hitA ) ) {

            if ( hitA.transform.name == gameObject.name ) {

                startA = hitA.point;

                //Now that we get a hit, we work backwards to get another ray cast, from the target to the source
                if ( Physics.Raycast( destination , ( ( destination - origin ) * -1f ).normalized , out hitB) ) {

                    if ( hitB.transform.name == gameObject.name ) {

                        startB = hitB.point;

                    }

                }

            }

        }


        Vector3[] points = new Vector3[ 2 ];
        
        points[0] = startA;
        points[1] = startB;
        


        return points;

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
            Vector3[] points = lineBoxIntersection(origin , destination , shield );
            
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

                if ( debug ) {

                    DrawLine(origin , closestPoint , Color.red);
                    DrawLine(closestPoint , furthestPoint , Color.green);

                }

                float materialAttenuationConstant = 10;//Please change later to match material

                attenuatedActivity = attenuate(attenuatedActivity, materialAttenuationConstant , thickness);
                origin = furthestPoint;

            }

        }

        attenuatedActivity = attenuate(attenuatedActivity , airAttenuation , Vector3.Distance(origin , destination));

        DrawLine(origin , destination , Color.blue);

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

    void DrawLine(Vector3 start , Vector3 end , Color color , float duration = 0.052f) {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.SetColors(color , color);
        lr.SetWidth(0.01f , 0.01f);
        lr.SetPosition(0 , start);
        lr.SetPosition(1 , end);
        GameObject.Destroy(myLine , duration);
    }

}
