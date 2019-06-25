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
using System.Net.Mail;

namespace Pop3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Pop3MailClient client;
        private static MainWindow mainWindow;

        public MainWindow()
        {
            InitializeComponent();
            mainWindow = this;
        }

        private void LetterDoubleClick(object sender, MouseButtonEventArgs e)
        {
            LetterClass letter = (LetterClass)(sender as ListView).SelectedItem;
            if (letter == null)
                return;
            LetterTextWindow letterText = new LetterTextWindow(letter.Text);
            letterText.Show();
        }

        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            if (client == null)
                return;
            DeleteWindow deleteWindow = new DeleteWindow();
            deleteWindow.Show();
        }

        private void GetClick(object sender, RoutedEventArgs e)
        {
            if (client == null)
                return;
            List<LetterClass> letters = new List<LetterClass>();
            List<int> EmailIds;
            client.GetEmailIdList(out EmailIds);
            string to, from, subject, date, size, text, email;
            foreach (var item in EmailIds)
            {
                client.GetRawEmail(item, out email);
                subject = LetterClass.GetSubjectFromText(email);
                to = LetterClass.GetReceiverFromText(email);
                from = LetterClass.GetSenderFromText(email);
                date = LetterClass.GetDateFromText(email);
                size = client.GetEmailSize(item).ToString();
                text = email;
                letters.Add(new LetterClass(subject, from, to, size, date, text, item));
            }
            list_letters.ItemsSource = letters;
        }

        private void DisconnectClick(object sender, RoutedEventArgs e)
        {
            if (client != null)
            {
                client.Disconnect();
                sb.Content = "Отключено от " + tb_address.Text + ".";
                client = null;
                List<LetterClass> list = new List<LetterClass>();
                list_letters.ItemsSource = list;
            }
        }

        private void ConnectClick(object sender, RoutedEventArgs e)
        {
            try
            {
                client = new Pop3.Pop3MailClient(tb_address.Text, Int32.Parse(tb_port.Text), (bool)cb_ssl.IsChecked, tb_username.Text, tb_password.Password);
                client.IsAutoReconnect = true;
                client.Connect();
                
                int NumberOfMails, MailboxSize;
                client.GetMailboxStats(out NumberOfMails, out MailboxSize);
                sb.Content = "Подключено к " + tb_address.Text + ". Кол-во писем: " + NumberOfMails;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }

        public static void DeleteLetters(string to)
        {
            if (mainWindow.client == null)
                return;
            foreach (LetterClass item in mainWindow.list_letters.ItemsSource)
            {
                if(item.To!=to)
                {
                    mainWindow.client.DeleteEmail(item.Id);
                }
            }
        }

    }
}
