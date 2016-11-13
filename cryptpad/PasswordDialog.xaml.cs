using System;
using System.Security;
using System.Text;
using System.Windows;

namespace cryptpad
{
    /// <summary>
    /// Interaction logic for PasswordDialog.xaml
    /// </summary>
    public partial class PasswordDialog : Window
    {
        public SecureString Password
        {
            get { return passwordBox.SecurePassword; }
        }

        private bool needsVerification;

        public bool NeedsVerifcation
        {
            set
            {
                if(needsVerification = value)
                {
                    passwordConfirmationBox.Visibility = Visibility.Visible;
                    Height = 235;
                }
                else
                {
                    passwordConfirmationBox.Visibility = Visibility.Collapsed;
                    Height = 205;
                }
            }
        }

        public EncryptionConfiguration EncryptionConfig;

        public Encoding TextEncoding;

        public PasswordDialog()
        {
            InitializeComponent();

            NeedsVerifcation = false;

            Loaded += new RoutedEventHandler((sender, args) => passwordBox.Focus());

            EncryptionConfig = new EncryptionConfiguration();

            foreach (EncryptionConfiguration.Algorithm algo in EncryptionConfiguration.Algorithms)
            {
                encAlgoComboBox.Items.Add(algo);
            }

            foreach(EncodingInfo encodingInfo in Encoding.GetEncodings())
            {
                textEncodingComboBox.Items.Add(encodingInfo.GetEncoding());
            }

            encAlgoComboBox.SelectedItem = EncryptionConfig.Algo;
            keySizeComboBox.SelectedItem = EncryptionConfig.KeySize;
            keyGenAlgoComboBox.SelectedItem = EncryptionConfig.KeyAlgo;
            textEncodingComboBox.SelectedItem = Encoding.UTF8;
        }

        public void EncryptionAlgorithmSelected(object sender, EventArgs args)
        {
            EncryptionConfig.Algo = (EncryptionConfiguration.Algorithm)encAlgoComboBox.SelectedItem;

            keySizeComboBox.Items.Clear();
            foreach (int keySize in EncryptionConfig.Algo.LegalKeySizes)
            {
                keySizeComboBox.Items.Add(keySize);
            }

            keySizeComboBox.SelectedItem = EncryptionConfig.Algo.DefaultKeySize;
        }

        public void KeySizeSelected(object sender, EventArgs args)
        {
            keyGenAlgoComboBox.Items.Clear();

            if (keySizeComboBox.SelectedItem != null)
            {
                EncryptionConfig.KeySize = (int)keySizeComboBox.SelectedItem;

                foreach (EncryptionConfiguration.KeyAlgorithm algo in EncryptionConfiguration.KeyAlgorithms)
                {
                    if (algo.CanGenerateKey(EncryptionConfig.KeySize))
                    {
                        keyGenAlgoComboBox.Items.Add(algo);
                    }
                }

                if(keyGenAlgoComboBox.Items.Contains(EncryptionConfig.KeyAlgo))
                {
                    keyGenAlgoComboBox.SelectedItem = EncryptionConfig.KeyAlgo;
                }
                else if(keyGenAlgoComboBox.Items.Contains(EncryptionConfiguration.DefaultKeyAlgorithm))
                {
                    keyGenAlgoComboBox.SelectedItem = EncryptionConfiguration.DefaultKeyAlgorithm;
                }
                else
                {
                    keyGenAlgoComboBox.SelectedIndex = 0;
                }
            }
        }

        public void KeyGenAlgorithmSelected(object sender, EventArgs args)
        {
            EncryptionConfig.KeyAlgo = (EncryptionConfiguration.KeyAlgorithm)keyGenAlgoComboBox.SelectedItem;
        }

        public void TextEncodingSelected(object sender, EventArgs args)
        {
            Encoding enc = (Encoding)textEncodingComboBox.SelectedItem;
            if(enc != null)
            {
                TextEncoding = enc;
            }
        }

        public void CancelClicked(object sender, EventArgs args)
        {
            DialogResult = false;
        }

        public void OKClicked(object sender, EventArgs args)
        {
            if(needsVerification)
            {
                using (SecureString pw1 = passwordBox.SecurePassword)
                {
                    using (SecureString pw2 = passwordConfirmationBox.SecurePassword)
                    {
                        using (SecureStringBytes pw1b = new SecureStringBytes(pw1, Encoding.UTF8))
                        {
                            using (SecureStringBytes pw2b = new SecureStringBytes(pw2, Encoding.UTF8))
                            {
                                if (!pw1b.Bytes.DeepEquals(pw2b.Bytes))
                                {
                                    MessageBox.Show(this,
                                        Properties.Resources.DialogPasswordsDontMatchText,
                                        Properties.Resources.DialogPasswordsDontMatchTitle,
                                        MessageBoxButton.OK, MessageBoxImage.Error);
                                    return;
                                }
                            }
                        }
                    }
                }
            }

            DialogResult = true;
        }
    }
}
