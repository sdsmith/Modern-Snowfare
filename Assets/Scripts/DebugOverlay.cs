using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DebugOverlay : MonoBehaviour {

    private List<KeyValuePair<string,string>> attributes;



    void Start() {
        attributes = new List<KeyValuePair<string, string>>();
    }

    void AddAttr(string key, string value) {
        attributes.Add(new KeyValuePair<string, string>(key, value));
    }

    void Update() {
        attributes.Clear();

        if (Camera.main) {
            AddAttr("player pos", Camera.main.transform.position.ToString());
            AddAttr("player fwd", Camera.main.transform.forward.ToString());
        }
    }


    void OnGUI() {
        string text = "";

        foreach (KeyValuePair<string, string> attr in attributes) {
            text += attr.Key + ": \t" + attr.Value + "\n";
        }

        GUI.TextArea(new Rect(10, 10, Screen.width / 3 - 10, Screen.height - 10), text);
    }
}
