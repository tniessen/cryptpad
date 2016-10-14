using Microsoft.Win32;
using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace cryptpad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string filePath;
        private bool fileChanged;

        /// <summary>
        /// Path to the currently opened file
        /// </summary>
        public string FilePath
        {
            get { return filePath; }

            set
            {
                filePath = value;

                if(value == null)
                {
                    Title = "Unnamed";
                }
                else
                {
                    Title = value;
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            Loaded += new RoutedEventHandler((sender, args) => textEditor.Focus());

            textEditor.TextChanged += new TextChangedEventHandler((sender, args) => fileChanged = true);

            // If a file was "opened with" cryptpad, try to load it
            App app = (App)Application.Current;
            if(app.StartupFilePath != null)
            {
                if(!Load(app.StartupFilePath))
                {
                    app.Shutdown();
                }
            }
            else
            {
                NewDocument();
            }
        }

        /// <summary>
        /// Creates a new document, clearing the file path and the undo history.
        /// </summary>
        private void NewDocument()
        {
            FilePath = null;
            NewHistory(() => textEditor.Clear());
        }

        /// <summary>
        /// Suggests saving the current file if it was changed since the last save.
        /// </summary>
        /// <returns>false if the operation is to be cancelled, true otherwise</returns>
        private bool SuggestSave()
        {
            if(fileChanged)
            {
                MessageBoxResult res = MessageBox.Show(this,
                    "The file contains unsaved changes. Do you want to save it?",
                    "Unsaved changes", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

                switch (res)
                {
                    case MessageBoxResult.Yes:
                        return Save();
                    case MessageBoxResult.No:
                        return true;
                    default:
                        return false;
                }
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Handler for File → New
        /// </summary>
        public void NewCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (!SuggestSave()) return;

            NewDocument();
        }

        /// <summary>
        /// Handler for File → Open
        /// </summary>
        public void OpenCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (!SuggestSave()) return;

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = "stxt";
            dialog.Filter = "Secure text file (*.stxt)|*.stxt|All files (*.*)|*.*";
            dialog.FilterIndex = 1;

            if (FilePath != null)
            {
                dialog.FileName = FilePath;
            }

            if (dialog.ShowDialog() == true)
            {
                Load(dialog.FileName);
            }
        }

        /// <summary>
        /// Specifies whether the "File → Save" command can be executed.
        /// </summary>
        public void CanExecuteSaveCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = fileChanged;
        }

        /// <summary>
        /// Handler for File → Save
        /// </summary>
        public void SaveCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Save();
        }

        private bool Save()
        {
            if (FilePath == null)
            {
                return SaveAs();
            }
            else
            {
                return Save(FilePath);
            }
        }

        private bool SaveAs()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = "stxt";
            dialog.Filter = "Secure text file (*.stxt)|*.stxt|All files (*.*)|*.*";
            dialog.FilterIndex = 1;

            if (FilePath != null)
            {
                dialog.FileName = FilePath;
            }

            if (dialog.ShowDialog() == true)
            {
                return Save(dialog.FileName);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Handler for File → Save as
        /// </summary>
        public void SaveAsCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SaveAs();
        }

        /// <summary>
        /// Loads a file from the given path.
        /// </summary>
        private bool Load(string path)
        {
            PasswordDialog dialog = new PasswordDialog();

            // If we are opening a file on startup, the main window has not been visible yet, and
            // thus is not a valid owner at this point.
            if(IsVisible)
            {
                dialog.Owner = this;
            }

            if (dialog.ShowDialog() == true)
            {
                SecureString password = dialog.Password;
                byte[] key = new byte[dialog.EncryptionConfig.KeySize / 8];
                using (SecureStringBytes pwBytes = new SecureStringBytes(password, Encoding.UTF8))
                {
                    password.Dispose();
                    dialog.EncryptionConfig.KeyAlgo.GenerateKey(pwBytes.Bytes, key);
                }

                string text;

                try
                {
                    SymmetricAlgorithm algo = dialog.EncryptionConfig.Algo.GetImplementation();
                    algo.KeySize = dialog.EncryptionConfig.KeySize;
                    algo.Key = key;

                    using (FileStream inStream = new FileStream(path, FileMode.Open))
                    {
                        byte[] iv = new byte[algo.IV.Length];
                        for (int i = 0; i < algo.IV.Length; i++)
                        {
                            iv[i] = (byte)inStream.ReadByte();
                        }
                        algo.IV = iv;

                        ICryptoTransform encryptor = algo.CreateDecryptor();
                        using (CryptoStream cryptoStream = new CryptoStream(inStream, encryptor, CryptoStreamMode.Read))
                        {
                            StreamReader reader = new StreamReader(cryptoStream, dialog.TextEncoding);
                            text = reader.ReadToEnd();
                        }
                    }

                    NewHistory(() => textEditor.Text = text);
                    FilePath = path;

                    return true;
                }
                catch (CryptographicException e)
                {
                    string message = "A cryptographic exception occurred while reading the file:\n";
                    message += e.Message;
                    message += "\n\nPlease ensure that";
                    message += "\n - the password is correct,";
                    message += "\n - you selected the correct encryption algorithm,";
                    message += "\n - you selected the correct key size and";
                    message += "\n - you selected the correct key generation algorithm.";
                    MessageBox.Show(this, message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception e)
                {
                    string message = "An unexpected exception occurred while reading the file:\n";
                    message += e.Message;
                    MessageBox.Show(this, message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    key.Fill<byte>(0, 0, key.Length);
                }
            }

            return false;
        }

        /// <summary>
        /// Saves the file to the given path.
        /// </summary>
        private bool Save(string path)
        {
            PasswordDialog dialog = new PasswordDialog();
            dialog.Owner = this;

            if (dialog.ShowDialog() == true)
            {
                string text = textEditor.Text;
                byte[] textBytes = dialog.TextEncoding.GetBytes(text);

                SecureString password = dialog.Password;
                byte[] key = new byte[dialog.EncryptionConfig.KeySize / 8];
                using (SecureStringBytes pwBytes = new SecureStringBytes(password, Encoding.UTF8))
                {
                    password.Dispose();
                    dialog.EncryptionConfig.KeyAlgo.GenerateKey(pwBytes.Bytes, key);
                }

                try
                {
                    SymmetricAlgorithm algo = dialog.EncryptionConfig.Algo.GetImplementation();
                    algo.KeySize = dialog.EncryptionConfig.KeySize;
                    algo.Key = key;

                    ICryptoTransform encryptor = algo.CreateEncryptor();

                    using (FileStream outStream = new FileStream(path, FileMode.Create))
                    {
                        outStream.Write(algo.IV, 0, algo.IV.Length);
                        using (CryptoStream cryptoStream = new CryptoStream(outStream, encryptor, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(textBytes, 0, textBytes.Length);
                        }
                    }

                    FilePath = path;
                    fileChanged = false;
                    return true;
                }
                catch (Exception e)
                {
                    string message = "An unexpected exception occurred while reading the file:\n";
                    message += e.Message;
                    MessageBox.Show(this, message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    key.Fill<byte>(0, 0, key.Length);
                }
            }

            return false;
        }

        /// <summary>
        /// Handler for File → Exit
        /// </summary>
        public void ExitCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (!SuggestSave()) return;

            Application.Current.Shutdown();
        }

        /// <summary>
        /// Ensures that the given action will be the starting point of the undo history of the
        /// editor.
        /// </summary>
        private void NewHistory(Action action)
        {
            // Clear history by disabling it
            textEditor.IsUndoEnabled = false;

            // Initialize document
            action();
            fileChanged = false;

            // Enable history
            textEditor.IsUndoEnabled = true;
        }
    }
}
