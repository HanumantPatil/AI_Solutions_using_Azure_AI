using Microsoft.Extensions.AI;

namespace ContosoHRAgent
{
    public class FileReference(string fieldId, string fileName, string quota, Citation citation)
    {
        public  string FieldId { get; } = fieldId;
        public  string FileName { get; } = fileName;
        public  string Quota { get; } = quota;
        public  Citation Citation { get; } = citation;
    }
}
