using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameDirector : MonoBehaviour
{
    private int zanki = 4;          // ���ڋ@�i�v���X�P����Ǝc�@���j
    private bool inPlay = false;    // �X���C�v�\���
    private bool inTry = false;     // ���Ȃ��ړ���

    // �Q�[���I�u�W�F�N�g
    GameObject[] monaka = new GameObject[5];
    GameObject score;
    GameObject highscore;

    // �X���C�v�ʌv��
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
                // ���Ȃ������A�j���[�V����
                StartCoroutine("MonakaSet");
                StartCoroutine("StockDown");
            }
        }
    }

    // ��ԉ��̂��e�[�u���ɑ傫���Ȃ�Ȃ��痎����
    IEnumerator MonakaSet()
    {
        for(int i = 0; i < 10; i++)
        {
            monaka[zanki].transform.Translate(0, -0.25f, 0);
            monaka[zanki].transform.localScale = new Vector2(0.5f+(float)i*0.05f, 0.5f+(float)i*0.05f);
            yield return new WaitForSeconds(0.01f);
        }
    }

    // �X�g�b�N���P�����ɗ��Ƃ�
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
