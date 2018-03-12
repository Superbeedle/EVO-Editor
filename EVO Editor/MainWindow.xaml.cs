using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using Extensions;

namespace EVO_Editor {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {

		public string filePath;
		
		public enum FileExtension {
			TXT,
			HTML
		}
		public FileExtension fileExtension;

		public MainWindow() {
			InitializeComponent();
			filePath = Properties.Settings.Default.filePath;
			if (filePath != "") {
				editor.AppendText(File.ReadAllText(filePath));
				Title = "EVO - " + filePath;
				string fileType = filePath.Substring(filePath.LastIndexOf('.') + 1).ToUpper();
				Enum.TryParse<FileExtension>(fileType, out fileExtension);
			}
			
		}

		private void Window_Closing(object sender, EventArgs e) {
			Properties.Settings.Default.filePath = filePath;
			Properties.Settings.Default.Save();
		}

		#region Menu Functions
		#region File Menu
		private void Save_Click(object sender, RoutedEventArgs e) {
			if (filePath != null) {
				File.WriteAllText(filePath, editor.GetText());
				if (Title.Substring(Title.Length - 1) == "*") {
					Title = Title.Substring(0, Title.Length - 1);
				}
			}
			else {
				// Save_As_Click does not use the RoutedEventArgs, so sending null is fine
				Save_As_Click(sender, null);
			}
		}

		private void Save_As_Click(object sender, RoutedEventArgs e) {
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "All Types (*.*)|*.*|Text File (*.txt)|*.txt|Hyper Text Markup Language (*.html;*.htm;*.xhtml;*.xhtm)|*.html;*.htm;*.xhtml;*.xhtm";
			if (saveFileDialog.ShowDialog() == true) {
				File.WriteAllText(saveFileDialog.FileName, editor.GetText());
				string safeFileName = saveFileDialog.SafeFileName;
				Title = "EVO - " + safeFileName;
				filePath = saveFileDialog.FileName;
				string fileType = safeFileName.Substring(safeFileName.LastIndexOf('.') + 1);
				fileType = fileType.ToUpper();
				Enum.TryParse<FileExtension>(fileType, out fileExtension);
				//Syntax_Coloring(fileExtension);
				Properties.Settings.Default.filePath = filePath;
			}
		}

		private void Open_Click(object sender, RoutedEventArgs e) {
			OpenFileDialog openFileDialog = new OpenFileDialog();
			if (openFileDialog.ShowDialog() == true) {
				editor.Document.Blocks.Clear();
				editor.AppendText(File.ReadAllText(openFileDialog.FileName));
				Title = "Evo - " + openFileDialog.SafeFileName;
				filePath = openFileDialog.FileName;
			}
		}
		#endregion
		#endregion

		/// <summary>
		/// Syntax colring and adds an asterisk to unsaved work.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Editor_TextChanged(object sender, TextChangedEventArgs e) {
			if (Title.Substring(Title.Length - 1) != "*") {
				Title += "*";
			}		
		}

		public static IEnumerable<TextRange> GetAllWordRanges(FlowDocument document, string regexPattern) {
			string pattern = @regexPattern;
			TextPointer pointer = document.ContentStart;
			while (pointer != null) {
				if (pointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text) {
					string textRun = pointer.GetTextInRun(LogicalDirection.Forward);
					MatchCollection matches = Regex.Matches(textRun, pattern);
					foreach (Match match in matches) {
						int startIndex = match.Index;
						int length = match.Length;
						TextPointer start = pointer.GetPositionAtOffset(startIndex);
						TextPointer end = start.GetPositionAtOffset(length);
						yield return new TextRange(start, end);
					}
				}

				pointer = pointer.GetNextContextPosition(LogicalDirection.Forward);
			}
		}

		private void Editor_PreviewKeyDown(object sender, KeyEventArgs e) {
			TextRange allText = new TextRange(editor.Document.ContentStart, editor.Document.ContentEnd);
			allText.ClearAllProperties();

			IEnumerable<TextRange> tagRanges = GetAllWordRanges(editor.Document, "<(.*?)>");
			bool decolorNextRange = false;
			if (fileExtension == FileExtension.HTML) {
				foreach (TextRange wordRange in tagRanges) {
					if (wordRange.Text.Contains("<") && wordRange.Text.Contains(">")) {
						wordRange.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Red);
						decolorNextRange = true;
					}	else if (decolorNextRange == true) {
						wordRange.ClearAllProperties();
						decolorNextRange = false;
					}
				}
				IEnumerable<TextRange> idRanges = GetAllWordRanges(editor.Document, "\\sid\\=");
				decolorNextRange = false;
				foreach (TextRange wordRange in idRanges) {
					if (wordRange.Text.Contains("id")) {
						wordRange.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Yellow);
						wordRange.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
						decolorNextRange = true;
					}	else if (decolorNextRange == true) {
						wordRange.ClearAllProperties();
						wordRange.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Red);
						decolorNextRange = false;
					}
				}
				IEnumerable<TextRange> classRanges = GetAllWordRanges(editor.Document, "\\sclass\\=");
				decolorNextRange = false;
				foreach (TextRange wordRange in classRanges) {
					if (wordRange.Text.Contains("class")) {
						wordRange.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Lime);
						wordRange.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
						decolorNextRange = true;
					} else if (decolorNextRange == true) {
						wordRange.ClearAllProperties();
						wordRange.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Red);
						decolorNextRange = false;
					}
				}
				IEnumerable<TextRange> quotesRanges = GetAllWordRanges(editor.Document, "\"(.*?)\"");
				decolorNextRange = false;
				foreach (TextRange wordRange in quotesRanges) {
					if (wordRange.Text.Contains("\"")) {
						wordRange.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.LightSkyBlue);
						wordRange.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Italic);
						decolorNextRange = true;
					} else if (decolorNextRange == true) {
						wordRange.ClearAllProperties();
						wordRange.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Red);
						decolorNextRange = false;
					}
				}
			}
			
			if (e.Key == Key.F5) {
				System.Diagnostics.Process.Start(@filePath);
			}
		}

		#region Resize Text
		// Resize text by scrolling Mouse Wheel. Reset by pressing ctrl+MMB
		private void Editor_PreviewMouseWheel(object sender, MouseWheelEventArgs e) {
			if (Keyboard.IsKeyDown(Key.LeftCtrl)) {
				if (e.Delta > 0) {
					editor.FontSize++;
				} else if (e.Delta < 0) {
					editor.FontSize--;
				}
			}
		}

		private void Editor_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
			if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.MiddleButton == MouseButtonState.Pressed) {
				editor.FontSize = 12;
			}
		}
		#endregion
	}
}
