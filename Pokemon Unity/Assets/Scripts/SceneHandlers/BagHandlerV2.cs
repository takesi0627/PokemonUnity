using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagHandlerV2 : MonoBehaviour {
    private struct SPartySlot
    {
		public GameObject slot;
        public Image BG;
        public Image PMIcon;
		public Image Status;
		public Image HoldItem;

        public Text Name;
        public Text Level;
        public Text Gender;

		public Image HPBar;
    }

    private SPartySlot[] m_kParty = new SPartySlot[6];
    
    void Awake ()
    {
		GameObject kParty = transform.Find ("Party").gameObject;
		for (int i = 0 ; i < m_kParty.GetLength (0) ; i++) 
        {
			GameObject kPartySlot = kParty.transform.Find ("Slot0" + (i + 1)).gameObject;
			m_kParty [i].BG = kPartySlot.transform.Find ("BG").GetComponent<Image> ();
			m_kParty [i].PMIcon = kPartySlot.transform.Find ("Icon").GetComponent<Image> ();
			m_kParty [i].Status = kPartySlot.transform.Find ("Status").GetComponent<Image> ();
			m_kParty [i].HoldItem = kPartySlot.transform.Find ("Item").GetComponent<Image> ();

			m_kParty [i].Name = kPartySlot.transform.Find ("Name").GetComponent<Text> ();
			m_kParty [i].Gender = kPartySlot.transform.Find ("Gender").GetComponent<Text> ();
			m_kParty [i].Level = kPartySlot.transform.Find ("StandardDisplay").gameObject.transform.Find ("LvText").GetComponent<Text> ();
			m_kParty [i].HPBar = kPartySlot.transform.Find ("StandardDisplay").gameObject.transform.Find ("HPBar").GetComponent<Image> ();

			m_kParty [i].slot = kPartySlot;
		}
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void updateParty (){
		Pokemon currentPokemon;
		for (int i = 0; i < GlobalVariables.MAX_BAG_PM_AMOUNT; i++) {
			currentPokemon = SaveData.currentSave.PC.boxes [0] [i];
			if (currentPokemon == null) {
				m_kParty [i].slot.gameObject.SetActive (false);
			} else {
				m_kParty [i].PMIcon = currentPokemon.GetIcons_ () [0];

			}
		}
	}
}