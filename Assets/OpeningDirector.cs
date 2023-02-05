using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpeningDirector : MonoBehaviour
{
    // ���ʉ��֘A
    AudioSource audioSource;
    public AudioClip[] sOpening = new AudioClip[2];

    // Start is called before the first frame update
    void Start()
    {
        // �����̃R���|�[�l���g���擾
        audioSource = GetComponent<AudioSource>();

        // �R���̂P�̊m�����͂��Ȃ��\���O
        if (Random.Range(0, 3) == 0)
        {
            audioSource.clip = sOpening[0];
        }
        else
        {
            audioSource.clip = sOpening[1];
        }
        audioSource.Play();

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene("Game1Scene");
        }
    }
}
