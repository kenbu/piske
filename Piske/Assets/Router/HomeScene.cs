using UnityEngine;
using System.Collections;
using kenbu.Piske;

public class HomeScene : Scene {

    public HomeScene(){
        ID = "HomeScene";
    } 

    //経由　向かう
    public override IEnumerator OnLoad(){
        Debug.Log ("OnLoad: " + ID);
        yield break;
    }

    //このシーンに到着
    public override IEnumerator OnArrival(){
        Debug.Log ("OnArrival: " + ID);
        yield break;
    }

    //このシーンから出発
    public override IEnumerator OnDiparture(){
        Debug.Log ("OnDiparture: " + ID);
        yield break;
    }

    //経由　戻る
    public override IEnumerator OnUnload(){
        Debug.Log ("OnUnload: " + ID);
        yield break;
    }
}
