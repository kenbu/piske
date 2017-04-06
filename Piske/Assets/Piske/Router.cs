using UnityEngine;
using System.Collections;
using kenbu.Piske;
using System.Collections.Generic;


namespace kenbu.Piske{

    public interface IRouter{

        void Setup (RootScene _root);
        //向かう
        void Goto(string path);

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

        //向かう
        public void Goto(string fullPath){
            Debug.Log ("Goto : " + fullPath);

            //todo: 最初からシーンが用意されてなくて動く前提で作る。

            //出発地点を現在地に
            DiparturedScene = CurrentScene;

            //クエリを分解
            SetQuery (fullPath);

            TargetScene  = CreateScenePath (fullPath);

            StartCoroutine (_Goto(TargetScene));
        }


        private IEnumerator _Goto(ScenePath targetScene){
        
            //初回
            if (CurrentScene == null) {
                yield return StartCoroutine (DownHierarchy(targetScene));
                yield break;
            }

            //同じ場合
            if (targetScene.fullPath == CurrentScene.fullPath) {
                yield break;
            }

            //Queryのみ変更
            if (targetScene.path == CurrentScene.path && targetScene.fullPath != CurrentScene.fullPath) {
                //todo: 変更通知。
                CurrentScene = targetScene;
                yield break;
            }


            //現在地を離れる
            if(CurrentScene != null) {
                yield return StartCoroutine (CurrentScene.scene.Diparture ());
            }

            //上に戻る
            if (IsChildHierarchy (targetScene.scene, CurrentScene.scene) == false && IsSameHierarchy (targetScene.scene, CurrentScene.scene) == false) {
                yield return StartCoroutine (UpHierarchy (targetScene));
            } else {
                if (IsChildHierarchy (targetScene.scene, CurrentScene.scene) == false) {
                    //兄弟
                    yield return StartCoroutine (CurrentScene.scene.Unload ());
                }
            }
            //下に下がる
            yield return StartCoroutine (DownHierarchy (targetScene));
        }


        //Current == null Donwだけ

        //SameHerarchyか調べる。
        //    -> DOWN
        //SameHerarchyではない。
        //  -> UP -> DOWN

        private IEnumerator UpHierarchy(ScenePath targetScene){
           
            yield return StartCoroutine (CurrentScene.scene.Unload ());
            var c = CurrentScene.scene.Parent;
            CurrentScene = CreateScenePath (c.Path);

            while (targetScene.scene.ID != c.ID && IsSameHierarchy (targetScene.scene, c) == false) {
                yield return StartCoroutine (c.Unload ());
                c = CurrentScene.scene.Parent;
                CurrentScene = CreateScenePath (c.Path);
            }
            yield break;
        }

        private IEnumerator DownHierarchy(ScenePath targetScene){

            var i = 0;
            if (CurrentScene != null) {
                i = CurrentScene.sceneList.Count - 1;
            }

            //直系なのか、兄弟なのか調べないといけない気がするぞ。

            for (; i < targetScene.sceneList.Count; i++) {
                //通る
                yield return StartCoroutine (targetScene.sceneList [i].Load ());
                CurrentScene = CreateScenePath (targetScene.sceneList [i].Path);
            }
            //到着
            yield return StartCoroutine (targetScene.scene.Arrival ());
        }

        //ヒストリーバック
        public void HistoryBack(){
            
        }

        //一階層上に戻る
        public void GoBack(){
            //Goto ();
            //CurrentScene.scene.Parent

        }


        //現在地
        public ScenePath CurrentScene{ get; private set;}

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