﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using CPF.Drawing;
using System.Linq;

namespace CPF
{
    /// <summary>
    /// Represents a size in device pixels.
    /// </summary>
    public readonly struct PixelSize
    {
        /// <summary>
        /// A size representing zero
        /// </summary>
        public static readonly PixelSize Empty = new PixelSize(0, 0);

        /// <summary>
        /// Initializes a new instance of the <see cref="PixelSize"/> structure.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public PixelSize(int width, int height)
        {
            Width = width;
            Height = height;
        }

        ///// <summary>
        ///// Gets the aspect ratio of the size.
        ///// </summary>
        //public double AspectRatio => (double)Width / Height;

        /// <summary>
        /// Gets the width.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Gets the height.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Checks for equality between two <see cref="PixelSize"/>s.
        /// </summary>
        /// <param name="left">The first size.</param>
        /// <param name="right">The second size.</param>
        /// <returns>True if the sizes are equal; otherwise false.</returns>
        public static bool operator ==(PixelSize left, PixelSize right)
        {
            return left.Width == right.Width && left.Height == right.Height;
        }

        /// <summary>
        /// Checks for inequality between two <see cref="Size"/>s.
        /// </summary>
        /// <param name="left">The first size.</param>
        /// <param name="right">The second size.</param>
        /// <returns>True if the sizes are unequal; otherwise false.</returns>
        public static bool operator !=(PixelSize left, PixelSize right)
        {
            return !(left == right);
        }

        public static PixelSize Parse(string s)
        {
            var parts = s.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToList();

            if (parts.Count == 2)
            {
                return new PixelSize(int.Parse(parts[0]), int.Parse(parts[1]));
            }
            else
            {
                throw new FormatException("Invalid Size.");
            }
        }

        /// <summary>
        /// Checks for equality between a size and an object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// True if <paramref name="obj"/> is a size that equals the current size.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is PixelSize other)
            {
                return this == other;
            }

            return false;
        }

        /// <summary>
        /// Returns a hash code for a <see cref="PixelSize"/>.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = (hash * 23) + Width.GetHashCode();
                hash = (hash * 23) + Height.GetHashCode();
                return hash;
            }
        }

        /// <summary>
        /// Returns a new <see cref="PixelSize"/> with the same height and the specified width.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <returns>The new <see cref="PixelSize"/>.</returns>
        public PixelSize WithWidth(int width) => new PixelSize(width, Height);

        /// <summary>
        /// Returns a new <see cref="PixelSize"/> with the same width and the specified height.
        /// </summary>
        /// <param name="height">The height.</param>
        /// <returns>The new <see cref="PixelSize"/>.</returns>
        public PixelSize WithHeight(int height) => new PixelSize(Width, height);

        /// <summary>
        /// Converts the <see cref="PixelSize"/> to a device-independent <see cref="Size"/> using the
        /// specified scaling factor.
        /// </summary>
        /// <param name="scale">The scaling factor.</param>
        /// <returns>The device-independent size.</returns>
        public Size ToSize(float scale) => new Size(Width / scale, Height / scale);

        /// <summary>
        /// Converts the <see cref="PixelSize"/> to a device-independent <see cref="Size"/> using the
        /// specified scaling factor.
        /// </summary>
        /// <param name="scale">The scaling factor.</param>
        /// <returns>The device-independent size.</returns>
        public Size ToSize(Vector scale) => new Size(Width / scale.X, Height / scale.Y);

        /// <summary>
        /// Converts the <see cref="PixelSize"/> to a device-independent <see cref="Size"/> using the
        /// specified dots per inch (DPI).
        /// </summary>
        /// <param name="dpi">The dots per inch.</param>
        /// <returns>The device-independent size.</returns>
        public Size ToSizeWithDpi(float dpi) => ToSize(dpi / 96);

        /// <summary>
        /// Converts the <see cref="PixelSize"/> to a device-independent <see cref="Size"/> using the
        /// specified dots per inch (DPI).
        /// </summary>
        /// <param name="dpi">The dots per inch.</param>
        /// <returns>The device-independent size.</returns>
        public Size ToSizeWithDpi(Vector dpi) => ToSize(new Vector(dpi.X / 96, dpi.Y / 96));

        /// <summary>
        /// Converts a <see cref="Size"/> to device pixels using the specified scaling factor.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <param name="scale">The scaling factor.</param>
        /// <returns>The device-independent size.</returns>
        public static PixelSize FromSize(Size size, double scale) => new PixelSize(
            (int)Math.Ceiling(size.Width * scale),
            (int)Math.Ceiling(size.Height * scale));

        /// <summary>
        /// Converts a <see cref="Size"/> to device pixels using the specified scaling factor.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <param name="scale">The scaling factor.</param>
        /// <returns>The device-independent size.</returns>
        public static PixelSize FromSize(Size size, Vector scale) => new PixelSize(
            (int)Math.Ceiling(size.Width * scale.X),
            (int)Math.Ceiling(size.Height * scale.Y));

        /// <summary>
        /// Converts a <see cref="Size"/> to device pixels using the specified dots per inch (DPI).
        /// </summary>
        /// <param name="size">The size.</param>
        /// <param name="dpi">The dots per inch.</param>
        /// <returns>The device-independent size.</returns>
        public static PixelSize FromSizeWithDpi(Size size, double dpi) => FromSize(size, dpi / 96);

        /// <summary>
        /// Converts a <see cref="Size"/> to device pixels using the specified dots per inch (DPI).
        /// </summary>
        /// <param name="size">The size.</param>
        /// <param name="dpi">The dots per inch.</param>
        /// <returns>The device-independent size.</returns>
        public static PixelSize FromSizeWithDpi(Size size, Vector dpi) => FromSize(size, new Vector(dpi.X / 96, dpi.Y / 96));

        /// <summary>
        /// Returns the string representation of the size.
        /// </summary>
        /// <returns>The string representation of the size.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}, {1}", Width, Height);
        }
    }
}
