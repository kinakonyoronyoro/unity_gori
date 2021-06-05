using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class Gem : MonoBehaviour
{
    
    

    private SpriteRenderer spriteRenderer;

    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        //プレーヤーを探す
        player = GameObject.Find("Player");
        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    //コライダーを通り抜けたらイベント（星ゲット）
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            
            //今の場所にVector3.up（上方向）をプラス
            //星が上に上がる
            Vector3 pos = transform.localPosition + Vector3.up * 1.5f;
            transform.DOLocalMove(pos, 0.25f);//その場所に0.25秒かけて移動

            //星が回転
            transform.DORotate(new Vector3(0, 180, 0), 1.0f);

            //星が徐々にフェードアウトする
            spriteRenderer.DOFade(0, 1.5f);

            //星のオブジェクト削除(すぐに消えないよう2秒後に消えるように設定)
            Invoke("DestoroyObject", 2); ;

        }
    }

    void DestroyObject()
    {
        Destroy(this.gameObject);
    }
}
