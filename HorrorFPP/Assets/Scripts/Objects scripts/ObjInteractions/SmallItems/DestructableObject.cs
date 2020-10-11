using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableObject : MonoBehaviour
{
    [SerializeField] private ItemBase item;

    private void OnCollisionEnter(Collision collision)
    {
        if(item.itemDrop && collision.collider.tag == "Surface")
        {
            item.itemDrop = false;
            item.SwitchToBreakMode();
            Debug.Log("BROKEN MODE");
        }

        //if (itemDrop && itemMode != enFoodCondition.cold && collision.collider.tag == "Surface")
        //{
        //    itemDrop = false;
        //    Debug.Log("Crash it!");
        //    this.transform.rotation = new Quaternion(180, 0, 0, 0);
        //    pork.transform.localPosition += new Vector3(-0.2f, 0, 0);
        //    potato.transform.localPosition += new Vector3(0.3f, 0, 0);
        //    salad.transform.localPosition += new Vector3(0, 0, -0.3f);
        //    plate.transform.localPosition += new Vector3(0, -0.08f, 0);
        //}
    }
}
