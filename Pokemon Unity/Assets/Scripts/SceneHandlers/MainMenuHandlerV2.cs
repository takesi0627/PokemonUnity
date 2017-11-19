using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Security.AccessControl;
using System;

public class MainMenuHandlerV2 : MonoBehaviour {
	private short MAX_FILE_AMOUNT = 3;
	private short BUTTON_AMOUNT = 3 ;

	enum EButtonType {
		eBT_Continue,
		eBT_NewGame,
		eBT_Settings,
		eBT_End,

		eBT_Begin = eBT_Continue,
	}

	private GameObject m_FileData;
	private GameObject m_ContinueBtn;


	// Continue Btn Components
//	private Image m_ContinueBtnBG;
	private Text m_ContinueTxt;
	private Text m_FileTxt;
	private Text[] m_File;

	// File Data Components
	private Image[] m_PokemonImg;
	[SerializeField] private Sprite[,] m_PokemonAnim;
	private Text m_FileFields;
	private Text m_FileDatasText;
	private Text m_FileMapName;

	private Image[] m_pkButtonsBG;


	private EButtonType m_SelectedBtn;
	private short m_SelectedFile;
	private bool running;
	private int m_FileCount;

	[SerializeField] private Sprite ButtonDimImg;
	[SerializeField] private Sprite ButtonSelectImg;

	[SerializeField] private Color m_OrigFileTextColor;
	void Awake () {
		SaveLoad.Load ();

		m_FileData = transform.Find ("FileData").gameObject;
		m_ContinueBtn = transform.Find ("ContinueBtn").gameObject;

		m_ContinueTxt = m_ContinueBtn.transform.Find ("ContinueTxt").GetComponent <Text> ();
		m_FileTxt = m_ContinueBtn.transform.Find ("FileText").GetComponent <Text> ();
		m_File = new Text[MAX_FILE_AMOUNT];
		for (int i = 0; i < MAX_FILE_AMOUNT; i++)
			m_File [i] = m_ContinueBtn.transform.Find ("File" + (i + 1)).GetComponent<Text> ();

		// File Data
		m_PokemonImg = new Image[GlobalVariables.MAX_BAG_PM_AMOUNT];
		for (int i = 0; i < GlobalVariables.MAX_BAG_PM_AMOUNT ; i++) {
			m_PokemonImg [i] = (m_FileData.transform.Find ("Pokemons").gameObject).transform.Find ("" + (i + 1)).GetComponent <Image> ();
		}
		m_PokemonAnim = new Sprite[GlobalVariables.MAX_BAG_PM_AMOUNT, 2];
		m_FileFields = m_FileData.transform.Find ("Fields").GetComponent <Text> ();
		m_FileDatasText = m_FileData.transform.Find ("Datas").GetComponent <Text> ();
		m_FileMapName = m_FileData.transform.Find ("MapName").GetComponent <Text> ();

		m_pkButtonsBG = new Image[BUTTON_AMOUNT];
		for (EButtonType i = EButtonType.eBT_Begin; i < EButtonType.eBT_End; i++) {
			switch (i) {
			case EButtonType.eBT_Continue:
				m_pkButtonsBG [i.GetHashCode ()] = m_ContinueBtn.transform.Find ("BG").GetComponent <Image> ();
				break;
			case EButtonType.eBT_NewGame:
				m_pkButtonsBG [i.GetHashCode ()] = (transform.Find ("NewGameBtn").gameObject).transform.Find ("BG").GetComponent <Image> ();
				break;
			case EButtonType.eBT_Settings:
				m_pkButtonsBG [i.GetHashCode ()] = (transform.Find ("SettingBtn").gameObject).transform.Find ("BG").GetComponent <Image> ();
				break;
			}
		}

//		SaveLoad.savedGames [0] = SaveData.currentSave;
	}

	// Use this for initialization
	void Start () {
		m_FileCount = SaveLoad.getSavedGamesCount ();
		if (m_FileCount == 0) {
			m_FileData.SetActive (false);
			m_ContinueBtn.SetActive (false);
			m_SelectedBtn = EButtonType.eBT_NewGame;
		} else {
			for (int i = m_FileCount ; i < MAX_FILE_AMOUNT; i++) {
				m_File [i].enabled = false;
			}
			m_SelectedBtn = EButtonType.eBT_Continue;
			m_SelectedFile = 0;
			UpdateFile (m_SelectedFile);
			UpdateBtn (m_SelectedBtn);
			StartCoroutine (PokemonAnimate ());
		}
		StartCoroutine (ControlHandle ());
	}

	public IEnumerator ControlHandle () {
		running = true;

		while (running) {
			if (Input.GetAxis ("Vertical") > 0) {
				m_SelectedBtn = (m_SelectedBtn == EButtonType.eBT_Begin ? EButtonType.eBT_End - 1 : m_SelectedBtn - 1);
				UpdateBtn (m_SelectedBtn);
				yield return new WaitForSeconds (0.2f);
			} else if (Input.GetAxis ("Vertical") < 0) {
				m_SelectedBtn = (m_SelectedBtn + 1 == EButtonType.eBT_End ? EButtonType.eBT_Begin : m_SelectedBtn + 1);
				UpdateBtn (m_SelectedBtn);
				yield return new WaitForSeconds (0.2f);
			} else if (Input.GetAxis ("Horizontal") > 0) {
				switch (m_SelectedBtn) {
				case EButtonType.eBT_Continue:
					m_SelectedFile = (short)Math.Min (m_SelectedFile + 1, m_FileCount - 1);
					UpdateFile (m_SelectedFile);
					yield return new WaitForSeconds (0.2f);
					break;
				}
			} else if (Input.GetAxis ("Horizontal") < 0) {
				switch (m_SelectedBtn) {
				case EButtonType.eBT_Continue:
					m_SelectedFile = (short)Math.Max (m_SelectedFile - 1, 0);
					UpdateFile (m_SelectedFile);
					yield return new WaitForSeconds (0.2f);
					break;
				}
			}
			yield return null;
		}
	}
	private void UpdateFile (int file_num) {
		for (int i = 0; i < m_FileCount; i++)
			m_File [i].color = (i == file_num ? Color.red : m_OrigFileTextColor);

		if (SaveLoad.savedGames[m_SelectedFile] != null)
		{
			int badgeTotal = 0;
			for (int i = 0; i < 12; i++)
			{
				if (SaveLoad.savedGames[m_SelectedFile].gymsBeaten[i])
				{
					badgeTotal += 1;
				}
			}
			string playerTime = "" + SaveLoad.savedGames[m_SelectedFile].playerMinutes;
			if (playerTime.Length == 1)
			{
				playerTime = "0" + playerTime;
			}
			playerTime = SaveLoad.savedGames[m_SelectedFile].playerHours + " : " + playerTime;

			m_FileMapName.text = SaveLoad.savedGames[m_SelectedFile].mapName;
			m_FileDatasText.text = SaveLoad.savedGames[m_SelectedFile].playerName
				+ "\n" + badgeTotal
				+ "\n" + "0" //Pokedex not yet implemented
				+ "\n" + playerTime;

			for (int i = 0; i < 6; i++)
			{
				if (SaveLoad.savedGames[m_SelectedFile].PC.boxes[0][i] != null)
				{
					m_PokemonAnim [i, 0] = SaveLoad.savedGames [m_SelectedFile].PC.boxes [0] [i].GetIcons_ () [0];
					m_PokemonAnim [i, 1] = SaveLoad.savedGames [m_SelectedFile].PC.boxes [0] [i].GetIcons_ () [1];
				}
				else
				{
					m_PokemonImg[i].sprite = null;
				}
			}
		}

	}

	private void UpdateBtn (EButtonType btn) {
		for (EButtonType i = EButtonType.eBT_Begin; i < EButtonType.eBT_End; i++) {
			m_pkButtonsBG [i.GetHashCode ()].sprite = (btn == i ? ButtonSelectImg : ButtonDimImg);
		}
	}

	private IEnumerator PokemonAnimate () {
		while (true) {
			for (int i = 0; i < GlobalVariables.MAX_BAG_PM_AMOUNT; i++) {
				if (m_PokemonAnim [i, 0] != null) {
					m_PokemonImg [i].enabled = true;
					m_PokemonImg [i].sprite = m_PokemonAnim [i, 0];
				} else {
					m_PokemonImg [i].enabled = false;
				}
			}
			yield return new WaitForSeconds (0.15f);
			for (int i = 0; i < GlobalVariables.MAX_BAG_PM_AMOUNT; i++) {
				if (m_PokemonAnim [i, 1] != null) {
					m_PokemonImg [i].enabled = true;
					m_PokemonImg [i].sprite = m_PokemonAnim [i, 1];
				} else {
					m_PokemonImg [i].enabled = false;
				}
			}
			yield return new WaitForSeconds (0.15f);

		}
	}
}
