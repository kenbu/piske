using UnityEngine;
using System.Collections;
using kenbu.Piske;
using System.Collections.Generic;
using System;


namespace kenbu.Piske{

    public interface IRouter{

        void Setup (RootScene _root);
        //向かう
        void Goto(string path, bool isAddHistory = true);

        //ヒストリーバック
        void HistoryBack();

        //一階層上に戻る
        void GoBack();

        //現在地
        ScenePath CurrentScene{ get;}

        //目的地
        ScenePath TargetScene{ get;}

        //出発元
        ScenePath DiparturedScene{ get;}

        Dictionary<string, string> Query { get;}
    }

    public class Router : MonoBehaviour, IRouter {

        private RootScene _root;
        public void Setup(RootScene root){
            _root = root;
        }

        private List<string> _histories = new List<string>();

        public bool isRouting = false;

        //向かう
        public void Goto(string pathWithQuery, bool isAddHistory = true){
            Debug.Log ("Goto1 : " + pathWithQuery);
            if (isRouting) {
                Debug.Log ("ルーティングがブロックされました。");
                return;
            }
            isRouting = true;

            //絶対パスに変換
            pathWithQuery = ToAbsolutePath (pathWithQuery);
            Debug.Log ("Goto2 : " + pathWithQuery);

            TargetScene  = CreateScenePath (pathWithQuery);
            if (TargetScene == null) {
                //ターゲットが見つからない場合。
                isRouting = false;
                throw new Exception (pathWithQuery + "は、ないよ");
                return;
            }
            //出発地点を現在地に
            DiparturedScene = CurrentScene;

            //クエリを分解
            SetQuery (pathWithQuery);

            if ((CurrentScene != null) && (TargetScene.path == CurrentScene.path)) {
                isRouting = false;
                Debug.Log ("同じパスにGOTOしました");
                return;
            }
            //ヒストリー登録
            if(isAddHistory) {
                _histories.Add (pathWithQuery);
            }

            StartCoroutine (_Goto2());
        }



        private IEnumerator _Goto2()
        {


            if (CurrentScene != null) {
                yield return StartCoroutine (CurrentScene.scene.Diparture ());
            }

            //TargetScene
            //CurrentScene2
            bool loop = true;
            while(loop){
                //yield return new WaitForSeconds (0.5f);
                if (CurrentScene == null) {
                    Debug.Log ("Currentなし");
                } else {
                    Debug.Log (CurrentScene.pathWithQuery);

                }
                //最初初期
                if (CurrentScene == null) {
                    CurrentScene = CreateScenePath (_root.Path);
                    yield return StartCoroutine (CurrentScene.scene.Load ());
                    continue;
                }
                //到着
                if (TargetScene.path == CurrentScene.path) {
                    yield return StartCoroutine (CurrentScene.scene.Arrival ());
                    loop = false;
                    continue;
                }

                if (IsChildHierarchy(TargetScene.scene, CurrentScene.scene)) {
                    //Currentから見てTargetが直径の場合
                    //CurrentScene2
                    //一つ下がる
                    foreach (var scene in CurrentScene.scene.Children) {
                        if (IsChildHierarchy (TargetScene.scene, scene)) {
                            CurrentScene = CreateScenePath(scene.Path);
                        }
                    }
                    yield return StartCoroutine (CurrentScene.scene.Load ());
                    continue;
                } else if(IsSameHierarchy(TargetScene.scene, CurrentScene.scene)) {
                    //Currentから見てターゲットが兄弟系の場合
                    //一つ下がる
                    foreach (var scene in CurrentScene.scene.Parent.Children) {
                        if (IsChildHierarchy (TargetScene.scene, scene)) {
                            yield return StartCoroutine (CurrentScene.scene.Unload ());
                            CurrentScene = CreateScenePath(scene.Path);
                        }
                    }
                    yield return StartCoroutine (CurrentScene.scene.Load ());
                    continue;
                } else {
                    //一個上に上がる
                    yield return StartCoroutine (CurrentScene.scene.Unload ());
                    CurrentScene = CreateScenePath(CurrentScene.scene.Parent.Path);
                    continue;
                }
            }

            isRouting = false;
            yield break;
        }



        //ヒストリーバック
        public void HistoryBack(){
            if (_histories.Count < 2 || isRouting) {
                return;
            }
            _histories.RemoveAt (_histories.Count - 1);
            string fullpath = _histories [_histories.Count - 1];
            Goto (fullpath, false);
        }

        //一階層上に戻る
        public void GoBack(){
            if (CurrentScene.scene.Parent == null) {
                Debug.Log ("最上階層です。");
                return;
            }
            Goto (CurrentScene.scene.Parent.Path);
        }


        //現在地
        private ScenePath _currentScene;
        public ScenePath CurrentScene{ 
            get{ 
                return _currentScene;
            }
            private set
            { 
                _currentScene = value;
            }
        }

        //目的地
        public ScenePath TargetScene {get; private set;}

        //出発元
        public ScenePath DiparturedScene{ get; private set;}

        //クエリ　?test = "みたいな。"
        private Dictionary<string, string> _query;
        public Dictionary<string, string> Query {
            get{ 
                return _query;
            }
        }

        //Utils
        public void SetQuery(string path){
            //todo: クエリ分解
            //var uri = new Uri(path);
            _query = new Dictionary<string, string> ();
        }

        //階層

        private bool IsSameHierarchy(IScene target, IScene current){
            if (current == null) {
                return false;
            }

            //自身の場合
            if (target.ID == current.ID) {
                return false;
            }

            //自身子供の場合
            if (IsChildHierarchy (target, current)) {
                return false;
            }

            //同じ親系ならok
            var currentParentPath = "";
            if (current.Parent != null) {
                currentParentPath = current.Parent.Path;
            }

            var targetParentPath = "";
            if (target.Parent != null) {
                targetParentPath = target.Parent.Path;
            }

            return targetParentPath.IndexOf (currentParentPath) >= 0;
        }
        //子階層
        private bool IsChildHierarchy(IScene target, IScene current){

            if (current == null) {
                return true;
            }
            return target.Path.IndexOf (current.Path) >= 0;
        }

        private string ToAbsolutePath(string path){
            //先頭が「/Test/test」の場合絶対リンク
            if(path.IndexOf ("/") == 0){
                return path;
            }
            // 先頭が　「../Test/test」の場合の相対リンク
            if(path.IndexOf ("../") == 0){

                int parentCnt = 0;
                while (path.IndexOf ("../") == 0) {
                    parentCnt++;
                    path = path.Remove (0, 3);
                }
                var s = CurrentScene.scene;
                for (var i = 0; i < parentCnt; i++) {
                    s = s.Parent;
                }
                return s.Path + "/" + path;
            }

            return CurrentScene.path + "/" + path;

        }

        private ScenePath CreateScenePath(string pathWithQuery){
            var scenePath = new ScenePath ();
            scenePath.pathWithQuery = pathWithQuery;

            //クエリーを取り除く
            char queryDelimiter = '?';

            string[] splited = pathWithQuery.Split (queryDelimiter);
            if (splited.Length > 1) {
                scenePath.query = pathWithQuery.Split (queryDelimiter)[1];
            }
            scenePath.path = pathWithQuery.Split (queryDelimiter)[0];
            scenePath.sceneList = _root.GetSceneList (scenePath.path);
            //シーンが見つからない場合。
            if (scenePath.sceneList == null) {
                return null;
            }

            scenePath.scene = scenePath.sceneList [scenePath.sceneList.Count - 1];



            return scenePath;
        }

    }

    public class ScenePath{
        public string path;
        public string query;
        public string pathWithQuery;
        public List<IScene> sceneList;
        public IScene scene;
    }
}