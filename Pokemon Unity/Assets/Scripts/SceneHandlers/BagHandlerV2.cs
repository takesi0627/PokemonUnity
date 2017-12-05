using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BagHandlerV2 : MonoBehaviour {
	public enum PageType {
		ePT_Items,
		ePT_Medicine,
		ePT_Berries,
		ePT_TM,
		ePT_KeyItems,
		ePT_Party
	}

    private struct SPartySlot
    {
		public GameObject slot;
		public Image BG;
		public Image PMIcon;
		public Sprite[] PMAnimate;
		public Image Status;
		public Image HoldItem;

        public Text Name;
        public Text Level;
        public Text Gender;

		public Slider HPBar;

    }

    private SPartySlot[] m_kParty = new SPartySlot[6];
    
	private Text m_kItemDiscription;
	private Text m_kBagName;

	private PageType m_nSelectedPage;
	private short m_nSelectedItem;
	private bool m_bShopMode;
	private string[] m_kCurrentItemList;
	private short m_nVisibleSlots;

	[SerializeField] private AudioClip m_kSelectClip;
	[SerializeField] private AudioClip m_kHealClip;
	[SerializeField] private AudioClip m_kTMBootupClip;
	[SerializeField] private AudioClip m_kForgetMoveClip;
	[SerializeField] private AudioClip m_kSaleClip;
	[SerializeField] private Sprite[] m_kBagForeground;

    void Awake ()
    {
		GameObject ItemList = transform.Find ("ItemList").gameObject;
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
			m_kParty [i].HPBar = kPartySlot.transform.Find ("StandardDisplay").gameObject.transform.Find ("HPBar").GetComponent<Slider> ();

			m_kParty [i].slot = kPartySlot;
			m_kParty [i].PMAnimate = new Sprite[2];
		}

		m_kItemDiscription = transform.Find ("ItemDiscription").GetComponent <Text> ();
		m_kBagName = transform.Find ("BagName").GetComponent <Text> ();

		m_nSelectedPage = PageType.ePT_Items;
    }

	// Use this for initialization
	void Start () {
		UpdateParty ();
		StartCoroutine (PokemonAnimate ());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void UpdateScreen () {
		switch (m_nSelectedPage) {
		case PageType.ePT_Items:
			m_nVisibleSlots = 6;
			m_kItemDiscription.gameObject.SetActive (true);
			m_kCurrentItemList = SaveData.currentSave.Bag.getItemTypeArray (ItemData.ItemType.ITEM);
			break;
		case PageType.ePT_Medicine:
			m_nVisibleSlots = 6;
			m_kItemDiscription.gameObject.SetActive (true);
			m_kCurrentItemList = SaveData.currentSave.Bag.getItemTypeArray (ItemData.ItemType.MEDICINE);
			break;
		case PageType.ePT_Berries:
			m_nVisibleSlots = 6;
			m_kItemDiscription.gameObject.SetActive (true);
			m_kCurrentItemList = SaveData.currentSave.Bag.getItemTypeArray (ItemData.ItemType.BERRY);
			break;
		case PageType.ePT_KeyItems:
			m_nVisibleSlots = 6;
			m_kItemDiscription.gameObject.SetActive (true);
			m_kCurrentItemList = SaveData.currentSave.Bag.getItemTypeArray (ItemData.ItemType.KEY);
			break;
		case PageType.ePT_TM:
			m_kItemDiscription.gameObject.SetActive (false);
			m_nVisibleSlots = 5;
			m_kCurrentItemList = SaveData.currentSave.Bag.getItemTypeArray (ItemData.ItemType.TM);
			break;
		default:
			return;	
		}
	}

	private void UpdateParty (){
		Pokemon currentPokemon;
		for (int i = 0; i < GlobalVariables.MAX_BAG_PM_AMOUNT; i++) {
			currentPokemon = SaveData.currentSave.PC.boxes [0] [i];
			if (currentPokemon == null) {
				m_kParty [i].slot.gameObject.SetActive (false);
			} else {
				// set icon
				m_kParty [i].PMAnimate = currentPokemon.GetIcons_ ();
				m_kParty [i].PMIcon.sprite = m_kParty [i].PMAnimate [0];

				// set name
				m_kParty [i].Name.text = currentPokemon.getName ();

				// set level
				m_kParty [i].Level.text = currentPokemon.getLevel ().ToString ();

				// set HPBar
				m_kParty [i].HPBar.value = Mathf.FloorToInt ((float)currentPokemon.getCurrentHP () / currentPokemon.getHP () * 100);
				Image kFill;
				kFill = m_kParty [i].HPBar.gameObject.transform.Find ("Fill Area").gameObject.transform.Find ("Fill").GetComponent <Image> ();
				if (m_kParty [i].HPBar.value > 50)
					kFill.color = new Color (0.125f, 1, 0.065f, 1);
				else if (m_kParty [i].HPBar.value > 25)
					kFill.color = new Color (1, 0.75f, 0, 1);
				else if (m_kParty [i].HPBar.value > 0)
					kFill.color = new Color (1, 0.125f, 0, 1);
				else
					kFill.color = new Color (0, 0, 0, 0);
					

				// set gender
				switch (currentPokemon.getGender ()) {
				case Pokemon.Gender.FEMALE:
					m_kParty [i].Gender.enabled = true;
					m_kParty [i].Gender.text = "♀";
					m_kParty [i].Gender.color = new Color (1, 0.2f, 0.2f, 1);
					break;
				case Pokemon.Gender.MALE:
					m_kParty [i].Gender.enabled = true;
					m_kParty [i].Gender.text = "♂";
					m_kParty [i].Gender.color = new Color (0.2f, 0.4f, 1, 1);
					break;
				default:
					m_kParty [i].Gender.enabled = false;
					break;
				}
			
				// status
				if (currentPokemon.getStatus () != Pokemon.Status.NONE) {
					m_kParty [i].Status.enabled = true;
					m_kParty [i].Status.sprite = Resources.Load<Sprite> ("PCSprites/status" + currentPokemon.getStatus ().ToString ());				
				} else {
					m_kParty [i].Status.sprite = null;
					m_kParty [i].Status.enabled = false;
				}

				// item
				if (!string.IsNullOrEmpty(currentPokemon.getHeldItem()))
					m_kParty [i].HoldItem.enabled = true;
				else
					m_kParty [i].HoldItem.enabled = false;
			}
		}
	}

	private IEnumerator PokemonAnimate () {
		while (true) {
			for (int i = 0; i < GlobalVariables.MAX_BAG_PM_AMOUNT; i++) {
				if (m_kParty [i].slot.gameObject.activeInHierarchy) {
					PokemonAnimate (i);
				}
			}
		}
	}
	private IEnumerator PokemonAnimate (int i) {
		if (m_kParty [i].PMAnimate [0] != null) {
			m_kParty [i].PMIcon.sprite = m_kParty [i].PMAnimate [0];
		}
		if (m_kParty [i].Status != null)
			yield return new WaitForSeconds (0.35f);
		else
			yield return new WaitForSeconds (0.15f);
		if (m_kParty [i].PMAnimate [1] != null) {
			m_kParty [i].PMIcon.sprite = m_kParty [i].PMAnimate [1];
		}
		if (m_kParty [i].Status != null)
			yield return new WaitForSeconds (0.35f);
		else
			yield return new WaitForSeconds (0.15f);
	}
}