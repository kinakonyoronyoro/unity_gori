using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Player : MonoBehaviour
{
    #region    //インスペクターで調整する

    [Header("移動速度")] public float speed;

    [Header("ジャンプ速度")] public float jumpSpeed;

    [Header("ジャンプする高さ")] public float jumpHeight;

    [Header("踏みつけ判定の高さの割合")] public float stepOnRate;

    [Header("ジャンプ制限時間")] public float jumpLimitTime;

    [Header("重力")] public float gravity;

    [Header("着地判定")] public GroundCheck ground;

    [Header("頭をぶつけた判定")] public GroundCheck head;

    [Header("ダッシュの速さ表現")] public AnimationCurve dashCurve;

    [Header("ジャンプの速さ表現")] public AnimationCurve JumpCurve;
    #endregion


    #region//プライベート変数
    private Rigidbody2D rb2d;

    private Animator anim = null;

    private CapsuleCollider2D capcol = null;

    private SpriteRenderer sr = null;

    private bool isGround = false;

    private bool isHead = false;

    private bool isJump = false;

    private bool isWalke = false;

    private bool isDamege = false;

    private bool isOtherJump = false;

    private bool isContinue = false;

    private bool nonDownAnim = false;

    private float continueTime = 0.0f;

    private float blinkTime = 0.0f;

    private float jumpPos = 0.0f;

    private float otherJumpHeight = 0.0f;

    private float jumpTime = 0.0f;

    private float dashTime = 0.0f;

    private float beforeKey = 0.0f;

    private string enemyTag = "Enemy";

    private string damegeMapTag = "Damege";

    private string deadAreaTag = "DeadArea";

    AudioSource audioSource;
    #endregion


    void Start()
    {
        //コンポーネントのインスタンスを捕まえる
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        capcol = GetComponent<CapsuleCollider2D>();
        sr = GetComponent<SpriteRenderer>();

        //SE取得（Soundファイルから）
        Sound.LoadSe("dead", "dead");

        //BGM取得
        audioSource = GetComponent<AudioSource>();

    }

    private void Update()
    {
        //フラグがオンの時、時間経過でオンとオフの切り替えをする
        if(isContinue)
        {
            //明滅　ついてる時の戻る
            if (blinkTime > 0.2f)
            {
                sr.enabled = true;
                blinkTime = 0.0f;
            }
            //明滅　消えているとき
            else if(blinkTime > 0.1f)
            {
                sr.enabled = false;
            }
            //明滅　ついているとき
            else
            {
                sr.enabled = true;
            }
            //1秒たったら明滅終わり（演出終了）
            if (continueTime > 1.0f)
            {
                //演出が終わったらリセットする
                isContinue = false;
                blinkTime = 0.0f;
                continueTime = 0.0f;
                sr.enabled = true;
            }
            else
            {
                //演出中は演出用の時間を進める
                blinkTime += Time.deltaTime;
                continueTime += Time.deltaTime;
            }
        }
    }

    void FixedUpdate()
    {
        if (!isDamege && !Gmanager.instance.isGemeOver)
        {
            //着地判定を受け取る
            isGround = ground.IsGround();
            isHead = head.IsGround();


            float xSpeed = GetXSpeed();
            float ySpeed = GetYSpeed();

            //アニメーションを適用
            SetAnimation();

            //移動速度を設定
            rb2d.velocity = new Vector2(xSpeed, ySpeed);
        }
        else
        {
            //ダメージ受けた場合重力だけが適用する(落下以外は行動不能とする）
            rb2d.velocity = new Vector2(0, -gravity);
        }
 
        
    }

    /// <summary>
    /// Y成分で必要な計算をし、速度を返す
    /// </summary>
    /// <returns>Y軸の速さ</returns>
    private float GetYSpeed()
    {
        float verticalKey = Input.GetAxis("Vertical");
        float ySpeed = -gravity;

        //何かを踏みつけたとき少し跳ねる処理（注意：地面にいる時の処理よりも上に記述すること）
        if (isOtherJump)
        {

            //現在の高さが飛べる高さより下か
            bool canHeight = jumpPos + jumpHeight > transform.position.y;

            //ジャンプ時間が長くなりすぎていないか
            bool canTime = jumpLimitTime > jumpTime;

            if ( canHeight && canTime && !isHead)
            {
                ySpeed = jumpSpeed;
                jumpTime += Time.deltaTime;
            }
            else
            {
                isOtherJump = false;
                jumpTime = 0.0f;
            }
        }

        //ジャンプ処理(地面についているとき）
        else if (isGround)
        {
            //かつ、上方向キーが押されているとき
            if (verticalKey > 0)
            {
                ySpeed = jumpSpeed;
                jumpPos = transform.position.y;//ジャンプした位置を記録する
                isJump = true;
                jumpTime = 0.0f;//ジャンプ時間をリセット
            }
            else
            {
                isJump = false;
            }
        }
        //ジャンプ中
        else if (isJump)
        {
            //上方向キーを押しているか
            bool pushUpKey = verticalKey > 0;

            //現在の高さが飛べる高さより下か
            bool canHeight = jumpPos + jumpHeight > transform.position.y;

            //ジャンプ時間が長くなりすぎていないか
            bool canTime = jumpLimitTime > jumpTime;

            if (pushUpKey && canHeight && canTime && !isHead)
            {
                ySpeed = jumpSpeed;
                jumpTime += Time.deltaTime;
            }
            else
            {
                isJump = false;
                jumpTime = 0.0f;
            }
        }

        if (isJump || isOtherJump)
        {
            ySpeed *= JumpCurve.Evaluate(jumpTime);
        }

        return ySpeed;

    }

    /// <summary>
    /// X成分で必要な計算をし、速度を返す
    /// </summary>
    /// <returns>X軸の速さ</returns>
    private float GetXSpeed() 
    {
        //キー入力されたら行動する
        float horizontalKey = Input.GetAxis("Horizontal");
        float xSpeed = 0.0f;

        //右のキーが押されている場合
        if (horizontalKey > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);//向きの変更
            isWalke = true;
            dashTime += Time.deltaTime;
            xSpeed = speed;
        }
        //左のキーが押されている場合
        else if (horizontalKey < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);//向きの変更
            isWalke = true;
            dashTime += Time.deltaTime;
            xSpeed = -speed;
        }
        //入力なし
        else
        {
            isWalke = false;
            xSpeed = 0.0f;//何も押していないとき速度を0に変更
            dashTime = 0.0f;
           
        }

        //前回の入力からダッシュの反転を判定して速度を変える
        if (horizontalKey > 0 && beforeKey < 0)
        {
            dashTime = 0.0f;
        }
        else if (horizontalKey < 0 && beforeKey > 0)
        {
            dashTime = 0.0f;
        }

        beforeKey = horizontalKey;

        //アニメーションカーブを速度に適用
        xSpeed *= dashCurve.Evaluate(dashTime);
        beforeKey = horizontalKey;

        return xSpeed;

    }

    /// <summary>
    /// アニメーションを設定する
    /// </summary>
    private void SetAnimation()
    {
        anim.SetBool("isJump", isJump || isOtherJump);
        anim.SetBool("ground", isGround);
        anim.SetBool("Walke", isWalke);
    }




    //トリガーがオンの時に発動
    //通り抜けたかどうか判定
    private void OnTriggerEnter2D(Collider2D collision)
    {
       
        if (collision.tag == deadAreaTag)
        {
            ReceiveDamage(false);
        }
        else if(collision.tag == damegeMapTag)
        {
            ReceiveDamage(true);
        }

        //ゴール判定
        if (collision.gameObject.tag == "Clear")//ゴールのオブジェクトに接触したらクリアシーンへの切り替え
        {
            //クリアと同時にBGMストップ
            audioSource.Stop();
            //Stage02に遷移
            SceneManager.LoadScene("Stage2");
        }


        //ゴール判定02
        if (collision.gameObject.tag == "Clear2")//ゴールのオブジェクトに接触したらクリアシーンへの切り替え
        {
            //クリアと同時にBGMストップ
            audioSource.Stop();
            //Stage03に遷移
            SceneManager.LoadScene("Stage3");
        }

        //ゴール判定03
        if (collision.gameObject.tag == "Clear3")//ゴールのオブジェクトに接触したらクリアシーンへの切り替え
        {
            //クリアと同時にBGMストップ
            audioSource.Stop();
            //クリア画面に遷移
            SceneManager.LoadScene("Clear");
        }
    }


    
    //乗ったかどうか判定
    private void OnCollisionEnter2D(Collision2D collision)
    { 
      if(collision.collider.tag == enemyTag)
        {
            //踏みつけ判定になる高さ
            float stepOnHeight = (capcol.size.y * (stepOnRate / 100f));

            //踏みつけ判定のワールド座標
            float judgePos = transform.position.y - (capcol.size.y / 2f) + stepOnHeight;

            foreach (ContactPoint2D p in collision.contacts) 
            {
                if (p.point.y < judgePos)
                {
                    //もう一度跳ねる
                    ObjectCollision o = collision.gameObject.GetComponent<ObjectCollision>();
                    if(o != null)
                    {
                        otherJumpHeight = o.boundHeight;//踏んずけた物から跳ねる高さを取得する
                        o.playerStepOn = true;//踏んずけた物に対して踏んづけたことを通知する
                        jumpPos = transform.position.y;//ジャンプした位置を記録する
                        isOtherJump = true;
                        isJump = false;
                        jumpTime = 0.0f;

                    }
                    else 
                    {
                        Debug.Log("ObjectCollisionがついてないよ！");
                    }

                }
                else
                {
                    ReceiveDamage(true);
                    break;//Damege時ループを抜ける
                    
                }
            }
           
        }
    }
    /// <summary>
    /// コンテニュー待機状態か
    /// </summary>
    /// <returns></returns>
    public bool IsContinueWaiting()
    {
        if(Gmanager.instance.isGemeOver)//ゲームオーバーならコンテニューしない
        {
            return false;
        }
        else
        {
            return IsDownAnimEnd() || nonDownAnim;
        }
        
    }

    //ダウンアニメーションが完了しているかどうか
    //完了ならtrueを返す
    private bool IsDownAnimEnd()
    {
        //やられているか判定
        if(isDamege && anim != null)
        {
            //現在再生しているアニメーションの状況取得
            AnimatorStateInfo currentState = anim.GetCurrentAnimatorStateInfo(0);
            if(currentState.IsName("Damege"))
            {
                //normalizedTimeの '1'=100%再生が完了している（アニメーションが終わっている）
                if (currentState.normalizedTime >= 1)
                {
                    return true;
                }
            }
        }
        return false;
    }
    /// <summary>
    /// コンテニューする
    /// </summary>
    public void ContinuePlayer()
    {
        //各フラグをリセットして最初の状態に戻す
        isDamege = false;
        anim.Play("Idole");
        isJump = false;
        isOtherJump = false;
        isWalke = false;
        isContinue = true;
        nonDownAnim = false;
    }

    /// <summary>
    /// やられた処理
    /// </summary>
    /// <param name="dounAnim"></param>
    private void ReceiveDamage(bool downAnim)
    {
        if (isDamege)
        {
            return;
        }
        else
        {
            //ダウンアニメーションをするかどうか切り分け
            if(downAnim)
            {
                anim.Play("Damege");

            }
            else
            {
                nonDownAnim = true;
            }
            isDamege = true;
            Gmanager.instance.SubHeartNum();
        }
       
    }
    


}
