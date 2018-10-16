using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class source_visual : MonoBehaviour {

    private ParticleSystem particleSystem;
    private double range;
    private double particleVelocity;

	// Use this for initialization
	void Start () {

        particleSystem = GetComponent<ParticleSystem>();
        range = calcRange(3);

        particleVelocity = particleSystem.startSpeed; //m/s
        particleSystem.startLifetime = ( float )( range / (particleVelocity / 100) );

    }

    // Update is called once per frame
    void Update () {

        


	}

    //Energy is in MeV
    //Return is in cm
    //Only for alpha particle
    private double calcRange( double energy ){

        if ( energy < 4 ){

            return energy * 0.56;

        }
        else {

            return (energy * 1.42) - 2.62;

        }
        
    }

}
