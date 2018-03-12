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
		public static string GetText(this RichTextBox box) {
			TextRange tr = new TextRange(box.Document.ContentStart, box.Document.ContentEnd);
			return (tr.Text);
		}
	}
}