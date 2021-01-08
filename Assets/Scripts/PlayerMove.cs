using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{

    public float moveSpeed = 7f;

    CharacterController cc;

    float gravity = -20f;

    public float yVelocity = 0;

    public float jumpPower = 10f;

    public bool isJumping = false;

    public int hp = 20;

    int maxHP = 20;

    public Slider hpSlider;

    public GameObject hitEffect;

    Animator anim;
    
    // Start is called before the first frame update

    void Start()
    {
        cc = GetComponent<CharacterController>();

        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.gm.gState != GameManager.GameState.Run)
        {
            return;
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(h, 0, v);

        dir = dir.normalized;

        anim.SetFloat("MoveMotion", dir.magnitude);

        dir = Camera.main.transform.TransformDirection(dir);

        if(cc.collisionFlags == CollisionFlags.Below)
        {
            if (isJumping)
            {
                isJumping = false;
            }

            yVelocity = 0;
        }

        if (Input.GetButtonDown("Jump") && !isJumping)
        {
            yVelocity = jumpPower;
            isJumping = true;
        }

       

        yVelocity += gravity * Time.deltaTime;

        dir.y = yVelocity;

        cc.Move(dir * moveSpeed * Time.deltaTime);

        hpSlider.value = (float)hp / (float)maxHP;
    }

    public void DamageAction(int damage)
    {
        hp -= damage;

        if(hp>0)
        {
            StartCoroutine(PlayerHitEffect());
        }
    }

    IEnumerator PlayerHitEffect()
    {
        hitEffect.SetActive(true);

        yield return new WaitForSeconds(0.3f);

        hitEffect.SetActive(false);
    }
}
