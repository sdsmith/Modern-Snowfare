using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DebugOverlay : MonoBehaviour {

    private static Dictionary<string,string> attributes = new Dictionary<string, string>();
    private static List<string> attributeOrder = new List<string>();
    private Vector2 overlayPosition;
    private Vector2 overlaySize;



    void Start() {
        overlayPosition = new Vector2(10, 10);
        overlaySize = new Vector2(Screen.width / 3 - 10, Screen.height - 10);
    }


    /**
     * Adds given attribute 'key' with given value 'value' to print on the screen.
     * If a value is added and not updated every frame, its last recorded value will
     * be used. Items are displayed in the order they are added.
     */
    public static void AddAttr(string key, string value) {
        if (attributeOrder.Contains(key)) {
            // Remove old value so it can be updated
            attributes.Remove(key);
        } else {
            // Register new atrtibute
            attributeOrder.Add(key);
        }

        attributes.Add(key, value);
    }


    /**
     * Removes attribute and stops it being displayed. If the same attribute is re-added
     * after being removed, it is not garanteed to display at the same position in the list.
     */
    public static void RemoveAttr(string key) {
        if (attributeOrder.Contains(key)) {
            attributeOrder.Remove(key);
            attributes.Remove(key);
        }
    }


    /**
     * Use this to add attributes that should always be displayed.
     */
    void Update() {
        if (Camera.main) {
            AddAttr("player pos", Camera.main.transform.position.ToString());
            AddAttr("player fwd", Camera.main.transform.forward.ToString());
        } else {
            AddAttr("player pos", "");
            AddAttr("player fwd", "");
        }
    }


    /**
     * Draws the debug overlay on the screen.
     */
    void OnGUI() {
        string text = "";
        string value;

        foreach (string key in attributeOrder) {
            attributes.TryGetValue(key, out value);
            text += key + ": \t" + value + "\n";
        }

        GUI.TextArea(new Rect(overlayPosition, overlaySize), text);
    }
}
