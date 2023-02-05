using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game8Director : MonoBehaviour
{
    private int zanki = 4;                  // 着目機（プラス１すると残機数）
    private int phase = 0;                  // 場面状態
    private bool inAnime = false;           // アニメ中
    private bool passingArea = false;       // クリアエリア通過フラグ
    const string SAVE_KEY = "HighScore";    // ハイスコア保存キー

    // ゲームオブジェクト
    GameObject[] monaka = new GameObject[5];
    GameObject[] person = new GameObject[10];
    GameObject monako;
    GameObject clearArea;
    GameObject tapmes;
    GameObject swipemes;
    GameObject TextScore;
    GameObject TextHighScore;
    GameObject TextPoint;
    GameObject TextClear;
    GameObject wallend;
    GameObject canvas;

    // もなこアニメーション用animator
    Animator animator;

    // お菓子のスプライト
    public Sprite[] Sweets = new Sprite[4];

    // 効果音関連
    AudioSource audioSource;
    public AudioClip[] vOisii = new AudioClip[4];
    public AudioClip vTabetaina;
    public AudioClip vMouikkai;
    private int[] ClearVoice = new int[5] {0,0,0,0,0};

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
        person[3] = GameObject.Find("sonota3");
        person[4] = GameObject.Find("sonota4");
        person[5] = GameObject.Find("sonota5");
        person[6] = GameObject.Find("sonota6");
        person[7] = GameObject.Find("sonota7");
        person[8] = GameObject.Find("sonota8");
        person[9] = GameObject.Find("sonota9");
        monako = GameObject.Find("monako");
        clearArea = GameObject.Find("clear");
        tapmes = GameObject.Find("tap");
        swipemes = GameObject.Find("swipe");
        TextScore = GameObject.Find("Score");
        TextHighScore = GameObject.Find("HighScore");
        TextPoint = GameObject.Find("Point");
        TextClear = GameObject.Find("Clear");
        wallend = GameObject.Find("wallEnd");
        canvas = GameObject.Find("Canvas");

        // スコアとハイスコアの読み込み
        score = ScoreStrage.Score;
        TextScore.GetComponent<Text>().text = "Score:" + ScoreStrage.Score.ToString("D4");
        TextHighScore.GetComponent<Text>().text = "HighScore:" + ScoreStrage.HighScore.ToString("D4");

        // もなこちゃんとクリアエリアの配置
        int pos = Random.Range(0, 10);
        person[pos].SetActive(false);
        monako.transform.Translate(1.2f * (float)pos, 0, 0);
        clearArea.transform.Translate(1.2f * (float)pos, 0, 0);

        // クリアエリアのセンターの計算
        center = (float)pos * 1.2f - 2.5f;

        // もなこちゃんの頭上に得点表示
        Vector2 pPos;
        RectTransform TextPointRect = TextPoint.GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            RectTransformUtility.WorldToScreenPoint(Camera.main, new Vector2(center, 0.9f)), 
            null, out pPos
        );
        TextPointRect.localPosition = pPos;

        // もなこちゃんアニメーションコンポーネントを取得
        animator = monako.GetComponent<Animator>();

        // 音声のコンポーネントを取得
        audioSource = GetComponent<AudioSource>();

        // 余計なものは消しておく
        swipemes.SetActive(false);
        TextClear.SetActive(false);

        // ３分の１の確率でランダムに１個だけ鯱もなか以外のものが入る
        if(Random.Range(0, 3) == 0)
        {
            int sPos = Random.Range(0, 5);
            int sSweets = Random.Range(1, 4);
            monaka[sPos].GetComponent<SpriteRenderer>().sprite = Sweets[sSweets];
            ClearVoice[sPos] = sSweets;
        }

        // 鯱もなかが食べたいセリフ
        audioSource.PlayOneShot(vTabetaina);
    }

    void Update()
    {
        switch (phase)
        {
            // ゲーム開始（もなかセット前）
            case 0:
                if (Input.GetMouseButtonDown(0))
                {
                    audioSource.Play();

                    // 余計なものは非表示に
                    tapmes.SetActive(false);

                    // もなか落ちアニメーション
                    inAnime = true;
                    StartCoroutine("MonakaSet");
                    StartCoroutine("StockDown");

                    // もなこちゃんセッティングアニメに遷移
                    animator.SetTrigger("SettingTrigger");
                    passingArea = false;

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
                point = 100 - (int)(Mathf.Abs(monaka[zanki].transform.position.x - center) * 100 / 1.3f);
                if (point > 1) passingArea = true;
                if (point < 0)
                {
                    point = 0;
                    if(passingArea)
                    {
                        // もなこちゃん失敗アニメに遷移
                        animator.SetTrigger("MissTrigger");
                        inAnime = true;
                        passingArea = false;
                    }
                } 
                TextPoint.GetComponent<Text>().text = point.ToString("D") + "point";

                // もなかが一定速度以下になるか画面外に出たら次の段階
                if (speed < 0.002 || monaka[zanki].transform.position.x > wallend.transform.position.x - 1.0f)
                {
                    phase++;
                }
                break;

            // 判定！
            case 5:
                audioSource.Stop();     // BGMを止める

                // もし得点が０だったら
                if (point == 0)
                {
                    // アニメしてなかったらもなこちゃん失敗アニメに遷移
                    if(!inAnime)
                    {
                        animator.SetTrigger("MissTrigger2");
                    }
                    inAnime = false;

                    // 残機が残っていたらもう１回
                    if (zanki > 0) {
                        audioSource.PlayOneShot(vMouikkai);
                        phase = 6;
                    }
                    else
                    {
                        // ゲームオーバー
                        phase = 7;
                    }

                }
                else
                {
                    // もなこちゃんステージクリアアニメに遷移
                    monaka[zanki].SetActive(false);
                    animator.SetTrigger("ClearTrigger");

                    // クリア表示
                    TextClear.SetActive(true);

                    // 「おいしい」スコア転送
                    audioSource.PlayOneShot(vOisii[ClearVoice[zanki]]);
                    StartCoroutine("PointAdd");

                    // ステージクリアへ
                    phase = 8;
                }
                break;

            // もう１回
            case 6:
                // 失敗もなかを消して残機減
                monaka[zanki].SetActive(false);
                zanki--;

                // タップしてセットを表示
                tapmes.SetActive(true);

                // 最初に戻る
                phase = 0;

                break;

            // ゲームオーバー
            case 7:
                // ゲームオーバー画面へ
                SceneManager.LoadScene("GameoverScene");
                break;

            // ステージクリア
            case 8:
                // タップして次のステージへ
                if (Input.GetMouseButtonDown(0))
                {
                    // スコアにポイントを加算
                    score += point;

                    // ハイスコア処理
                    if (score > ScoreStrage.HighScore)
                    {
                        ScoreStrage.HighScore = score;
                        TextHighScore.GetComponent<Text>().text = "HighScore:" + score.ToString("D4");
                        PlayerPrefs.SetInt(SAVE_KEY, score);
                        PlayerPrefs.Save();
                    }

                    // 現在のスコアを次のステージに受け継ぐ
                    ScoreStrage.Score = score;

                    // 次のステージへ
                    SceneManager.LoadScene("Game9Scene");
                }
                break;

            default:
                break;
        }
    }

    // 一番下のが大きくなりながらテーブルに落ちる
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
        int i, div, dscore = score;

        // スコアを拡大表示
        for(i=0; i<10; i++)
        {
            TextPoint.GetComponent<Text>().fontSize +=3;
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(1.5f);

        // スコアをポイントエリアに移動追加
        i = point;
        while(i > 0)
        {
            if(i > 30)
            {
                div = 10;
            } else if(i > 10)
            {
                div = 5;
            } else
            {
                div = 1;
            }
            dscore += div;
            i -= div;
            TextPoint.GetComponent<Text>().text = i.ToString("D") + "point";
            TextScore.GetComponent<Text>().text = "Score:" + dscore.ToString("D4");
            yield return new WaitForSeconds(0.05f);
        }
    }
}