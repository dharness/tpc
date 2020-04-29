using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterClothes : MonoBehaviour
{
    public PlayerInput playerInput;
    int currentMeshIndex = 0;
    public GameObject[] meshes;

    public void Awake()
    {
        for(int i = 0; i < meshes.Length; i++)
        {
            bool active = i == currentMeshIndex;
            meshes[i].SetActive(active);
        }
        playerInput.onChangeClothesButtonPressed += this.ChangeClothes;
    }

    void ChangeClothes()
    {
        var nextIndex = (currentMeshIndex + 1) % meshes.Length;
        meshes[currentMeshIndex].SetActive(false);
        meshes[nextIndex].SetActive(true);
        currentMeshIndex = nextIndex;
    }
}
