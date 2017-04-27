using UnityEngine;
using System.Collections;
using kenbu.Piske;

namespace RouterTest
{
    public class WorkWebScene : Scene {

        [SerializeField]
        private WorkWebView _view;

        public override IEnumerator OnInit(){
            _view.OnBack = ()=>{
                Router.GoBack ();
            };
            yield break;
        }

        //経由　向かう
        public override IEnumerator OnLoad(){
            yield break;
        }

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

        //経由　戻る
        public override IEnumerator OnUnload(){
            yield break;
        }
    }
}