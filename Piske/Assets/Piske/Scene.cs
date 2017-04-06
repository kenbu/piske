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
        //シーン追加
        public void AddChild (IScene scene){
            _children.Add (scene);
            scene.Transform.SetParent (transform);
        }
        public void OnAddedParent (IScene parent){
            Parent = parent;
            Router = parent.Router;
        }

        //childrenは使わない方がいいかも。

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
        public IRouter Router{get; protected set;}

        //シーンルート
        public RootScene Root{get;private set;}

        //IScene
        public IScene Parent{get;private set;}

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

        //初期化ルートシーンが生成時に一回だけ呼ばれる。
        public virtual void Setup(string id){
            ID = id;
            Debug.Log ("Setup: " + ID);


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
                return pathString;
            }
        }


        private bool _isLoaded = false;

        //初期化ルートシーンが生成時に一回だけ呼ばれる。
        public IEnumerator Init(){
            yield return StartCoroutine (OnInit());
            foreach (var child in _children) {
                yield return StartCoroutine (child.Init ());
            }
        }
        //シーン初期化
        public IEnumerator SceneInit(){
            yield return StartCoroutine (OnSceneInit());
        }

        //経由　向かう
        public IEnumerator Load(){
            if (_isLoaded == false) {
                yield return StartCoroutine (OnLoad ());
            } else {
                yield break;
            }
            _isLoaded = true;

        }

        //このシーンに到着
        public IEnumerator Arrival(){
            yield return StartCoroutine (OnArrival());
        }

        //このシーンから出発
        public IEnumerator Diparture(){
            yield return StartCoroutine (OnDiparture());
        }

        //経由　戻る
        public IEnumerator Unload(){
            _isLoaded = false;
            yield return StartCoroutine (OnUnload());
        }

        //テンプレートメソッド
        public virtual IEnumerator OnInit(){
            Debug.Log ("OnInit: " + ID);
            yield return null;
        }
        //シーン初期化
        public virtual IEnumerator OnSceneInit(){
            Debug.Log ("OnSceneInit: " + ID);
            yield break;
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


    }

    public interface IScene  {

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
        void Setup (string id);

        //メソッド
        IEnumerator Init();
        //シーン初期化
        IEnumerator SceneInit();

        //経由　向かう
        IEnumerator Load();

        //このシーンに到着
        IEnumerator Arrival();

        //このシーンから出発
        IEnumerator Diparture();

        //経由　戻る
        IEnumerator Unload();

        Transform Transform{get;}
    }

}
