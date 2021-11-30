using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ItemCollect : NetworkBehaviour
{
    private Dictionary<Item.VegetableType, int> ItemInventory = new Dictionary<Item.VegetableType, int>();

    public delegate void CollectItem(Item.VegetableType item);
    public static event CollectItem ItemCollected;

    Collider ItemCollider = null;
    // Start is called before the first frame update
    void Start()
    {
        foreach (Item.VegetableType item in System.Enum.GetValues(typeof(Item.VegetableType)))
        {
            ItemInventory.Add(item, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (ItemCollider && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("PRESSSSSSSED SPACEE");
            Item item = ItemCollider.gameObject.GetComponent<Item>();
            AddToInventory(item);
            PrintInventory();

            CmdItemCollected(item.typeOfVeggie);
        }
    }

    [Command]
    void CmdItemCollected(Item.VegetableType itemType)
    {
        ItemCollected?.Invoke(itemType);
        RpcItemCollected(itemType);
    }

    [ClientRpc]
    void RpcItemCollected(Item.VegetableType itemType)
    {
        if (isServer) return;
        ItemCollected?.Invoke(itemType);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isLocalPlayer)
        {
            return;
        }
        if (other.CompareTag("Item"))
        {
            ItemCollider = other;
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isLocalPlayer)
        {
            return;
        }
        if (other.CompareTag("Item") && Input.GetKeyDown(KeyCode.Space))
        {
            ItemCollider = null;
        }
    }

    private void AddToInventory(Item item)
    {
        ItemInventory[item.typeOfVeggie]++;
    }

    private void PrintInventory()
    {
        string output = "";

        foreach (KeyValuePair<Item.VegetableType, int> kvp in ItemInventory)
        {
            output += string.Format("{0}: {1} ", kvp.Key, kvp.Value);
        }
        Debug.Log(output);
    }
}


