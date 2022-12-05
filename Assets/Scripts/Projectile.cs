using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Projectile : NetworkBehaviour
{
    public WeaponController parent;
    [SerializeField]
    private GameObject  hitParticle;

    private Rigidbody  rb;

    public float shootForce;


    // Start is called before the first frame update
    void Start()
    {
         rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
         rb.velocity =  rb.transform.forward * shootForce;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!IsOwner)
        {
            return;
        }
        InstantiateHitParticlesServerRpc();
        parent.DestroyServerRpc();
    }

    [ServerRpc]
    private void InstantiateHitParticlesServerRpc()
    {
        GameObject hitImpact = Instantiate(hitParticle, transform.position, Quaternion.identity);
        hitImpact.GetComponent<NetworkObject>().Spawn();
    }
}
