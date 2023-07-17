using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class openInstagram : MonoBehaviour
{
    public void instaUserNameLink(GameObject textParent)
    {
        string name = textParent.GetComponent<Text>().text;
        Application.OpenURL("https://www.instagram.com/" + name + "?utm_medium=copy_link");
    }
}
