using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    public GameObject player;
    AudioSource audioSource;

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

    //プレイヤーと接触したら次のステージに遷移
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            //クリアと同時にBGMストップ
            //audioSource.Stop();

            //Stage03に遷移
          //  SceneManager.LoadScene("Stage03");
        }
    }
}
