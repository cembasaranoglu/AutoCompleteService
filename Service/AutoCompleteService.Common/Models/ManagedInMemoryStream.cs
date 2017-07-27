using System.IO;

namespace AutoCompleteService.Common.Models
{
    public class ManagedInMemoryStream : MemoryStream
    {
        public ManagedInMemoryStream(byte[] buffer)
            : base(buffer)
        { }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }
    }
}