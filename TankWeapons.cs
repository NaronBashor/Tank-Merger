using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankWeapons : MonoBehaviour
{
    TankMovement tankMove;
    Tank tank;
    Animator anim;

    [SerializeField] private List<Transform> cannonPos = new List<Transform>();

    [SerializeField] private List<GameObject> projectileList = new List<GameObject>();

    [SerializeField] private Transform projectileParent;

    private float shootCD;
    private float shootDelayTimer;

    private float initialShootDelay = 2f;
    private float minShootDelay = 0.5f;
    private int maxLevel = 10;

    private int number;

    private void Start()
    {
        tank = GetComponent<Tank>();
        tankMove = GetComponent<TankMovement>();
        anim = GetComponent<Animator>();
        number = RandomNumber();
        projectileParent = GameObject.Find("Projectiles").GetComponent<Transform>();
    }

    private void Update()
    {
        shootCD -= Time.deltaTime;
        if (shootCD <= 0) {
            if (tankMove.canShoot) {
                SoundManager.Instance.TriggerSoundEvent("shot");
                ShootBullet();
            }
            shootDelayTimer = CalculateShootDelay(tank.currentTankLevel);
            shootCD = shootDelayTimer;
        }
    }

    private float CalculateShootDelay(int level)
    {
        level = Mathf.Clamp(level + 1, 1, maxLevel);

        float scaleFactor = 0.75f; // Increase to make scaling faster

        float t = Mathf.Pow((float)(level - 1) / (maxLevel - 1), scaleFactor);
        //Debug.Log(Mathf.Lerp(initialShootDelay, minShootDelay, t));
        return Mathf.Lerp(initialShootDelay, minShootDelay, t);
    }

    public void ShootBullet()
    {
        anim.SetTrigger("Shoot");
        if (cannonPos.Count > 1) {
            GameObject bulletOne = Instantiate(projectileList[number], cannonPos[0].position, Quaternion.identity, projectileParent);
            GameObject bulletTwo = Instantiate(projectileList[number], cannonPos[1].position, Quaternion.identity, projectileParent);
            bulletOne.GetComponent<Projectile>().upgradeLevel = GetComponent<Tank>().currentTankLevel;
            bulletTwo.GetComponent<Projectile>().upgradeLevel = GetComponent<Tank>().currentTankLevel;
            StartCoroutine(FireDelay());
        } else {
            GameObject bullet = Instantiate(projectileList[number], cannonPos[0].position, Quaternion.identity, projectileParent);
            bullet.GetComponent<Projectile>().upgradeLevel = GetComponent<Tank>().currentTankLevel;
        }
    }

    public int RandomNumber()
    {
        int randomNumber = Random.Range(0, projectileList.Count);
        return randomNumber;
    }

    IEnumerator FireDelay()
    {
        yield return new WaitForSeconds(0.125f);
        Instantiate(projectileList[0], cannonPos[1].position, Quaternion.identity, projectileParent);
    }
}