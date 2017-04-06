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

	// Use this for initialization
	void Start () {
        _buttonList = GetComponentsInChildren <Button>();
        foreach(var b in _buttonList){
            //b.gameObject.GetComponentInChildren <Text> ().text
            b.onClick.AddListener (ClickButton(b.gameObject.GetComponentInChildren <Text> ().text));
        }
        //Root
        _rootScene.Setup ("Scene1");

        //Scene1
        var id = "Scene1_1";
        var scene1_go = new GameObject (id);
        var scene1 = scene1_go.AddComponent (Type.GetType ("kenbu.Piske.Scene"))as Scene;
        scene1.Setup (id);


        //大ルーターと小ルーター

        _rootScene.AddChild (scene1);

        string SceneJsonString = Resources.Load ("Router").ToString ();

        //パーサーを書くか。
        /**
         * SceneDTO的な
         * Scene1: {
         *   Scene1_1: {
         *    class: "aaaa"
         *    children: [];
         * }
         * /
        Dictionary<string, object> SceneJson = JsonUtility.FromJson<Dictionary<string, object>>(SceneJsonString);


        _rootScene.SetupRoot (SceneJson, ()=>{
            _rootScene.Router.Goto ("Scene1");
        });
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
