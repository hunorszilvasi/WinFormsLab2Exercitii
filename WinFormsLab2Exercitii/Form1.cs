using System.Threading;
using System.Collections.Concurrent;

namespace WinFormsLab2Exercitii
{
    public partial class Form1 : Form
    {
        private int befejezettSzalak = 0;
        private readonly ConcurrentDictionary<int, CancellationTokenSource> leallitasok = new();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            int darab = (int)numericUpDown1.Value;
            befejezettSzalak = 0;
            button1.Enabled = false;

            for (int i = 0; i < darab; i++)
            {
                var leallitas = new CancellationTokenSource();

                Thread szal = new Thread(() =>
                {
                    int id = Thread.CurrentThread.ManagedThreadId;
                    leallitasok[id] = leallitas;

                    Kiir($"Thread {id} has started.");

                    bool megszakitva = leallitas.Token.WaitHandle.WaitOne(
                        TimeSpan.FromSeconds(10));

                    if (megszakitva)
                        Kiir($"Thread {id} was cancelled.");
                    else
                        Kiir($"Thread {id} has finished Running.");

                    leallitasok.TryRemove(id, out _);
                    leallitas.Dispose();

                    if (Interlocked.Increment(ref befejezettSzalak) == darab)
                    {
                        BeginInvoke(new Action(() => button1.Enabled = true));
                    }
                });

                szal.IsBackground = true;
                szal.Start();
            }
        }

        private void Kiir(string uzenet)
        {
            if (listBox1.InvokeRequired)
            {
                listBox1.BeginInvoke(new Action(() => listBox1.Items.Add(uzenet)));
            }
            else
            {
                listBox1.Items.Add(uzenet);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(textBox1.Text, out int id))
            {
                Kiir("Adj meg egy érvényes szálazonosítót.");
                return;
            }

            if (leallitasok.TryGetValue(id, out CancellationTokenSource leallitas))
            {
                try
                {
                    leallitas.Cancel();
                }
                catch (ObjectDisposedException)
                {
                    Kiir($"Thread {id} has already finished.");
                }
            }
            else
            {
                Kiir($"Thread {id} was not found or has already finished.");
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
