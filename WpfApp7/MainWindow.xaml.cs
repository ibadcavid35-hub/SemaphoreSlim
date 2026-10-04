using System.Runtime.ConstrainedExecution;
using System.Windows;
using System.Windows.Input;

namespace WpfApp7
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SemaphoreSlim semaphore;
        private int threadCount = 0;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            int key = int.Parse(textBox1.Text);
            textBox1.Text = (key + 1).ToString();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            int key = int.Parse(textBox1.Text);
            if (key > 1)
                textBox1.Text = (key - 1).ToString();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            threadCount++;
            listBox3.Items.Add($"Thread {threadCount}");
        }

        private void listBox3_MouseDoubleClick_2(object sender, MouseButtonEventArgs e)
        {
            var item = listBox3.SelectedItem;
            if (item == null) return;

            listBox3.Items.Remove(item);
            listBox2.Items.Add(item);

            if (semaphore == null)
            {
                int limit = int.Parse(textBox1.Text);
                semaphore = new SemaphoreSlim(limit, limit);
            }

            Thread thread = new Thread(() =>
            {
                semaphore.Wait();

                Dispatcher.Invoke(() =>
                {
                    listBox2.Items.Remove(item);
                });

                for (int i = 10; i >= 0; i--)
                {
                    Dispatcher.Invoke(() =>
                    {
                        listBox1.Items.Remove($"{item} -> {i + 1}");
                        listBox1.Items.Add($"{item} -> {i}");
                    });

                    Thread.Sleep(1000);
                }

                Dispatcher.Invoke(() =>
                {
                    listBox1.Items.Remove($"{item} -> 0");
                });

                semaphore.Release();
            });

            thread.IsBackground = true;
            thread.Start();
        }


        private void listBox1_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {

        }
    }
}