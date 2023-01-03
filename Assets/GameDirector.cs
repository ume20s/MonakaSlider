using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameDirector : MonoBehaviour
{
    private int zanki = 4;                  // 着目機（プラス１すると残機数）
    private int phase = 0;                  // 場面状態
    private bool inAnime = false;           // もなかアニメ中

    // ゲームオブジェクト
    GameObject[] monaka = new GameObject[5];
    GameObject[] person = new GameObject[3];
    GameObject monako;
    GameObject clearArea;
    GameObject tapmes;
    GameObject swipemes;
    GameObject TextScore;
    GameObject TextHighScore;
    GameObject TextPoint;
    GameObject wallend;

    GameObject canvas;

    // スワイプ量計測
    float startPos = 0.0f;
    float endPos = 0.0f;

    // もろもろ
    float speed = 0f;
    int point = 0;
    float center;
    int score;

    // Start is called before the first frame update
    void Start()
    {
        // オブジェクトの取得
        monaka[0] = GameObject.Find("monaka1");
        monaka[1] = GameObject.Find("monaka2");
        monaka[2] = GameObject.Find("monaka3");
        monaka[3] = GameObject.Find("monaka4");
        monaka[4] = GameObject.Find("monaka5");
        person[0] = GameObject.Find("sonota0");
        person[1] = GameObject.Find("sonota1");
        person[2] = GameObject.Find("sonota2");
        monako = GameObject.Find("monako");
        clearArea = GameObject.Find("clear");
        tapmes = GameObject.Find("tap");
        swipemes = GameObject.Find("swipe");
        TextScore = GameObject.Find("Score");
        TextHighScore = GameObject.Find("HighScore");
        TextPoint = GameObject.Find("Point");
        wallend = GameObject.Find("wallEnd");
        canvas = GameObject.Find("Canvas");

        // もなこちゃんとクリアエリアの配置
        int pos = Random.Range(0,3);
        person[pos].SetActive(false);
        monako.transform.Translate(4.0f * (float)pos, 0, 0);
        clearArea.transform.Translate(4.0f * (float)pos, 0, 0);

        // クリアエリアのセンターの計算
        center = (float)(pos * 4 - 1);

        // もなこちゃんの頭上に得点表示
        Vector2 pPos;
        RectTransform TextPointRect = TextPoint.GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            RectTransformUtility.WorldToScreenPoint(Camera.main, new Vector2(center, 3.5f)), 
            null, out pPos
        );
        TextPointRect.localPosition = pPos;

        // スワイプメッセージは消しておく
        swipemes.SetActive(false);
    }

    void Update()
    {
        switch (phase)
        {
            // ゲーム開始（もなかセット前）
            case 0:
                if (Input.GetMouseButtonDown(0))
                {
                    // もなか落ちアニメーション
                    tapmes.SetActive(false);
                    inAnime = true;
                    StartCoroutine("MonakaSet");
                    StartCoroutine("StockDown");
                    phase++;
                }
                break;

            // もなか落ちアニメーションが終わるまで待つ
            case 1:
                if (!inAnime)
                {
                    swipemes.SetActive(true);
                    phase++;
                }
                break;

            // スワイプ開始
            case 2:
                if (Input.GetMouseButtonDown(0))
                {
                    // スワイプ開始位置を取得
                    startPos = Input.mousePosition.x;
                    phase++;
                }
                break;

            // スワイプ終了
            case 3:
                if (Input.GetMouseButtonUp(0))
                {
                    // スワイプ終了位置を取得
                    endPos = Input.mousePosition.x;

                    // スワイプ距離からもなか速度を決定
                    float swipeLength = endPos - startPos;
                    speed = swipeLength / 500.0f;   // 500.0って値はさじ加減
                    swipemes.SetActive(false);
                    phase++;
                }
                break;

            // もなか移動中
            case 4:
                // もなかに速度付加してちょっとだけ速度を遅くする
                monaka[zanki].transform.Translate(speed, 0, 0);
                speed *= 0.98f;

                // 得点判定
                point = 100 - (int)(Mathf.Abs(monaka[zanki].transform.position.x - center) * 100 / 2.8f);
                if (point < 0) point = 0; 
                TextPoint.GetComponent<Text>().text = point.ToString("D") + "point";

                // もなかが一定速度以下になるか画面外に出たら次の段階
                if (speed < 0.001 || monaka[zanki].transform.position.x > wallend.transform.position.x)
                {
                    phase++;
                }
                break;

            // 判定！
            case 5:
                // もし得点が０だったら
                if(point == 0)
                {
                    // 残機が残っていたら即刻もう１回
                    if (zanki > 0) {
                        monaka[zanki].SetActive(false);
                        zanki--;
                        tapmes.SetActive(true);
                        phase = 0;
                    }
                    else
                    {
                        // ゲームオーバー表示
                        // タップして次のステージ
                        // 未実装
                    }

                }
                else
                {
                    // ステージクリア表示
                    // 未実装

                    // タップしてスコア転送
                    if (Input.GetMouseButtonDown(0))
                    {
                        StartCoroutine("PointAdd");
                    }
                }
                break;

            default:
                break;
        }
    }

    // 一番下のがテーブルに大きくなりながら落ちる
    IEnumerator MonakaSet()
    {
        for (int i = 0; i < 10; i++)
        {
            monaka[zanki].transform.Translate(0, -0.25f, 0);
            monaka[zanki].transform.localScale = new Vector2(0.5f + (float)i * 0.05f, 0.5f + (float)i * 0.05f);
            yield return new WaitForSeconds(0.01f);
        }
    }

    // ストックを１個分下に落とす
    IEnumerator StockDown()
    {
        if (zanki == 0)
        {
            inAnime = false;
        }
        else
        {
            for (int i = zanki - 1; i >= 0; i--)
            {
                for (int j = 0; j < 10; j++)
                {
                    monaka[i].transform.Translate(0, -0.1f, 0);
                    yield return new WaitForSeconds(0.01f);
                }
            }
            inAnime = false;
        }
    }

    // ポイントをスコアに移動追加
    IEnumerator PointAdd()
    {
        for (int i = point; i >= 0; i--)
        {
            TextPoint.GetComponent<Text>().text = i.ToString("D") + "point";
            TextScore.GetComponent<Text>().text = "Score:" + score.ToString("D4");
            score++;
            yield return new WaitForSeconds(0.05f);
        }
    }
}