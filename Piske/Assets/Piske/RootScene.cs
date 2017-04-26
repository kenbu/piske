using UnityEngine;
using System.Collections;
using kenbu.Piske;
using System.Collections.Generic;


namespace kenbu.Piske{
    
    public class RootScene : Scene {

        public bool debugMode;

        public override void Setup(){
            Router = gameObject.AddComponent (typeof(Router))as Router;
            Router.Setup (this);

            base.Setup ();
        }


        /// <summary>
        /// パスからシーンのリストを取得します。
        /// </summary>
        /// <returns>The scene list.</returns>
        /// <param name="path">Path.</param>
        public List<IScene> GetSceneList(string path){

            char queryDelimiter = '?';
            string pathWithoutQuery = path.Split (queryDelimiter)[0];

            //
            char delimiter = '/';
            string[] pathStrings = pathWithoutQuery.Split(delimiter);
            List<IScene> sceneList = new List<IScene> ();
            IScene scene = this;
            sceneList.Add (scene);

            for (int i = 1; i < pathStrings.Length; i++) {

                string id = pathStrings [i];
                IScene s = scene.GetChild (id);
                sceneList.Add(s);
                scene = s;
            }

            return sceneList;
        }

        private string _traceHierarchyString = "";
        public void TraceHierarchy(){
            Debug.Log ("- - - - - TraceHierarchy - - - - - ");
            _traceHierarchyString = "";
            TraceHierarchy (this, 0);
            Debug.Log (_traceHierarchyString);
        }

        public void TraceHierarchy(IScene scene, int hierarchy){
            string tab = "";
            for(int i = 0; i<hierarchy; i++){
                tab += "　";
            }
            _traceHierarchyString += tab + scene.ID + "\n";
            scene.Children.ForEach ((s)=>{
                TraceHierarchy (s, hierarchy + 1);
            });

        }

    }
}