using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace GrooveClone.Models
{
    public class Song
    {
        public string Album { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public int Index { get; set; }
        public string Duration { get; set; }
        public byte[] Img { get; set; } 
        public string Path { get; set; }

        public ImageSource imageSource()
        {
            ImageSource source = ImageSource.FromStream(() => new MemoryStream(Img));
            return source;
        }
        public Song(int count)
        {
            Img = new byte[count];
        }
    }
}
