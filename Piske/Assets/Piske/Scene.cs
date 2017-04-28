using UnityEngine;
using System.Collections;
using kenbu.Piske;
using System.Collections.Generic;


/*
・軽量
・画面遷移
・実行時生成
・mono
・入れ子 可能

・ページ管理
・ポップアップ

ピスケ
ネーネーネコ


//向かう予告
//通る
//到着
*/



namespace kenbu.Piske{

    public class Scene : MonoBehaviour, IScene {

        public void DebugMode(){
        
        }


        //シーン追加
        public void AddChild (IScene child){
            _children.Add (child);
            child.OnAddedParent (this);
        }
        public void OnAddedParent (IScene parent){
            Parent = parent;
            Router = parent.Router;
            Root = parent.Root;
        }
            
        //シーン削除
        public void RemoveChild (IScene scene){
            //このメソッドはいらんのかもしれん。
            int index = _children.IndexOf (scene);
            if (index >= 0) {
                _children.RemoveAt (index);
                scene.Dispose ();
            } else {
                Debug.LogWarning (scene.ID + "が見つかりませんでした。");
            }
        }

        //破棄
        public void Dispose(){
            
            foreach(var child in _children){
                child.Dispose ();
            }
            _children = null;
            Destroy (gameObject);
        }

        //参照系
        //ルーター
        public IRouter Router{get; set;}

        //シーンルート
        public RootScene Root{get;protected set;}

        //IScene
        public IScene Parent{get;protected set;}

        //子シーン
        protected List<IScene> _children = new List<IScene>(); 

        public List<IScene> Children{
            get
            {
                return _children;
            }
        }

        public IScene GetChild(string id) {
            foreach (var child in Children) {
                if (child.ID == id) {
                    return child;
                } 
            }
            return null;
        }

        //自身プロパティ
        //ID
        public string ID {
            get;
            protected set;
        }

        public SceneStatus SceneStatus{ get; private set;}


        //初期化ルートシーンが生成時に一回だけ呼ばれる。
        public virtual void Setup(){
            ID = name;
            Children.ForEach ((c) =>{
                c.Setup ();
            });
        }

        public string Path{
            get
            {
                string pathString = ID;
                IScene p = Parent;
                while (p != null) {
                    pathString = p.ID + "/" + pathString;
                    p = p.Parent;
                }
                return "/" + pathString;
            }
        }


        private bool _isLoaded = false;

        //初期化ルートシーンが生成時に一回だけ呼ばれる。
        public IEnumerator Init(System.Action callback = null){
            SceneStatus = SceneStatus.UNLOAD;
            UpdateSceneNameWithStatus ();
            yield return StartCoroutine (OnInit());
            foreach (var child in _children) {
                yield return StartCoroutine (child.Init ());
            }
            if (callback != null) {
                callback.Invoke ();
            }
        }

        //経由　向かう
        public IEnumerator Load(){
            if (_isLoaded == false) {
                SceneStatus = SceneStatus.LOAD;
                UpdateSceneNameWithStatus();
                yield return StartCoroutine (OnLoad ());
            } else {
                yield break;
            }
            _isLoaded = true;

        }

        //このシーンに到着
        public IEnumerator Arrival(){
            SceneStatus = SceneStatus.ARRIIVAL;

            UpdateSceneNameWithStatus();
            yield return StartCoroutine (OnArrival());
        }

        //このシーンから出発
        public IEnumerator Diparture(){
            SceneStatus = SceneStatus.LOAD;
            UpdateSceneNameWithStatus();
            yield return StartCoroutine (OnDiparture());
        }

        //経由　戻る
        public IEnumerator Unload(){
            SceneStatus = SceneStatus.UNLOAD;
            UpdateSceneNameWithStatus();
            _isLoaded = false;
            yield return StartCoroutine (OnUnload());
        }

        public void ChangeQuery(){
            OnChangeQuery ();
        }

        //テンプレートメソッド
        public virtual IEnumerator OnInit(){
            Debug.Log ("OnInit: " + ID);
            yield return null;
        }

        //経由　向かう
        public virtual IEnumerator OnLoad(){
            Debug.Log ("OnLoad: " + ID);
            yield break;
        }

        //このシーンに到着
        public virtual IEnumerator OnArrival(){
            Debug.Log ("OnArrival: " + ID);
            yield break;
        }

        public virtual void OnChangeQuery(){
            Debug.Log ("OnChangeQuery: " + ID);
        }



        //このシーンから出発
        public virtual IEnumerator OnDiparture(){
            Debug.Log ("OnDiparture: " + ID);
            yield break;
        }

        //経由　戻る
        public virtual IEnumerator OnUnload(){
            Debug.Log ("OnUnload: " + ID);
            yield break;
        }


        public Transform Transform{
            get{ 
                return transform;
            }
        }

        private void UpdateSceneNameWithStatus(){
            string header = "-";

            switch (SceneStatus) {
            case SceneStatus.LOAD:
                header = ">";
                break;
            case SceneStatus.ARRIIVAL:
                header = ">>";
                break;
            }

            name = header + ID;
        }

    }

    public enum SceneStatus{
        UNLOAD,
        LOAD,
        ARRIIVAL
    }

    public interface IScene  {
        
        SceneStatus SceneStatus{get;}

        //シーン追加
        void AddChild (IScene scene);
        //シーン
        void OnAddedParent (IScene parent);

        //シーン削除
        void RemoveChild (IScene scene);

        //破棄
        void Dispose();

        //参照系
        //ルーター
        IRouter Router{get;}

        //シーンルート
        RootScene Root{ get;}

        //IScene
        IScene Parent{ get;}

        string Path{get;}

        //子シーン
        List<IScene> Children{ get;}
        IScene GetChild (string id);

        //自身プロパティ
        //ID
        string ID{get;}

        //初期化ルートシーンが生成時に呼ばれる。
        void Setup ();

        //メソッド
        //初期化
        IEnumerator Init(System.Action callback = null);

        //経由　向かう
        IEnumerator Load();

        //このシーンに到着
        IEnumerator Arrival();

        //このシーンから出発
        IEnumerator Diparture();

        //経由　戻る
        IEnumerator Unload();

        Transform Transform{get;}

        //このシーンに到着
        void ChangeQuery();

    }

}
