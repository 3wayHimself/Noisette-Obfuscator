﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Noisette
{
    class RoundRectangle
    {
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateRoundRectRgn(int x1, int y1, int x2, int y2, int cx, int cy);
        public static object DrawRoundRectangle(Rectangle rct, int r, int MSAA, Color FillColor, Color BorderColor)
        {
            Bitmap b = new Bitmap(rct.Width * MSAA + MSAA, rct.Height * MSAA + MSAA);
            Graphics g = Graphics.FromImage(b);
            IntPtr RegH = CreateRoundRectRgn(rct.Left, rct.Top, (rct.Width) * MSAA, (rct.Height) * MSAA, r * MSAA, r * MSAA);
            IntPtr RegH1 = CreateRoundRectRgn(rct.Left + MSAA / 1, rct.Top + MSAA / 1, (rct.Width - 1) * MSAA, (rct.Height - 1) * MSAA, (r - 2) * MSAA, (r - 2) * MSAA);
            Region Reg = Region.FromHrgn(RegH);
            Region Reg1 = Region.FromHrgn(RegH1);

            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.FillRegion(new SolidBrush(BorderColor), Reg);
            g.SetClip(Reg1, CombineMode.Replace);
            g.Clear(FillColor);
            return b;
        }
    }

    public class RoundButton : Button
    {
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            GraphicsPath grPath = new GraphicsPath();
            grPath.AddEllipse(-2, -2, ClientSize.Width , ClientSize.Height);
            this.Region = new System.Drawing.Region(grPath);
            base.OnPaint(e);
        }
    }

    public class ShapedPanel : Panel
    {

        private Pen pen = new Pen(_borderColor, penWidth);
        private static readonly float penWidth = 2f;

        public ShapedPanel()
        {

        }
        private static Color _borderColor = Color.White;
        [Browsable(true)]
        public Color BorderColor
        {
            get { return _borderColor; }
            set
            {
                _borderColor = value;
                pen = new Pen(_borderColor, penWidth);
                Invalidate();
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            ExtendedDraw(e);
            DrawBorder(e.Graphics);
        }
        private int _edge = 50;
        [Browsable(true)]
        public int Edge
        {
            get { return _edge; }
            set
            {
                _edge = value;
                Invalidate();
            }
        }
        private Rectangle GetLeftUpper(int e)
        {
            return new Rectangle(0, 0, e, e);
        }
        private Rectangle GetRightUpper(int e)
        {
            return new Rectangle(Width - e, 0, e, e);
        }
        private Rectangle GetRightLower(int e)
        {
            return new Rectangle(Width - e, Height - e, e, e);
        }
        private Rectangle GetLeftLower(int e)
        {
            return new Rectangle(0, Height - e, e, e);
        }
        private void ExtendedDraw(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            GraphicsPath path = new GraphicsPath();
            path.StartFigure();
            path.StartFigure();
            path.AddArc(GetLeftUpper(Edge), 180, 90);
            path.AddLine(Edge, 0, Width - Edge, 0);
            path.AddArc(GetRightUpper(Edge), 270, 90);
            path.AddLine(Width, Edge, Width, Height - Edge);
            path.AddArc(GetRightLower(Edge), 0, 90);
            path.AddLine(Width - Edge, Height, Edge, Height);
            path.AddArc(GetLeftLower(Edge), 90, 90);
            path.AddLine(0, Height - Edge, 0, Edge);
            path.CloseFigure();
            Region = new Region(path);
        }
        private void DrawSingleBorder(Graphics graphics)
        {
            graphics.DrawArc(pen, new Rectangle(0, 0, Edge, Edge), 180, 90);
            graphics.DrawArc(pen, new Rectangle(Width - Edge - 1, -1, Edge, Edge), 270, 90);
            graphics.DrawArc(pen, new Rectangle(Width - Edge - 1, Height - Edge - 1, Edge, Edge), 0, 90);
            graphics.DrawArc(pen, new Rectangle(0, Height - Edge - 1, Edge, Edge), 90, 90);
            graphics.DrawRectangle(pen, 0f, 0f, Convert.ToSingle((Width - 1)), Convert.ToSingle((Height - 1)));
        }
        private void Draw3DBorder(Graphics graphics)
        {
            //TODO Implement 3D border
        }
        private void DrawBorder(Graphics graphics)
        {
            DrawSingleBorder(graphics);
        }
    }
}
