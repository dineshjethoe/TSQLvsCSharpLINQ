namespace SQLvsLINQ
{
    class Model
    {
        public int ModelId { get; set; }
        public string ModelName { get; set; }
        public int? BrandId { get; set; }

        public override string ToString()
        {
            return $"{nameof(ModelId)}: {ModelId}, {nameof(ModelName)}: {ModelName}, {nameof(BrandId)}: {BrandId}";
        }
    }
}
