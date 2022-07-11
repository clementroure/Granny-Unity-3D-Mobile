using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using FantomLib;

//Change item/value dynamically sample.
public class DynamicTest : MonoBehaviour {

    public SelectDialogController selectControl;
    public SingleChoiceDialogController singleChoiceControl;
    public MultiChoiceDialogController multiChoiceControl;

    public SliderDialogController sliderControl;
    public SwitchDialogController switchControl;
    public CustomDialogController customControl;


    // Use this for initialization
    private void Start () {
        
    }
    
    // Update is called once per frame
    //private void Update () {
        
    //}

    
#pragma warning disable 0219

    public void DynamicSelect()
    {
        string[] texts = { "Dymanic1", "Dymanic2" };

        SelectDialogController.Item[] items = {
            new SelectDialogController.Item("Dymanic1", "value1"),
            new SelectDialogController.Item("Dymanic2", "value2"),
            //new SelectDialogController.Item("Dymanic3", "value3"),
        };

        if (selectControl != null)
        {
            selectControl.Show(texts);
            //selectControl.SetItem(texts);
            //selectControl.SetItem(items);
        }
    }

    public void DynamicSingleChoice()
    {
        string[] texts = { "Dymanic1", "Dymanic2" };
        SingleChoiceDialogController.Item[] items = {
            new SingleChoiceDialogController.Item("Dymanic1", "value1"),
            new SingleChoiceDialogController.Item("Dymanic2", "value2"),
            //new SingleChoiceDialogController.Item("Dymanic3", "value3"),
        };

        if (singleChoiceControl != null)
        {
            singleChoiceControl.Show(texts);
            //singleChoiceControl.SetItem(texts);
            //singleChoiceControl.SetItem(items);
            //singleChoiceControl.CurrentValue = 1;
            XDebug.Log("CurrentValue : " + singleChoiceControl.CurrentValue);
        }
    }

    public void DynamicMultiChoice()
    {
        string[] texts = { "Dymanic1", "Dymanic2" };
        MultiChoiceDialogController.Item[] items = {
            new MultiChoiceDialogController.Item("Dymanic1", "value1"),
            new MultiChoiceDialogController.Item("Dymanic2", "value2"),
            //new MultiChoiceDialogController.Item("Dymanic3", "value3"),
        };

        if (multiChoiceControl != null)
        {
            multiChoiceControl.Show(texts);
            //multiChoiceControl.SetItem(texts);
            //multiChoiceControl.SetItem(items);
            XDebug.Log("CurrentValue : " + multiChoiceControl.CurrentValue.Select(e => e.ToString()).Aggregate((s,a) => s+", "+a));
        }
    }


    public void DynamicSlider()
    {
        string[] texts = { "Dymanic1", "Dymanic2" };
        SliderDialogController.Item[] items = {
            new SliderDialogController.Item("Dymanic1", "key1"),
            new SliderDialogController.Item("Dymanic2", "key2"),
            //new SliderDialogController.Item("Dymanic3", "key3"),
        };

        if (sliderControl != null)
        {
            sliderControl.Show(texts);
            //sliderControl.SetItem(texts);
            //sliderControl.SetItem(texts, 80);
            //sliderControl.SetItem(items);
            XDebug.Log("CurrentValue : " + sliderControl.CurrentValue.Select(e => e.Key+"="+e.Value).Aggregate((s,a) => s+", "+a));
        }
    }

    public void DynamicSwitch()
    {
        string[] texts = { "Dymanic1", "Dymanic2" };
        SwitchDialogController.Item[] items = {
            new SwitchDialogController.Item("Dymanic1", "key1"),
            new SwitchDialogController.Item("Dymanic2", "key2"),
            //new SwitchDialogController.Item("Dymanic3", "key3"),
        };

        if (switchControl != null)
        {
            switchControl.Show(texts);
            //switchControl.SetItem(texts);
            //switchControl.SetItem(texts, true);
            //switchControl.SetItem(items);
            XDebug.Log("CurrentValue : " + switchControl.CurrentValue.Select(e => e.Key+"="+e.Value).Aggregate((s,a) => s+", "+a));
        }
    }

    public void DynamicCustom()
    {
        DivisorItem divisorItem = new DivisorItem(1);
        TextItem textItem = new TextItem("Dynamic Test!", Color.blue);
        SwitchItem switchItem1 = new SwitchItem("Switch item1", "switch1", true);
        SwitchItem switchItem2 = new SwitchItem("Switch item2", "switch2", false);
        SliderItem sliderItem1 = new SliderItem("Slider item1", "slider1", 50, 0, 100, 0, 0, "");
        SliderItem sliderItem2 = new SliderItem("Slider item2", "slider2", 0.5f, 0, 1, 2, 0, "");
        ToggleItem toggleItem = new ToggleItem(
            new String[] { "Toggle1", "Toggle2" },
            "toggles",
            new String[] { "toggle value1", "toggle value2" },
            "value2");
        DialogItem[] items = new DialogItem[] {
            divisorItem, textItem, switchItem1, switchItem2, sliderItem1, sliderItem2, toggleItem
        };

        if (customControl != null)
        {
            customControl.SetItem(items);
            customControl.Show();
            XDebug.Log("CurrentValue : " + customControl.CurrentValue.Select(e => e.Key+"="+e.Value).Aggregate((s,a) => s+", "+a));
        }
    }

}
