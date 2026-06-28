using System.Collections;
using TMPro;
using UnityEngine;

public class TypewriterText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textBox;
    [SerializeField] TalkingSFX talkingSFX;
    [Header("Typing")]
    [SerializeField] float characterDelay = 0.025f;

    Coroutine typingRoutine;

    public bool IsTyping { get; private set; }

    public void ShowText(string message)
    {
        if (typingRoutine != null)
            StopCoroutine(typingRoutine);

        typingRoutine = StartCoroutine(TypeRoutine(message));
    }

    IEnumerator TypeRoutine(string message)
    {
        IsTyping = true;

        // Set the entire sentence immediately
        textBox.text = message;

        // Force TMP to build its mesh
        textBox.ForceMeshUpdate();

        // Hide all characters
        textBox.maxVisibleCharacters = 0;

        int totalCharacters = textBox.textInfo.characterCount;

        for (int i = 0; i < totalCharacters; i++)
        {
            // Reveal one more character
            textBox.maxVisibleCharacters = i + 1;

            char currentCharacter = message[i];
            talkingSFX?.TryPlaySound(currentCharacter);
            float delay = Random.Range(characterDelay * 0.9f, characterDelay * 1.1f);

            switch (currentCharacter)
            {
                case '.':
                case '!':
                case '?':
                    delay *= 8f;
                    break;

                case ',':
                    delay *= 4f;
                    break;

                case ' ':
                    delay *= 0.4f;
                    break;
            }

            yield return new WaitForSecondsRealtime(delay);
        }

        // Make absolutely sure everything is visible
        textBox.maxVisibleCharacters = int.MaxValue;

        IsTyping = false;
    }
}