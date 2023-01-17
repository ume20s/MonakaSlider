using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game1Director : MonoBehaviour
{
    private int zanki = 4;                  // ���ڋ@�i�v���X�P����Ǝc�@���j
    private int phase = 0;                  // ��ʏ��
    private bool inAnime = false;           // �A�j����
    private bool passingArea = false;       // �N���A�G���A�ʉ߃t���O

    // �Q�[���I�u�W�F�N�g
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

    // ���Ȃ��A�j���[�V�����panimator
    Animator animator;

    // ���Ȃ��ȊO�̂��َq�̃X�v���C�g
    public Sptite Sweets;
    public Sprite FruitCake;
    public Sprite FriendFinancier;
    public Sprite FriendCookie;

    // ���ʉ��֘A
    AudioSource audioSource;
    public AudioClip vMonakaOisii;
    public AudioClip vMouikkai;

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
        monako = GameObject.Find("monako");
        clearArea = GameObject.Find("clear");
        tapmes = GameObject.Find("tap");
        swipemes = GameObject.Find("swipe");
        TextScore = GameObject.Find("Score");
        TextHighScore = GameObject.Find("HighScore");
        TextPoint = GameObject.Find("Point");
        wallend = GameObject.Find("wallEnd");
        canvas = GameObject.Find("Canvas");

        // ���Ȃ������ƃN���A�G���A�̔z�u
        int pos = Random.Range(0,3);
        person[pos].SetActive(false);
        monako.transform.Translate(4.0f * (float)pos, 0, 0);
        clearArea.transform.Translate(4.0f * (float)pos, 0, 0);

        // �N���A�G���A�̃Z���^�[�̌v�Z
        center = (float)(pos * 4 - 1);

        // ���Ȃ������̓���ɓ��_�\��
        Vector2 pPos;
        RectTransform TextPointRect = TextPoint.GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            RectTransformUtility.WorldToScreenPoint(Camera.main, new Vector2(center, 3.5f)), 
            null, out pPos
        );
        TextPointRect.localPosition = pPos;

        // ���Ȃ������A�j���[�V�����R���|�[�l���g���擾
        animator = monako.GetComponent<Animator>();

        // ���ʉ��̃R���|�[�l���g���擾
        audioSource = GetComponent<AudioSource>();

        // �]�v�Ȃ��̂͏����Ă���
        swipemes.SetActive(false);

        // DEBUGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGG
        // monaka[2].GetComponent<SpriteRenderer>().sprite = fruit;
    }

    void Update()
    {
        switch (phase)
        {
            // �Q�[���J�n�i���Ȃ��Z�b�g�O�j
            case 0:
                if (Input.GetMouseButtonDown(0))
                {
                    // �]�v�Ȃ��͔̂�\����
                    tapmes.SetActive(false);

                    // ���Ȃ������A�j���[�V����
                    inAnime = true;
                    StartCoroutine("MonakaSet");
                    StartCoroutine("StockDown");

                    // ���Ȃ������Z�b�e�B���O�A�j���ɑJ��
                    animator.SetTrigger("SettingTrigger");
                    passingArea = false;

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
                if (point > 1) passingArea = true;
                if (point < 0)
                {
                    point = 0;
                    if(passingArea)
                    {
                        // ���Ȃ�����񎸔s�A�j���ɑJ��
                        animator.SetTrigger("MissTrigger");
                        inAnime = true;
                        passingArea = false;
                    }
                } 
                TextPoint.GetComponent<Text>().text = point.ToString("D") + "point";

                // ���Ȃ�����葬�x�ȉ��ɂȂ邩��ʊO�ɏo���玟�̒i�K
                if (speed < 0.002 || monaka[zanki].transform.position.x > wallend.transform.position.x - 1.0f)
                {
                    phase++;
                }
                break;

            // ����I
            case 5:
                // �������_���O��������
                if(point == 0)
                {
                    // �A�j�����ĂȂ���������Ȃ�����񎸔s�A�j���ɑJ��
                    if(!inAnime)
                    {
                        animator.SetTrigger("MissTrigger2");
                    }
                    inAnime = false;

                    // �c�@���c���Ă���������P��
                    if (zanki > 0) {
                        audioSource.PlayOneShot(vMouikkai);
                        phase = 6;
                    }
                    else
                    {
                        // �Q�[���I�[�o�[
                        phase = 7;
                    }

                }
                else
                {
                    // ���Ȃ������X�e�[�W�N���A�A�j���ɑJ��
                    monaka[zanki].SetActive(false);
                    animator.SetTrigger("ClearTrigger");

                    // �u���������v�X�R�A�]��
                    audioSource.PlayOneShot(vMonakaOisii);
                    StartCoroutine("PointAdd");

                    // �X�e�[�W�N���A��
                    phase = 8;
                }
                break;

            // �����P��
            case 6:
                // ���s���Ȃ��������Ďc�@��
                monaka[zanki].SetActive(false);
                zanki--;

                // �^�b�v���ăZ�b�g
                tapmes.SetActive(true);

                phase = 0;

                break;

            // �Q�[���I�[�o�[
            case 7:
                // �Q�[���I�[�o�[��ʂ�
                SceneManager.LoadScene("GameoverScene");
                break;

            // �X�e�[�W�N���A
            case 8:
                // �^�b�v���Ď��̃X�e�[�W�ւ�
                if (Input.GetMouseButtonDown(0))
                {
                    SceneManager.LoadScene("Game2Scene");
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
        int i;
        for(i=0; i<30; i++)
        {
            TextPoint.GetComponent<Text>().fontSize +=2;
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(2.0f);
        for (i = point; i >= 0; i--)
        {
            TextPoint.GetComponent<Text>().text = i.ToString("D") + "point";
            TextScore.GetComponent<Text>().text = "Score:" + score.ToString("D4");
            score++;
            yield return new WaitForSeconds(0.05f);
        }
    }
}