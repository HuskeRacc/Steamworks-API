using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CharacterSelect : NetworkBehaviour
{
    [SerializeField] private GameObject characterSelectDisplay = default;
    [SerializeField] private Transform characterPreviewParent = default;
    [SerializeField] private TMP_Text characterNameText = default;
    [SerializeField] private float turnspeed = 90f;
    [SerializeField] private Character[] characters = default;
    [SerializeField] private GameObject networkPlayer;

    private int currentCharacterIndex = 0;
    private List<GameObject> characterInstances = new List<GameObject>();

    public override void OnStartClient()
    {
        networkPlayer = GameObject.FindGameObjectWithTag("NetworkPlayer");
        Destroy(networkPlayer);
        if (characterPreviewParent.childCount == 0)
        {
            foreach (var character in characters)
            {
                GameObject characterInstance = Instantiate(character.CharacterPreviewPrefab, characterPreviewParent);

                characterInstance.SetActive(false);

                characterInstances.Add(characterInstance);
            }
        }

        characterInstances[currentCharacterIndex].SetActive(true);
        characterNameText.text = characters[currentCharacterIndex].CharacterName;

        characterSelectDisplay.SetActive(true);
    }
    public void Right()
    {
        characterInstances[currentCharacterIndex].SetActive(false);

        currentCharacterIndex = (currentCharacterIndex + 1) % characterInstances.Count;

        characterInstances[currentCharacterIndex].SetActive(true);
        characterNameText.text = characters[currentCharacterIndex].CharacterName;
    }

    public void Left()
    {
        characterInstances[currentCharacterIndex].SetActive(false);

        currentCharacterIndex--;
        if(currentCharacterIndex < 0)
        {
            currentCharacterIndex += characterInstances.Count;
        }

        characterInstances[currentCharacterIndex].SetActive(true);
        characterNameText.text = characters[currentCharacterIndex].CharacterName;
    }

    private void Update()
    {
        characterPreviewParent.RotateAround(characterPreviewParent.position, characterPreviewParent.up, turnspeed * Time.deltaTime);
    }

    public void Select()
    {
        CmdSelect(currentCharacterIndex);
        characterSelectDisplay.SetActive(false);
    }

    [Command(requiresAuthority = false)]
    public void CmdSelect(int characterIndex, NetworkConnectionToClient sender = null)
    {
        GameObject characterInstance = Instantiate(characters[characterIndex].GameplayCharacterPrefab);
        NetworkServer.ReplacePlayerForConnection(sender, characterInstance);
    }

}
