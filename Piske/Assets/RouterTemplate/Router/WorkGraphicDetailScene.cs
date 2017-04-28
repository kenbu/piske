using UnityEngine;
using System.Collections;
using kenbu.Piske;

namespace RouterTest
{
    public class WorkGraphicDetailScene : Scene {

        [SerializeField]
        private WorkGrahicDetailView _view;

        public override IEnumerator OnInit(){
            _view.OnClose = ()=>{
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
            var query = Router.Query;
            _view.UpdateDate ();
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