using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace interp_cubic {
    public partial class Form1 : Form {
        private class BicubicInterp2 {
            private double[,,,] matrices = new double[4, 4, 3, 3];

            private double[,] extract_chan(Color[,] cols, int channel) {
                double[,] v = new double[4,4];
                switch (channel) {
                    case 0:
                        for (int x = 0; x < 4; x++)
                            for (int y = 0; y < 4; y++)
                                v[x, y] = cols[x, y].B;
                        break;
                    case 1:
                        for (int x = 0; x < 4; x++)
                            for (int y = 0; y < 4; y++)
                                v[x, y] = cols[x, y].G;
                        break;
                    case 2:
                        for (int x = 0; x < 4; x++)
                            for (int y = 0; y < 4; y++)
                                v[x, y] = cols[x, y].R;
                        break;
                    default: throw new Exception($"invalid channel requested in BicubicInterp constructor ({channel})");
                }
                return v;
            }

            public BicubicInterp2(Color[,] cols, int channel) {
                double[,] base_matrix = extract_chan(cols, channel);
                //expand matrix
                for (int xm = 0; xm < 3; xm++)
                    for (int ym = 0; ym < 3; ym++) {
                        matrices[0, 0, xm, ym] = base_matrix[Math.Max(xm - 1, 0), Math.Max(ym - 1, 0)];
                        matrices[1, 0, xm, ym] = base_matrix[Math.Max(xm - 1, 0), ym];
                        matrices[0, 1, xm, ym] = base_matrix[xm, Math.Max(ym - 1, 0)];

                        matrices[1, 1, xm, ym] = base_matrix[xm, ym];
                        matrices[1, 2, xm, ym] = base_matrix[xm, ym+1];
                        matrices[2, 1, xm, ym] = base_matrix[xm+1, ym];

                        matrices[2, 2, xm, ym] = base_matrix[xm+1, ym+1];
                        matrices[2, 3, xm, ym] = base_matrix[xm+1, Math.Min(ym+2, 3)];
                        matrices[3, 2, xm, ym] = base_matrix[Math.Min(xm+2, 3), ym + 1];

                        //matrices[3, 3, xm, ym] =  
                    }

            }
        }
        private class BicubicInterpSlow {
            public static double interp(double v0, double v1, double v2, double v3, double x) {
                double A = (v3 - v2) - (v0 - v1);
                double B = (v0 - v1) - A;
                double C = v2 - v0;
                double D = v1;

                return A * x * x * x + B * x * x + C * x + D;
            }

            public static double[,] crtomat(Color[,] c, int chan) {
                double[,] v = new double[4, 4];
                switch (chan) {
                    case 0:
                        for (int x = 0; x < 4; x++)
                            for (int y = 0; y < 4; y++)
                                v[y, x] = c[x, y].B;
                        break;
                    case 1:
                        for (int x = 0; x < 4; x++)
                            for (int y = 0; y < 4; y++)
                                v[y, x] = c[x, y].G;
                        break;
                    case 2:
                        for (int x = 0; x < 4; x++)
                            for (int y = 0; y < 4; y++)
                                v[y, x] = c[x, y].R;
                        break;
                    default: throw new Exception($"invalid channel requested in BicubicInterp constructor ({chan})");
                }
                return v;
            }

            public static double interp2d(double[,] matrix, double x, double y) {
                double[] xs = new double[4];
                for (int xi = 0; xi < 4; xi++)
                    xs[xi] = interp(matrix[xi, 0], matrix[xi, 1], matrix[xi, 2], matrix[xi, 3], x);
                return interp(xs[0], xs[1], xs[2], xs[3], y);
            }

            public static int tocr(double c) {
                return (int)Math.Max(Math.Min(c, 255), 0);
            }
        }
        private class BicubicInterp {
            public int[,] a = new int[4, 4];
            private int[,] f = new int[4, 4];
            private readonly int[,] A = {
                { 1, 0, 0, 1 },
                { 0, 0, 1, 0 },
                { -3, 3, -2, -1 },
                { 2, -2, 1, 1 }
            };

            private readonly int[,] B = {
                { 1, 0, -3, 2 },
                { 0, 0, 3, -2 },
                { 0, 1, -2, 1 },
                { 0, 0, -1, 1 }
            };
            
            public int matrix_multiply3_ip(double[] xt, int[,] at, double[] yt) {
                double[] t = new double[4];
                for (int i = 0; i < 4; i++)
                    for (int j = 0; j < 4; j++)
                        t[i] += xt[i] * at[i, j];

                double r = 0;
                for (int i = 0; i < 4; i++)
                    r += t[i] * yt[i];

                return (int)r;
            }

            public int interpolate(double x, double y) {
                double[] xt = { 1, x, x * x, x * x * x };
                double[] yt = { 1, y, y * y, y * y * y };
                return matrix_multiply3_ip(xt, a, yt);
            }
            
            private void init_derivatives(int[,] inputs) {
                for (int x = 1; x < 3; x++)
                    for (int y = 1; y < 3; y++) {
                        f[x - 1, y - 1] = inputs[x, y];
                        f[x - 1, y + 1] = (inputs[x + 1, y] - inputs[x - 1, y]) / 2;
                        f[x + 1, y - 1] = (inputs[x, y + 1] - inputs[x, y - 1]) / 2;
                        f[x + 1, x + 1] = (inputs[x + 1, y + 1] - inputs[x - 1, y]
                                           - inputs[x, y - 1] + inputs[x, y])/4;
                    }
            }

            public int clamp(int i, int min = 0, int max = 255) 
                => Math.Min(max,Math.Max(i, min));
            

            private void init_coefficients() {
                int[,] n = new int[4, 4];

                for (int x = 0; x < 4; x++)
                    for (int y = 0; y < 4; y++)
                        n[x, y] += A[x, y] * f[y, x];

                for (int x = 0; x < 4; x++)
                    for (int y = 0; y < 4; y++)
                        a[x, y] += n[x, y] * B[y, x];
            }

            public BicubicInterp(Color[,] inputs, int channel) {
                int[,] v = new int[4,4];
                switch (channel) {
                    case 0:
                        for (int x = 0; x < 4; x++) 
                            for (int y = 0; y < 4; y++) 
                                v[x, y] = inputs[x, y].B;
                        break;
                    case 1:
                        for (int x = 0; x < 4; x++) 
                            for (int y = 0; y < 4; y++) 
                                v[x, y] = inputs[x, y].G;
                        break;
                    case 2:
                        for (int x = 0; x < 4; x++) 
                            for (int y = 0; y < 4; y++) 
                                v[x, y] = inputs[x, y].R;
                        break;
                    default: throw new Exception($"invalid channel requested in BicubicInterp constructor ({channel})");
                }

                init_derivatives(v);
                init_coefficients();
            }
        }

        private Stopwatch sw_bicubic, sw_bilinear, sw_nearest;
        private Bitmap bm_bicubic, bm_bilinear, bm_nearest;

        private EventWaitHandle e_start = new EventWaitHandle(false, EventResetMode.ManualReset);
        private Color[,] cols;

        Thread[] ts = new Thread[3];

        private class Blerp {
            int[,] q = new int[2, 2];

            public int interpolate(double x, double y, int min = 0, int max = 255) {
                double y1 = (1 - x) * q[0, 0];
                y1 += x * q[1, 0];

                double y2 = (1 - x) * q[0, 1];
                y2 += x * q[1, 1];

                double q1 = (1 - y);
                double q2 = y;

                return Math.Max(Math.Min((int)(q1 * y1 + q2 * y2), max), min);

            }

            public Blerp(Color[,] inputs, int channel, int xo = 0, int yo = 0) {
                switch (channel) {
                    case 0:
                        q[0, 0] = inputs[xo, yo].B;
                        q[1, 0] = inputs[xo + 1, yo].B;
                        q[0, 1] = inputs[xo, yo + 1].B;
                        q[1, 1] = inputs[xo + 1, yo + 1].B;
                        break;
                    case 1:
                        q[0, 0] = inputs[xo, yo].G;
                        q[1, 0] = inputs[xo + 1, yo].G;
                        q[0, 1] = inputs[xo, yo + 1].G;
                        q[1, 1] = inputs[xo + 1, yo + 1].G;
                        break;
                    case 2:
                        q[0, 0] = inputs[xo, yo].R;
                        q[1, 0] = inputs[xo+1, yo].R;
                        q[0, 1] = inputs[xo, yo+1].R;
                        q[1, 1] = inputs[xo + 1, yo+1].R;
                        break;
                    default: throw new Exception($"invalid channel requested in Blerp constructor ({channel})");
                }
            }
        }

        public Form1() {
            InitializeComponent();
        }

        private void bw_bl_DoWork(object sender, DoWorkEventArgs e) {
            ts[1].Start();
            ts[1].Join(-1);

        }

        private void bw_bc_DoWork(object sender, DoWorkEventArgs e) {
            ts[2].Start();
            ts[2].Join(-1);

            e_start.Reset();
        }

        private void bw_nn_DoWork(object sender, DoWorkEventArgs e) {
            ts[0].Start();
            ts[0].Join(-1);

        }

        private void bw_bl_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            pictureBox2.Image = bm_bilinear;
            label2.Text = sw_bilinear.Elapsed.ToString();
        }

        private void bw_bc_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            pictureBox1.Image = bm_bicubic;
            label1.Text = sw_bicubic.Elapsed.ToString();
        }

        private void bw_nn_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            pictureBox3.Image = bm_nearest;
            label3.Text = sw_nearest.Elapsed.ToString();
        }

        private void label2_Click(object sender, EventArgs e) {

        }

        private void interp_nn() {
            Bitmap refb = new Bitmap(512, 512);
            sw_nearest = new Stopwatch();
            e_start.WaitOne();
            sw_nearest.Start();
            int mag = 512 / 4;
            for (int x = 0; x < 512; x++)
                for (int y = 0; y < 512; y++) {
                    refb.SetPixel(x, y, cols[x / mag, y / mag]);
                }
            sw_nearest.Stop();
            bm_nearest = refb;
        }

        private void interp_bl() {
            Bitmap refe = new Bitmap(512, 512);
            sw_bilinear = new Stopwatch();
            e_start.WaitOne();
            sw_bilinear.Start();
            Blerp[,] lr = new Blerp[3, 3],
                 lg = new Blerp[3, 3],
                 lb = new Blerp[3, 3];

            for (int x = 0; x < 3; x++)
                for (int y = 0; y < 3; y++) {
                    lr[x, y] = new Blerp(cols, 2, x, y);
                    lg[x, y] = new Blerp(cols, 1, x, y);
                    lb[x, y] = new Blerp(cols, 0, x, y);
                }
            double diff = 512.0 / 3.0;
            for (int x = 0; x < 512; x++)
                for (int y = 0; y < 512; y++) {
                    double dfx = x,
                    dfy = y;
                    dfx /= diff;
                    dfy /= diff;

                    int xi = (int)(x / diff);
                    int yi = (int)(y / diff);

                    if (xi > 2) xi = 2;
                    if (yi > 2) yi = 2;

                    refe.SetPixel(x, y, Color.FromArgb(
                        lr[xi, yi].interpolate(dfx - xi, dfy - yi),
                        lg[xi, yi].interpolate(dfx - xi, dfy - yi),
                        lb[xi, yi].interpolate(dfx - xi, dfy - yi)
                        )
                    );
                }
            sw_bilinear.Start();
            bm_bilinear = refe;
        }

        private void interp_bc() {
            Bitmap r = new Bitmap(512, 512);

            sw_bicubic = new Stopwatch();
            e_start.WaitOne();
            sw_bicubic.Start();
            Color[,][,] matrices = new Color[3,3][,];

            for (int x = 0; x < 3; x++) 
                for (int y = 0; y < 3; y++) {
                    matrices[x, y] = new Color[4, 4];
                    

                    for (int mx = 0; mx < 4; mx++)
                        for (int my = 0; my < 4; my++) {
                            int sx = Math.Max(mx + (x-1), 0);
                            int sy = Math.Max(my + (y-1), 0);

                            sx = Math.Min(sx, 3);
                            sy = Math.Min(sy, 3);
                            matrices[x, y][mx, my] = cols[sx, sy];
                        }
                }
            
            double diff = 512.0 / 3.0;

            for (int x = 0; x < 512; x++) 
                for (int y = 0; y < 512; y++) {

                    int xi = (int)((x-2) / diff);
                    int yi = (int)((y-2) / diff);

                    double fx = ((double)(x-2)) / diff, 
                           fy = ((double)(y-2)) / diff;

                    fx %= 1;
                    fy %= 1;

                    r.SetPixel(x, y, Color.FromArgb(
                        BicubicInterpSlow.tocr(BicubicInterpSlow.interp2d(BicubicInterpSlow.crtomat(matrices[xi, yi], 2), fx, fy)),
                        BicubicInterpSlow.tocr(BicubicInterpSlow.interp2d(BicubicInterpSlow.crtomat(matrices[xi, yi], 1), fx, fy)),
                        BicubicInterpSlow.tocr(BicubicInterpSlow.interp2d(BicubicInterpSlow.crtomat(matrices[xi, yi], 0), fx, fy))
                        )
                    );
                    
                  
                }
            sw_bicubic.Stop();
            bm_bicubic = r;
        }

        private void button1_Click(object sender, EventArgs e) {
            Random r = new Random();
            cols = new Color[4, 4];

            Bitmap test = new Bitmap(512, 512);
            Stopwatch t = new Stopwatch();
            t.Start();
            for (int x = 0; x < 512; x++)
                for (int y = 0; y < 512; y++)
                    test.SetPixel(x, y, Color.Red);
            t.Stop();
            test.Dispose();
            label11.Text = t.Elapsed.ToString();

            for (int i = 0; i < 4; i++) 
                for (int j = 0; j < 4; j++) 
                    cols[i, j] = Color.FromArgb(255, Color.FromArgb(r.Next()));

            pictureBox1.Image = null;
            pictureBox2.Image = null;
            pictureBox3.Image = null;
            
            ts[0] = new Thread(interp_nn);
            ts[1] = new Thread(interp_bl);
            ts[2] = new Thread(interp_bc);

            bw_bc.RunWorkerAsync();
            bw_bl.RunWorkerAsync();
            bw_nn.RunWorkerAsync();

            Thread.Sleep(100);
            e_start.Set();
        }
    }
}
