using UnityEngine;
using System.Collections;
using kenbu.Piske;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;


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

        //Event
        event RouterEvent.RouterEventHandler OnChangeTargetScene;

    }

    public class RouterEvent{
        public delegate void RouterEventHandler();
    }

    public class Router : MonoBehaviour, IRouter {


        /// <summary>
        /// イベント
        /// </summary>
        public event RouterEvent.RouterEventHandler OnChangeTargetScene;


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

            if (CurrentScene != null) {
                //パス、クエリともに同じ
                if (TargetScene.pathWithQuery == CurrentScene.pathWithQuery) {
                    isRouting = false;
                    Debug.Log ("同じパスにGOTOしたので止めます");
                    return;
                }

                //クエリだけが変更になった
                if (TargetScene.path == CurrentScene.path && TargetScene.pathWithQuery != CurrentScene.pathWithQuery) {
                   
                    CurrentScene = CreateScenePath (pathWithQuery);
                    CurrentScene.scene.ChangeQuery ();
                    //
                    if(isAddHistory) {
                        _histories.Add (pathWithQuery);
                    }
                    OnChangeTargetScene ();
                    isRouting = false;
                    return;
                }
            }
            //ヒストリー登録
            if(isAddHistory) {
                _histories.Add (pathWithQuery);
            }

            OnChangeTargetScene ();

            StartCoroutine (_Goto());
        }



        private IEnumerator _Goto()
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
        public ScenePath CurrentScene { 
            get;
            private set;
        }

        //目的地
        public ScenePath TargetScene {get; private set;}

        //出発元
        public ScenePath DiparturedScene{ get; private set;}

        //クエリ　?test = "みたいな。"
        public Dictionary<string, string> Query {
            get;
            private set;
        }

        //Utils
        public void SetQuery(string path){
            Dictionary<string, string> result = new Dictionary<string, string> ();

            if (path.Contains ("?") == false) {
                //クエリなし
                Query = result;
                return;
            }

            //id=1&name=aaaa&test=oh
            string query = path.Split ('?')[1];
            //[id=1, name=aaaa, test=oh]
            string[] queryList = query.Split ('&');
            foreach(var q in queryList){
                //id=1
                var splited = q.Split ('=');
                //id
                var k = splited [0];
                //1
                var v = splited [1];
                result [k] = v;
            }

            Query = result;
        }

        //兄弟階層
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
        //絶対パスに変換
        private string ToAbsolutePath(string inputPath){
            //先頭が「/Test/test」の場合絶対リンク
            if(inputPath.IndexOf ("/") == 0){
                return inputPath;
            }
            // 先頭が　「../Test/test」の場合の相対リンク
            if(inputPath.IndexOf ("../") == 0){

                int parentCnt = 0;
                while (inputPath.IndexOf ("../") == 0) {
                    parentCnt++;
                    inputPath = inputPath.Remove (0, 3);
                }
                var s = CurrentScene.scene;
                for (var i = 0; i < parentCnt; i++) {
                    s = s.Parent;
                }
                return s.Path + "/" + inputPath;
            }
            //先頭がクエリのみの変更
            if (inputPath.IndexOf ("?") == 0) {
                return CurrentScene.path + inputPath;
            }
            //相対パス
            return CurrentScene.path + "/" + inputPath;

        }
        //
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