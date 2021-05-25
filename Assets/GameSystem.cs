using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSystem : MonoBehaviour
{
	//　スタートボタンを押したら'Stage01画面'に遷移
	public void StartGame()
	{
		SceneManager.LoadScene("Stage01");
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
}
