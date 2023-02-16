using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpeningDirector : MonoBehaviour
{
    // ���ʉ��֘A
    AudioSource audioSource;
    public AudioClip[] sOpening = new AudioClip[3];

    // Start is called before the first frame update
    void Start()
    {
        // �����̃R���|�[�l���g���擾
        audioSource = GetComponent<AudioSource>();

        // �����_�����͂��Ȃ��\���O���̓o���[�h
        int song = Random.Range(0, 10);
        switch (song)
        {
            case 0:
            case 1:
            case 2:
                audioSource.clip = sOpening[0];
                break;
            case 3:
                audioSource.clip = sOpening[1];
                break;
            default:
                audioSource.clip = sOpening[2];
                break;
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
