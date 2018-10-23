using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class source : MonoBehaviour {

    private ParticleSystem sourceParticles;
    private Light sourceLight;
    
    private bool[] particleTypes = new bool[ 3 ]; //If the particle emits a particle then its index will be true. Order: Alpha, Beta, Gamma
    private double[] particleEnergies = new double[ 3 ]; //In keV

    public string radioNuke = "";
    public Color gammaColour;
    public Color betaColour;


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

        if ( particleTypes[ 1 ] || particleTypes[ 2 ] ) {

            sourceParticles.enableEmission = true;

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
 

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    //Will return the point where a line passes through a plane, if there is no intersection then return null
    private Vector3 linePlaneIntersection(Vector3 origin , Vector3 destination , Vector3 planePosistion , Vector2 planeSize , Vector3 rotation) {

        Vector3 intersectionPoint = null;

        return intersectionPoint;

    }

    private double getAttenuatedDose( double initialIntensity , Vector3 origin , Vector3 destination ) {

        double attenuatedIntensity = 0;

        

        return attenuatedIntensity;

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


}
