using UnityEngine;
using System.Collections;
using kenbu.Piske;
using System.Collections.Generic;
using System;



namespace kenbu.Piske{

    public class SceneBuilder {
        /// <summary>
        /// ヒエラルキーの構造からシーンを作る。　SubContent向き
        /// </summary>
        /// <returns><c>true</c>, if from hierarchy was built, <c>false</c> otherwise.</returns>
        /// <param name="root">Root.</param>
        static public bool BuildFromHierarchy(RootScene root){

            bool success = BuildRelation (root);
            if (success == false) {
                throw new Exception ("kenbu.Piske.SceneBuilder.BuildRelation:: ビルドに失敗しました。");
            }
            root.TraceHierarchy ();
            return success;
        }

        //別で作った方が良さそうだなぁ。
        static public bool BuildFromJson(string json){
            //
            return false;
        }

        static private bool BuildRelation(IScene scene){
            //重複シーンネームチェック
            Dictionary<string, bool> sceneNameCheckDic = new Dictionary<string, bool> ();
            scene.Setup ();

            //直下のSceneを探す
            foreach(Transform child in scene.Transform){
                var childScene = child.GetComponent <IScene>();
                if (childScene == null) {
                    throw new Exception ("kenbu.Piske.SceneBuilder.BuildRelation:: " + child.name + "はISceneアタッチされていません。");
                }
                //ネームチェック
                if (sceneNameCheckDic.ContainsKey (child.name)) {
                    throw new Exception ("kenbu.Piske.SceneBuilder.BuildRelation:: " + child.name + "すでに同名のシーンが登録されています。。");
                }
                sceneNameCheckDic [child.name] = true;
                //自身の子供に登録
                scene.AddChild (childScene);
                //子供のフローへ
                BuildRelation (childScene);
            }
            return true;
        }

    }
}
