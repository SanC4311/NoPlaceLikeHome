using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace TinyGiantStudio.Text
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Tiny Giant Studio/Modular 3D Text/Horizontal Selector", order: 20006)]
    [HelpURL("https://ferdowsur.gitbook.io/modular-3d-text/ui/horizontal-selector")]
    public class HorizontalSelector : MonoBehaviour
    {
        /// <summary>
        /// The 3D text used to show the current value
        /// </summary>
        [Tooltip("The 3D text used to show the current value")]
        public Modular3DText text;

        /// <summary>
        /// If keyboard control is enabled, selected = you can control via selected
        /// <para>This value will be controlled by list, if it is in one</para>
        /// <para>If you are looking for the selected option, that's the int 'Value'.</para>
        /// </summary>
        [Tooltip("If keyboard control is enabled, selected = you can control via selected. \nThis value will be controlled by list, if it is in one")]
        public bool focused = false;

        /// <summary>
        /// Can this be interacted with. If disabled, can't be selected in list
        /// </summary>
        [Tooltip("If keyboard control is enabled, selected = you can control via selected\nOr selected/deselected in a List")]
        public bool interactable = true;

        /// <summary>
        /// Available options for horizontal selector
        /// </summary>
        [Tooltip("Available options for horizontal selector. \nVariable name: options")]
        public List<string> options = new List<string>
        (
            new string[] { "Option 1", "Option 2", "Option 3" }
        );

        [SerializeField]
        [FormerlySerializedAs("value")]
        private int _value;
        /// <summary>
        /// Currently selected option
        /// </summary>
        public int Value
        {
            get { return _value; }
            set
            {
                this._value = value;
                UpdateText();
                onValueChangedEvent.Invoke();
            }
        }



        public AudioClip valueChangeSoundEffect;
        public AudioSource audioSource;

        public UnityEvent onSelectEvent;
        public UnityEvent onValueChangedEvent;
        public UnityEvent onValueIncreasedEvent;
        public UnityEvent onValueDecreasedEvent;





        /// <summary>
        /// Increases the selected number. 
        /// <para>If the number is greater/equal(>=) than the options count, sets it to 0</para>
        /// </summary>
        public void Increase()
        {
            _value++;
            if (_value >= options.Count)
                _value = 0;

            UpdateText();
            onValueChangedEvent.Invoke();
            onValueIncreasedEvent.Invoke();

            if (audioSource && valueChangeSoundEffect)
                audioSource.PlayOneShot(valueChangeSoundEffect);
        }

        /// <summary>
        /// Decreases the selected number. 
        /// <para>If the number is less than zero, sets it to max</para>
        /// </summary>
        public void Decrease()
        {
            _value--;
            if (_value < 0)
                _value = options.Count - 1;

            UpdateText();
            onValueChangedEvent.Invoke();
            onValueDecreasedEvent.Invoke();

            if (audioSource && valueChangeSoundEffect)
                audioSource.PlayOneShot(valueChangeSoundEffect);
        }




        /// <summary>
        /// Updates current text to match the currently selected value
        /// </summary>
        public void UpdateText()
        {
            if (options.Count == 0 || _value < 0 || options.Count <= _value)
                return;

            if (text)
                text.UpdateText(options[_value]);
            else
            {
                Debug.LogError("No text is attached to Horizontal selector: " + gameObject.name, gameObject);
            }
        }






        /// <summary>
        /// Selects/Deselects the component
        /// </summary>
        /// <param name="enable">true = selected, false = deselected</param>
        public void Focus(bool enable)
        {
            focused = enable;
            if (focused && interactable)
            {
                onSelectEvent.Invoke();
            }
            else this.enabled = false;
        }
    }
}