﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrPlayer : MonoBehaviour
{
    /// <summary>
    /// ----------------------------------------------------------------------------------
    /// DESCRIPCIÓ
    ///         Script utilitzat per tota la programació relacionada amb el player: moviment,
    ///         habilitats, animacions, característiques...
    /// AUTOR:  Lídia García Romero
    /// DATA:   10/05/2021
    /// VERSIÓ: 2.2
    /// CONTROL DE VERSIONS
    ///         1.0: Es crea el player i es programa el seu moviment i salt.
    ///         1.1: Es comença a programar el moviment per "steps", però encara no funciona.
    ///         2.0: Es canvia el métode. Ara es temps de moviment en comptes de "steps". S'aplica
    ///              i funciona el moviment, però no la gestió de torns.
    ///         2.1: Es perfeccionen aspectes de constrains. Moviment en torns funciona perfecte.
    ///         2.2: Es perfecciona el moviment del player. No es pot moure fins que 
    ///             l'animació del dau hagi acabat.
    /// ----------------------------------------------------------------------------------
    /// </summary>

    [SerializeField] int playerID; //de valor 1 o 2
    
    //Pel moviment del player_____________________________________________________________
    Rigidbody2D rb;
    float moviment;
    [SerializeField] float velocitat = 3, fSalt = 400;
    bool miraDreta = true;
    //____________________________________________________________________________________

    //Pel salt____________________________________________________________________________
    bool salta = false, onTerra = false;
    [SerializeField] Transform checkTerra;
    [SerializeField] public LayerMask filtreCapes; //Per la capa dels terres
    float radi = 0.2f;
    //____________________________________________________________________________________

    //Pel temps de tirada_________________________________________________________________
    public float tTirada = 2;
    bool haAtacat = false; //perque només pugui atacar una vegada per torn
    [SerializeField] GameObject controlTorns, aniDau;
    //____________________________________________________________________________________


    void Start()
    {
        //Pel moviment del player_____________________________________________________________
        rb = GetComponent<Rigidbody2D>();
        //____________________________________________________________________________________

        //Pel temps de tirada_________________________________________________________________
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        //____________________________________________________________________________________
    }


    void Update()
    {
        //Pel moviment del player_____________________________________________________________
        moviment = Input.GetAxis("Horizontal");

        if (((moviment < 0 && miraDreta) || (moviment > 0 && !miraDreta)) && controlTorns.GetComponent<ScrTorns>().tornActual == playerID) FlipSprite();
        //____________________________________________________________________________________

        //Pel salt____________________________________________________________________________
        if (Input.GetButtonDown("Jump") && onTerra) salta = true;
        //____________________________________________________________________________________

        //Pel temps de tirada_________________________________________________________________
        if (controlTorns.GetComponent<ScrTorns>().tornActual == playerID && tTirada > 0 && aniDau.GetComponent<ScrAniDau>().aniAcabada) //és el teu torn i et pots moure
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            tTirada -= Time.deltaTime;
        }

         else if (tTirada < 0) //s'ha acabat el torn i ja no et pots moure
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            controlTorns.GetComponent<ScrTorns>().tornActual += 2;
            tTirada = 0;
        }
        //____________________________________________________________________________________
    }
    

    private void FixedUpdate()
    {
        //Pel moviment del player_____________________________________________________________
        rb.velocity = new Vector2(moviment * velocitat, rb.velocity.y); //posem velocity.y al eix y perque sino tindria conflictes amb la gravetat
        //____________________________________________________________________________________

        //Pel salt____________________________________________________________________________
        onTerra = Physics2D.OverlapCircle(checkTerra.position, radi, filtreCapes); //detectar si està entrant en contacte amb el terra

        if(salta && onTerra) //salt
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(0, fSalt));
        }

        salta = false;
        //____________________________________________________________________________________
    }


    void FlipSprite()
    {
        miraDreta = !miraDreta;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z); //fem el flip multiplicant per -1 la escala x 
    }
}