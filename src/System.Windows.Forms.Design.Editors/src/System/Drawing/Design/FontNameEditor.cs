﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace System.Drawing.Design
{
    /// <summary>
    ///   Provides a <see cref="T:System.Drawing.Design.UITypeEditor" /> that paints a glyph for the font name.
    /// </summary>
    [System.Security.SecurityCritical]
    [System.Security.Permissions.PermissionSetAttribute(System.Security.Permissions.SecurityAction.InheritanceDemand, Name = "FullTrust")]
    public class FontNameEditor : UITypeEditor
    {
        private static readonly FontStyle[] s_FontStyles = new[]
        {
             FontStyle.Regular,
             FontStyle.Italic,
             FontStyle.Bold,
             FontStyle.Bold | FontStyle.Italic
        };

        /// <summary>
        ///   Determines if this editor supports the painting of a representation of an object's value.
        /// </summary>
        /// <param name="context">A type descriptor context that can be used to provide additional context information.</param>
        /// <returns>
        ///   <see langword="true" /> as this editor supports the painting of a representation of an object's value.
        /// </returns>
        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        ///   Paints a representative value of the given object to the provided canvas. 
        ///   Painting should be done within the boundaries of the provided rectangle.
        /// </summary>
        /// <param name="e">What to paint and where to paint it. </param>
        public override void PaintValue(PaintValueEventArgs e)
        {
            string fontName = e.Value as string;
            if (string.IsNullOrWhiteSpace(fontName))
            {
                // don't draw anything if we don't have a value.
                return;
            }

            try
            {
                using (var family = new FontFamily(fontName))
                {
                    e.Graphics.FillRectangle(SystemBrushes.ActiveCaption, e.Bounds);

                    // Believe it or not, not all font families have a "normal" face.  Try normal, then italic, 
                    // then bold, then bold italic, then give up.
                    foreach (var fontStyle in s_FontStyles)
                    {
                        try
                        {
                            DrawFontSample(e, family, fontStyle);
                            break;
                        }
                        catch
                        {
                            // no-op
                        }
                    }
                }

                e.Graphics.DrawLine(SystemPens.WindowFrame, e.Bounds.Right, e.Bounds.Y, e.Bounds.Right, e.Bounds.Bottom);
            }
            catch
            {
                // Ignore the exception if the fontName does not exist or is invalid...
                // we just won't render a preview of the font at all
            }
        }

        /// <summary>
        /// Tries to render sample of text in specified font and style,
        /// throwing exception if specified font does not support that style...
        /// </summary>
        private static void DrawFontSample(PaintValueEventArgs e, FontFamily fontFamily, FontStyle fontStyle)
        {
            float fontSize = (float)(e.Bounds.Height / 1.2);
            using (var font = new Font(fontFamily, fontSize, fontStyle, GraphicsUnit.Pixel))
            {
                e.Graphics.DrawString("abcd", font, SystemBrushes.ActiveCaptionText, e.Bounds);
            }
        }
    }
}
