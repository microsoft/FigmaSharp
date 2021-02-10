using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


namespace BasicRendering.Wpf
{
    /// <summary>
    /// Interaction logic for InputWindow.xaml
    /// </summary>
    /// 
    public sealed partial class InputWindow : UserControl
    {
        private string m_TextMessage;
        private TaskCompletionSource<bool> m_TaskCompletionSource;
        public InputWindow(string label)
        {
            Label = label;
            InitializeComponent();
        }

        public async Task<bool> ShowAsync()
        {
            InitFields();
            m_Popup.IsOpen = true;

            m_TaskCompletionSource = new TaskCompletionSource<bool>();
            return await m_TaskCompletionSource.Task;
        }

        public void InitFields()
        {
            m_TextBox.Width = 300;
            m_TextBlock.Text = Label;
        }

        public string Label
        {
            get { return m_TextMessage; }
            set { m_TextMessage = value; }
        }

        public TextBox TextBox
        {
            get { return m_TextBox; }
        }

        private void OkClicked(object sender, RoutedEventArgs e)
        {
            m_TaskCompletionSource.SetResult(true);
            m_Popup.IsOpen = false;
        }

        private void CancelClicked(object sender, RoutedEventArgs e)
        {
            m_TaskCompletionSource.SetResult(false);
            //m_Popup.IsOpen = false;
        }
    }
}
