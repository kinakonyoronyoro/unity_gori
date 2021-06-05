using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSystem : MonoBehaviour
{
	[Header("フェード")] public FadeImage fade;

	private bool firstPush = false;
	private bool goNextScene = false;

	//　スタートボタンを押したら'Stage01画面'に遷移
	public void StartGame()
	{
		if(!firstPush)
        {
			//スタートボタンを押したらフェードアウト実行
			fade.StartFadeOut();
			firstPush = true;
		}
		
	}

	//　タイトルボタンを押したら'タイトル画面'に遷移
	public void ReStartGame()
	{
		SceneManager.LoadScene("Title");
	}

	//　ゲーム終了ボタンを押したら実行する（ゲーム終了）
	public void EndGame()
	{

#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
		Application.OpenURL("http://www.yahoo.co.jp/");
#else
		Application.Quit();
#endif
	}

    private void Update()
    {
        if(!goNextScene && fade.IsFadeOutComplete())
        {
			SceneManager.LoadScene("Stage1");
			goNextScene = true;
		}
    }
}
