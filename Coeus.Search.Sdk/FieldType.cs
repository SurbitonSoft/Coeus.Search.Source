namespace Coeus.Search.Sdk
{
    using org.apache.lucene.document;

    public class SearchFields
    {
        public static FieldType StoreOnly
        {
            get;
            private set;
        }

        public static FieldType KeywordField
        {
            get;
            private set;
        }

        public static FieldType AnalyzedField
        {
            get;
            private set;
        }

        public static FieldType NumericField
        {
            get;
            private set;
        }

        static SearchFields()
        {
            // StoreOnly Field
            StoreOnly = new FieldType();
            StoreOnly.setIndexed(false);
            StoreOnly.setStored(true);
            StoreOnly.setOmitNorms(false);
            StoreOnly.setIndexOptions(org.apache.lucene.index.FieldInfo.IndexOptions.DOCS_ONLY);

            // Keyword Field
            KeywordField = new FieldType();
            KeywordField.setIndexed(true);
            KeywordField.setTokenized(true);
            KeywordField.setStored(false);
            KeywordField.setOmitNorms(false);
            KeywordField.setIndexOptions(org.apache.lucene.index.FieldInfo.IndexOptions.DOCS_ONLY);

            // AnalyzedField Field
            AnalyzedField = new FieldType();
            AnalyzedField.setIndexed(true);
            AnalyzedField.setTokenized(false);
            AnalyzedField.setStored(false);
            AnalyzedField.setOmitNorms(true);
            AnalyzedField.setIndexOptions(org.apache.lucene.index.FieldInfo.IndexOptions.DOCS_ONLY);
        }
    }
}
