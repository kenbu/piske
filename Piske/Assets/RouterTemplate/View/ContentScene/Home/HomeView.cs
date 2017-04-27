using UnityEngine;
using System.Collections;

public class HomeView : MonoBehaviour {

    public void Show(){
        gameObject.SetActive (true);
    }

    public void Hide(){
        gameObject.SetActive (false);
    }
}
