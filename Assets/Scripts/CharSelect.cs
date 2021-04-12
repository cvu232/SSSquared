using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharSelect : MonoBehaviour {

    [HideInInspector]
    public CharSelectButton selectedButton = null;
    public RectTransform playerIndicator;
    public UnityEngine.UI.Button startButton;

    public int picker;

    private UIPanel parent;
    private bool _show;
    private TMPro.TextMeshProUGUI playerIndicatorLabel;
    private List<CharSelectButton> buttons = new List<CharSelectButton>();

    // Start is called before the first frame update
    void Start() {

        parent = GetComponentInParent<UIPanel>();

        for (int i = 0; i < transform.childCount; i++)
            if (transform.GetChild(i).GetComponent<CharSelectButton>())
                buttons.Add(transform.GetChild(i).GetComponent<CharSelectButton>());

        playerIndicatorLabel = playerIndicator.GetComponentInChildren<TMPro.TextMeshProUGUI>();

    }

    // Update is called once per frame
    void Update() {

        int selectedButtonIndex = 0;

        if (selectedButton != null)
            selectedButtonIndex = selectedButton.transform.GetSiblingIndex();

        for (int i = 0; i < buttons.Count; i++) {

            if (selectedButton == null) {
                buttons[i].status = CharSelectButtonStatus.regular;
            } else {
                buttons[i].status = (selectedButtonIndex == i) ? CharSelectButtonStatus.expanded : CharSelectButtonStatus.compact;
            }

        }

        if (parent.show && !_show) {
            picker = 0;
            GameOptions.instance.charactersPerPlayer = new List<Characters>();
            playerIndicator.gameObject.SetActive(true);
            foreach (CharSelectButton button in buttons)
                button.takenBy = -1;
        }

        _show = parent.show;

        if (picker >= GameOptions.instance.playerCount) {

            foreach (CharSelectButton button in buttons)
                if (button.takenBy == -1)
                    button.takenBy = -2;

            startButton.interactable = true;

            return;

        } else

            startButton.interactable = false;

        playerIndicator.transform.position = Vector3.Lerp(playerIndicator.transform.position, Input.mousePosition, Time.deltaTime * 10);
        playerIndicatorLabel.text = "Player " + (picker + 1);

    }

    public void RegisterCharacter (int index) {

        if (GameOptions.instance.charactersPerPlayer.Count <= picker)
            GameOptions.instance.charactersPerPlayer.Add((Characters) index);
        else
            GameOptions.instance.charactersPerPlayer[picker] = (Characters)index;

        RectTransform newIndicator = Instantiate(playerIndicator.gameObject).GetComponent<RectTransform>();

        buttons[index].playerIndicator = newIndicator;
        buttons[index].takenBy = picker;
        newIndicator.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "P" + (picker + 1);
        newIndicator.GetComponentInChildren<TMPro.TextMeshProUGUI>().fontSize *= 2;
        newIndicator.transform.parent = playerIndicator.parent;
        newIndicator.transform.localPosition = playerIndicator.transform.localPosition;

        if (GameOptions.instance.playerCount - 1 == picker)
            playerIndicator.gameObject.SetActive(false);

        picker++;

    }

}