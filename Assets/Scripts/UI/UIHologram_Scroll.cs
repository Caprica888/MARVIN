using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHologram_Scroll : MonoBehaviour{
    #region variables
    public Text numberDisplay;
    public GameObject extensionPlane;
    public GameObject handle, fill; //  to change scroll image color

    private Color color;
    #endregion

    public void setColor(Color c)
    {
        color = c;
    }

    private void Start()
    {
        GetComponentInChildren<Renderer>().material.color = color;
        GetComponentInChildren<Material>().color = GetComponentInChildren<Renderer>().material.color;
    }

    // Update is called once per frame
    void Update () {
       
    }
}
