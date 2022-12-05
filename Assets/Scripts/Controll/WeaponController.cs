using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class WeaponController : NetworkBehaviour
{
    [SerializeField]
    private List<GameObject> spawnedProjectilies = new List<GameObject>();

    [SerializeField]
    private Transform  weaponHold;
    [SerializeField]
    private Transform  muzzle;
    [SerializeField]
    private Weapon  startingWeapon;

    private Weapon  equippedWeapon;
    private float  nextShootTime;

    private void Start()
    {
        if( startingWeapon != null)
        {
            EquipWeapon( startingWeapon);
        }
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        if (Input.GetMouseButton(0))
        {
            ShootServerRpc();
        }
    }

    public void EquipWeapon(Weapon weaponToEquip)
    {
        if( equippedWeapon != null)
        {
            Destroy( equippedWeapon.gameObject);
        }
         equippedWeapon = Instantiate(weaponToEquip,  weaponHold.position,  weaponHold.rotation);
         equippedWeapon.transform.Rotate(new Vector3(-90,0,0));
         equippedWeapon.transform.SetParent( weaponHold);
    }

    [ServerRpc]
    private void ShootServerRpc()
    {
        if (Time.time > nextShootTime)
        {
            nextShootTime = Time.time + equippedWeapon.msBetweenShots / 1000;
            Projectile newProjectile = Instantiate(equippedWeapon.projectile, muzzle.position, muzzle.rotation);
            GameObject projectileGo = newProjectile.gameObject;
            newProjectile.shootForce = equippedWeapon.muzzleVelocity;
            spawnedProjectilies.Add(projectileGo);
            newProjectile.parent = this;
            projectileGo.GetComponent<NetworkObject>().Spawn();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyServerRpc()
    {
        GameObject toDestroy = spawnedProjectilies[0];
        toDestroy.GetComponent<NetworkObject>().Despawn();
        spawnedProjectilies.Remove(toDestroy);
        Destroy(toDestroy);
    }
}
