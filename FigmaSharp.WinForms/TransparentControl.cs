using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

public class TransparentControl : Control
{
    public bool drag = false;
    public bool enab = false;
    private float m_opacity = 1;

    private int alpha;
    public TransparentControl()
    {
        SetStyle (ControlStyles.SupportsTransparentBackColor, true);
        SetStyle (ControlStyles.Opaque, true);
        this.BackColor = Color.Transparent;
    }

    public float Opacity {
        get {
            if (m_opacity > 1) {
                m_opacity = 1;
            } else if (m_opacity < 0) {
                m_opacity = 0;
            }
            return this.m_opacity;
        }
        set {
            this.m_opacity = value;
            if (this.Parent != null) {
                Parent.Invalidate (this.Bounds, true);
            }
        }
    }

    protected override CreateParams CreateParams {
        get {
            CreateParams cp = base.CreateParams;
            cp.ExStyle = cp.ExStyle | 0x20;
            return cp;
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        Rectangle bounds = new Rectangle (0, 0, this.Width - 1, this.Height - 1);

        Color frmColor = this.Parent.BackColor;
        Brush bckColor = default (Brush);

        alpha = (int) (m_opacity * 255);

        if (drag) {
            Color dragBckColor = default (Color);

            if (BackColor != Color.Transparent) {
                int Rb = BackColor.R * alpha / 255 + frmColor.R * (255 - alpha) / 255;
                int Gb = BackColor.G * alpha / 255 + frmColor.G * (255 - alpha) / 255;
                int Bb = BackColor.B * alpha / 255 + frmColor.B * (255 - alpha) / 255;
                dragBckColor = Color.FromArgb (Rb, Gb, Bb);
            } else {
                dragBckColor = frmColor;
            }

            alpha = 255;
            bckColor = new SolidBrush (Color.FromArgb (alpha, dragBckColor));
        } else {
            bckColor = new SolidBrush (Color.FromArgb (alpha, this.BackColor));
        }

        if (this.BackColor != Color.Transparent | drag) {
            g.FillRectangle (bckColor, bounds);
        }

        bckColor.Dispose ();
        g.Dispose ();
        base.OnPaint (e);
    }

    protected override void OnBackColorChanged(EventArgs e)
    {
        if (this.Parent != null) {
            Parent.Invalidate (this.Bounds, true);
        }
        base.OnBackColorChanged (e);
    }

    protected override void OnParentBackColorChanged(EventArgs e)
    {
        this.Invalidate ();
        base.OnParentBackColorChanged (e);
    }
}