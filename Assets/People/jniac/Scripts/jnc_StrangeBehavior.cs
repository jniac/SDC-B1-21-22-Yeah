using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CubeSnapping), typeof(CubeMove))]
public class jnc_StrangeBehavior : MonoBehaviour
{
    public string blockTagAndLayer = "PlayerBlock";

    public bool IsBlock { get; private set; }

    void BlockEnter()
    {
        if (IsBlock == false)
        {
            IsBlock = true;

            GetComponent<CubeMove>().enabled = false;

            transform.Find("Ghost").gameObject.SetActive(false);
            transform.Find("GreyCube").gameObject.SetActive(true);

            foreach (var child in Utils.AllChildren(transform))
            {
                child.gameObject.tag = blockTagAndLayer;
                child.gameObject.layer = LayerMask.NameToLayer(blockTagAndLayer);
            }
        }
    }

    void BlockExit()
    {
        if (IsBlock == true)
        {
            IsBlock = false;

            GetComponent<CubeMove>().enabled = true;

            transform.Find("Ghost").gameObject.SetActive(true);
            transform.Find("GreyCube").gameObject.SetActive(false);

            foreach (var child in Utils.AllChildren(transform))
            {
                child.gameObject.tag = "Player";
                child.gameObject.layer = LayerMask.NameToLayer("Player");
            }
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Block"))
            BlockEnter();

        if (Input.GetButtonUp("Block"))
            BlockExit();
    }

    void LateUpdate()
    {
        if (IsBlock)
        {
            var (position, rotation) = GetComponent<CubeSnapping>().GetSnapPositionRotation();
            transform.position = position;
            transform.rotation = rotation;
        }
    }
}
