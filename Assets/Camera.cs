using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    //プレーヤーオブジェクトを取得
    public GameObject Player;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //カメラ座標設定（カメラがプレーヤーについてくるようにする
        transform.position = new Vector3(
            Player.transform.position.x,    //プレイヤーのX座標（yとzはそのまま）
            transform.position.y,
            transform.position.z
            );

        //画面の端ではカメラがついてこない(もし、x座標が0より小さかったらx座標を0にする)
        if (transform.position.x < 0)
        {
            transform.position = new Vector3(
            0,
            transform.position.y,
            transform.position.z
            );
        }
    }
}
