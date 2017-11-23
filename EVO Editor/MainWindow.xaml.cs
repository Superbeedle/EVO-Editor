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
		public MainWindow() {
			InitializeComponent();
			filePath = Properties.Settings.Default.filePath;
			if (filePath != "") {
				editor.AppendText(File.ReadAllText(filePath));
				Title = "Evo - " + filePath;//.Substring(filePath.LastIndexOf('\\'));
				//Syntax_Coloring(FileExtension.HTML);
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
				Enum.TryParse<FileExtension>(fileType, out FileExtension fileExtension);
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

		private void Syntax_Coloring(FileExtension fileExtension) {
			if (fileExtension == FileExtension.HTML) {
				// cyan
				SolidColorBrush brush = new SolidColorBrush(Colors.Green);//(Color)ColorConverter.ConvertFromString("#00dbd7ff"));
																		  //editor.ColorTags('<', '>', brush);
				editor.ColorKeyword("red", new SolidColorBrush(Colors.Red));
			}
		}

		private void Editor_KeyDown(object sender, KeyEventArgs e) {
			if (e.Key == Key.Enter) {
				Syntax_Coloring(FileExtension.HTML);
			}
		}
	}
}
