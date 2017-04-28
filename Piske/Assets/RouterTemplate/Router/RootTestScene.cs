using UnityEngine;
using System.Collections;
using kenbu.Piske;

namespace RouterTest
{
    public class RootTestScene : RootScene {

        [SerializeField]
        private GlobalMenu _menu;

        public override IEnumerator OnInit(){

            _menu.OnGoto = ((path) => {
                Router.Goto (path);
            });
            _menu.OnHistoryBack = (() => {
                Router.HistoryBack ();
            });
            _menu.gameObject.SetActive (false);


            Router.OnChangeTargetScene+= () => {
                _menu.UpdateData(Router.TargetScene.pathWithQuery);
            };
            yield break;
        }

        //経由　向かう
        public override IEnumerator OnLoad(){
            _menu.gameObject.SetActive (true);
            yield break;
        }
            
        //経由　戻る
        public override IEnumerator OnUnload(){
            _menu.gameObject.SetActive (false);
            yield break;
        }

    }
}