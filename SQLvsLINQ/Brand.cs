using System;

namespace SQLvsLINQ
{
    class Brand : IEquatable<Brand>
    {
        public int BrandId { get; set; }
        public string BrandName { get; set; }

        public override string ToString()
        {
            return $"{nameof(BrandId)}: {BrandId}, {nameof(BrandName)}: {BrandName}";
        }

        public bool Equals(Brand other)
        {
            if (other is null)
                return false;

            return this.BrandName == other.BrandName;
        }

        public override bool Equals(object obj) => Equals(obj as Brand);
        public override int GetHashCode() => (BrandName, BrandId).GetHashCode();
    }
}
