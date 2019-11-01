namespace GdiShaders.Core
{
    using System.Drawing;

    public enum ImageNames
    {
        None,
        Abstract1,
        London,
        Organic2,
        RGBANoiseMedium,
    }

    public enum WrapModes
    {
        Clamp,
        Repeat,
    }

    public class samplerXX
    {
        public Bitmap bmp;

        public samplerXX(int w, int h)
        {
            bmp = new Bitmap(w, h);
        }
        
        public WrapModes WrapMode { get; set; }

        private samplerXX()
        {
        }

        internal static samplerXX FromImage(ImageNames image)
        {
            var sampler = new samplerXX();

            switch (image)
            {
                case ImageNames.Abstract1:
                    sampler.bmp = new Bitmap(Properties.Resources.abstract_1);
                    break;
                case ImageNames.London:
                    sampler.bmp = new Bitmap(Properties.Resources.london);
                    break;
                case ImageNames.Organic2:
                    sampler.bmp = new Bitmap(Properties.Resources.organic_2);
                    break;
                case ImageNames.RGBANoiseMedium:
                    sampler.bmp = new Bitmap(Properties.Resources.RGBA_Noise_Medium);
                    break;
            }

            return sampler;
        }
    }
}