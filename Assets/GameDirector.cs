using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameDirector : MonoBehaviour
{
    private int zanki = 4;          // 着目機（プラス１すると残機数）
    private bool inPlay = false;    // スワイプ可能状態
    private bool inTry = false;     // もなか移動中

    // ゲームオブジェクト
    GameObject[] monaka = new GameObject[5];
    GameObject score;
    GameObject highscore;

    // スワイプ量計測
    float startPos = 0.0f;
    float endPos = 0.0f;

    float speed = 0f;

    // Start is called before the first frame update
    void Start()
    {
        monaka[0] = GameObject.Find("monaka1");
        monaka[1] = GameObject.Find("monaka2");
        monaka[2] = GameObject.Find("monaka3");
        monaka[3] = GameObject.Find("monaka4");
        monaka[4] = GameObject.Find("monaka5");
        score = GameObject.Find("Score");
        highscore = GameObject.Find("HighScore");
    }

    void Update()
    {
        if (inPlay)
        {
            if (Input.GetMouseButtonDown(0) && !inTry)
            {
                startPos = Input.mousePosition.x;
            }
            else if (Input.GetMouseButtonUp(0) && !inTry)
            {
                endPos = Input.mousePosition.x;
                float swipeLength = endPos - startPos;
                speed = swipeLength / 500.0f;
                inTry = true;
            }
            monaka[zanki].transform.Translate(speed, 0, 0);
            speed *= 0.98f;
            if(inTry && speed < 0.01)
            {
                speed = 0;
                zanki--;
                inTry = false;
                inPlay = false;
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                // もなか落ちアニメーション
                StartCoroutine("MonakaSet");
                StartCoroutine("StockDown");
            }
        }
    }

    // 一番下のがテーブルに大きくなりながら落ちる
    IEnumerator MonakaSet()
    {
        for(int i = 0; i < 10; i++)
        {
            monaka[zanki].transform.Translate(0, -0.25f, 0);
            monaka[zanki].transform.localScale = new Vector2(0.5f+(float)i*0.05f, 0.5f+(float)i*0.05f);
            yield return new WaitForSeconds(0.01f);
        }
    }

    // ストックを１個分下に落とす
    IEnumerator StockDown()
    {
        for (int i = zanki-1; i >= 0; i--)
        {
            for (int j = 0; j < 10; j++)
            {
                monaka[i].transform.Translate(0, -0.1f, 0);
                yield return new WaitForSeconds(0.01f);
            }
        }
        inPlay = true;
    }
}
