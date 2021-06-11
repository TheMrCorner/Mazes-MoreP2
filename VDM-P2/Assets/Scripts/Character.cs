using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Sprite component of the character")]
    private SpriteRenderer characterSprite;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ChangeCharacterColor(Color color)
    {
        characterSprite.color = color;
    }
}
