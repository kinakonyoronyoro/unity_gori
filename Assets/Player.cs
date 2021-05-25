using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Player : MonoBehaviour
{
    //インスペクターで調整する
    public float speed = 10;

    public GroundCheck ground;

    public float JumpForce = 1500;

    public LayerMask groundLayer;

    //プライベート変数
    private Rigidbody2D rb2d;

    private Animator anim = null;

    private bool isGround = false;

    private SpriteRenderer spRenderer;

    

    private bool isSloped;

    private bool isDead = false;

    AudioSource audioSource;






    // Start is called before the first frame update
    void Start()
    {
        //コンポーネントのインスタンスを捕まえる
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spRenderer = GetComponent<SpriteRenderer>();
        //SE取得（Soundファイルから）
        Sound.LoadSe("dead", "dead");
        //BGM取得
        audioSource = GetComponent<AudioSource>();



    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //設置判定を受け取る
        isGround = ground.IsGround();


        //キー入力されたら行動する
        float horizontalKey = Input.GetAxis("Horizontal");
        float xSpeed = 0.0f;

        //右のキーが押されている場合
        if (horizontalKey > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);//向きの変更
            anim.SetBool("Walke", true);
            xSpeed = speed;
        }
        //左のキーが押されている場合
        else if (horizontalKey < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);//向きの変更
            anim.SetBool("Walke", true);
            xSpeed = -speed;
        }
        //入力なし
        else
        {
            anim.SetBool("Walke", false);
            xSpeed = 0.0f;//何も押していないとき速度を0に変更
        }
        rb2d.velocity = new Vector2(xSpeed, rb2d.velocity.y);
        

       
        
    }

    

     



       

    IEnumerator Dead()
    {
        anim.SetBool("isDamege", true);

        yield return new WaitForSeconds(0.5f);


        //やられた感を出すためにちょっと上に上がる
        rb2d.AddForce(Vector2.up * JumpForce * 5);

        rb2d.AddForce(Vector2.up * JumpForce * 5);
        //ダメージを受けた後下に落ちるようにするため、当たり判定をOFFにする
        GetComponent<CircleCollider2D>().enabled = false;

        //BGMを停止させる
        audioSource.Stop();

        //死んだときのSEが鳴る
        Sound.PlaySe("Dead", 0);

        //ゲームオーバー画面に遷移
        SceneManager.LoadScene("GameOver");


    }


    //トリガーがオンの時に発動
    //通り抜けたかどうか判定
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {

            isDead = true;
            StartCoroutine("Dead");
        }

        //ゴール判定
        if (collision.gameObject.tag == "Clear")//ゴールのオブジェクトに接触したらクリアシーンへの切り替え
        {
            //クリアと同時にBGMストップ
            audioSource.Stop();
            //Stage02に遷移
            SceneManager.LoadScene("Stage02");
        }


        //ゴール判定02
        if (collision.gameObject.tag == "Clear02")//ゴールのオブジェクトに接触したらクリアシーンへの切り替え
        {
            //クリアと同時にBGMストップ
            audioSource.Stop();
            //Stage03に遷移
            SceneManager.LoadScene("Stage03");
        }

        //ゴール判定03
        if (collision.gameObject.tag == "Clear03")//ゴールのオブジェクトに接触したらクリアシーンへの切り替え
        {
            //クリアと同時にBGMストップ
            audioSource.Stop();
            //クリア画面に遷移
            SceneManager.LoadScene("Clear");
        }
    }


    //ダメージ床の判定
    //乗ったかどうか判定
    void OnCollisionEnter2D(Collision2D collision)
    {
        //敵に乗った時ジャンプする
        if (collision.gameObject.tag == "Enemy")
        {
            anim.SetBool("isJump", true);
            rb2d.AddForce(Vector2.up * JumpForce);
        }

        //当たったタグを見てダメージマップか判定をする
        if (collision.gameObject.tag == "Damage")
        {
            isDead = true;
            //コルーチンの呼び出しをする
            StartCoroutine("Dead");
        }

     
    }



}
