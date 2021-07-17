using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantCtrl : MonoBehaviour
{
    [Header("加算スコア")] public int myScore;
    [Header("攻撃された時に再生するSE")] public AudioClip Enemy_Damege;

    private Animator anim;

    public GameObject player;

    private bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        //プレーヤーを探す
        player = GameObject.Find("Player");
      

        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //プレイヤーの座標と自分の座標を用意
        Vector2 pPos = player.transform.position;
        Vector2 myPos = transform.position;

        //距離がわかるDistance関数を使用
        float distance = Vector2.Distance(pPos, myPos);

        //プレイヤーと距離が小さくなったら攻撃アニメーションする(+頭上にいるときは攻撃しない)
        if (distance < 2 & (pPos.y - myPos.y) < 1)
        {
            anim.SetTrigger("TrgAttack");
        }

        //死んでいたら点滅
        if (isDead)
        {
            //Sin関数で点滅させてる
            float level = Mathf.Abs(Mathf.Sin(Time.time * 20));
            GetComponent<SpriteRenderer>().color =
                new Color(1f, 1f, 1f, level);
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Gmanager.instance.PlaySE(Enemy_Damege);
        anim.SetTrigger("TrgDead");
        //当たり判定無効
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;
        //死んだら消える
        StartCoroutine("Dead");
    }

    IEnumerator Dead()
    {
        isDead = true;
        //ウェイト入れる
        yield return new WaitForSeconds(1.5f);

        
        ////スコア加算の処理
        if (Gmanager.instance != null)
        {
            Gmanager.instance.score += myScore;//スコア加算の処理
        }
        //Plantオブジェクトの消去
        Destroy(gameObject);
    }
}
