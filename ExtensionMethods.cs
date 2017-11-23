using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Evo_Editor {
	//Extension methods must be defined in a static class
	public static class StringExtension {
		// This is the extension method.
		// The first parameter takes the "this" modifier
		// and specifies the type for which the method is defined.
		private void CheckKeyword(this RichTextBox editor, string word, Color color, int startIndex)
    {
        if (editor.Text.Contains(word))
        {
            int index = -1;
            int selectStart = editor.SelectionStart;

            while ((index = editor.Text.IndexOf(word, (index + 1))) != -1)
            {
                editor.Select((index + startIndex), word.Length);
                editor.SelectionColor = color;
                editor.Select(selectStart, 0);
                editor.SelectionColor = Color.Black;
            }
        }
    }

	}
}