using UnityEngine;
using System.Collections;
using kenbu.Piske;
using System.Collections.Generic;


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
        public void Goto(string fullPath, bool isAddHistory = true){
            Debug.Log ("Goto : " + fullPath);
            if (isRouting) {
                Debug.Log ("ルーティングがブロックされました。");
                return;
            }
            isRouting = true;


            //出発地点を現在地に
            DiparturedScene = CurrentScene;

            //クエリを分解
            SetQuery (fullPath);

            TargetScene  = CreateScenePath (fullPath);


            if ((CurrentScene != null) && (TargetScene.path == CurrentScene.path)) {
                isRouting = false;
                Debug.Log ("同じパスにGOTOしました");
                return;
            }
            //ヒストリー登録
            if(isAddHistory) {
                _histories.Add (fullPath);
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


        private ScenePath CreateScenePath(string fullPath){
            var scenePath = new ScenePath ();
            scenePath.fullPath = fullPath;

            //クエリーを取り除く
            char queryDelimiter = '?';

            string[] splited = fullPath.Split (queryDelimiter);
            if (splited.Length > 1) {
                scenePath.query = fullPath.Split (queryDelimiter)[1];
            }
            scenePath.path = fullPath.Split (queryDelimiter)[0];
            scenePath.sceneList = _root.GetSceneList (scenePath.path);
            scenePath.scene = scenePath.sceneList [scenePath.sceneList.Count - 1];
            return scenePath;
        }

    }

    public class ScenePath{
        public string path;
        public string query;
        public string fullPath;
        public List<IScene> sceneList;
        public IScene scene;
    }
}