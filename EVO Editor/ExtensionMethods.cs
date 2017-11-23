using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace Extensions {
	//Extension methods must be defined in a static class
	public static class StringExtension {
		// This is the extension method.
		// The first parameter takes the "this" modifier
		// and specifies the type for which the method is defined.
		public static void ColorText_OLD(this RichTextBox box, char firstIndex, char lastIndex, SolidColorBrush color) {
			TextRange boxText = new TextRange(box.Document.ContentStart, box.Document.ContentEnd);
			TextPointer beginningPointer = box.CaretPosition.GetPositionAtOffset(boxText.Text.IndexOf('<'));
			TextPointer endingPointer = box.CaretPosition.GetPositionAtOffset(boxText.Text.IndexOf('>') + 1);
			if (beginningPointer != null && endingPointer != null) {
				TextRange tr = new TextRange(beginningPointer, endingPointer);
				tr.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Red);
			}
		}

		static int start = 0;
		public static void ColorKeyword(this RichTextBox box, string keyword, SolidColorBrush color) {
			string tempText = new TextRange(box.Document.ContentStart, box.Document.ContentEnd).Text;
			//verify if keyword is there
			if (!tempText.Contains(keyword)) {
				return;
			}

			// coloring keyword

			try {
				for (int i = 0; i < tempText.Length - 3; i++) {
					if (tempText[i + 1] == '\r') {
						tempText = tempText.Insert(i, "qq");
						i += 2;
					}
				}
			} catch (Exception) {
				throw;
			}
			
			int index1 = tempText.IndexOf(keyword[0]);
			int index2 = tempText.IndexOf(keyword[keyword.Length - 1]) + 1;

			TextPointer caretPos = box.CaretPosition;
			box.CaretPosition = box.Document.ContentStart;
			TextPointer beginningPointer = box.CaretPosition.GetPositionAtOffset(index1);
			TextPointer endingPointer = box.CaretPosition.GetPositionAtOffset(index2);
			box.CaretPosition = caretPos;

			if (beginningPointer == null) { box.AppendText("beginpointer is null"); return; }
			if (endingPointer == null) { box.AppendText("endpointer is null"); return; }

			TextRange tr = new TextRange(beginningPointer, endingPointer);
			tr.ApplyPropertyValue(TextElement.ForegroundProperty, color);
			
			
		}

		public static void ColorTags(this RichTextBox box, char firstIndex, char lastIndex, SolidColorBrush color) {
			TextRange boxText = new TextRange(box.Document.ContentStart, box.Document.ContentEnd);
			string tempText = boxText.Text;
			if (tempText == "" ||
				tempText == "\r\n" ||
				!tempText.Contains(firstIndex) ||
				!tempText.Contains(lastIndex)) {
				return;
			}
			int index1 = -1;
			int index2 = -1;
			bool checkForFirstIndex = true;
			bool repeat = true;
			while (repeat) {
				for (int i = start; i < tempText.Length; i++) {
					if (i == tempText.Length - 1) {
						repeat = false;
						break;
					}
					if (checkForFirstIndex) {
						if (tempText[i] == firstIndex) {
							index1 = i;
							checkForFirstIndex = false;
						}
					} else {
						if (tempText[i] == lastIndex) {
							index2 = i;
							start = i + 1;
							break;
						}
					}
				} // end for
				if (index1 + index2 < 0) { return; }
				//repeat = false;
				ChangeColor(index1, index2, box);
				index1 = -1;
				index2 = -1;
				checkForFirstIndex = true;
			}
			start = 0;
		}

		private static void ChangeColor(int index1, int index2, RichTextBox box) {
			TextPointer start = box.Document.ContentStart;

			//TextPointer beginningPointer = GetTextPointAt(start, index1);
			//TextPointer endingPointer = GetTextPointAt(start, index2);
			TextPointer beginningPointer = box.CaretPosition.GetPositionAtOffset(index1);
			TextPointer endingPointer = box.CaretPosition.GetPositionAtOffset(index2);
			if (beginningPointer != null && endingPointer != null) {
				TextRange tr = new TextRange(beginningPointer, endingPointer);
				tr.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Red);
			}
		}
		public static string GetText(this RichTextBox box) {
			TextRange tr = new TextRange(box.Document.ContentStart, box.Document.ContentEnd);
			return (tr.Text);
		}

		private static TextPointer GetTextPointAt(TextPointer from, int pos) {
			TextPointer ret = from;
			int i = 0;

			while ((i < pos) && (ret != null)) {
				if ((ret.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.Text) || (ret.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.None))
					i++;

				if (ret.GetPositionAtOffset(1, LogicalDirection.Forward) == null)
					return ret;

				ret = ret.GetPositionAtOffset(1, LogicalDirection.Forward);
			}

			return ret;
		}
	}
}