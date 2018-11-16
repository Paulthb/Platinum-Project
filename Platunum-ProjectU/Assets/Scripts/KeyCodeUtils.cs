using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyCodeUtils {

	public static KeyCode GetKeyCode(string KeyString)
    {
        return (KeyCode)System.Enum.Parse(typeof(KeyCode), KeyString);
    }
}
