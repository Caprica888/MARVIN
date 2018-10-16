using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class walk : MonoBehaviour {

    public Transform vrCamera;
    public Transform leftControllerPos;
    public Transform rightControllerPos;

    private float speed = 2f;

    private bool moveForward;
    private CharacterController characterController;
    private SteamVR_Input leftController;

    private Vector3 lastPosRight;
    private float[] rightZ = new float[ 5 ];
    private float[] time = new float[5];
    private int i = 0;

	// Use this for initialization
	void Start () {
        
        characterController = GetComponent<CharacterController>();
        lastPosRight = rightControllerPos.position;

	}
	
	// Update is called once per frame
	void Update () {


        Vector3 rightVelocity = ( rightControllerPos.position - lastPosRight ) / Time.deltaTime;

        lastPosRight = rightControllerPos.position;



        if ( i < rightZ.Length ) {

            rightZ[i] = rightVelocity.z;
            time[i] = Time.deltaTime;

            i++;

        }
        else {

            for ( int x = 0 ; x < rightZ.Length - 1 ; x++ ) {

                rightZ[x] = rightZ[x + 1];
                time[x] = time[x + 1];

            }

            rightZ[rightZ.Length - 1] = rightVelocity.z;
            time[rightZ.Length - 1] = Time.deltaTime;

            float totalTime = 0f;
            float totalVelocity = 0f;

            for ( int x = 0 ; x < rightZ.Length ; x++ ) {

                totalTime += time[x];
                totalVelocity += rightZ[x];

            }

            float rightAccel = totalVelocity / totalTime;

            if ( rightAccel > 40 ) {

                moveForward = true;

            }
            else {

                moveForward = false;

            }

        }


        if ( moveForward ) {

            Vector3 forward = vrCamera.TransformDirection(Vector3.forward);

            characterController.SimpleMove(forward * speed);

        }

	}

}
