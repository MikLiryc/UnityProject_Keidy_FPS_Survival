using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public string gunName;
    public float range;
    public float accuracy;
    public float fireRate;
    public float reloadTime;

    public int damage;

    public int reloadBulletCount;   //재장전 총알 개수
    public int currentBulletCount;  //현재 탄알집에 남아있는 총알개수
    public int maxBulletCount;      //최대 총알 소지개수
    public int carryBulletCount;    //현재 소유하고 있는 총알 개수

    public float retroActionForce;  //반동 세기
    public float retroActionFineSightForce; //정조준 시의 반동 세기

    public Vector3 fineSightOriginPos;      //정조준 시 총 위치
    public Animator anim;
    public ParticleSystem muzzleFlash;      //총구 섬광
    public AudioClip fireSound;             //소리
    

}
