using System;
using System.Drawing;
using System.IO;
using System.Timers;
using System.Windows.Forms;

public class VoxelViewer3D : Form
{
    private byte[,,] voxelData=new byte[1,1,1];
    private int sizeX, sizeY, sizeZ,sizexx;
    private readonly Color[] vgaColors;
    private double rotation = 0;
    private System.Timers.Timer timer;

    public VoxelViewer3D(string filePath)
    {
        
        this.Text = ".3D Voxel Viewer";
        this.DoubleBuffered = true;
        this.ClientSize = new Size(800, 600);
        this.BackColor = Color.Yellow;
        //this.Show();
        vgaColors = new Color[]
        {
            Color.Black, Color.Blue, Color.Green, Color.Cyan,
            Color.Red, Color.Magenta, Color.Brown, Color.LightGray,
            Color.DarkGray, Color.LightBlue, Color.LightGreen, Color.LightCyan,
            Color.LightCoral, Color.LightPink, Color.Yellow, Color.White
        };

        LoadVoxelData(filePath);

        timer = new System.Timers.Timer(600);
        timer.Elapsed += (s, e) => { rotation += Math.PI / 8; this.Invalidate(); };
        timer.Start();

    }

    private void LoadVoxelData(string filePath)
    {
        byte[] raw = File.ReadAllBytes(filePath);
        if (raw[0] != '3' || raw[1] != 'D')
            throw new InvalidDataException("Formato inválido");

        sizeX = raw[2];
        sizexx = raw[3];
        sizeY = raw[4];
        sizeZ = raw[5];

        voxelData = new byte[sizeZ, sizeY, sizeX];

        int index = 6;
        for (int z = 0; z < sizeZ; z++)
            for (int y = 0; y < sizeY; y++)
                for (int x = 0; x < sizeX; x++)
                    voxelData[z, y, x] = raw[index++];
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        Graphics g = e.Graphics;
        g.Clear(Color.Yellow);

        float scale = 10f;
        float ox = this.ClientSize.Width / 2;
        float oy = this.ClientSize.Height / 2;

        for (int z = 0; z < sizeZ; z++)
        {
            float depthScale = 1.0f + (z / (float)sizeZ);
            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    byte colorIndex = voxelData[z, y, x];
                    if (colorIndex == 0) continue;
                    Color color = vgaColors[colorIndex % 16];

                    var pos = Rotate3D(x - sizeX / 2f, y - sizeY / 2f, z - sizeZ / 2f);
                    float bx = ox + (float)(pos[0] * scale * depthScale);
                    float by = oy + (float)(pos[1] * scale * depthScale);
                    float size = scale * depthScale;
                    using (Brush brush = new SolidBrush(color))
                    {
                        g.FillRectangle(brush, bx, by, size, size);
                    }
                }
            }
        }

    }

    private double[] Rotate3D(double x, double y, double z)
    {
        double cos = Math.Cos(rotation);
        double sin = Math.Sin(rotation);
        double x2 = x * cos - z * sin;
        double z2 = x * sin + z * cos;
        return new double[] { x2, y, z2 };
    }

    [STAThread]
    public static void Main()
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "3D files (*.3d)|*.3d";

        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            
            Application.Run(new VoxelViewer3D(openFileDialog.FileName));
            //MessageBox.Show(openFileDialog.FileName);
        }
        else
        {
            MessageBox.Show("Nenhum ficheiro selecionado.");
        }
        
    }
}
