using UnityEngine;
using System.Collections;
using kenbu.Piske;
using System.Collections.Generic;
using System;
using System.Linq;

namespace kenbu.Piske{
    
    public class RootScene : Scene {

        public bool debugMode;

        public override void Setup(){
            //ルータ
            Router = gameObject.AddComponent (typeof(Router))as Router;
            Router.Setup (this);

            //ルート
            Root = this;

            base.Setup ();
        }


        /// <summary>
        /// パスからシーンのリストを取得します。
        /// </summary>
        /// <returns>The scene list.</returns>
        /// <param name="path">Path.</param>
        public List<IScene> GetSceneList(string pathWithQuery){

            char queryDelimiter = '?';
            string path = pathWithQuery.Split (queryDelimiter)[0];

            //
            char delimiter = '/';
            //List<string> pathStrings = new List<string>();
            //pathStrings.AddRange (path.Split (delimiter));
            //空は除去する
            var pathStrings = path.Split (delimiter).Where((p)=>{
                return string.IsNullOrEmpty (p) == false;
            }).ToList ();

            List<IScene> sceneList = new List<IScene> ();
            IScene scene = this;
            sceneList.Add (scene);

            for (int i = 1; i < pathStrings.Count; i++) {
                try{
                    string id = pathStrings [i];
                    IScene s = scene.GetChild (id);
                    sceneList.Add(s);
                    scene = s;
                }catch(NullReferenceException e){
                    //Debug.LogWarning (pathWithQuery + "は、見つかりませんでした。" + e.Message);
                    return null;
                }
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