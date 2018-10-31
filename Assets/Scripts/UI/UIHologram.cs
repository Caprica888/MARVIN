using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHologram : MonoBehaviour {
    #region variables
    public GameObject player;
    public GameObject plane;    //  holoUI background
    public GameObject UI_HologramExtension; //  scrollbar extension
    public Canvas thisCanvas;
    public List<GameObject> buttons = new List<GameObject>();
    public List<string> text = new List<string>();
    public bool isActive = false;   //  is the UI in the world?
    public float displayHeight; //  how high from the spawn point the plane should be
    public float sizeModifier = 1.9f;   //  used to help modify the dynamic size of the holo-plane

    private Text displayText;
    private Color planeColor;
    private Color textColor;
    private float size;
    #endregion

    public void setLines(string s, int index) {
        //text[index] = s;
        text.Add(s);
    }

    public void setPlaneColor(Color col) { planeColor = col; }

    public void setTextColor(Color col) { textColor = col; }

    private void Start() {
        size = plane.GetComponent<RectTransform>().localScale.z * sizeModifier;
        displayText = this.GetComponentInChildren<Text>();
        GetComponentInChildren<Renderer>().material.color = planeColor;
        GetComponentInChildren<Material>().color = GetComponentInChildren<Renderer>().material.color;
    }

    // Update is called once per frame
    void Update () {
        GetComponent<Transform>().LookAt(player.transform.position);  //  billboarding

        if (!isActive)  //  if the UI isn't already displayed, display it
        {          
            for (int i = 0; i < text.Count; i++)
            {
                displayText.text += text[i] + "\n";
                plane.GetComponent<RectTransform>().localScale += new Vector3(0, 0, size);
                plane.transform.position -= new Vector3(0, size * (sizeModifier + 0.15f), 0);
            }
            isActive = true;
            displayText.color = textColor;
        }
    }
}
