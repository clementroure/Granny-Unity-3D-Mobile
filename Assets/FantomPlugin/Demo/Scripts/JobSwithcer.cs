using UnityEngine;
using UnityEngine.UI;

//Classic と JobSystem を切り替える
public class JobSwithcer : MonoBehaviour {

    public GameObject[] sampleItems;    //[0] is classic
    public Toggle[] toggleItems;        //[0] is classic


    // Use this for initialization
    private void Start () {
        if (toggleItems != null)
        {
            for (int i = 1; i < toggleItems.Length; i++)
            {
                if (toggleItems[i] != null)
                {
#if UNITY_2018_1_OR_NEWER   //JobSystem available
                    toggleItems[i].interactable = true;
#else
                    toggleItems[i].interactable = false;
#endif
                }
            }
        }
    }
        
    // Update is called once per frame
    //private void Update () {
            
    //}


    int oldIndex = 0;   //For change check

    //Register 'Toggle.OnValueChanged'
    public void ChangeToggle(Toggle toggle)
    {
        if (!toggle.isOn)
            return;

        for (int i = 0; i < toggleItems.Length; i++)
        {
            if (toggleItems[i] == toggle)
            {
                if (oldIndex != i)  //changed
                {
                    sampleItems[i].SetActive(true);
                    oldIndex = i;
                }
            }
            else
            {
                sampleItems[i].SetActive(false);
            }
        }
    }
}

