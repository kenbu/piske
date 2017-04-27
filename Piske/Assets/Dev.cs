using UnityEngine;
using System.Collections;
using kenbu.Piske;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using System.Collections.Generic;

public class Dev : MonoBehaviour {

    [SerializeField]
    private RootScene _rootScene;

    //大ルーター　Sceneの切り替え　 Outgame/Ingame
    //中ルーター　Gacha, 強化
    //小ルーター　強化トップ、強化選択、強化



	// Use this for initialization
	void Start () {
        
        //Root

        //デフォルトシーン設定が欲しいな。
        SceneBuilder.BuildFromHierarchy(_rootScene);
        StartCoroutine (_rootScene.Init (()=>{
            _rootScene.Router.Goto ("Root/Home");
        }));
    }

}
