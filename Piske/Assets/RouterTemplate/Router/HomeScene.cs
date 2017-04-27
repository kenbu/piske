using UnityEngine;
using System.Collections;
using kenbu.Piske;

namespace RouterTest
{
    public class HomeScene : Scene {

        [SerializeField]
        private HomeView _view;
        public override IEnumerator OnInit(){
            Debug.Log ("ホーム");
            yield break;
        }

        /*
        //経由　向かう
        public override IEnumerator OnLoad(){
            Debug.Log ("OnLoad: " + ID);
            yield break;
        }
        */
        //このシーンに到着
        public override IEnumerator OnArrival(){
            _view.Show ();
            yield break;
        }

        //このシーンから出発
        public override IEnumerator OnDiparture(){
            _view.Hide ();
            yield break;
        }
        /*
        //経由　戻る
        public override IEnumerator OnUnload(){
            Debug.Log ("OnUnload: " + ID);
            yield break;
        }
        */
    }
}