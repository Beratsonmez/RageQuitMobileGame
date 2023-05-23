using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")] //Kullanýcaðýmýz deðiþtirilebilirleri kategorize için isimlendirdim.
    public float movementSpeed = 10.0f;

    private float inputDirection; //Gelen verinin yönü

    private bool isRun;
    private bool facingRight = true; //Gelen veriyi karþýlaþtýrýp tersine çevirmek için
    private bool isGrounded;

    private int facingDirection = 1;

    private bool canMove; //Karakterin kayarken dönüþünü ve hareketini kýsýtlamak için
    private bool canFlip;

    [Header("Jump")]
    public int numberOfJumps = 1;

    public float jumpForce = 16.0f;
    public float groundCheckRadius; //Yeri tespit edicek dairenin çapý
    public float wallCheckDistance; //Duvarý tespit edicek ýþýnýn uzunluðu
    public float forceInAir; //Havada sürtünme kuvveti
    public float airDragMultiplier = 0.95f; //Havada sürtünme kuvvetinin çarpaný
    public float veriableJumpMultiplier = 0.5f; //Ayarlanabilir zýplama çarpaný
    public float jumpTimerSet = 0.15f; //Zýplamanýn aralarýna gecikme koymak için
    public float turnTimerSet = 0.01f; //Zamaný geri döndürmek için

    private bool canNormalJump; //Normal jump yapabilip yapamicaðýna göre true yada false göndericek
    private bool isAttemptingToJump; //Karakterin gecikme sonucu zýplamaya hazýr olup olmadýðýný göstericek bir týk bunny yapmasýný engelliyomuþ gibi düþün
    private bool checkJumpMultiplier; // Çarpanýn kontrol edilmesi gerekip gerekmediðini döndürücek

    private float turnTimer; // Dönüþ zamanýný tutucak
    private float jumpTimer; // Son zýplama zamanýný tutucak

    private int numberOfJumpsLeft; // Kalan zýplama sayýnýsý tutacak



    [Header("WallSlide")]
    public float wallSlideSpeed; // Duvarda kayma hýzý

    private bool isWallSliding; // Duvarda kayýp kaymadýðýmý döndürücek
    private bool isTouchingWall; // Duvara deyip deymediðimizi kontrol edecek

    [Header("WallJump")]
    public float wallJumpForce; // WallJump gücü
    public float wallJumpTimerSet = 0.5f; // WallJump timer'ý

    private float wallJumpTimer;

    private bool canWallJump; // WallJump yapabilip yapamicaðýmýzý true yada false olarak döndürücek
    private bool hasWallJumped; // WallJump yaptýkmý onu söylicek

    private int lastWallJumpDirection; // En son yaptýðýmýz WallJump yönü

    public Joystick joystick;

    private Rigidbody2D rb;
    private Animator anim;

    public Vector2 wallJumpDirection; // WallJump yönünü Vector halinde hesaplamamýza yaricak

    public Transform groundCheck; // Yere deyip deymediðimizi kontrol edicek olan daire'nin konum,çap ve benzeri özelliklerini barýndýrýcak bir öðe unity de scale'la oynadýðýn yerle ayný mantýk kýsmen
    public Transform wallCheck; // Aynýsý sadece duvar tespiti için

    public LayerMask whatIsGround; // Karakterin zeminin ne olduðunu anlayabilmesi ve onu deðerlendire bilmesi için bir layer

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        numberOfJumpsLeft = numberOfJumps; // Belirlediðimiz zýplama sayýsýný oyun baþladýðýnda kalan zýplamaya eþitliyo
        wallJumpDirection.Normalize(); // WallJump direction'ý oyun ilk açýldýðýnda bizim belirlediðimiz varsayýlan deðerlerine döndürüyo
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

    private void UpdateAnimations() // Animator penceresindeki Bool ve diðer deðerleri burdaki verilerin deðerlerine eþitliyo
    {
        anim.SetBool("isRun", isRun);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isWallSliding", isWallSliding);
    }

    private void CheckInput()
    {
        inputDirection = joystick.Horizontal;

       

        if (joystick.Horizontal >= .2f && isTouchingWall) // Duvarda kayarken kaydýðýmýz duvara doðru tuþa bastýðýmýz için karakterin o tarafa hareket etmemesini yada dönmemesini saðlýyo
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
            turnTimer -= Time.deltaTime; // knk burda timerdaki anlýk sayýyý azaltýyo hani eðer bi bekleme süresi varsa ne biliyim karakter zýplamýþtýr diðer zýplamayý yapmak için bekliyodur vs o süreyi azaltýyo

            if (turnTimer <= 0) // süre sýfýrýn altýndaysa bu hareket edebilir diyo
            {
                canMove = true;
                canFlip = true;
            }
        }

        
    }

    public void Jump()
    {
            if (isGrounded || (numberOfJumpsLeft > 0 && isTouchingWall)) // Zýplamasýný eðer zýplayamazsa zýplamaya hazýr hale gelmesini saðlýyo
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
        if (checkJumpMultiplier) // Space ten eli çektiðimizde çarpanla o anki yükseklik hýzýmýzý çarpýp ayarlanabilir bir zýplama yüksekliði elde etmemizi saðlýyo
        {
            checkJumpMultiplier = false;
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * veriableJumpMultiplier);
        }
    }

    private void ApplyMovement()
    {

        if (!isGrounded && !isWallSliding && inputDirection == 0) // Yere ve Duvara deðmiyorsak ve eðer havadaysak karakterin havadaki hareketine Sürtünme kuvveti dahil etmeyi saðlýyo
        {
            rb.velocity = new Vector2(rb.velocity.x * airDragMultiplier, rb.velocity.y);
        }
        else if (canMove) // Yukarýdakiler karþýlanmayýnca otomatik karakter yerde demek oluyo normal hareket kodu devreye giriyo
        {
            rb.velocity = new Vector2(movementSpeed * inputDirection, rb.velocity.y);
        }

        //Moving platform için isOnPlatform sorgunusu eklediðinde canMove && !isOnPlatform olarak sorgulatmayý dene


        if (isWallSliding) // Karakter duvarda kayýyosa karakterin y eksenindeki hýzýný belirlediðimiz kayma hýzýna düþürüyo
        {
            if (rb.velocity.y < -wallSlideSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
            }
        }
    }

    private void CheckIfCanJump()
    {
        if (isGrounded && rb.velocity.y <= 0.01f) /*Karakter yerdeyse ve karakter herhangi bir þekilde 0.01 deðerinde bile yüksekte deðilse kalan zýplamayý belirlediðimiz zýplama sayýsýna geri döndürüyo
                                  *Karakterin yüksekliðinide bu þekilde denememiz önemli karakter yere düþmeye çok yakýnken yeri algýlamaya baþlicaðý için yere deðmeden zýplaya biliyo olucak oyüzden yüksekliðide 0.01 ile deðerlendirdim */
        {
            numberOfJumpsLeft = numberOfJumps;
        }

        if (isTouchingWall) // Eðer duvara deyiyorsa zýplama çarpanýný devre dýþý býrakýp wallJump yapabilir hale getiricek
        {
            checkJumpMultiplier = false;
            canWallJump = true;
        }

        if (numberOfJumpsLeft <= 0) // Zýplama sayýsý sýfýrýn altýnda veya sýfýra eþitse karakterin zýplamamasýný saðlicak
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
        if (jumpTimer > 0) // Jump timer zýplamaya hazýr ise
        {
            //WallJump
            if (!isGrounded && isTouchingWall && inputDirection != 0 && inputDirection != facingDirection) /* Karakter yeredeðmiyor , duvara temas ediyor, hareket yönü verisi gelmeye devam ediyor ve gelen veri karakterin
                                                                                                            * yüzünün dönük olduðu yöne tam tersi þekilde geliyor ise wall jump yapabilir */
        {
                WallJump();
            }
            else if (isGrounded) // karakter sadece yere deyiyosa zaten bi zahmet normal zýplasýn
            {
                NormalJump();
            }
        }

        if (isAttemptingToJump) // Bu da jump timer ý azaltýyo ki zýplaya bilek
        {
            jumpTimer -= Time.deltaTime;
        }

        if (wallJumpTimer > 0) // Burayý ne sensor ne ben söyliyim karakter diðer duvara zýpladýmý zýplamadýmý onun yönü artýk zýplayabilirmi cart curt onu kontrol ediyo
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
            rb.velocity = new Vector2(rb.velocity.x, jumpForce); // karakterin hýzý = hali hazýrdaki x hýzý + zýplama günü "yani y ekseninde zýplama gücü kadar kuvvet uygula"
            numberOfJumpsLeft--; //zýplama hakkýný eksilt
            jumpTimer = 0; // Tekrar zýplaya bilmek için zýplama gecikmesini sýfýrla
            isAttemptingToJump = false; // Zýplamaya hazýrlýk evresini tamamlat
            checkJumpMultiplier = true; // Ayarlana bilir zýplama çarpanýný kontrol edilebilmesi için true yap
        }
    }

    private void WallJump()//YORUM YAZ
    {
        if (canWallJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0.0f); //Karakterin anlýk y yüksekliðini sýfýrlýyoki karakterin o an duvarda bulunduðu konum onun zýplama baþlangýcý olsun
            isWallSliding = false; // kaymayý durduruyo
            numberOfJumpsLeft = numberOfJumps; // zaten
            numberOfJumpsLeft--; // e zaten
            Vector2 forceToAdd = new Vector2(wallJumpForce * wallJumpDirection.x * inputDirection, wallJumpForce * wallJumpDirection.y); // Karaktere normal zýplamadan farklý olarak direkt o eksenlerde kontrol edilemicek bir güç uyguluyoruz
            rb.AddForce(forceToAdd, ForceMode2D.Impulse); // o gücü rigidbody ye uyguluyo patlama þeklinde gücün nasýl uygulanýcaðýný deðiþtire biliyosun
            jumpTimer = 0; // zaten
            isAttemptingToJump = false; // Artýk zýplamaz diðer duvara inene kadar diyon
            checkJumpMultiplier = true; // burda zýpladýðý için ayarlanabilir çarpaný açýyousun
            turnTimer = 0; // zaten
            canMove = true; // duvarda karakterin hareketlerini kitlediðimiz için onlarý açýyosun
            canFlip = true;
            hasWallJumped = true; // Hali hazýrda zýpladý true yapýyosun
            wallJumpTimer = wallJumpTimerSet; // wallJumpTimer'ý iki duvar arasýnda spam yapýlmasýn diye koyduðumuz delay süresine geri eþitliyosun çünkü o sýfýra kadar sayýyo sýfýr olunca atlaya bilyoz
            lastWallJumpDirection = -facingDirection; // bu da bir önceki duvar konumunun baktýðýmýz yönün tam tersi olduðunu kayýt ediyo

        }
    }

    private void CheckIfWallSliding()
    {
        if (isTouchingWall && rb.velocity.y < 0) // Bu bildiðin duvardan kaymayý aç kapa duvara deyiyosa yüksekliði deðiþmiyosa falan fiþman
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
        if (facingRight && inputDirection < 0) //Bu karaktere baktýðý yönü saða bakýyosa  biz sol tuþa basýyosak flip methodunu çalýþtýrýyo altýndakide ayný iþlev ama tam tersi yön için
        {
            Flip();
        }
        else if (!facingRight && inputDirection > 0)
        {
            Flip();
        }

        if ((Mathf.Abs(rb.velocity.x) >= 0.01f)) // Bu karakterin x eksenindeki hýzýnýn mutlak deðerini alýyo aldýðý deðer 0.01 den fazlaysa koþabilir diyo mutlak deðer almamýzýn sebebiyse karakterin hýzý saða giderken pozitif sola giderken negatif ondan
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

    private void Flip() //Karakteri herhangi bir gameobjesinden component çekmeden döndürme
    {
        if (!isWallSliding && canFlip)
        {
            facingDirection *= -1;
            facingRight = !facingRight;
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }
    }

    private void CheckSurroundings() // Geldik bizim poþetleri siyah hayale koy ustam kýsmýna knk bunu direkt uzunca anlatýcam iki methodun ortasýnda iyi oku
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);
    }

    /*Knk þimdi bu yukardaki CheckSurroundings kýsmý bizim belirlediðimiz çapta ve pozisyonda bir daire ve ýþý oluþturuyo isGrounded daireyi oluþturan isTouchingWall'da çizgiyi yani ýþýný oluþturan
     * Þimdi þöyleki groundCheck.position dediðimiz bu dairenin konumunu belirliyo groundCheckRadius dediðimiz de dairenin büyüklüðünü yani çapýný belirliyo 
     * en sondaki whatIsGround yani layermask kýsmýndaysa o yuvarlak içine giren þeyleri tanýmamýzý saðlýyo mesela layerMask þuanda ground olarak belli gidip bu tileMap çizdiðin ground objesi varya onun da layer'ýný ground yapýnca artýk
     *o dairenin içine giren birþeyi diyoki ha bu ground deðil ben þuan havadayým yere deyiyoya ha bu ground ben yerdeyim*/

    /*Alttaki metodun ne iþe yaradýðýný söylüyorum alttaki bu yukarda çizdiðimiz ýþýn ve yuvarlaðý görmemizi saðlýyo yani o yuvarlaðý ve ýþýný çiziyo sana görebilmen için abi bak bunlar böyle þuan diyo
     * ama alttaki olmasada olur sadece görmeni saðlýyo yani baþka hiçbir etkisi iþlevi yok*/

    private void OnDrawGizmos()//YORUM YAZ
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
    }

}
