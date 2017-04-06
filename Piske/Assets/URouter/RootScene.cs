using UnityEngine;
using System.Collections;
using kenbu.URouter;
using System.Collections.Generic;


namespace kenbu.URouter{
    
    public class RootScene : Scene {


        public void SetupRoot(System.Action callback){
            Router = GetComponent <IRouter>();

            base.Setup (this, null);


            StartCoroutine (_SetupRoot(callback));
        }

        private IEnumerator _SetupRoot(System.Action callback){

            yield return StartCoroutine (Init ());
            Router.Setup (this);
            callback.Invoke ();
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

    }
}