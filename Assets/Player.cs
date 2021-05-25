using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Player : MonoBehaviour
{
    //インスペクターで調整する
    public float speed = 30;

    public float JumpForce = 1500;

    public LayerMask groundLayer;

    //プライベート変数
    private Rigidbody2D rb2d;

    private Animator anim;

    private SpriteRenderer spRenderer;

    private bool isGround;

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
    void Update()
    {

        float x = Input.GetAxisRaw("Horizontal");//左-1・何もしない0・右1

        //スプライトの向きを変える
        if (x < 0)
        {
            spRenderer.flipX = true;
        }
        else if (x > 0)
        {
            spRenderer.flipX = false;
        }

        //死んでなければ動かせる
        if (!isDead)
        {
            //rb2d.velocity = new Vector2(speed, rb2d.velocity.y);
             rb2d.AddForce( Vector2.right  *  x  *  speed );//横方向に力を加える
             anim.SetFloat("Speed", Mathf.Abs(x * speed));//歩くアニメーション
        //ボタンを離すと止まる
        } else {
            rb2d.velocity = Vector2.zero;
        }


        //ジャンプ（縦方向にAddForceを使う）※Input.GetButtonDownでキー入力取得
        //条件追加：ジャンプキーが押された　かつ　地面にいるならジャンプモーション
        if ( Input.GetButtonDown("Jump") & isGround) {
            rb2d.AddForce( Vector2.up  *  JumpForce );
            anim.SetBool( "isJump", true);
        }


        //坂道を進む
        if (isSloped)
        {
            gameObject.transform.Translate(0.9f * x, 0.0f, 0.0f);//物理無視のTranslate使用
        }



        //地面にいるときはジャンプと落ちるモーションをOFFにする
        if (isGround)
        {
            anim.SetBool( "isJump" , false );
            anim.SetBool( "isFall" , false );
        }

        //横移動が早すぎるので早くなりすぎないよう調整(velocity=速さ)

        float velX = rb2d.velocity.x;
        float velY = rb2d.velocity.y;

        if (velY > 0.5f)//velocityが上向きに働いていたらジャンプ
        {
            anim.SetBool("isJump", true);
        }

        if (velY < -0.1f)//velocityが下向きに働いていたら落ちる
        {
            anim.SetBool("isFall", true);
        }

        //Mathf.Abs（絶対値）
        if (Mathf.Abs(velX) > 5)
        {
            if (velX > 5.0f)
            {
                rb2d.velocity = new Vector2(5.0f, velY);
            }
            if (velX < -5.0f)
            {
                rb2d.velocity = new Vector2(-5.0f, velY);
            }
        }
    }

    //地面の当たり判定(プレイヤーの場所を保存　□の大きさを決める)

    private void FixedUpdate()
    {
        isGround = false;

        float x = Input.GetAxisRaw("Horizontal");

        //自分の立っている場所
        Vector2 groundPos =
            new Vector2(
                transform.position.x,
                transform.position.y
        );
        //地面判定エリア
        Vector2 groundArea = new Vector2(1.2f, 1.2f);

        //坂道当たり判定
        Vector2 wallArea1 = new Vector2(x * 0.8f, 0.5f);
        Vector2 wallArea2 = new Vector2(x * 1.3f, 0.8f);

        //壁用用当たり判定
        Vector2 wallArea3 = new Vector2(x * 1.5f, 1.2f);
        Vector2 wallArea4 = new Vector2(x * 2.0f, 0.1f);

        Debug.DrawLine(groundArea, groundPos - wallArea2, Color.red);
        Debug.Log(isGround);
        //Debug.DrawLine(groundPos + wallArea1, groundPos - wallArea2, Color.red);
        // Debug.DrawLine(groundPos + wallArea3, groundPos - wallArea4, Color.red);

        //指定のエリアに触れたらigGroundがtrueになる
        isGround =
            Physics2D.OverlapArea(
                groundPos + groundArea,
                groundPos - groundArea,
                groundLayer
            );

        bool area1 = false;
        bool area2 = false;

        area1 =
            Physics2D.OverlapArea(
                groundPos + wallArea1,
                groundPos + wallArea2,
                groundLayer
                );
        area2 =
           Physics2D.OverlapArea(
               groundPos + wallArea3,
               groundPos + wallArea4,
               groundLayer
               );



        if (!area1 & area2)
        {
            isSloped = true;
        }
        else
        {
            isSloped = false;
        }
        //Debug.Log(isSloped);
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
