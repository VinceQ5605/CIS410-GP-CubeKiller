using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    float X;
    float Y;
    Quaternion fromRotation;
    Quaternion toRotation;
    public Camera cam;

    public GameObject grenade;
    Rigidbody grenadeRB;
    public Transform shotPos;
    public GameObject explosion;
    public float firePower;
    public int selected;
    

    void Start()
    {
        selected = 0;
    }

    // Update is called once per frame
    void Update()
    {
        /*RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit))
        {
            Debug.Log(hit.transform.name);
        }*/
        //Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        //if(Physics.Raycast(ray, out hit)){
        //        X -= Input.GetAxis("Mouse Y");
        //        Y += Input.GetAxis("Mouse X");
        //        fromRotation = transform.rotation;
        //        toRotation = Quaternion.Euler(X, Y, 0);
        //    transform.rotation = Quaternion.Lerp(fromRotation, toRotation, Time.deltaTime);   
        //}
        if (Input.GetMouseButtonDown(0))
        {
            FireGrenadeBasic();
        }
        if (Input.GetMouseButtonDown(1))
        {
            FireSelectedSpecialGrenade();
        }
    }

    void FireGrenadeBasic()
    {
        Vector3 direction = cam.transform.forward;
        GameObject grenadeCopy = Instantiate(grenade, shotPos.position, shotPos.rotation) as GameObject;
        grenadeCopy.GetComponent<GrenadeController>().SetValues(0, .5f, 3, 50);
        grenadeRB = grenadeCopy.GetComponent<Rigidbody>();
        grenadeRB.AddForce(27 * direction, ForceMode.Impulse);
    }

    void FireSelectedSpecialGrenade()
    {
        Debug.Log("Firing " + selected.ToString());
        if (selected == 0)
        {
            FireGrenadeSpecial_0();
        }
        else if (selected == 1)
        {
            FireGrenadeSpecial_1();
        }
        else if (selected == 2)
        {
            FireGrenadeSpecial_2();
        }
        else if (selected == 3)
        {
            FireGrenadeSpecial_3();
        }
        else if (selected == 4)
        {
            FireGrenadeSpecial_4();
        }
    }

    void FireGrenadeSpecial_0()
    {
        Vector3 direction = cam.transform.forward;
        GameObject grenadeCopy = Instantiate(grenade, shotPos.position, shotPos.rotation) as GameObject;
        grenadeCopy.GetComponent<GrenadeController>().SetValues(1, .5f, 6, 100);
        grenadeRB = grenadeCopy.GetComponent<Rigidbody>();
        grenadeRB.AddForce(17 * direction, ForceMode.Impulse);
    }

    void FireGrenadeSpecial_1()
    {
        Vector3 direction = cam.transform.forward;
        GameObject grenadeCopy = Instantiate(grenade, shotPos.position, shotPos.rotation) as GameObject;
        grenadeCopy.GetComponent<GrenadeController>().SetValues(2, .5f, 6, 100);
        grenadeRB = grenadeCopy.GetComponent<Rigidbody>();
        grenadeRB.AddForce(17 * direction, ForceMode.Impulse);
    }

    void FireGrenadeSpecial_2()
    {
        Vector3 direction = cam.transform.forward;
        GameObject grenadeCopy = Instantiate(grenade, shotPos.position, shotPos.rotation) as GameObject;
        grenadeCopy.GetComponent<GrenadeController>().SetValues(3, .5f, 6, 100);
        grenadeRB = grenadeCopy.GetComponent<Rigidbody>();
        grenadeRB.AddForce(17 * direction, ForceMode.Impulse);
    }

    void FireGrenadeSpecial_3()
    {
        Vector3 direction = cam.transform.forward;
        GameObject grenadeCopy = Instantiate(grenade, shotPos.position, shotPos.rotation) as GameObject;
        grenadeCopy.GetComponent<GrenadeController>().SetValues(4, .5f, 6, 100);
        grenadeRB = grenadeCopy.GetComponent<Rigidbody>();
        grenadeRB.AddForce(17 * direction, ForceMode.Impulse);
    }

    void FireGrenadeSpecial_4()
    {
        Vector3 direction = cam.transform.forward;
        GameObject grenadeCopy = Instantiate(grenade, shotPos.position, shotPos.rotation) as GameObject;
        grenadeCopy.GetComponent<GrenadeController>().SetValues(5, .5f, 6, 100);
        grenadeRB = grenadeCopy.GetComponent<Rigidbody>();
        grenadeRB.AddForce(17 * direction, ForceMode.Impulse);
    }



}
