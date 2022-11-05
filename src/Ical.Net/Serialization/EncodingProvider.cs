using System;
using System.Text;

namespace Ical.Net.Serialization
{
    internal sealed class EncodingProvider : IEncodingProvider
    {
        private delegate string EncoderDelegate(byte[] data);

        private delegate byte[] DecoderDelegate(string value);

        private readonly SerializationContext _mSerializationContext;

        public EncodingProvider(SerializationContext ctx)
        {
            _mSerializationContext = ctx;
        }

        private byte[] Decode7Bit(string value)
        {
            try
            {
                var utf7 = new UTF7Encoding();
                return utf7.GetBytes(value);
            }
            catch
            {
                return null;
            }
        }

        private byte[] Decode8Bit(string value)
        {
            try
            {
                var utf8 = new UTF8Encoding();
                return utf8.GetBytes(value);
            }
            catch
            {
                return null;
            }
        }

        private byte[] DecodeBase64(string value)
        {
            try
            {
                return Convert.FromBase64String(value);
            }
            catch
            {
                return null;
            }
        }

        private DecoderDelegate GetDecoderFor(string encoding)
        {
            if (encoding == null)
            {
                return null;
            }

            switch (encoding.ToUpper())
            {
                case "7BIT":
                    return Decode7Bit;
                case "8BIT":
                    return Decode8Bit;
                case "BASE64":
                    return DecodeBase64;
                default:
                    return null;
            }
        }

        private string Encode7Bit(byte[] data)
        {
            try
            {
                var utf7 = new UTF7Encoding();
                return utf7.GetString(data);
            }
            catch
            {
                return null;
            }
        }

        private string Encode8Bit(byte[] data)
        {
            try
            {
                var utf8 = new UTF8Encoding();
                return utf8.GetString(data);
            }
            catch
            {
                return null;
            }
        }

        private string EncodeBase64(byte[] data)
        {
            try
            {
                return Convert.ToBase64String(data);
            }
            catch
            {
                return null;
            }
        }

        private EncoderDelegate GetEncoderFor(string encoding)
        {
            if (encoding == null)
            {
                return null;
            }

            return encoding.ToUpper() switch
            {
                "7BIT" => Encode7Bit,
                "8BIT" => Encode8Bit,
                "BASE64" => EncodeBase64,
                _ => null
            };
        }

        public string Encode(string encoding, byte[] data)
        {
            if (encoding == null || data == null)
            {
                return null;
            }

            var encoder = GetEncoderFor(encoding);
            //var wrapped = TextUtil.FoldLines(encoder?.Invoke(data));
            //return wrapped;
            return encoder?.Invoke(data);
        }

        public string DecodeString(string encoding, string value)
        {
            if (encoding == null || value == null)
            {
                return null;
            }

            var data = DecodeData(encoding, value);
            if (data == null)
            {
                return null;
            }

            // Decode the string into the current encoding
            var encodingStack = _mSerializationContext.GetService(typeof (EncodingStack)) as EncodingStack;
            return encodingStack.Current.GetString(data);
        }

        public byte[] DecodeData(string encoding, string value)
        {
            if (encoding == null || value == null)
            {
                return null;
            }

            var decoder = GetDecoderFor(encoding);
            return decoder?.Invoke(value);
        }
    }
}