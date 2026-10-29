using UnityEngine;
using System.Collections.Generic;


namespace Demos
{
    /// <summary>
    /// Data container for pages of text.
    /// </summary>
	[CreateAssetMenu(fileName = "DemoScreen_Data", menuName = "Demos/TextScreen Data", order = 5)]
	public class TextScreenSO : DescriptionSO
	{
        [SerializeField] private string m_Title;
        [TextArea(8, 25)]
        [SerializeField] private List<string> m_BodyText;


        public string Title => m_Title;
        public List<string> BodyText => m_BodyText;

    }
}