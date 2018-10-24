using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHologram : MonoBehaviour {
    #region variables
    public GameObject player;
    public bool isActive = false;   //  is the UI in the world?
    public float displayHeight; //  how high from the spawn point the plane should be
    public static int lineCount = 6; // only 6 because the plane is only so big
    public string[] text = new string[lineCount];    ///  not meant to be edited through here; only through UIHologram_ObjectScript
    
    private Text displayText;
    private Color planeColor;
    private Color textColor;
    #endregion

    public void setLines(string s, int index) { text[index] = s; }

    public void setPlaneColor(Color col) { planeColor = col; }

    public void setTextColor(Color col) { textColor = col; }

    private void Start() {
        displayText = this.GetComponentInChildren<Text>();
        GetComponentInChildren<Renderer>().material.color = planeColor;
        GetComponentInChildren<Material>().color = GetComponentInChildren<Renderer>().material.color;
    }

    // Update is called once per frame
    void Update () {
        GetComponent<Transform>().LookAt(player.transform.position);  //  billboarding

        if (!isActive)  //  if the UI isn't already displayed, display it
        {          
            for (int i = 0; i < lineCount; i++)
            {
                displayText.text += text[i] + "\n";
            }
            isActive = true;
            displayText.color = textColor;
        }
    }
}
