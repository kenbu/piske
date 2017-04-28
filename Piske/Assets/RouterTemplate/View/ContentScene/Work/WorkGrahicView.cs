using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WorkGrahicView : MonoBehaviour {

    [SerializeField]
    private Button _backButton;
    [SerializeField]
    private Button _work1;
    [SerializeField]
    private Button _work2;
    [SerializeField]
    private Button _work3;

    public System.Action OnBack;
    public System.Action<string> OnShowDetail;

    private void Awake(){
        _backButton.onClick.AddListener (()=>{
            OnBack.Invoke ();
        });

        _work1.onClick.AddListener (()=>{
            OnShowDetail.Invoke ("item1");
        });
        _work2.onClick.AddListener (()=>{
            OnShowDetail.Invoke ("item2");
        });
        _work3.onClick.AddListener (()=>{
            OnShowDetail.Invoke ("item3");
        });

    }

    public void Show(){
        gameObject.SetActive (true);
    }

    public void Hide(){
        gameObject.SetActive (false);
    }
}
