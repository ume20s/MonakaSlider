using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpeningDirector : MonoBehaviour
{
    // 効果音関連
    AudioSource audioSource;
    public AudioClip[] sOpening = new AudioClip[2];

    // Start is called before the first frame update
    void Start()
    {
        // 音声のコンポーネントを取得
        audioSource = GetComponent<AudioSource>();

        // ３分の１の確率で鯱もなかソング
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
