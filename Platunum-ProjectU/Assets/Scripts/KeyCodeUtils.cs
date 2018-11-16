using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyCode {

	public static KeyCodeUtils GetKeyCode(string KeyString)
    {
        return (KeyCode)System.Enum.Parse(typeof(KeyCode), KeyString);
    }
}
