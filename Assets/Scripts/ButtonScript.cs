using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour {
     
	public void unHighlight()
    {
        var button = this.GetComponent<Button>();
        button.OnDeselect(null);
    }
}
