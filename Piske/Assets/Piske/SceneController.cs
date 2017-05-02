using UnityEngine;
using System.Collections;
namespace kenbu.Piske
{


    public class SceneController : MonoBehaviour {

        private Router _router;
        private Router Router {
            get{
                if(_router== null) {
                    _router = GetComponent <Router> ();
                }
                return _router;
                
            }
        }

        public string Current{
            get{ 
                if (Router == null) {
                    return "";
                }
                if (Router.CurrentScene == null) {
                    return "";
                }
                return Router.CurrentScene.pathWithQuery;
            }
        }

        public void Goto(string path){
            Router.Goto (path);
        }
    }
}


