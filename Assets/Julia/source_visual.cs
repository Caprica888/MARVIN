using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class source_visual : MonoBehaviour {

    private ParticleSystem particleSystem;
    private double range;
    private double particleVelocity;

    //0 is alpha
    //1 is beta
    //2 is gamma
    private int particleType = 0;

	// Use this for initialization
	void Start () {

        particleSystem = GetComponent<ParticleSystem>();

        if ( particleType == 0 ) {

            particleSystem.startSpeed = 1;
            particleSystem.startLifetime = 2;
            particleSystem.emissionRate = 20;

        }


    }

    // Update is called once per frame
    void Update () {

        


	}

    //Energy is in MeV
    //Return is in m
    //Only for alpha particle
    private double calcRange( double energy ){

        if ( particleType == 0 ) {

            if ( energy < 4 ) {

                return ( energy * 0.56 ) / 100;

            }
            else {

                return ( ( energy * 1.42 ) - 2.62 );

            }

        }

        return 0;
        
    }

}
