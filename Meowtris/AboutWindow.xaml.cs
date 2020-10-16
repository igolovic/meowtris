using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Meowtris
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
            this.PreviewKeyDown += new KeyEventHandler(Key_PreviewKeyDown);
        }

        #region Event handlers

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                string pathMeowtris = System.Reflection.Assembly.GetExecutingAssembly().Location;
                lblVersionMeowtris.Content = "Meowtris " + FileVersionInfo.GetVersionInfo(pathMeowtris).ProductVersion;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void LnkRepository_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
                e.Handled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void LnkDocumentation_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder codeBaseUri = new UriBuilder(codeBase);
                string codeBasePath = Uri.UnescapeDataString(codeBaseUri.Path);
                string documentationFilePath = Path.Combine(Path.GetDirectoryName(codeBasePath), "SessionTimeDocumentation.txt");

                if (File.Exists(documentationFilePath))
                {
                    Process.Start(new ProcessStartInfo(documentationFilePath));
                    e.Handled = true;
                }
                else
                    MessageBox.Show("Cannot find SessionTimeDocumentation.txt in application folder.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Key_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        #endregion
    }
}