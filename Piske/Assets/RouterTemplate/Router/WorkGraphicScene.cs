using UnityEngine;
using System.Collections;
using kenbu.Piske;

namespace RouterTest
{
    public class WorkGraphicScene : Scene {

        [SerializeField]
        private WorkGrahicView _view;

        public override IEnumerator OnInit(){
            _view.OnBack = ()=>{
                Router.GoBack ();
            };
            _view.OnShowDetail = (id)=>{
                Router.Goto("Detail?id=" + id);
            };
            _view.OnShowWeb = ()=>{
                Router.Goto("../Web");
            };
            yield break;
        }

        //経由　向かう
        public override IEnumerator OnLoad(){
            _view.Show ();
            yield break;
        }

        //このシーンに到着
        public override IEnumerator OnArrival(){
            yield break;
        }

        //このシーンから出発
        public override IEnumerator OnDiparture(){
            yield break;
        }

        //経由　戻る
        public override IEnumerator OnUnload(){
            _view.Hide ();
            yield break;
        }
    }
}