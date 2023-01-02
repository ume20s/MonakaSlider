using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameDirector : MonoBehaviour
{
    private int zanki = 4;                  // ���ڋ@�i�v���X�P����Ǝc�@���j
    private int phase = 0;                  // ��ʏ��
    private bool inAnime = false;           // ���Ȃ��A�j����

    // �Q�[���I�u�W�F�N�g
    GameObject[] monaka = new GameObject[5];
    GameObject[] person = new GameObject[3];
    GameObject[] tpoint = new GameObject[3];
    GameObject monako;
    GameObject clearArea;
    GameObject tapmes;
    GameObject swipemes;
    GameObject TextScore;
    GameObject TextHighScore;
    GameObject TextPoint;
    GameObject wallend;

    // �X���C�v�ʌv��
    float startPos = 0.0f;
    float endPos = 0.0f;

    // �������
    float speed = 0f;
    int point = 0;
    float center;
    int score;

    // Start is called before the first frame update
    void Start()
    {
        // �I�u�W�F�N�g�̎擾
        monaka[0] = GameObject.Find("monaka1");
        monaka[1] = GameObject.Find("monaka2");
        monaka[2] = GameObject.Find("monaka3");
        monaka[3] = GameObject.Find("monaka4");
        monaka[4] = GameObject.Find("monaka5");
        person[0] = GameObject.Find("sonota0");
        person[1] = GameObject.Find("sonota1");
        person[2] = GameObject.Find("sonota2");
        tpoint[0] = GameObject.Find("Point0");
        tpoint[1] = GameObject.Find("Point1");
        tpoint[2] = GameObject.Find("Point2");
        monako = GameObject.Find("monako");
        clearArea = GameObject.Find("clear");
        tapmes = GameObject.Find("tap");
        swipemes = GameObject.Find("swipe");
        TextScore = GameObject.Find("Score");
        TextHighScore = GameObject.Find("HighScore");
        wallend = GameObject.Find("wallEnd");

        // �Ƃ肠�������_�\��������
        for(int i=0; i<3; i++)
        {
            tpoint[i].SetActive(false);
        }

        // ���Ȃ������Ɠ��_�\���ƃN���A�G���A�̔z�u
        int pos = Random.Range(0,3);
        person[pos].SetActive(false);
        tpoint[pos].SetActive(true);
        TextPoint = tpoint[pos];
        monako.transform.Translate(4.0f * (float)pos, 0, 0);
        clearArea.transform.Translate(4.0f * (float)pos, 0, 0);

        // �N���A�G���A�̃Z���^�[�̌v�Z
        center = (float)(pos * 4 - 1);

        // �X���C�v���b�Z�[�W�͏����Ă���
        swipemes.SetActive(false);
    }

    void Update()
    {
        switch (phase)
        {
            // �Q�[���J�n�i���Ȃ��Z�b�g�O�j
            case 0:
                if (Input.GetMouseButtonDown(0))
                {
                    // ���Ȃ������A�j���[�V����
                    tapmes.SetActive(false);
                    inAnime = true;
                    StartCoroutine("MonakaSet");
                    StartCoroutine("StockDown");
                    phase++;
                }
                break;

            // ���Ȃ������A�j���[�V�������I���܂ő҂�
            case 1:
                if (!inAnime)
                {
                    swipemes.SetActive(true);
                    phase++;
                }
                break;

            // �X���C�v�J�n
            case 2:
                if (Input.GetMouseButtonDown(0))
                {
                    // �X���C�v�J�n�ʒu���擾
                    startPos = Input.mousePosition.x;
                    phase++;
                }
                break;

            // �X���C�v�I��
            case 3:
                if (Input.GetMouseButtonUp(0))
                {
                    // �X���C�v�I���ʒu���擾
                    endPos = Input.mousePosition.x;

                    // �X���C�v����������Ȃ����x������
                    float swipeLength = endPos - startPos;
                    speed = swipeLength / 500.0f;   // 500.0���Ēl�͂�������
                    swipemes.SetActive(false);
                    phase++;
                }
                break;

            // ���Ȃ��ړ���
            case 4:
                // ���Ȃ��ɑ��x�t�����Ă�����Ƃ������x��x������
                monaka[zanki].transform.Translate(speed, 0, 0);
                speed *= 0.98f;

                // ���_����
                point = 100 - (int)(Mathf.Abs(monaka[zanki].transform.position.x - center) * 100 / 2.8f);
                if (point < 0) point = 0; 
                TextPoint.GetComponent<Text>().text = point.ToString("D") + "point";

                // ���Ȃ�����葬�x�ȉ��ɂȂ邩��ʊO�ɏo���玟�̒i�K
                if (speed < 0.001 || monaka[zanki].transform.position.x > wallend.transform.position.x)
                {
                    phase++;
                }
                break;

            // ����I
            case 5:
                // �������_���O��������
                if(point == 0)
                {
                    // �c�@���c���Ă����瑦�������P��
                    if (zanki > 0) {
                        monaka[zanki].SetActive(false);
                        zanki--;
                        tapmes.SetActive(true);
                        phase = 0;
                    }
                    else
                    {
                        // �Q�[���I�[�o�[�\��
                        // �^�b�v���Ď��̃X�e�[�W
                        // ������
                    }

                }
                else
                {
                    // �X�e�[�W�N���A�\��
                    // ������

                    // �^�b�v���ăX�R�A�]��
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

    // ��ԉ��̂��e�[�u���ɑ傫���Ȃ�Ȃ��痎����
    IEnumerator MonakaSet()
    {
        for (int i = 0; i < 10; i++)
        {
            monaka[zanki].transform.Translate(0, -0.25f, 0);
            monaka[zanki].transform.localScale = new Vector2(0.5f + (float)i * 0.05f, 0.5f + (float)i * 0.05f);
            yield return new WaitForSeconds(0.01f);
        }
    }

    // �X�g�b�N���P�����ɗ��Ƃ�
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

    // �|�C���g���X�R�A�Ɉړ��ǉ�
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