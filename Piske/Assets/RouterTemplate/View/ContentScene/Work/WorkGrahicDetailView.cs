using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class WorkGrahicDetailView : MonoBehaviour {

    [SerializeField]
    private Button _closeButton;

    [SerializeField]
    private Button _item1;
    [SerializeField]
    private Button _item2;
    [SerializeField]
    private Button _item3;


    [SerializeField]
    private Text _title;
    [SerializeField]
    private Image _image;

    public System.Action OnClose;
    public System.Action<string> OnChangeItem;

    private void Awake(){
        _closeButton.onClick.AddListener (()=>{
            OnClose.Invoke ();
        });
        _item1.onClick.AddListener (()=>{
            OnChangeItem.Invoke ("item1");
        });
        _item2.onClick.AddListener (()=>{
            OnChangeItem.Invoke ("item2");
        });
        _item3.onClick.AddListener (()=>{
            OnChangeItem.Invoke ("item3");
        });
    }

    public void UpdateDate(Dictionary<string, string> query){
        //本当はViewにわたる段階でなんかクラスに変えとく。
        if(query == null) {
            return;
        }
        _title.text = query["id"];
    }

    public void Show(){
        gameObject.SetActive (true);
    }

    public void Hide(){
        gameObject.SetActive (false);
    }
}
