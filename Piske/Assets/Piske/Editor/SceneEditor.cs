using UnityEditor;                              
using kenbu.Piske;
using UnityEngine;

[CustomEditor(typeof(SceneController))]               
public class SceneControllerEditor : Editor          
{
    private static string _url = "URL";
    public override void OnInspectorGUI()
    {
        SceneController sceneController = target as SceneController;
        //Current
        EditorGUILayout.LabelField ("Url", sceneController.Current);

        //
        _url = EditorGUILayout.TextField ("Url", _url);

        if( GUILayout.Button( "GOTO" ) )
        {
            sceneController.Goto (_url);
        }

    }


        
}
