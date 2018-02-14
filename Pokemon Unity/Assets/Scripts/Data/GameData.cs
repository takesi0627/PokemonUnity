using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public static class GameData {
	public delegate bool Deserialize (string [] stream);

	public static void LoadDB () {
		bool result = true;
		string rootPath = Application.dataPath + "/GameData";

		Debug.Log ("====================Start Initalize Database=====================");
		// Pokedex.ini
		result = Init (rootPath + "/Pokedex.ini", new Deserialize (PokemonDatabase.Deserialize));
	}


	private static bool Init (string filePath, Deserialize deserialize) {
		System.IO.StreamReader stream = System.IO.File.OpenText (filePath);

        string buffer = "";
        while ((buffer = stream.ReadLine ()) != null) {
			string [] sstream = buffer.Split ('|');
			Debug.Assert (deserialize (sstream), "[ERROR] Database Initalize Failed When Loading File " + filePath);
		}

		return true;
	}




	public static int GetIntFromStream (string [] stream, int index) {
		return Utilities.ParseFast (stream.GetValue (index).ToString ());
	}
	public static float GetFloatFromStream (string [] stream, int index) {
		return Convert.ToSingle (stream.GetValue (index).ToString ());
	}
	public static object GetEnumFromStream (string [] stream, int index, Type enumType) {
		return Enum.Parse (enumType, stream.GetValue (index).ToString ());
	}
	public static string GetStrFromStream (string [] stream, int index) {
		return stream.GetValue (index).ToString ();
	}
}
