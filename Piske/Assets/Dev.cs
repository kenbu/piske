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

    private Button[] _buttonList;

    //大ルーター　Sceneの切り替え　 Outgame/Ingame
    //中ルーター　Gacha, 強化
    //小ルーター　強化トップ、強化選択、強化



	// Use this for initialization
	void Start () {
        _buttonList = GetComponentsInChildren <Button>();
        foreach(var b in _buttonList){
            b.onClick.AddListener (ClickButton(b.gameObject.GetComponentInChildren <Text> ().text));
        }

        //Root
        SceneBuilder.BuildFromHierarchy(_rootScene);
        StartCoroutine (_rootScene.Init ());
    }

    private UnityAction ClickButton(string path){
        return () => {
            _rootScene.Router.Goto (path);
        };
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
