using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game8Director : MonoBehaviour
{
    private int zanki = 4;                  // ���ڋ@�i�v���X�P����Ǝc�@���j
    private int phase = 0;                  // ��ʏ��
    private bool inAnime = false;           // �A�j����
    private bool passingArea = false;       // �N���A�G���A�ʉ߃t���O
    const string SAVE_KEY = "HighScore";    // �n�C�X�R�A�ۑ��L�[

    // �Q�[���I�u�W�F�N�g
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

    // ���Ȃ��A�j���[�V�����panimator
    Animator animator;

    // ���َq�̃X�v���C�g
    public Sprite[] Sweets = new Sprite[4];

    // ���ʉ��֘A
    AudioSource audioSource;
    public AudioClip[] vOisii = new AudioClip[4];
    public AudioClip vTabetaina;
    public AudioClip vMouikkai;
    private int[] ClearVoice = new int[5] {0,0,0,0,0};

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

        // �X�R�A�ƃn�C�X�R�A�̓ǂݍ���
        score = ScoreStrage.Score;
        TextScore.GetComponent<Text>().text = "Score:" + ScoreStrage.Score.ToString("D4");
        TextHighScore.GetComponent<Text>().text = "HighScore:" + ScoreStrage.HighScore.ToString("D4");

        // ���Ȃ������ƃN���A�G���A�̔z�u
        int pos = Random.Range(0, 10);
        person[pos].SetActive(false);
        monako.transform.Translate(1.2f * (float)pos, 0, 0);
        clearArea.transform.Translate(1.2f * (float)pos, 0, 0);

        // �N���A�G���A�̃Z���^�[�̌v�Z
        center = (float)pos * 1.2f - 2.5f;

        // ���Ȃ������̓���ɓ��_�\��
        Vector2 pPos;
        RectTransform TextPointRect = TextPoint.GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            RectTransformUtility.WorldToScreenPoint(Camera.main, new Vector2(center, 0.9f)), 
            null, out pPos
        );
        TextPointRect.localPosition = pPos;

        // ���Ȃ������A�j���[�V�����R���|�[�l���g���擾
        animator = monako.GetComponent<Animator>();

        // �����̃R���|�[�l���g���擾
        audioSource = GetComponent<AudioSource>();

        // �]�v�Ȃ��̂͏����Ă���
        swipemes.SetActive(false);
        TextClear.SetActive(false);

        // �R���̂P�̊m���Ń����_���ɂP�����͂��Ȃ��ȊO�̂��̂�����
        if(Random.Range(0, 3) == 0)
        {
            int sPos = Random.Range(0, 5);
            int sSweets = Random.Range(1, 4);
            monaka[sPos].GetComponent<SpriteRenderer>().sprite = Sweets[sSweets];
            ClearVoice[sPos] = sSweets;
        }

        // �͂��Ȃ����H�ׂ����Z���t
        audioSource.PlayOneShot(vTabetaina);
    }

    void Update()
    {
        switch (phase)
        {
            // �Q�[���J�n�i���Ȃ��Z�b�g�O�j
            case 0:
                if (Input.GetMouseButtonDown(0))
                {
                    audioSource.Play();

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
                point = 100 - (int)(Mathf.Abs(monaka[zanki].transform.position.x - center) * 100 / 1.3f);
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
                audioSource.Stop();     // BGM���~�߂�

                // �������_���O��������
                if (point == 0)
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

                    // �N���A�\��
                    TextClear.SetActive(true);

                    // �u���������v�X�R�A�]��
                    audioSource.PlayOneShot(vOisii[ClearVoice[zanki]]);
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

                // �^�b�v���ăZ�b�g��\��
                tapmes.SetActive(true);

                // �ŏ��ɖ߂�
                phase = 0;

                break;

            // �Q�[���I�[�o�[
            case 7:
                // �Q�[���I�[�o�[��ʂ�
                SceneManager.LoadScene("GameoverScene");
                break;

            // �X�e�[�W�N���A
            case 8:
                // �^�b�v���Ď��̃X�e�[�W��
                if (Input.GetMouseButtonDown(0))
                {
                    // �X�R�A�Ƀ|�C���g�����Z
                    score += point;

                    // �n�C�X�R�A����
                    if (score > ScoreStrage.HighScore)
                    {
                        ScoreStrage.HighScore = score;
                        TextHighScore.GetComponent<Text>().text = "HighScore:" + score.ToString("D4");
                        PlayerPrefs.SetInt(SAVE_KEY, score);
                        PlayerPrefs.Save();
                    }

                    // ���݂̃X�R�A�����̃X�e�[�W�Ɏ󂯌p��
                    ScoreStrage.Score = score;

                    // ���̃X�e�[�W��
                    SceneManager.LoadScene("Game9Scene");
                }
                break;

            default:
                break;
        }
    }

    // ��ԉ��̂��傫���Ȃ�Ȃ���e�[�u���ɗ�����
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
        int i, div, dscore = score;

        // �X�R�A���g��\��
        for(i=0; i<10; i++)
        {
            TextPoint.GetComponent<Text>().fontSize +=3;
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(1.5f);

        // �X�R�A���|�C���g�G���A�Ɉړ��ǉ�
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