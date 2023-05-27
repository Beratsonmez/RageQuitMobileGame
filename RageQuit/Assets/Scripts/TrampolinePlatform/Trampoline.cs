using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
    Animator anim;

    [SerializeField]
    private float jumpForce;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision) //Trambolinin box coliderine eri�tik
    {
        if (collision.transform.CompareTag("Player")) // e�er Player ismindeki bir Transforma temas ederse.
        {
            collision.gameObject.GetComponent<Rigidbody2D>().velocity = (Vector2.up * jumpForce); //Burda collision gameobjenin rigidbody sini yakalay�p velocitysini al�uo ve vector2 de yukar� hareket yaparken JumpForce ile �arp�yo.
            anim.Play("Jump");//Animasyonu oynat.
        }
    }

}
