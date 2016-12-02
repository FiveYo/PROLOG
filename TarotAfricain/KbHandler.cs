using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TarotAfricain
{
    public class KbHandler
    { 
    private Keys[] lastPressedKeys;
    public string text = "";

    public KbHandler()
    {
        lastPressedKeys = new Keys[0];
    }

    public void Update()
    {
        KeyboardState kbState = Keyboard.GetState();
        Keys[] pressedKeys = kbState.GetPressedKeys();

        //check if any of the previous update's keys are no longer pressed
        foreach (Keys key in lastPressedKeys)
        {
            if (!pressedKeys.Contains(key))
                OnKeyUp(key);
        }

        //check if the currently pressed keys were already pressed
        foreach (Keys key in pressedKeys)
        {
            if (!lastPressedKeys.Contains(key))
                OnKeyDown(key);
        }

        //save the currently pressed keys so we can compare on the next update
        lastPressedKeys = pressedKeys;
    }

    //Create your own   
    private void OnKeyDown(Keys key)
    {

    }

    private void OnKeyUp(Keys key)
    {
            switch (key)
            {
                case Keys.D0:
                    text += "0";
                    break;
                case Keys.D1:
                    text += "1";
                    break;
                case Keys.D2:
                    text += "2";
                    break;
                case Keys.D3:
                    text += "3";
                    break;
                case Keys.D4:
                    text += "4";
                    break;
                case Keys.D5:
                    text += "5";
                    break;
                case Keys.D6:
                    text += "6";
                    break;
                case Keys.D7:
                    text += "7";
                    break;
                case Keys.D8:
                    text += "8";
                    break;
                case Keys.D9:
                    text += "9";
                    break;
                case Keys.NumPad0:
                    text += "0";
                    break;
                case Keys.NumPad1:
                    text += "1";
                    break;
                case Keys.NumPad2:
                    text += "2";
                    break;
                case Keys.NumPad3:
                    text += "3";
                    break;
                case Keys.NumPad4:
                    text += "4";
                    break;
                case Keys.NumPad5:
                    text += "5";
                    break;
                case Keys.NumPad6:
                    text += "6";
                    break;
                case Keys.NumPad7:
                    text += "7";
                    break;
                case Keys.NumPad8:
                    text += "8";
                    break;
                case Keys.NumPad9:
                    text += "9";
                    break;
                case Keys.OemPeriod:
                    text += ".";
                    break;
                case Keys.Enter:
                    break;
                case Keys.Space:
                    text += " ";
                    break;
                case Keys.Back:
                    {
                        if (text.Length > 0)
                        {
                            text = text.Remove(text.Length - 1, 1);
                        }
                        break;
                    }
                default:
                    {
                        text += key.ToString();
                        break;
                    }
            }
        }
}
}