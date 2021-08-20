using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class WeaponPickUp : MonoBehaviour
{
    public GameObject weaponPrefab;

    //if pick up and not in weapons list, instantiate and set to current
    //otherwise, just give ammo

    // Start is called before the first frame update
    void Start()
    {
        if (weaponPrefab != null)
        {
            GameObject temp = Instantiate(weaponPrefab.GetComponentInChildren<MeshRenderer>().gameObject);
            temp.transform.SetParent(gameObject.transform);
            temp.transform.localPosition = Vector3.zero;
            temp.transform.localScale = Vector3.one / 2f;
        }
        else if (name[0] != 'A' && name[0] != 'H')
        {
            Debug.Log("!Warning! WeaponPickup at [ " + transform.position + " ] has no weapon");
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, 1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if(weaponPrefab == null)
            {
                //Destroy(gameObject);
                return;
            }

            PlayerController player = other.gameObject.GetComponent<PlayerController>();

            bool check = false;
            int checkIndex = 0;

            foreach(BaseWeapon weapon in player.weapons)
            {
                if(weapon.name == weaponPrefab.name || weapon.name == weaponPrefab.name+"(Clone)")
                {
                    check = true;
                    checkIndex = player.weapons.IndexOf(weapon);
                    break;
                }
            }

            //if (player.weapons.Contains(weaponPrefab.GetComponent<BaseWeapon>()))
            if (check)
            {
                //player.weapons[player.weapons.IndexOf(weaponPrefab.GetComponent<BaseWeapon>())].AddAmmo();
                player.weapons[checkIndex].AddAmmo();
                //play this weapon's reload sound at player location
            }
            else
            {
                GameObject temp = Instantiate(weaponPrefab);
                player.weapons.Add(temp.GetComponent<BaseWeapon>());
                player.SwitchWeapons(player.weapons.IndexOf(temp.GetComponent<BaseWeapon>()));
                //play current weapon's reload sound
            }

            //play a sound?
            Destroy(gameObject);
        }
    }
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (weaponPrefab != null)
            Handles.Label(transform.position, weaponPrefab.name);
        else if(name[0] =='A')
            Handles.Label(transform.position, "Ammo");
        else if (name[0] == 'H')
            Handles.Label(transform.position, "Health");
        else
            Handles.Label(transform.position, "null");
    }
#endif
}
