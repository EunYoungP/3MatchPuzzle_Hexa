using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scriptable;

public class BlockBehaviour : MonoBehaviour
{
    Block block;
    [SerializeField] BlockResource blockResource;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        block.blockBehaviour.UpdateBlockImage();
    }

    public void SetBlock(Block block)
    {
        this.block = block;
    }

    public void UpdateBlockImage()
    {
        if(block.type == BlockType.EMPTY)
        {
            spriteRenderer.sprite = null;
        }
        else if(block.type==BlockType.BASIC)
        {
            spriteRenderer.sprite = blockResource.basicBlocksprites[(int)block.blockVariety];
        }
    }
}
