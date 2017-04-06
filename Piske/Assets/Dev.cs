using UnityEngine;
using System.Collections;
using kenbu.URouter;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

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

        _rootScene.SetupRoot (()=>{
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
