using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")] //Kullan�ca��m�z de�i�tirilebilirleri kategorize i�in isimlendirdim.
    public float movementSpeed = 10.0f;

    private float inputDirection; //Gelen verinin y�n�

    private bool isRun;
    private bool facingRight = true; //Gelen veriyi kar��la�t�r�p tersine �evirmek i�in
    private bool isGrounded;

    private int facingDirection = 1;

    private bool canMove; //Karakterin kayarken d�n���n� ve hareketini k�s�tlamak i�in
    private bool canFlip;

    [Header("Jump")]
    public int numberOfJumps = 1;

    public float jumpForce = 16.0f;
    public float groundCheckRadius; //Yeri tespit edicek dairenin �ap�
    public float wallCheckDistance; //Duvar� tespit edicek ���n�n uzunlu�u
    public float forceInAir; //Havada s�rt�nme kuvveti
    public float airDragMultiplier = 0.95f; //Havada s�rt�nme kuvvetinin �arpan�
    public float veriableJumpMultiplier = 0.5f; //Ayarlanabilir z�plama �arpan�
    public float jumpTimerSet = 0.15f; //Z�plaman�n aralar�na gecikme koymak i�in
    public float turnTimerSet = 0.01f; //Zaman� geri d�nd�rmek i�in

    private bool canNormalJump; //Normal jump yapabilip yapamica��na g�re true yada false g�ndericek
    private bool isAttemptingToJump; //Karakterin gecikme sonucu z�plamaya haz�r olup olmad���n� g�stericek bir t�k bunny yapmas�n� engelliyomu� gibi d���n
    private bool checkJumpMultiplier; // �arpan�n kontrol edilmesi gerekip gerekmedi�ini d�nd�r�cek

    private float turnTimer; // D�n�� zaman�n� tutucak
    private float jumpTimer; // Son z�plama zaman�n� tutucak

    private int numberOfJumpsLeft; // Kalan z�plama say�n�s� tutacak



    [Header("WallSlide")]
    public float wallSlideSpeed; // Duvarda kayma h�z�

    private bool isWallSliding; // Duvarda kay�p kaymad���m� d�nd�r�cek
    private bool isTouchingWall; // Duvara deyip deymedi�imizi kontrol edecek

    [Header("WallJump")]
    public float wallJumpForce; // WallJump g�c�
    public float wallJumpTimerSet = 0.5f; // WallJump timer'�

    private float wallJumpTimer;

    private bool canWallJump; // WallJump yapabilip yapamica��m�z� true yada false olarak d�nd�r�cek
    private bool hasWallJumped; // WallJump yapt�km� onu s�ylicek

    private int lastWallJumpDirection; // En son yapt���m�z WallJump y�n�

    public Joystick joystick;

    private Rigidbody2D rb;
    private Animator anim;

    public Vector2 wallJumpDirection; // WallJump y�n�n� Vector halinde hesaplamam�za yaricak

    public Transform groundCheck; // Yere deyip deymedi�imizi kontrol edicek olan daire'nin konum,�ap ve benzeri �zelliklerini bar�nd�r�cak bir ��e unity de scale'la oynad���n yerle ayn� mant�k k�smen
    public Transform wallCheck; // Ayn�s� sadece duvar tespiti i�in

    public LayerMask whatIsGround; // Karakterin zeminin ne oldu�unu anlayabilmesi ve onu de�erlendire bilmesi i�in bir layer

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        numberOfJumpsLeft = numberOfJumps; // Belirledi�imiz z�plama say�s�n� oyun ba�lad���nda kalan z�plamaya e�itliyo
        wallJumpDirection.Normalize(); // WallJump direction'� oyun ilk a��ld���nda bizim belirledi�imiz varsay�lan de�erlerine d�nd�r�yo
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        CheckMovementDirection();
        UpdateAnimations();
        CheckIfCanJump();
        CheckIfWallSliding();
        CheckJump();
    }

    private void FixedUpdate()
    {
        CheckSurroundings();
        ApplyMovement();
    }

    private void UpdateAnimations() // Animator penceresindeki Bool ve di�er de�erleri burdaki verilerin de�erlerine e�itliyo
    {
        anim.SetBool("isRun", isRun);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isWallSliding", isWallSliding);
    }

    private void CheckInput()
    {
        inputDirection = joystick.Horizontal;

       

        if (joystick.Horizontal >= .2f && isTouchingWall) // Duvarda kayarken kayd���m�z duvara do�ru tu�a bast���m�z i�in karakterin o tarafa hareket etmemesini yada d�nmemesini sa�l�yo
        {
            if (!isGrounded && inputDirection != facingDirection)
            {
                canMove = false;
                canFlip = false;

                turnTimer = turnTimerSet;
            }
        }

        if (turnTimer >= 0)
        {
            turnTimer -= Time.deltaTime; // knk burda timerdaki anl�k say�y� azalt�yo hani e�er bi bekleme s�resi varsa ne biliyim karakter z�plam��t�r di�er z�plamay� yapmak i�in bekliyodur vs o s�reyi azalt�yo

            if (turnTimer <= 0) // s�re s�f�r�n alt�ndaysa bu hareket edebilir diyo
            {
                canMove = true;
                canFlip = true;
            }
        }

        
    }

    public void Jump()
    {
            if (isGrounded || (numberOfJumpsLeft > 0 && isTouchingWall)) // Z�plamas�n� e�er z�playamazsa z�plamaya haz�r hale gelmesini sa�l�yo
            {
                NormalJump();
            }
            else
            {
                jumpTimer = jumpTimerSet;
                isAttemptingToJump = true;
            }
    }

    public void VeriableJump()
    {
        if (checkJumpMultiplier) // Space ten eli �ekti�imizde �arpanla o anki y�kseklik h�z�m�z� �arp�p ayarlanabilir bir z�plama y�ksekli�i elde etmemizi sa�l�yo
        {
            checkJumpMultiplier = false;
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * veriableJumpMultiplier);
        }
    }

    private void ApplyMovement()
    {

        if (!isGrounded && !isWallSliding && inputDirection == 0) // Yere ve Duvara de�miyorsak ve e�er havadaysak karakterin havadaki hareketine S�rt�nme kuvveti dahil etmeyi sa�l�yo
        {
            rb.velocity = new Vector2(rb.velocity.x * airDragMultiplier, rb.velocity.y);
        }
        else if (canMove) // Yukar�dakiler kar��lanmay�nca otomatik karakter yerde demek oluyo normal hareket kodu devreye giriyo
        {
            rb.velocity = new Vector2(movementSpeed * inputDirection, rb.velocity.y);
        }

        //Moving platform i�in isOnPlatform sorgunusu ekledi�inde canMove && !isOnPlatform olarak sorgulatmay� dene


        if (isWallSliding) // Karakter duvarda kay�yosa karakterin y eksenindeki h�z�n� belirledi�imiz kayma h�z�na d���r�yo
        {
            if (rb.velocity.y < -wallSlideSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
            }
        }
    }

    private void CheckIfCanJump()
    {
        if (isGrounded && rb.velocity.y <= 0.01f) /*Karakter yerdeyse ve karakter herhangi bir �ekilde 0.01 de�erinde bile y�ksekte de�ilse kalan z�plamay� belirledi�imiz z�plama say�s�na geri d�nd�r�yo
                                  *Karakterin y�ksekli�inide bu �ekilde denememiz �nemli karakter yere d��meye �ok yak�nken yeri alg�lamaya ba�lica�� i�in yere de�meden z�playa biliyo olucak oy�zden y�ksekli�ide 0.01 ile de�erlendirdim */
        {
            numberOfJumpsLeft = numberOfJumps;
        }

        if (isTouchingWall) // E�er duvara deyiyorsa z�plama �arpan�n� devre d��� b�rak�p wallJump yapabilir hale getiricek
        {
            checkJumpMultiplier = false;
            canWallJump = true;
        }

        if (numberOfJumpsLeft <= 0) // Z�plama say�s� s�f�r�n alt�nda veya s�f�ra e�itse karakterin z�plamamas�n� sa�licak
        {
            canNormalJump = false;
        }
        else
        {
            canNormalJump = true;
        }

    }



    private void CheckJump()//YORUM YAZ
    {
        if (jumpTimer > 0) // Jump timer z�plamaya haz�r ise
        {
            //WallJump
            if (!isGrounded && isTouchingWall && inputDirection != 0 && inputDirection != facingDirection) /* Karakter yerede�miyor , duvara temas ediyor, hareket y�n� verisi gelmeye devam ediyor ve gelen veri karakterin
                                                                                                            * y�z�n�n d�n�k oldu�u y�ne tam tersi �ekilde geliyor ise wall jump yapabilir */
        {
                WallJump();
            }
            else if (isGrounded) // karakter sadece yere deyiyosa zaten bi zahmet normal z�plas�n
            {
                NormalJump();
            }
        }

        if (isAttemptingToJump) // Bu da jump timer � azalt�yo ki z�playa bilek
        {
            jumpTimer -= Time.deltaTime;
        }

        if (wallJumpTimer > 0) // Buray� ne sensor ne ben s�yliyim karakter di�er duvara z�plad�m� z�plamad�m� onun y�n� art�k z�playabilirmi cart curt onu kontrol ediyo
        {
            if (hasWallJumped && inputDirection == -lastWallJumpDirection) 
            {
                rb.velocity = new Vector2(rb.velocity.x, 0.0f);
                hasWallJumped = false;
            }
            else if (wallJumpTimer <= 0)
            {
                hasWallJumped = false;
            }
            else
            {
                wallJumpTimer -= Time.deltaTime;
            }
        }
    }

    private void NormalJump()
    {
        if(canNormalJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce); // karakterin h�z� = hali haz�rdaki x h�z� + z�plama g�n� "yani y ekseninde z�plama g�c� kadar kuvvet uygula"
            numberOfJumpsLeft--; //z�plama hakk�n� eksilt
            jumpTimer = 0; // Tekrar z�playa bilmek i�in z�plama gecikmesini s�f�rla
            isAttemptingToJump = false; // Z�plamaya haz�rl�k evresini tamamlat
            checkJumpMultiplier = true; // Ayarlana bilir z�plama �arpan�n� kontrol edilebilmesi i�in true yap
        }
    }

    private void WallJump()//YORUM YAZ
    {
        if (canWallJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0.0f); //Karakterin anl�k y y�ksekli�ini s�f�rl�yoki karakterin o an duvarda bulundu�u konum onun z�plama ba�lang�c� olsun
            isWallSliding = false; // kaymay� durduruyo
            numberOfJumpsLeft = numberOfJumps; // zaten
            numberOfJumpsLeft--; // e zaten
            Vector2 forceToAdd = new Vector2(wallJumpForce * wallJumpDirection.x * inputDirection, wallJumpForce * wallJumpDirection.y); // Karaktere normal z�plamadan farkl� olarak direkt o eksenlerde kontrol edilemicek bir g�� uyguluyoruz
            rb.AddForce(forceToAdd, ForceMode2D.Impulse); // o g�c� rigidbody ye uyguluyo patlama �eklinde g�c�n nas�l uygulan�ca��n� de�i�tire biliyosun
            jumpTimer = 0; // zaten
            isAttemptingToJump = false; // Art�k z�plamaz di�er duvara inene kadar diyon
            checkJumpMultiplier = true; // burda z�plad��� i�in ayarlanabilir �arpan� a��yousun
            turnTimer = 0; // zaten
            canMove = true; // duvarda karakterin hareketlerini kitledi�imiz i�in onlar� a��yosun
            canFlip = true;
            hasWallJumped = true; // Hali haz�rda z�plad� true yap�yosun
            wallJumpTimer = wallJumpTimerSet; // wallJumpTimer'� iki duvar aras�nda spam yap�lmas�n diye koydu�umuz delay s�resine geri e�itliyosun ��nk� o s�f�ra kadar say�yo s�f�r olunca atlaya bilyoz
            lastWallJumpDirection = -facingDirection; // bu da bir �nceki duvar konumunun bakt���m�z y�n�n tam tersi oldu�unu kay�t ediyo

        }
    }

    private void CheckIfWallSliding()
    {
        if (isTouchingWall && rb.velocity.y < 0) // Bu bildi�in duvardan kaymay� a� kapa duvara deyiyosa y�ksekli�i de�i�miyosa falan fi�man
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }
    }


    private void CheckMovementDirection()
    {
        if (facingRight && inputDirection < 0) //Bu karaktere bakt��� y�n� sa�a bak�yosa  biz sol tu�a bas�yosak flip methodunu �al��t�r�yo alt�ndakide ayn� i�lev ama tam tersi y�n i�in
        {
            Flip();
        }
        else if (!facingRight && inputDirection > 0)
        {
            Flip();
        }

        if ((Mathf.Abs(rb.velocity.x) >= 0.01f)) // Bu karakterin x eksenindeki h�z�n�n mutlak de�erini al�yo ald��� de�er 0.01 den fazlaysa ko�abilir diyo mutlak de�er almam�z�n sebebiyse karakterin h�z� sa�a giderken pozitif sola giderken negatif ondan
        {
            isRun = true;
        }
        else
        {
            isRun = false;
        }
    }

    public void DisableFlip()
    {
        canFlip = false;
    }

    public void EnableFlip()
    {
        canFlip = true;
    }

    private void Flip() //Karakteri herhangi bir gameobjesinden component �ekmeden d�nd�rme
    {
        if (!isWallSliding && canFlip)
        {
            facingDirection *= -1;
            facingRight = !facingRight;
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }
    }

    private void CheckSurroundings() // Geldik bizim po�etleri siyah hayale koy ustam k�sm�na knk bunu direkt uzunca anlat�cam iki methodun ortas�nda iyi oku
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);
    }

    /*Knk �imdi bu yukardaki CheckSurroundings k�sm� bizim belirledi�imiz �apta ve pozisyonda bir daire ve ��� olu�turuyo isGrounded daireyi olu�turan isTouchingWall'da �izgiyi yani ���n� olu�turan
     * �imdi ��yleki groundCheck.position dedi�imiz bu dairenin konumunu belirliyo groundCheckRadius dedi�imiz de dairenin b�y�kl���n� yani �ap�n� belirliyo 
     * en sondaki whatIsGround yani layermask k�sm�ndaysa o yuvarlak i�ine giren �eyleri tan�mam�z� sa�l�yo mesela layerMask �uanda ground olarak belli gidip bu tileMap �izdi�in ground objesi varya onun da layer'�n� ground yap�nca art�k
     *o dairenin i�ine giren bir�eyi diyoki ha bu ground de�il ben �uan havaday�m yere deyiyoya ha bu ground ben yerdeyim*/

    /*Alttaki metodun ne i�e yarad���n� s�yl�yorum alttaki bu yukarda �izdi�imiz ���n ve yuvarla�� g�rmemizi sa�l�yo yani o yuvarla�� ve ���n� �iziyo sana g�rebilmen i�in abi bak bunlar b�yle �uan diyo
     * ama alttaki olmasada olur sadece g�rmeni sa�l�yo yani ba�ka hi�bir etkisi i�levi yok*/

    private void OnDrawGizmos()//YORUM YAZ
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
    }

}
